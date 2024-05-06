using System;
using Unity.Mathematics;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    public HandTrackerReceiver HandTrackerReceiver;

    [HideInInspector]
    public Transform LeftShoulder;
    [HideInInspector]
    public Transform LeftElbow;
    [HideInInspector]
    public Transform LeftWrist;

    [HideInInspector]
    public Transform RightShoulder;
    [HideInInspector]
    public Transform RightElbow;
    [HideInInspector]
    public Transform RightWrist;

    [HideInInspector]
    public Transform AvatarLeftShoulder;

    [HideInInspector]
    public Transform AvatarLeftThumbCmc;
    [HideInInspector]
    public Transform AvatarLeftThumbPole;
    [HideInInspector]
    public Transform AvatarLeftThumbTarget;

    [HideInInspector]
    public Transform AvatarLeftIndexMcp;
    [HideInInspector]
    public Transform AvatarLeftIndexPole;
    [HideInInspector]
    public Transform AvatarLeftIndexTarget;

    [HideInInspector]
    public Transform AvatarLeftMiddleMcp;
    [HideInInspector]
    public Transform AvatarLeftMiddlePole;
    [HideInInspector]
    public Transform AvatarLeftMiddleTarget;

    [HideInInspector]
    public Transform AvatarLeftRingMcp;
    [HideInInspector]
    public Transform AvatarLeftRingPole;
    [HideInInspector]
    public Transform AvatarLeftRingTarget;

    [HideInInspector]
    public Transform AvatarLeftPinkyMcp;
    [HideInInspector]
    public Transform AvatarLeftPinkyPole;
    [HideInInspector]
    public Transform AvatarLeftPinkyTarget;
    [HideInInspector]
    public float AvatarLeftPinkyScale = 1.0f;

    [HideInInspector]
    public Transform AvatarRightShoulder;
    /*[HideInInspector]
    public Transform AvatarRightThumbCmc;
    [HideInInspector]
    public Transform AvatarRightIndexMcp;
    [HideInInspector]
    public Transform AvatarRightMiddleMcp;
    [HideInInspector]
    public Transform AvatarRightRingMcp;
    [HideInInspector]
    public Transform AvatarRightPinkyMcp;*/

    private void Awake()
    {
        /*AvatarLeftThumbPole = new GameObject("ltp").transform;
        AvatarLeftIndexPole = new GameObject("lip").transform;
        AvatarLeftMiddlePole = new GameObject("lmp").transform;
        AvatarLeftRingPole = new GameObject("lrp").transform;
        AvatarLeftPinkyPole = new GameObject("lpp").transform;*/
    }

    // Update is called once per frame
    void Update()
    {
        if (HandTrackerReceiver.PoseTrackingData != null)
        {
            var data = HandTrackerReceiver.PoseTrackingData;

            var LeftReference = new Vector3(AvatarLeftShoulder.position.x - data.LeftShoulder.x, AvatarLeftShoulder.position.y - data.LeftShoulder.y, AvatarLeftShoulder.position.z - data.LeftShoulder.z);
            var RightReference = new Vector3(AvatarRightShoulder.position.x - data.RightShoulder.x, AvatarRightShoulder.position.y - data.RightShoulder.y, AvatarRightShoulder.position.z - data.RightShoulder.z);

            /*Debug.DrawLine(LeftReference + data.LeftShoulder, LeftReference + data.LeftElbow, Color.red);
            Debug.DrawLine(LeftReference + data.LeftElbow, LeftReference + data.LeftWrist, Color.blue);

            Debug.DrawLine(RightReference + data.RightShoulder, RightReference + data.RightElbow, Color.red);
            Debug.DrawLine(RightReference + data.RightElbow, RightReference + data.RightWrist, Color.blue);*/

            LeftWrist.position = Vector3.Lerp(LeftWrist.position, LeftReference + data.LeftWrist, 0.2f);
            LeftElbow.position = Vector3.Lerp(LeftElbow.position, LeftReference + data.LeftElbow, 0.2f);
            RightWrist.position = Vector3.Lerp(RightWrist.position, RightReference + data.RightWrist, 0.2f);
            RightElbow.position = Vector3.Lerp(RightElbow.position, RightReference + data.RightElbow, 0.2f);
        }
        if (HandTrackerReceiver.HandTrackingData != null)
        {
            var data = HandTrackerReceiver.HandTrackingData;

            if (data.Left != null)
            {
                var referenceThumb = AvatarLeftThumbCmc.position - data.Left.ThumbCmc;

                /*AvatarLeftThumbTarget.position = Vector3.Lerp(AvatarLeftThumbTarget.position, referenceThumb + data.Left.ThumbIp * Scale, 0.2f);
                AvatarLeftThumbPole.position = Vector3.Lerp(AvatarLeftThumbPole.position, referenceThumb + data.Left.ThumbMcp * Scale, 0.2f);
                Debug.DrawLine(AvatarLeftThumbTarget.position, AvatarLeftThumbPole.position);*/

                Debug.DrawLine(referenceThumb + data.Left.ThumbCmc, referenceThumb + data.Left.ThumbMcp, Color.blue);
                Debug.DrawLine(referenceThumb + data.Left.ThumbMcp, referenceThumb + data.Left.ThumbIp, Color.blue);
                Debug.DrawLine(referenceThumb + data.Left.ThumbIp, referenceThumb + data.Left.ThumbTip, Color.blue);

                /*var referenceIndex = AvatarLeftIndexMcp.position - data.Left.IndexMcp * Scale;

                AvatarLeftIndexTarget.position = Vector3.Lerp(AvatarLeftIndexTarget.position, referenceIndex + data.Left.IndexDip * Scale, 0.2f);
                AvatarLeftIndexPole.position = Vector3.Lerp(AvatarLeftIndexPole.position, referenceIndex + data.Left.IndexPip * Scale, 0.2f);
                Debug.DrawLine(AvatarLeftIndexTarget.position, AvatarLeftIndexPole.position);*/

                /*Debug.DrawLine(referenceIndex + data.Left.IndexMcp, referenceIndex + data.Left.IndexPip, Color.green);
                Debug.DrawLine(referenceIndex + data.Left.IndexPip, referenceIndex + data.Left.IndexDip, Color.green);
                Debug.DrawLine(referenceIndex + data.Left.IndexDip, referenceIndex + data.Left.IndexTip, Color.green);*/

                /*var referencePinky = AvatarLeftPinkyMcp.position - data.Left.PinkyMcp * Scale;

                AvatarLeftPinkyTarget.position = Vector3.Lerp(AvatarLeftPinkyTarget.position, referencePinky + data.Left.PinkyDip * Scale, 0.2f);
                AvatarLeftPinkyPole.position = Vector3.Lerp(AvatarLeftPinkyPole.position, referencePinky + data.Left.PinkyPip * Scale, 0.2f);
                Debug.DrawLine(AvatarLeftPinkyTarget.position, AvatarLeftPinkyPole.position);*/

                /*Debug.DrawLine(referencePinky + data.Left.PinkyMcp, referencePinky + data.Left.PinkyPip, Color.cyan);
                Debug.DrawLine(referencePinky + data.Left.PinkyPip, referencePinky + data.Left.PinkyDip, Color.cyan);
                Debug.DrawLine(referencePinky + data.Left.PinkyDip, referencePinky + data.Left.PinkyTip, Color.cyan);*/

                var forward = data.Left.PinkyMcp - data.Left.Wrist;
                var right = data.Left.IndexMcp - data.Left.PinkyMcp;
                var up = Vector3.Cross(forward, right).normalized;

                LeftWrist.rotation = Quaternion.Lerp(LeftWrist.rotation, Quaternion.LookRotation(right, up), 0.5f);

            }
            if (data.Right != null)
            {
                //RightWrist.localPosition = Reference + data.Right.Wrist;

                var forward = data.Right.PinkyMcp - data.Right.Wrist;
                var right = data.Right.IndexMcp - data.Right.PinkyMcp;
                var up = Vector3.Cross(forward, right).normalized;

                RightWrist.rotation = Quaternion.Lerp(RightWrist.rotation, Quaternion.LookRotation(right, -up), 0.5f);
            }
        }
    }

}
