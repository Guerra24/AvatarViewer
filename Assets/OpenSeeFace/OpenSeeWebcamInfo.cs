using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
#if UNITY_STANDALONE_LINUX
using Iot.Device.Media;
#endif
using UnityEngine;

namespace OpenSee
{

    [Serializable]
    public enum OpenSeeWebcamType { Unknown = -1, DirectShow, Blackmagic };

    [Serializable]
    public enum OpenSeeWebcamFormat { Any = 0, Unknown = 1, ARGB = 100, XRGB, I420 = 200, NV12, YV12, Y800, YVYU = 300, YUY2, UYVY, HDYC, MJPEG = 400, H264 };

    [Serializable]
    public struct OpenSeeWebcamCapability
    {
        public int id;

#if UNITY_STANDALONE_WIN
        // DirectShow
        public int minCX;
        public int minCY;
        public int maxCX;
        public int maxCY;
        public int granularityCX;
        public int granularityCY;
        public OpenSeeWebcamFormat format;
#endif
#if UNITY_STANDALONE_LINUX
        public uint minCX;
        public uint minCY;
        public uint maxCX;
        public uint maxCY;
        public VideoPixelFormat format;
#endif

        public int minInterval;
        public int maxInterval;
        public int rating;

#if UNITY_STANDALONE_WIN
        // Blackmagic
        public int bmTimescale;
        public int bmFrameduration;
        public int bmModecode;
#endif

        public string GetPrettyCapability()
        {
            float fps = 10000000f / (float)minInterval;
            return maxCX + "x" + maxCY + ", " + fps.ToString("0.##") + "fps (" + format.ToString() + ")";
        }
    }

    [Serializable]
    public class OpenSeeWebcam
    {
        public OpenSeeWebcamType type = OpenSeeWebcamType.Unknown;
        public int id;
        public string name;
#if UNITY_STANDALONE_WIN

        // DirectShow
        public string path;

        // Blackmagic
        public long bmId;
        public int bmFlags;
#endif
        // General
        public OpenSeeWebcamCapability[] caps;

        public List<string> prettyCaps = null;

        private List<Tuple<OpenSeeWebcamCapability, int>> splitCaps = null;

        private int CompareCaps(OpenSeeWebcamCapability a, OpenSeeWebcamCapability b)
        {
            if (a.rating < b.rating)
                return -1;
            if (a.rating > b.rating)
                return 1;
            if (a.minCX * a.minCY > b.minCX * b.minCY)
                return -1;
            if (a.minCX * a.minCY < b.minCX * b.minCY)
                return 1;
            if (a.minInterval < b.minInterval)
                return -1;
            if (a.minInterval > b.minInterval)
                return 1;
            return 0;
        }

        public List<string> GetPrettyCapabilities()
        {
            if (splitCaps != null && prettyCaps != null)
                return prettyCaps;

            splitCaps = new List<Tuple<OpenSeeWebcamCapability, int>>();

            for (int i = 0; i < caps.Length; i++)
            {
                if (caps[i].minCX == caps[i].maxCX && caps[i].minCY == caps[i].maxCY)
                    splitCaps.Add(new Tuple<OpenSeeWebcamCapability, int>(caps[i], i));
                else
                {
                    OpenSeeWebcamCapability min = caps[i];
                    OpenSeeWebcamCapability max = caps[i];
                    min.maxCX = min.minCX;
                    min.maxCY = min.minCY;
                    max.minCX = max.maxCX;
                    max.minCY = max.maxCY;
                    splitCaps.Add(new Tuple<OpenSeeWebcamCapability, int>(min, i));
                    splitCaps.Add(new Tuple<OpenSeeWebcamCapability, int>(max, i));
                }
            }

            splitCaps.Sort((a, b) => CompareCaps(a.Item1, b.Item1));

            prettyCaps = new List<string>();
            if (type == OpenSeeWebcamType.DirectShow)
                prettyCaps.Add("Default settings");
            foreach (var cap in splitCaps)
            {
                prettyCaps.Add(cap.Item1.GetPrettyCapability());
            }

            return prettyCaps;
        }

    }

