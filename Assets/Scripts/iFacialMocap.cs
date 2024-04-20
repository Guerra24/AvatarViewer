using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Globalization;
using VRM;
using static OpenSeeVRMDriver;

public class iFacialMocap : MonoBehaviour
{
    // broadcast address
    public bool gameStartWithConnect = true;
    public string iOS_IPAddress = "255.255.255.255";
    private UdpClient client;
    private bool StartFlag = true;

    //object names
    /*public string faceObjectGroupName = "";
	public string headBoneName = "";
	public string rightEyeBoneName = "";
	public string leftEyeBoneName = "";
	public string headPositionObjectName = "";*/

    private UdpClient udp;
    private Thread thread;
    //public SkinnedMeshRenderer meshTarget;
    //public List<SkinnedMeshRenderer> meshTargetList;
    public GameObject headObject;
    public GameObject rightEyeObject;
    public GameObject leftEyeObject;
    //public GameObject headPositionObject;

    public Vector3 headPositionReference;

    private Vector3 headPositionCalibration = Vector3.zero;
    private Quaternion headRotationCalibration = Quaternion.identity;

    public OpenSeeBlendShapeProxy openSeeBlendShapeProxy { get; } = new();

    private string messageString = "";
    public int LOCAL_PORT = 49983;

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

    // Start is called
    void StartFunction()
    {
        if (StartFlag == true)
        {
            StartFlag = false;

            //FindGameObjectsInsideUnitySettings();

            //Send to iOS
            if (gameStartWithConnect == true)
            {
                Connect_to_iOS_App();
            }

            //Recieve udp from iOS
            CreateUdpServer();
        }
    }

    void Start()
    {
        StartFunction();
    }

    void CreateUdpServer()
    {
        udp = new UdpClient(LOCAL_PORT);
        udp.Client.ReceiveTimeout = 5;

        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
    }

    IEnumerator WaitProcess(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
    }

    void Connect_to_iOS_App()
    {
        //iFacialMocap
        SendMessage_to_iOSapp("iFacialMocap_sahuasouryya9218sauhuiayeta91555dy3719", 49983);

        //Facemotion3d
        SendMessage_to_iOSapp("FACEMOTION3D_OtherStreaming", 49993);
    }

    void StopStreaming_iOS_App()
    {
        SendMessage_to_iOSapp("StopStreaming_FACEMOTION3D", 49993);
    }

    //iOSアプリに通信開始のメッセージを送信
    //Send a message to the iOS application to start streaming
    void SendMessage_to_iOSapp(string sendMessage, int send_port)
    {
        try
        {
            client = new UdpClient();
            client.Connect(iOS_IPAddress, send_port);
            byte[] dgram = Encoding.UTF8.GetBytes(sendMessage);
            client.Send(dgram, dgram.Length);
            client.Send(dgram, dgram.Length);
            client.Send(dgram, dgram.Length);
            client.Send(dgram, dgram.Length);
            client.Send(dgram, dgram.Length);
        }
        catch { }
    }

    private void LateUpdate()
    {
        try
        {
            SetAnimation_inside_Unity_settings();
        }
        catch
        { }
    }

    void Update()
    {
    }

    //BlendShapeの設定
    //set blendshapes
    /*void SetBlendShapeWeightFromStrArray(string[] strArray2)
    {
        string mappedShapeName = strArray2[0];//.Replace("_R", "Right").Replace("_L", "Left");

        float weight = float.Parse(strArray2[1], CultureInfo.InvariantCulture);

        if (BlendshapeMapping.TryGetValue(mappedShapeName, out var blendShape))
        {
            openSeeBlendShapeProxy.AccumulateValue(blendShape, weight * 0.01f);
        }
        else
        {
            openSeeBlendShapeProxy.AccumulateValue(BlendShapeKey.CreateUnknown(mappedShapeName), weight * 0.01f);
        }
    }*/


    //BlendShapeとボーンの回転の設定
    //set blendshapes & bone rotation
    void SetAnimation_inside_Unity_settings()
    {
        try
        {
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
//                        SetBlendShapeWeightFromStrArray(strArray2);
                    }
                }
                openSeeBlendShapeProxy.Clear();

                openSeeBlendShapeProxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_L), BlendshapeMapping["eyeBlink_L"]);
                openSeeBlendShapeProxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_R), BlendshapeMapping["eyeBlink_R"]);

                float browsDown = (BlendshapeMapping["browDown_L"] + BlendshapeMapping["browDown_R"]) / 2.0f;
                float browsUp = (BlendshapeMapping["browOuterUp_L"] + BlendshapeMapping["browOuterUp_R"] + BlendshapeMapping["browInnerUp"]) / 3.0f;

                openSeeBlendShapeProxy.AccumulateValue(BlendShapeKey.CreateUnknown("Brows down"), browsDown);
                openSeeBlendShapeProxy.AccumulateValue(BlendShapeKey.CreateUnknown("Brows up"), browsUp);

                openSeeBlendShapeProxy.Apply();

                foreach (string message in strArray1[1].Split('|'))
                {
                    string[] strArray2 = message.Split('#');

                    if (strArray2.Length == 2)
                    {
                        string[] commaList = strArray2[1].Split(',');
                        if (strArray2[0] == "head")
                        {
                            //foreach (GameObject headObject in headObjectArray)
                            {
                                var target = Quaternion.Euler(float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), -float.Parse(commaList[2], CultureInfo.InvariantCulture));
                                if (headRotationCalibration == Quaternion.identity)
                                    headRotationCalibration = target;
                                headObject.transform.localRotation = target * Quaternion.Inverse(headRotationCalibration);
                            }

                            //foreach (GameObject headPositionObject in headPositionObjectArray)
                            {

                                var target = new Vector3(-float.Parse(commaList[3], CultureInfo.InvariantCulture), float.Parse(commaList[4], CultureInfo.InvariantCulture), float.Parse(commaList[5], CultureInfo.InvariantCulture));
                                if (headPositionCalibration == Vector3.zero)
                                    headPositionCalibration = target;
                                headObject.transform.localPosition = headPositionReference + (target - headPositionCalibration);
                            }
                        }
                        else if (strArray2[0] == "rightEye")
                        {
                            //foreach (GameObject rightEyeObject in rightEyeObjectArray)
                            {
                                rightEyeObject.transform.localRotation = Quaternion.Euler(float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), float.Parse(commaList[2], CultureInfo.InvariantCulture));
                                //Debug.Log($"{float.Parse(commaList[0], CultureInfo.InvariantCulture)} {-float.Parse(commaList[1], CultureInfo.InvariantCulture)} {float.Parse(commaList[2], CultureInfo.InvariantCulture)}");
                            }
                        }
                        else if (strArray2[0] == "leftEye")
                        {
                            //foreach (GameObject leftEyeObject in leftEyeObjectArray)
                            {
                                leftEyeObject.transform.localRotation = Quaternion.Euler(float.Parse(commaList[0], CultureInfo.InvariantCulture), float.Parse(commaList[1], CultureInfo.InvariantCulture), float.Parse(commaList[2], CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
            }
        }
        catch
        {
        }
    }

    /*void FindGameObjectsInsideUnitySettings()
	{
		//Find BlendShape Objects
		meshTargetList = new List<SkinnedMeshRenderer>();

		GameObject faceObjGrp = GameObject.Find(faceObjectGroupName);
		if (faceObjGrp != null)
		{
			List<GameObject> list = FM3D_and_iFacialMocap_GetAllChildren.GetAll(faceObjGrp);

			foreach (GameObject obj in list)
			{
				meshTarget = obj.GetComponent<SkinnedMeshRenderer>();
				if (meshTarget != null)
				{
					if (HasBlendShapes(meshTarget) == true)
					{
						meshTargetList.Add(meshTarget);
					}
				}
			}
		}

		//Find Bone Objects
		headObjectArray = new List<GameObject>();
		foreach (string headString in headBoneName.Split(','))
		{
			GameObject headObject = GameObject.Find(headString);
			if (headObject != null)
			{
				headObjectArray.Add(headObject);
			}
		}

		rightEyeObjectArray = new List<GameObject>();
		foreach (string rightEyeString in rightEyeBoneName.Split(','))
		{
			GameObject rightEyeObject = GameObject.Find(rightEyeString);
			if (rightEyeObject != null)
			{
				rightEyeObjectArray.Add(rightEyeObject);
			}
		}

		leftEyeObjectArray = new List<GameObject>();
		foreach (string leftEyeString in leftEyeBoneName.Split(','))
		{
			GameObject leftEyeObject = GameObject.Find(leftEyeString);
			if (leftEyeObject != null)
			{
				leftEyeObjectArray.Add(leftEyeObject);
			}
		}

		headPositionObjectArray = new List<GameObject>();
		foreach (string headPositionString in headPositionObjectName.Split(','))
		{
			GameObject headPositionObject = GameObject.Find(headPositionString);
			if (headPositionObject != null)
			{
				headPositionObjectArray.Add(headPositionObject);
			}
		}

	}*/

    void ThreadMethod()
    {
        //Process once every 5ms
        long next = DateTime.Now.Ticks + 50000;
        long now;

        while (true)
        {
            try
            {
                IPEndPoint remoteEP = null;
                byte[] data = udp.Receive(ref remoteEP);
                messageString = Encoding.ASCII.GetString(data);
            }
            catch
            {
            }

            do
            {
                now = DateTime.Now.Ticks;
            }
            while (now < next);
            next += 50000;
        }
    }


    public string GetMessageString()
    {
        return messageString;
    }

    void OnEnable()
    {
        StartFunction();
    }

    void OnDisable()
    {
        try
        {
            OnApplicationQuit();
        }
        catch
        {
        }
    }

    void OnApplicationQuit()
    {
        if (StartFlag == false)
        {
            StartFlag = true;
            StopUDP();
        }
    }


    public void StopUDP()
    {
        if (gameStartWithConnect == true)
        {
            StopStreaming_iOS_App();
        }
        udp.Dispose();
        thread.Abort();
    }

    private bool HasBlendShapes(SkinnedMeshRenderer skin)
    {
        if (!skin.sharedMesh)
        {
            return false;
        }

        if (skin.sharedMesh.blendShapeCount <= 0)
        {
            return false;
        }

        return true;
    }

}
public static class FM3D_and_iFacialMocap_GetAllChildren
{
    public static List<GameObject> GetAll(this GameObject obj)
    {
        List<GameObject> allChildren = new List<GameObject>();
        allChildren.Add(obj);
        GetChildren(obj, ref allChildren);
        return allChildren;
    }

    public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform ob in children)
        {
            allChildren.Add(ob.gameObject);
            GetChildren(ob.gameObject, ref allChildren);
        }
    }
}