using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AvatarViewer;
using AvatarViewer.Trackers;
using UnityEngine;

namespace OpenSee
{

    public class OpenSee : MonoBehaviour
    {
        private const int nPoints = 68;
        private const int packetFrameSize = 8 + 4 + 2 * 4 + 2 * 4 + 1 + 4 + 3 * 4 + 3 * 4 + 4 * 4 + 4 * 68 + 4 * 2 * 68 + 4 * 3 * 70 + 4 * 14;
        private UdpClient udp;

        [Header("Tracking data")]
        [Tooltip("This is an informational property that tells you how many packets have been received")]
        public int receivedPackets = 0;
        [Tooltip("This contains the actual tracking data")]
        public OpenSeeData[] trackingData = null;

        public bool listening { get; private set; } = false;

        [HideInInspector]
        public float maxFit3DError = 100f;

        [System.Serializable]
        public class OpenSeeData
        {
            [Tooltip("The time this tracking data was captured at.")]
            public double time;
            [Tooltip("This is the id of the tracked face. When tracking multiple faces, they might get reordered due to faces coming and going, but as long as tracking is not lost on a face, its id should stay the same. Face ids depend only on the order of first detection and locations of the faces.")]
            public int id;
            [Tooltip("This field gives the resolution of the camera or video being tracked.")]
            public Vector2 cameraResolution;
            [Tooltip("This field tells you how likely it is that the right eye is open.")]
            public float rightEyeOpen;
            [Tooltip("This field tells you how likely it is that the left eye is open.")]
            public float leftEyeOpen;
            [Tooltip("This field contains the rotation of the right eyeball.")]
            public Quaternion rightGaze;
            [Tooltip("This field contains the rotation of the left eyeball.")]
            public Quaternion leftGaze;
            [Tooltip("This field tells you if 3D points have been successfully estimated from the 2D points. If this is false, do not rely on pose or 3D data.")]
            public bool got3DPoints;
            [Tooltip("This field contains the error for fitting the original 3D points. It shouldn't matter much, but it it is very high, something is probably wrong")]
            public float fit3DError;
            [Tooltip("This is the rotation vector for the 3D points to turn into the estimated face pose.")]
            public Vector3 rotation;
            [Tooltip("This is the translation vector for the 3D points to turn into the estimated face pose.")]
            public Vector3 translation;
            [Tooltip("This is the raw rotation quaternion calculated from the OpenCV rotation matrix. It does not match Unity's coordinate system, but it still might be useful.")]
            public Quaternion rawQuaternion;
            [Tooltip("This is the raw rotation euler angles calculated by OpenCV from the rotation matrix. It does not match Unity's coordinate system, but it still might be useful.")]
            public Vector3 rawEuler;
            [Tooltip("This field tells you how certain the tracker is.")]
            public float[] confidence;
            [Tooltip("These are the detected face landmarks in image coordinates. There are 68 points. The last too points are pupil points from the gaze tracker.")]
            public Vector2[] points;
            [Tooltip("These are 3D points estimated from the 2D points. The should be rotation and translation compensated. There are 70 points with guesses for the eyeball center positions being added at the end of the 68 2D points.")]
            public Vector3[] points3D;
            [Tooltip("This field contains a number of action unit like features.")]
            public OpenSeeFeatures features;

            public Vector3 rightGazeVec;
            public Vector3 leftGazeVec;

            [System.Serializable]
            public class OpenSeeFeatures
            {
                [Tooltip("This field indicates whether the left eye is opened(0) or closed (-1). A value of 1 means open wider than normal.")]
                public float EyeLeft;
                [Tooltip("This field indicates whether the right eye is opened(0) or closed (-1). A value of 1 means open wider than normal.")]
                public float EyeRight;
                [Tooltip("This field indicates how steep the left eyebrow is, compared to the median steepness.")]
                public float EyebrowSteepnessLeft;
                [Tooltip("This field indicates how far up or down the left eyebrow is, compared to its median position.")]
                public float EyebrowUpDownLeft;
                [Tooltip("This field indicates how quirked the left eyebrow is, compared to its median quirk.")]
                public float EyebrowQuirkLeft;
                [Tooltip("This field indicates how steep the right eyebrow is, compared to the average steepness.")]
                public float EyebrowSteepnessRight;
                [Tooltip("This field indicates how far up or down the right eyebrow is, compared to its median position.")]
                public float EyebrowUpDownRight;
                [Tooltip("This field indicates how quirked the right eyebrow is, compared to its median quirk.")]
                public float EyebrowQuirkRight;
                [Tooltip("This field indicates how far up or down the left mouth corner is, compared to its median position.")]
                public float MouthCornerUpDownLeft;
                [Tooltip("This field indicates how far in or out the left mouth corner is, compared to its median position.")]
                public float MouthCornerInOutLeft;
                [Tooltip("This field indicates how far up or down the right mouth corner is, compared to its median position.")]
                public float MouthCornerUpDownRight;
                [Tooltip("This field indicates how far in or out the right mouth corner is, compared to its median position.")]
                public float MouthCornerInOutRight;
                [Tooltip("This field indicates how open or closed the mouth is, compared to its median pose.")]
                public float MouthOpen;
                [Tooltip("This field indicates how wide the mouth is, compared to its median pose.")]
                public float MouthWide;
            }

