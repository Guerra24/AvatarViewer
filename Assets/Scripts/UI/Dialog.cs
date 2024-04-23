using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    private TMP_Text Title;
    private TMP_Text Content;
    private Button Ok;
    private Button Cancel;

    private void Awake()
    {
        Title = transform.Find("Title").GetComponent<TMP_Text>();
        Content = transform.Find("Content").GetComponent<TMP_Text>();
        Ok = transform.Find("Actions/Ok").GetComponent<Button>();
        Cancel = transform.Find("Actions/Cancel").GetComponent<Button>();
        Ok.onClick.AddListener(() => Destroy(gameObject.transform.parent.gameObject));
        Cancel.onClick.AddListener(() => Destroy(gameObject.transform.parent.gameObject));
    }

    public void SetTitle(string title)
    {
        Title.text = title;
    }

    public void SetContent(string content)
    {
        Content.text = content;
    }

    public void SetOnOkAction(UnityAction action)
    {
        Ok.onClick.AddListener(action);
    }

    public void SetOnCancelAction(UnityAction action)
    {
        Cancel.onClick.AddListener(action);
    }
}
