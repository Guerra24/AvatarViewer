using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    /*
    public class LoadValue : MonoBehaviour
    {
        public SettingsMapper SettingsMapper;

        public string Property;

        public FieldType Type;

        public void Start()
        {
            switch (Type)
            {
                case FieldType.Text:
                    var inputField = GetComponent<TMP_InputField>();

                    inputField.text = SettingsMapper.GetProperty<string>(Property);
                    break;
                case FieldType.Boolean:
                    var toggleField = GetComponent<Toggle>();

                    toggleField.isOn = SettingsMapper.GetProperty<bool>(Property);
                    break;
                case FieldType.DropdownInteger:
                    var dropdownField = GetComponent<TMP_Dropdown>();

                    var value = SettingsMapper.GetProperty<int>(Property);

                    dropdownField.value = dropdownField.options.FindIndex(o => ((IdDropdownData)o).id == value);
                    break;
                case FieldType.DropdownIndex:
                    dropdownField = GetComponent<TMP_Dropdown>();

                    dropdownField.value = SettingsMapper.GetProperty<int>(Property);
                    break;
            }
        }
    }

    public enum FieldType
    {
        Text, Boolean, DropdownInteger, DropdownIndex
    }
    */
}