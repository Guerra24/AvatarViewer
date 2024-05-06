using System.Collections;
using AvatarViewer;
using AvatarViewer.Twitch;
using AvatarViewer.Ui;
using TwitchLib.PubSub.Events;
using UnityEngine;

public class CameraRewardController : MonoBehaviour
{
    [SerializeField]
    private Camera Camera;
    [SerializeField]
    private MainController MainController;

    private TwitchController twitchController;

    private bool running = false;

    private void Awake()
    {
        twitchController = GameObject.Find("TwitchController").GetComponent<TwitchController>();
        twitchController.PubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
    }

    private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
    {
        if (ApplicationPersistence.AppSettings.Rewards.TryGetValue(e.RewardRedeemed.Redemption.Reward.Id, out var r) && r is CameraReward reward)
        {
            if (running)
                return;
            running = true;
            MainThreadDispatcher.AddOnUpdate(() => StartCoroutine(ApplyPreset(reward)));
        }
    }

    private IEnumerator ApplyPreset(CameraReward reward)
    {
        Apply(ApplicationPersistence.AppSettings.CameraPresets[reward.CameraPreset]);

        yield return new WaitForSeconds(reward.Timeout);

        Apply(MainController.CurrentCameraPreset);
        running = false;
    }

    private void Apply(CameraPreset preset)
    {
        var angles = preset.Rotation.eulerAngles;
        var rotx = angles.x > 180 ? angles.x - 360 : angles.x;
        var roty = angles.y > 180 ? angles.y - 360 : angles.y;
        var rotz = angles.z > 180 ? angles.z - 360 : angles.z;
        Camera.transform.localRotation = Quaternion.Euler(rotx, roty, rotz);

        if (preset.Absolute)
            Camera.transform.position = preset.Position;
        else
            Camera.transform.localPosition = preset.Position;

        float _1OverAspect = 1f / Camera.aspect;
        Camera.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(preset.FOV * Mathf.Deg2Rad * 0.5f) * _1OverAspect) * Mathf.Rad2Deg;
    }

    private void OnDestroy()
    {
        twitchController.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
    }

    private void OnApplicationQuit()
    {
        twitchController.PubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
    }
}
