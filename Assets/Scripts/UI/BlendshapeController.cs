using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN
using UnityRawInput;
#endif

namespace AvatarViewer.UI
{
    public class BlendshapeController : MonoBehaviour
    {

        public AvatarBlendshape AvatarBlendshape;

        public TMP_Text Title;
        public Toggle Enable;
        public Toggle Base;
        public Toggle Additive;
        public Toggle Hold;
        public Toggle Toggle;
        public Button Hotkey;
        public Slider Transition;
        public TMP_Text TransitionValue;

        private bool registerHotkey, cancel, escapeWasPressed;
#if UNITY_STANDALONE_WIN
        private List<RawKey> keys = new();
#else
        private List<int> keys = new();
#endif
        public void Awake()
        {
            Enable.onValueChanged.AddListener(OnEnableChanged);
            Base.onValueChanged.AddListener(OnBaseToggled);
            Hold.onValueChanged.AddListener(OnHoldToggled);
            Transition.onValueChanged.AddListener(OnTransitionCHanged);
            Hotkey.onClick.AddListener(ChangeHotkey);
        }

        public void Update()
        {
            if (!registerHotkey)
                return;
#if UNITY_STANDALONE_WIN
            var pressedKeys = RawInput.PressedKeys.ToList();
            pressedKeys.Remove(RawKey.Escape);
            pressedKeys.Remove(RawKey.Return);

            if (pressedKeys.Count >= keys.Count)
            {
                keys = pressedKeys;
                cancel = false;
            }

            if (RawInput.IsKeyDown(RawKey.Return))
            {
                AvatarBlendshape.Hotkey.Clear();
                AvatarBlendshape.Hotkey.AddRange(keys);
                registerHotkey = false;
            }
            if (RawInput.IsKeyDown(RawKey.Escape) && !escapeWasPressed)
            {
                escapeWasPressed = true;
                if (keys.Count > 0 && !cancel)
                {
                    keys.Clear();
                    cancel = true;
                }
                else
                {
                    registerHotkey = false;
                    cancel = false;
                }
            }
            if (!RawInput.IsKeyDown(RawKey.Escape) && escapeWasPressed)
                escapeWasPressed = false;
#endif
            Hotkey.GetComponentInChildren<TMP_Text>().text = string.Concat(keys.Select(k => k.ToString() + "+").ToList()).TrimEnd('+');
        }

        public void LoadValues(string name)
        {
            Title.text = name;
            Enable.isOn = AvatarBlendshape.Enabled;
            Base.isOn = AvatarBlendshape.Type == AvatarBlendshapeType.Base;
            Additive.isOn = AvatarBlendshape.Type == AvatarBlendshapeType.Additive;
            Hold.isOn = AvatarBlendshape.Mode == AvatarBlendshapeMode.Hold;
            Toggle.isOn = AvatarBlendshape.Mode == AvatarBlendshapeMode.Toggle;
            Hotkey.GetComponentInChildren<TMP_Text>().text = string.Concat(AvatarBlendshape.Hotkey.Select(k => k.ToString() + "+").ToList()).TrimEnd('+');
            Transition.value = AvatarBlendshape.Transition;
        }

        private void OnEnableChanged(bool state)
        {
            AvatarBlendshape.Enabled = state;
        }

        private void OnBaseToggled(bool state)
        {
            AvatarBlendshape.Type = state ? AvatarBlendshapeType.Base : AvatarBlendshapeType.Additive;
        }

        private void OnHoldToggled(bool state)
        {
            AvatarBlendshape.Mode = state ? AvatarBlendshapeMode.Hold : AvatarBlendshapeMode.Toggle;
        }

        private void ChangeHotkey()
        {
            registerHotkey = true;
        }

        private void OnTransitionCHanged(float value)
        {
            AvatarBlendshape.Transition = value;
            TransitionValue.text = $"{value}ms";
        }

    }
}