            public OpenSeeData()
            {
                confidence = new float[nPoints];
                points = new Vector2[nPoints];
                points3D = new Vector3[nPoints + 2];
            }

            private Vector3 swapX(Vector3 v)
            {
                v.x = -v.x;
                return v;
            }

            private float readFloat(byte[] b, ref int o)
            {
                float v = System.BitConverter.ToSingle(b, o);
                o += 4;
                return v;
            }

            private Quaternion readQuaternion(byte[] b, ref int o)
            {
                float x = readFloat(b, ref o);
                float y = readFloat(b, ref o);
                float z = readFloat(b, ref o);
                float w = readFloat(b, ref o);
                Quaternion q = new Quaternion(x, y, z, w);
                return q;
            }

            private Vector3 readVector3(byte[] b, ref int o)
            {
                Vector3 v = new Vector3(readFloat(b, ref o), -readFloat(b, ref o), readFloat(b, ref o));
                return v;
            }

            private Vector2 readVector2(byte[] b, ref int o)
            {
                Vector2 v = new Vector2(readFloat(b, ref o), readFloat(b, ref o));
                return v;
            }

            public void readFromPacket(byte[] b, int o)
            {
                time = System.BitConverter.ToDouble(b, o);
                o += 8;
                id = System.BitConverter.ToInt32(b, o);
                o += 4;

                cameraResolution = readVector2(b, ref o);
                rightEyeOpen = readFloat(b, ref o);
                leftEyeOpen = readFloat(b, ref o);

                byte got3D = b[o];
                o++;
                got3DPoints = false;
                if (got3D != 0)
                    got3DPoints = true;

                fit3DError = readFloat(b, ref o);
                rawQuaternion = readQuaternion(b, ref o);
                Quaternion convertedQuaternion = new Quaternion(-rawQuaternion.x, rawQuaternion.y, -rawQuaternion.z, rawQuaternion.w);
                rawEuler = readVector3(b, ref o);

                rotation = rawEuler;
                rotation.z = (rotation.z - 90) % 360;
                rotation.x = -(rotation.x + 180) % 360;

                float x = readFloat(b, ref o);
                float y = readFloat(b, ref o);
                float z = readFloat(b, ref o);
                translation = new Vector3(-y, x, -z);

                for (int i = 0; i < nPoints; i++)
                {
                    confidence[i] = readFloat(b, ref o);
                }

                for (int i = 0; i < nPoints; i++)
                {
                    points[i] = readVector2(b, ref o);
                }

                for (int i = 0; i < nPoints + 2; i++)
                {
                    points3D[i] = readVector3(b, ref o);
                }

                rightGaze = Quaternion.LookRotation(swapX(points3D[66]) - swapX(points3D[68])) * Quaternion.AngleAxis(180, Vector3.right) * Quaternion.AngleAxis(180, Vector3.forward);
                leftGaze = Quaternion.LookRotation(swapX(points3D[67]) - swapX(points3D[69])) * Quaternion.AngleAxis(180, Vector3.right) * Quaternion.AngleAxis(180, Vector3.forward);

                features = new OpenSeeFeatures();
                features.EyeLeft = readFloat(b, ref o);
                features.EyeRight = readFloat(b, ref o);
                features.EyebrowSteepnessLeft = readFloat(b, ref o);
                features.EyebrowUpDownLeft = readFloat(b, ref o);
                features.EyebrowQuirkLeft = readFloat(b, ref o);
                features.EyebrowSteepnessRight = readFloat(b, ref o);
                features.EyebrowUpDownRight = readFloat(b, ref o);
                features.EyebrowQuirkRight = readFloat(b, ref o);
                features.MouthCornerUpDownLeft = readFloat(b, ref o);
                features.MouthCornerInOutLeft = readFloat(b, ref o);
                features.MouthCornerUpDownRight = readFloat(b, ref o);
                features.MouthCornerInOutRight = readFloat(b, ref o);
                features.MouthOpen = readFloat(b, ref o);
                features.MouthWide = readFloat(b, ref o);
            }

