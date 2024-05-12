using TMPro;
using UnityEngine;

namespace AvatarViewer.Ui.Items
{
    public class CameraRewardListItem : RewardListItem<CameraReward>
    {

        [SerializeField]
        private TMP_Dropdown CameraPreset;
        [SerializeField]
        private TMP_InputField Timeout;

        protected override void Awake()
        {
            base.Awake();
            foreach (var preset in ApplicationPersistence.AppSettings.CameraPresets)
                CameraPreset.options.Add(new GuidDropdownData(preset.Value.Name, preset.Key));
            CameraPreset.RefreshShownValue();

            CameraPreset.onValueChanged.AddListener(OnCameraPresetValueChanged);
            Timeout.onEndEdit.AddListener(OnTimeoutEditEnd);
        }

        private void OnCameraPresetValueChanged(int value)
        {
            Reward.CameraPreset = ((GuidDropdownData)CameraPreset.options[value]).guid;
        }

        private void OnTimeoutEditEnd(string value)
        {
            Reward.Timeout = float.Parse(value);
        }

        public override void LoadValues(string id)
        {
            base.LoadValues(id);

            CameraPreset.value = CameraPreset.options.FindIndex(g => ((GuidDropdownData)g).guid == Reward.CameraPreset);
            Timeout.text = Reward.Timeout.ToString();
        }
    }

}