    [DefaultExecutionOrder(-50)]
    public class OpenSeeWebcamInfo : MonoBehaviour
    {
#if UNITY_STANDALONE_WIN
        #region DllImport
        // DirectShow
        [DllImport("dshowcapture_x64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "create_capture")]
        private static extern System.IntPtr create_capture_x64();

        [DllImport("dshowcapture_x64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json_length")]
        private static extern int get_json_length_x64(System.IntPtr cap);

        [DllImport("dshowcapture_x64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json")]
        private static extern void get_json_x64(System.IntPtr cap, [Out] StringBuilder namebuffer, int bufferlength);

        [DllImport("dshowcapture_x64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "destroy_capture")]
        private static extern void destroy_capture_x64(System.IntPtr cap);

        [DllImport("dshowcapture_x86", CallingConvention = CallingConvention.Cdecl, EntryPoint = "create_capture")]
        private static extern System.IntPtr create_capture_x86();

        [DllImport("dshowcapture_x86", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json_length")]
        private static extern int get_json_length_x86(System.IntPtr cap);

        [DllImport("dshowcapture_x86", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json")]
        private static extern void get_json_x86(System.IntPtr cap, [Out] StringBuilder namebuffer, int bufferlength);

        [DllImport("dshowcapture_x86", CallingConvention = CallingConvention.Cdecl, EntryPoint = "destroy_capture")]
        private static extern void destroy_capture_x86(System.IntPtr cap);

        // Blackmagic
        [DllImport("libminibmcapture64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json_length")]
        private static extern int bm_get_json_length_x64();

        [DllImport("libminibmcapture64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json")]
        private static extern void bm_get_json_x64([Out] StringBuilder namebuffer, int bufferlength);

        [DllImport("libminibmcapture32", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json_length")]
        private static extern int bm_get_json_length_x86();

        [DllImport("libminibmcapture32", CallingConvention = CallingConvention.Cdecl, EntryPoint = "get_json")]
        private static extern void bm_get_json_x86([Out] StringBuilder namebuffer, int bufferlength);
        #endregion
#endif

        public List<OpenSeeWebcam> cameras;
        public bool dumpJson = false;

        private static bool dumpJsonStatic = false;
#if UNITY_STANDALONE_WIN
        static private string ListCameraDetails_x64()
        {
            System.IntPtr cap = create_capture_x64();
            int length = get_json_length_x64(cap);
            StringBuilder buffer = new StringBuilder(length);
            get_json_x64(cap, buffer, length);
            destroy_capture_x64(cap);
            return buffer.ToString();
        }

        static private string ListCameraDetails_x86()
        {
            System.IntPtr cap = create_capture_x86();
            int length = get_json_length_x86(cap);
            StringBuilder buffer = new StringBuilder(length);
            get_json_x86(cap, buffer, length);
            destroy_capture_x86(cap);
            return buffer.ToString();
        }

        static private string ListBlackMagicDetails_x64()
        {
            int length = bm_get_json_length_x64();
            StringBuilder buffer = new StringBuilder(length);
            bm_get_json_x64(buffer, length);
            return buffer.ToString();
        }

        static private string ListBlackMagicDetails_x86()
        {
            int length = bm_get_json_length_x86();
            StringBuilder buffer = new StringBuilder(length);
            bm_get_json_x86(buffer, length);
            return buffer.ToString();
        }
#endif

        static public List<OpenSeeWebcam> ListCameraDetails(bool includeBlackMagic)
        {
#if UNITY_STANDALONE_WIN
            string jsonData = null;
            string bmJsonData = null;
            if (Environment.Is64BitProcess)
            {
                jsonData = ListCameraDetails_x64();
                if (includeBlackMagic)
                    bmJsonData = ListBlackMagicDetails_x64();
            }
            else
            {
                jsonData = ListCameraDetails_x86();
                if (includeBlackMagic)
                    bmJsonData = ListBlackMagicDetails_x86();
            }
            if (dumpJsonStatic)
            {
                UnityEngine.Debug.Log("Camera JSON: " + jsonData);
                if (includeBlackMagic)
                    UnityEngine.Debug.Log("Blackmagic JSON: " + bmJsonData);
            }
            //List<OpenSeeWebcam> details = new List<OpenSeeWebcam>();
            //var parsed = JsonParser.Parse(jsonData);
            //parsed.Deserialize(ref details);
            List<OpenSeeWebcam> details = JsonConvert.DeserializeObject<List<OpenSeeWebcam>>(jsonData);
            foreach (var cam in details)
                cam.type = OpenSeeWebcamType.DirectShow;
            if (includeBlackMagic)
            {
                //List<OpenSeeWebcam> bmDetails = new List<OpenSeeWebcam>();
                //parsed = JsonParser.Parse(bmJsonData);
                //parsed.Deserialize(ref bmDetails);
                List<OpenSeeWebcam> bmDetails = JsonConvert.DeserializeObject<List<OpenSeeWebcam>>(bmJsonData);
                foreach (var cam in bmDetails)
                    cam.type = OpenSeeWebcamType.Blackmagic;
                details.AddRange(bmDetails);
            }
#endif
#if UNITY_STANDALONE_LINUX
            List<OpenSeeWebcam> details = new();
            for (int i = 0; i < 10; i++)
            {
                VideoConnectionSettings settings = new(i, (640, 480), VideoPixelFormat.JPEG);
                using var device = VideoDevice.Create(settings);
                try
                {
                    var caps = new List<OpenSeeWebcamCapability>();
                    foreach (var pixelFormats in device.GetSupportedPixelFormats())
                        foreach (var resolution in device.GetPixelFormatResolutions(pixelFormats))
                            caps.Add(new OpenSeeWebcamCapability
                            {
                                format = pixelFormats,
                                minCX = resolution.MinWidth,
                                minCY = resolution.MinHeight,
                                maxCX = resolution.MaxWidth,
                                maxCY = resolution.MaxHeight
                            });
                    var camera = new OpenSeeWebcam();
                    camera.id = i;
                    camera.caps = caps.ToArray();
                    camera.name = $"{device.DevicePath}{device.Settings.BusId}";
                    details.Add(camera);
                }
                catch (IOException e)
                {
                    Debug.LogException(e);
                    break;
                }
            }
#endif
            return details;
        }

        void Start()
        {
            Initialize(false);
        }

        public void Initialize(bool includeBlackMagic)
        {
            dumpJsonStatic = dumpJson;
            cameras = ListCameraDetails(includeBlackMagic);
            foreach (var camera in cameras)
                camera.GetPrettyCapabilities();
        }

        public static void AOTCall()
        {
            /*GenericDeserializer<JsonValue, OpenSeeWebcamCapability[]>.GenericArrayDeserializer<OpenSeeWebcamCapability>(default(ListTreeNode<JsonValue>));
            JsonObjectValidator.GenericDeserializer<JsonValue, OpenSeeWebcamCapability>.DeserializeField<System.Int32>(default(JsonSchema), default(ListTreeNode<JsonValue>));
            JsonObjectValidator.GenericDeserializer<JsonValue, OpenSeeWebcamCapability>.DeserializeField<OpenSeeWebcamFormat>(default(JsonSchema), default(ListTreeNode<JsonValue>));
            GenericDeserializer<JsonValue, OpenSeeWebcam[]>.GenericArrayDeserializer<OpenSeeWebcam>(default(ListTreeNode<JsonValue>));*/
            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }
    }
}