using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnableOnPlatform : MonoBehaviour
{
    private Selectable Selectable;

    public RuntimePlatform[] Platforms;

    private void Awake()
    {
        Selectable = GetComponent<Selectable>();
        if (!Platforms.Contains(Application.platform))
            Selectable.interactable = false;
    }
}
