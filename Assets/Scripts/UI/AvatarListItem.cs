using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AvatarViewer.Ui
{
    public class AvatarListItem : Selectable
    {

        public Avatar Avatar { get; set; }

        public override void OnSelect(BaseEventData eventData)
        {
            Debug.Log(Avatar.Title);
            ApplicationState.CurrentAvatar = Avatar;
        }

        public override bool Equals(object obj)
        {
            if (obj is AvatarListItem other)
                return Avatar.Guid.Equals(other.Avatar.Guid);
            return false;
        }

        public override int GetHashCode()
        {
            return Avatar.Guid.GetHashCode();
        }

    }
}
