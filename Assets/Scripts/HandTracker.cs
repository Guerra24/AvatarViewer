using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    /*public HandTrackerReceiver HandTrackerReceiver;

    public Transform LeftWrist;
    public Transform LeftThumb;
    public Transform LeftIndex;
    public Transform LeftMiddle;
    public Transform LeftRing;
    public Transform LeftPinky;
    public Transform RightWrist;

    public Vector3 Reference;

    // Update is called once per frame
    void Update()
    {
        if (HandTrackerReceiver.PoseTrackingData != null)
        {
            var data = HandTrackerReceiver.PoseTrackingData;

            LeftWrist.localPosition = Reference + data.LeftWrist;
            RightWrist.localPosition = Reference + data.RightWrist;
        }
        if (HandTrackerReceiver.HandTrackingData != null)
        {
            var data = HandTrackerReceiver.HandTrackingData;

            if (data.Left != null)
            {
                LeftWrist.localPosition = Reference + data.Left.Wrist;
                LeftThumb.localPosition = Reference + data.Left.Wrist + data.Left.ThumbTip;
                LeftIndex.localPosition = Reference + data.Left.Wrist + data.Left.IndexTip;
                LeftMiddle.localPosition = Reference + data.Left.Wrist + data.Left.MiddleTip;
                LeftRing.localPosition = Reference + data.Left.Wrist + data.Left.RingTip;
                LeftPinky.localPosition = Reference + data.Left.Wrist + data.Left.PinkyTip;

                {
                    var wristTransform = data.Left.Wrist;
                    var indexFinger = data.Left.IndexMcp;
                    var middleFinger = data.Left.MiddleMcp;

                    var vectorToMiddle = middleFinger - wristTransform;
                    var vectorToIndex = indexFinger - wristTransform;
                    //to get ortho vector of middle finger from index finger
                    Vector3.OrthoNormalize(ref vectorToMiddle, ref vectorToIndex);

                    //vector normal to wrist
                    Vector3 normalVector = Vector3.Cross(vectorToIndex, vectorToMiddle);

                    //Debug.DrawRay(wristTransform.position, normalVector, Color.white);
                    //Debug.DrawRay(wristTransform.position, vectorToIndex, Color.yellow);
                    LeftWrist.localRotation = Quaternion.LookRotation(normalVector, vectorToIndex);
                }
            }
            if (data.Right != null)
            {
                RightWrist.localPosition = Reference + data.Right.Wrist;
            }
        }
    }*/

}
