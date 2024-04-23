using UnityEngine;


public class ModernBase : MonoBehaviour
{

    public bool ShowIcon;
    public bool ShowDescription;

    public GameObject Icon;
    public RectTransform Panel;
    public GameObject Description;

    private void Start()
    {
        Icon.SetActive(ShowIcon);
        if (ShowIcon)
            Panel.offsetMin = new Vector2(53, Panel.offsetMin.y);
        else
            Panel.offsetMin = new Vector2(17, Panel.offsetMin.y);
        Description.SetActive(ShowDescription);
        var rectTransform = GetComponent<RectTransform>();
        if (ShowDescription)
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 69);
        else
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 53);
    }

}
