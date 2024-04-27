using UnityEngine;
using UnityEngine.UI;

public class SliderResetOnButtonUp : MonoBehaviour
{
    public float ResetValue;

    private RectTransform rectTransform;
    private Slider slider;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            slider.value = ResetValue;
        }
    }
}
