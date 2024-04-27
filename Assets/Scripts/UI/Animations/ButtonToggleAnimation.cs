using UnityEngine;
using UnityEngine.UI;

public class ButtonToggleAnimation : MonoBehaviour
{
    public AnimationEasing Easing;
    public BaseAnimation Animation;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Animation.Easing = Easing;
            Animation.StartAnimation();
        });
    }
}
