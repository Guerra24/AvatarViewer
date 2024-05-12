using TMPro;
using UnityEngine;

namespace AvatarViewer.Ui.Items
{
    public class AvatarRewardListItem : RewardListItem<AvatarReward>
    {

        [SerializeField]
        private TMP_Dropdown Avatar;

        protected override void Awake()
        {
            base.Awake();
            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars)
                Avatar.options.Add(new GuidDropdownData(avatar.Title, avatar.Guid));
            Avatar.RefreshShownValue();

            Avatar.onValueChanged.AddListener(OnAvatarValueChanged);
        }

        private void OnAvatarValueChanged(int value)
        {
            Reward.Avatar = ((GuidDropdownData)Avatar.options[value]).guid;
        }

        public override void LoadValues(string id)
        {
            base.LoadValues(id);

            Avatar.value = Avatar.options.FindIndex(g => ((GuidDropdownData)g).guid == Reward.Avatar);
        }
    }

}
