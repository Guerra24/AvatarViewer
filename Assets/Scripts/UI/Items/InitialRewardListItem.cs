using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Items
{
    public class InitialRewardListItem : RewardListItem<Reward>
    {

        [SerializeField]
        private Button Configure;
        [SerializeField]
        private TMP_Dropdown Type;

        protected override void Awake()
        {
            base.Awake();
            Configure.onClick.AddListener(OnConfigureClick);
            Type.AddOptions(Enum.GetNames(typeof(InitialRewardListItemType)).ToList());
        }

        private void OnConfigureClick()
        {
            var itemList = GetComponentInParent<RewardList>();
            var settings = ApplicationPersistence.AppSettings;

            switch((InitialRewardListItemType)Type.value)
            {
                case InitialRewardListItemType.Item:
                    settings.Rewards[Id] = new ItemReward(Reward);
                    break;
                case InitialRewardListItemType.Camera:
                    settings.Rewards[Id] = new CameraReward(Reward);
                    break;
            }
            itemList.RefreshList();
        }
    }

    public enum InitialRewardListItemType
    {
        Item, Camera
    }
}
