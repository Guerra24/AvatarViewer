using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Items
{
    public class PickAvatarRewardListItem : RewardListItem<PickAvatarReward>
    {

        [SerializeField] private TMP_Dropdown Avatar;
        [SerializeField] private Button Add;
        [SerializeField] private RectTransform Choices;
        [SerializeField] private GameObject _templateChoice;

        protected override void Awake()
        {
            base.Awake();
            foreach (var avatar in ApplicationPersistence.AppSettings.Avatars)
                Avatar.options.Add(new GuidDropdownData(avatar.Title, avatar.Guid));
            Avatar.RefreshShownValue();

            Add.onClick.AddListener(OnAddClick);
        }

        private void OnAddClick()
        {
            Reward.Choices.Add(new ChoicePickAvatarReward("", ((GuidDropdownData)Avatar.options[Avatar.value]).guid));
            RefreshList();
        }

        public override void LoadValues(string id)
        {
            base.LoadValues(id);
            RefreshList();
        }

        private void RefreshList()
        {
            for (int i = 0; i < Choices.childCount; i++)
                Destroy(Choices.GetChild(i).gameObject);
            foreach (var choice in Reward.Choices)
            {
                var item = Instantiate(_templateChoice, Choices, false);
                item.transform.Find("Panel/Title").GetComponent<TMP_Text>().text = ApplicationPersistence.AppSettings.Avatars.Find(a => a.Guid == choice.Avatar).Title;
                var text = item.transform.Find("Content/ChoiceText").GetComponent<TMP_InputField>();
                text.text = choice.Text;
                text.onEndEdit.AddListener((value) => choice.Text = value);
                item.transform.Find("Content/Delete").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Reward.Choices.Remove(choice);
                    Destroy(item);
                });
            }
        }
    }

}
