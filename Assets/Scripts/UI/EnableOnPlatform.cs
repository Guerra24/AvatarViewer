using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class EnableOnPlatform : MonoBehaviour
{

    public RuntimePlatform[] Platforms;

    private void Awake()
    {
        if (!Platforms.Contains(Application.platform))
            GetComponent<Selectable>().interactable = false;
    }
}
