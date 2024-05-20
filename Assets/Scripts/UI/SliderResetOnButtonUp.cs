using UnityEngine;
using UnityEngine.UI;

public class SliderResetOnButtonUp : MonoBehaviour
{
    public float ResetValue;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0))
            slider.value = ResetValue;
    }
}
