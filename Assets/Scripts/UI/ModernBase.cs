using UnityEngine;
using UnityEngine.UI;


public class ModernBase : MonoBehaviour
{

    public bool ShowIcon;
    public bool ShowDescription;

    public GameObject Icon;
    public RectTransform Panel;
    public GameObject Description;

    private LayoutElement LayoutElement;

    private void Awake()
    {
        LayoutElement = GetComponent<LayoutElement>();
    }

    private void Start()
    {
        Icon.SetActive(ShowIcon);
        if (ShowIcon)
            Panel.offsetMin = new Vector2(53, Panel.offsetMin.y);
        else
            Panel.offsetMin = new Vector2(17, Panel.offsetMin.y);
        Description.SetActive(ShowDescription);
        //var rectTransform = GetComponent<RectTransform>();
        if (ShowDescription)
        {
            LayoutElement.preferredHeight = 69;
            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 69);
        }
        else
        {
            LayoutElement.preferredHeight = 53;
            //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 53);
        }
    }

}
