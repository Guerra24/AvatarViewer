using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AvatarViewer
{
    public static class FloatExtensions
    {

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

    }

    public static class UIExtensions
    {
        public static void SetupSlider(this GameObject root, UnityAction<float> sliderEvent)
        {
            var text = root.transform.Find("Content/Text").gameObject.GetComponent<TMP_Text>();
            var slider = root.transform.Find("Content/Slider").gameObject.GetComponent<Slider>();
            slider.onValueChanged.AddListener(sliderEvent);
            slider.onValueChanged.AddListener((value) => text.text = value.ToString("n2"));
        }

        public static void LoadSlider(this GameObject root, float value)
        {
            root.transform.Find("Content/Slider").gameObject.GetComponent<Slider>().value = value;
            root.transform.Find("Content/Text").gameObject.GetComponent<TMP_Text>().text = value.ToString("n2");
        }
    }

    public static class StringExtensions
    {
        public static string AsFormat(this string format, params object[] args) => string.Format(format, args);
    }
}
