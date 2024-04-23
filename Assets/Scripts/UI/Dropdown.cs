using System;
using TMPro;
using UnityEngine;

namespace AvatarViewer.Ui
{

    public class IdDropdownData : TMP_Dropdown.OptionData
    {
        [SerializeField]
        private int m_Id;

        public int id
        {
            get => m_Id;
            set { m_Id = value; }
        }

        public IdDropdownData(string text, int id)
        {
            this.text = text;
            this.id = id;
        }
    }

    public class GuidDropdownData : TMP_Dropdown.OptionData
    {
        [SerializeField]
        private Guid m_Guid;

        public Guid guid
        {
            get => m_Guid;
            set { m_Guid = value; }
        }

        public GuidDropdownData(string text, Guid guid)
        {
            this.text = text;
            this.guid = guid;
        }
    }
}
