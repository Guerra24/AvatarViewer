using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AvatarViewer;
using UnityEngine;

public class HandTrackerReceiver : MonoBehaviour
{

    private UdpClient udp;
    private Thread receiveThread = null;
    private volatile bool stopReception = false;
    public volatile HandTrackingData HandTrackingData;
    public volatile PoseTrackingData PoseTrackingData;

    private Vector3 ReadVector(string segment)
    {
        var fields = segment.Split(',');
        return new Vector3(float.Parse(fields[0], CultureInfo.InvariantCulture), float.Parse(fields[1], CultureInfo.InvariantCulture), float.Parse(fields[2], CultureInfo.InvariantCulture));
    }

    private HandTrackingHand ProcessSegment(string[] segments)
    {
        int index = int.Parse(segments[0], CultureInfo.InvariantCulture);
        string categoryName = segments[1];
        return new HandTrackingHand
        {
            Index = index,
            Wrist = ReadVector(segments[2]),
            ThumbCmc = ReadVector(segments[3]),
            ThumbMcp = ReadVector(segments[4]),
            ThumbIp = ReadVector(segments[5]),
            ThumbTip = ReadVector(segments[6]),
            IndexMcp = ReadVector(segments[7]),
            IndexPip = ReadVector(segments[8]),
            IndexDip = ReadVector(segments[9]),
            IndexTip = ReadVector(segments[10]),
            MiddleMcp = ReadVector(segments[11]),
            MiddlePip = ReadVector(segments[12]),
            MiddleDip = ReadVector(segments[13]),
            MiddleTip = ReadVector(segments[14]),
            RingMcp = ReadVector(segments[15]),
            RingPip = ReadVector(segments[16]),
            RingDip = ReadVector(segments[17]),
            RingTip = ReadVector(segments[18]),
            PinkyMcp = ReadVector(segments[19]),
            PinkyPip = ReadVector(segments[20]),
            PinkyDip = ReadVector(segments[21]),
            PinkyTip = ReadVector(segments[22]),
        };
    }

    private void performReceptionMediapipe()
    {
        var senderRemote = new IPEndPoint(IPAddress.Any, 0);
        while (!stopReception)
        {
            try
            {
                byte[] data = udp.Receive(ref senderRemote);

                var packet = Encoding.ASCII.GetString(data).Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (packet[0] == "hand")
                {
                    var parts = packet[1].Split('|', StringSplitOptions.RemoveEmptyEntries);

                     var trackingData = new HandTrackingData();

                     if (parts.Length > 0)
                     {
                         var hand = ProcessSegment(parts[0].Split('#'));
                         if (hand.Index == 1)
                             trackingData.Left = hand;
                         else
                             trackingData.Right = hand;
                     }
                     if (parts.Length > 1)
                     {
                         var hand = ProcessSegment(parts[1].Split('#'));
                         if (hand.Index == 1)
                             trackingData.Left = hand;
                         else
                             trackingData.Right = hand;
                     }
                     HandTrackingData = trackingData;
                }
                else if (packet[0] == "body")
                {
                    var trackingData = new PoseTrackingData();

                    var segments = packet[1].Split('#', StringSplitOptions.RemoveEmptyEntries);

                    trackingData.RightWrist = ReadVector(segments[0]);
                    trackingData.RightElbow = ReadVector(segments[1]);
                    trackingData.RightShoulder = ReadVector(segments[2]);
                    trackingData.LeftWrist = ReadVector(segments[3]);
                    trackingData.LeftElbow = ReadVector(segments[4]);
                    trackingData.LeftShoulder = ReadVector(segments[5]);

                    PoseTrackingData = trackingData;
                }
            }
            catch { }
        }
    }

    void Start()
    {
        if (udp == null)
        {
            var tracker = ApplicationPersistence.AppSettings.Tracker;
            var trackerSettings = ApplicationPersistence.AppSettings.Trackers[tracker];

            if (!IPAddress.TryParse(trackerSettings.UseLocalTracker ? "127.0.0.1" : trackerSettings.ListenAddress, out var ip))
            {
                Debug.LogWarning($"Invalid ip {trackerSettings.ListenAddress}");
                return;
            }

            udp = new UdpClient(new IPEndPoint(ip, trackerSettings.Port + 1));
            udp.Client.ReceiveTimeout = 5000;
        }
        stopReception = false;
        receiveThread = new Thread(() => performReceptionMediapipe());
        receiveThread.Start();
    }

    void Update()
    {
        if (receiveThread != null && !receiveThread.IsAlive)
        {
            Start();
        }
    }

    void EndReceiver()
    {
        if (receiveThread != null)
        {
            stopReception = true;
            receiveThread.Interrupt();
        }
        if (udp != null)
            udp.Dispose();
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

public class HandTrackingData
{
    public HandTrackingHand Left;
    public HandTrackingHand Right;
}

public class HandTrackingHand
{
    public int Index;
    public Vector3 Wrist;
    public Vector3 ThumbCmc;
    public Vector3 ThumbMcp;
    public Vector3 ThumbIp;
    public Vector3 ThumbTip;
    public Vector3 IndexMcp;
    public Vector3 IndexPip;
    public Vector3 IndexDip;
    public Vector3 IndexTip;
    public Vector3 MiddleMcp;
    public Vector3 MiddlePip;
    public Vector3 MiddleDip;
    public Vector3 MiddleTip;
    public Vector3 RingMcp;
    public Vector3 RingPip;
    public Vector3 RingDip;
    public Vector3 RingTip;
    public Vector3 PinkyMcp;
    public Vector3 PinkyPip;
    public Vector3 PinkyDip;
    public Vector3 PinkyTip;
}

public class PoseTrackingData
{
    public Vector3 LeftWrist;
    public Vector3 LeftElbow;
    public Vector3 LeftShoulder;
    public Vector3 RightWrist;
    public Vector3 RightElbow;
    public Vector3 RightShoulder;

}