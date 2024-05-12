using UnityEngine;
using UnityEngine.InputSystem;
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
        if (!Mouse.current.leftButton.isPressed)
        {
            slider.value = ResetValue;
        }
    }
}