            public void readFromPacket(string messageString)
            {
                time = (DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;

                id = 0;
                cameraResolution = new Vector2(0, 0);

                string[] strArray1 = messageString.Split('=');

                if (strArray1.Length >= 2)
                {
                    //blendShapes
                    foreach (string message in strArray1[0].Split('|'))
                    {
                        string[] strArray2 = new string[3];
                        if (message.Contains("&"))
                        {
                            strArray2 = message.Split('&');
                        }
                        else
                        {
                            strArray2 = message.Split('-');
                        }

                        if (strArray2.Length == 2)
                        {
                            BlendshapeMapping[strArray2[0]] = float.Parse(strArray2[1], CultureInfo.InvariantCulture) * 0.01f;
                        }
                    }

                    rightEyeOpen = -BlendshapeMapping["eyeBlink_R"] + 1;
                    leftEyeOpen = -BlendshapeMapping["eyeBlink_L"] + 1;

                    got3DPoints = false;

                    fit3DError = 0;
                    /*rawQuaternion = Quaternion.identity;
                    rawEuler = Vector3.zero;*/

                    foreach (string message in strArray1[1].Split('|'))
                    {
                        string[] strArray2 = message.Split('#');

                        if (strArray2.Length == 2)
                        {
                            string[] commaList = strArray2[1].Split(',');
                            if (strArray2[0] == "head")
                            {
                                rawEuler = rotation = new Vector3(float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), -float.Parse(commaList[2], CultureInfo.InvariantCulture));

                                rawQuaternion = Quaternion.Euler(-rotation.y, -rotation.x, -rotation.z);

                                translation = new Vector3(-float.Parse(commaList[3], CultureInfo.InvariantCulture), float.Parse(commaList[4], CultureInfo.InvariantCulture), -float.Parse(commaList[5], CultureInfo.InvariantCulture));
                            }
                            else if (strArray2[0] == "rightEye")
                            {
                                rightGazeVec = new Vector3(-float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), float.Parse(commaList[2], CultureInfo.InvariantCulture));
                            }
                            else if (strArray2[0] == "leftEye")
                            {
                                leftGazeVec = new Vector3(-float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), float.Parse(commaList[2], CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }

                for (int i = 0; i < nPoints; i++)
                {
                    confidence[i] = 1;
                }

                float browsDown = (BlendshapeMapping["browDown_L"] + BlendshapeMapping["browDown_R"]) / 2.0f;
                float browsUp = (BlendshapeMapping["browOuterUp_L"] + BlendshapeMapping["browOuterUp_R"] + BlendshapeMapping["browInnerUp"]) / 3.0f;

                features = new OpenSeeFeatures();
                features.EyeLeft = 0;
                features.EyeRight = 0;
                features.EyebrowSteepnessLeft = 0;
                features.EyebrowUpDownLeft = browsUp - browsDown;
                features.EyebrowQuirkLeft = 0;
                features.EyebrowSteepnessRight = 0;
                features.EyebrowUpDownRight = browsUp - browsDown;
                features.EyebrowQuirkRight = 0;
                features.MouthCornerUpDownLeft = 0;
                features.MouthCornerInOutLeft = 0;
                features.MouthCornerUpDownRight = 0;
                features.MouthCornerInOutRight = 0;
                features.MouthOpen = BlendshapeMapping["jawOpen"];
                features.MouthWide = 0;
            }

            private Dictionary<string, float> BlendshapeMapping = new Dictionary<string, float>
            {
                {"browInnerUp", 0},
                {"browDown_L", 0},
                {"browDown_R", 0},
                {"browOuterUp_L", 0},
                {"browOuterUp_R", 0},
                {"eyeLookUp_L", 0},
                {"eyeLookUp_R", 0},
                {"eyeLookDown_L", 0},
                {"eyeLookDown_R", 0},
                {"eyeLookIn_L", 0},
                {"eyeLookIn_R", 0},
                {"eyeLookOut_L", 0},
                {"eyeLookOut_R", 0},
                {"eyeBlink_L", 0},
                {"eyeBlink_R", 0},
                {"eyeSquint_L", 0},
                {"eyeSquint_R", 0},
                {"eyeWide_L", 0},
                {"eyeWide_R", 0},
                {"cheekPuff", 0},
                {"cheekSquint_L", 0},
                {"cheekSquint_R", 0},
                {"noseSneer_L", 0},
                {"noseSneer_R", 0},
                {"jawOpen", 0},
                {"jawForward", 0},
                {"jawLeft", 0},
                {"jawRight", 0},
                {"mouthFunnel", 0},
                {"mouthPucker", 0},
                {"mouthLeft", 0},
                {"mouthRight", 0},
                {"mouthRollUpper", 0},
                {"mouthRollLower", 0},
                {"mouthShrugUpper", 0},
                {"mouthShrugLower", 0},
                {"mouthClose", 0},
                {"mouthSmile_L", 0},
                {"mouthSmile_R", 0},
                {"mouthFrown_L", 0},
                {"mouthFrown_R", 0},
                {"mouthDimple_L", 0},
                {"mouthDimple_R", 0},
                {"mouthUpperUp_L", 0},
                {"mouthUpperUp_R", 0},
                {"mouthLowerDown_L", 0},
                {"mouthLowerDown_R", 0},
                {"mouthPress_L", 0},
                {"mouthPress_R", 0},
                {"mouthStretch_L", 0},
                {"mouthStretch_R", 0},
                {"tongueOut", 0},
            };
        }

        private Dictionary<int, OpenSeeData> openSeeDataMap = new();
        private Thread receiveThread = null;
        private volatile bool stopReception = false;

        public OpenSeeData GetOpenSeeData(int faceId)
        {
            if (openSeeDataMap == null)
                return null;
            if (!openSeeDataMap.ContainsKey(faceId))
                return null;
            return openSeeDataMap[faceId];
        }

        void performReceptionOpenSee()
        {
            IPEndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);
            listening = true;
            while (!stopReception)
            {
                try
                {
                    var buffer = udp.Receive(ref senderRemote);
                    receivedPackets++;
                    OpenSeeData newData = new OpenSeeData();

                    newData.readFromPacket(buffer, 0);

                    openSeeDataMap[newData.id] = newData;
                    trackingData = new OpenSeeData[openSeeDataMap.Count];
                    openSeeDataMap.Values.CopyTo(trackingData, 0);
                }
                catch { }
            }
        }

        void performReceptionMediapipe()
        {
            listening = true;
            IPEndPoint senderRemote = new IPEndPoint(IPAddress.Any, 0);
            while (!stopReception)
            {
                try
                {
                    byte[] data = udp.Receive(ref senderRemote);
                    receivedPackets++;
                    var newData = new OpenSeeData();

                    newData.readFromPacket(Encoding.ASCII.GetString(data));

                    openSeeDataMap[newData.id] = newData;
                    trackingData = new OpenSeeData[openSeeDataMap.Count];
                    openSeeDataMap.Values.CopyTo(trackingData, 0);
                }
                catch { }
            }
        }

        void Start()
        {
            stopReception = false;

            var tracker = ApplicationPersistence.AppSettings.Tracker;
            var trackerSettings = ApplicationPersistence.AppSettings.Trackers[tracker];

            if (udp == null)
            {
                if (!IPAddress.TryParse(trackerSettings.UseLocalTracker ? "127.0.0.1" : trackerSettings.ListenAddress, out var ip))
                {
                    UnityEngine.Debug.LogWarning($"Invalid ip {trackerSettings.ListenAddress}");
                    return;
                }

                udp = new UdpClient(new IPEndPoint(ip, trackerSettings.Port));
                udp.Client.ReceiveTimeout = 5000;
            }

            switch (tracker)
            {
                case Tracker.OpenSee:
                    receiveThread = new Thread(() => performReceptionOpenSee());
                    break;
                case Tracker.Mediapipe:
                    receiveThread = new Thread(() => performReceptionMediapipe());
                    break;
            }
            receiveThread.Start();
        }

#if !UNITY_EDITOR
        void Update()
        {
            if (!receiveThread.IsAlive)
            {
                Start();
            }
        }
#endif

        void EndReceiver()
        {
            if (receiveThread != null && receiveThread.IsAlive)
            {
                stopReception = true;
                receiveThread.Join();
            }
            if (udp != null)
            {
                udp.Dispose();
            }
        }

        void OnApplicationQuit()
        {
            EndReceiver();
        }

        void OnDestroy()
        {
            EndReceiver();
        }
    }

}