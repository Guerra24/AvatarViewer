using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class HandTrackerReceiver : MonoBehaviour
{

    /*private UdpClient udp;
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
        var wrist = ReadVector(segments[2]);
        var thumbTip = ReadVector(segments[3]);
        var indexTip = ReadVector(segments[4]);
        var middleTip = ReadVector(segments[5]);
        var ringTip = ReadVector(segments[6]);
        var pinkyTip = ReadVector(segments[7]);
        var indexMcp = ReadVector(segments[8]);
        var middleMcp = ReadVector(segments[9]);
        return new HandTrackingHand
        {
            Index = index,
            Wrist = wrist,
            ThumbTip = thumbTip,
            IndexTip = indexTip,
            MiddleTip = middleTip,
            RingTip = ringTip,
            PinkyTip = pinkyTip,
            IndexMcp = indexMcp,
            MiddleMcp = middleMcp
        };
    }

    private void performReceptionMediapipe()
    {
        while (!stopReception)
        {
            try
            {
                IPEndPoint remoteEP = null;
                byte[] data = udp.Receive(ref remoteEP);

                var message = Encoding.ASCII.GetString(data);
                var parts = message.Split('|', StringSplitOptions.RemoveEmptyEntries);

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

                var trackingData = new PoseTrackingData();

                var segments = message.Split('#', StringSplitOptions.RemoveEmptyEntries);

                trackingData.LeftWrist = ReadVector(segments[0]);
                trackingData.LeftElbow = ReadVector(segments[1]);
                trackingData.LeftShoukder = ReadVector(segments[2]);
                trackingData.RightWrist = ReadVector(segments[3]);
                trackingData.RightElbow = ReadVector(segments[4]);
                trackingData.RightShoulder = ReadVector(segments[5]);

                PoseTrackingData = trackingData;

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    void Start()
    {
        if (udp == null)
        {
            udp = new UdpClient(49983 + 1);
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
            receiveThread.Join();
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
    }*/
}
/*
public class HandTrackingData
{
    public HandTrackingHand Left;
    public HandTrackingHand Right;
}

public class HandTrackingHand
{
    public int Index;
    public Vector3 Wrist;
    public Vector3 ThumbTip;
    public Vector3 IndexTip;
    public Vector3 MiddleTip;
    public Vector3 RingTip;
    public Vector3 PinkyTip;
    public Vector3 IndexMcp;
    public Vector3 MiddleMcp;
}

public class PoseTrackingData
{
    public Vector3 LeftWrist;
    public Vector3 LeftElbow;
    public Vector3 LeftShoukder;
    public Vector3 RightWrist;
    public Vector3 RightElbow;
    public Vector3 RightShoulder;
}*/