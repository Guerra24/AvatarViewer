using UnityEngine;
using UnityEngine.UI;

public class ButtonOpenLink : MonoBehaviour
{

    [SerializeField] private string Url;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(Url));
    }
}
