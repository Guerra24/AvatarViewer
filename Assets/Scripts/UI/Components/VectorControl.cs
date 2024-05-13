using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarViewer.Ui.Components
{
    public class VectorControl : MonoBehaviour
    {

        [SerializeField] float Speed = 1f;
        [SerializeField] private TMP_InputField AbsoluteX;
        [SerializeField] private TMP_InputField AbsoluteY;
        [SerializeField] private TMP_InputField AbsoluteZ;
        [SerializeField] private Slider RelativeX;
        [SerializeField] private Slider RelativeY;
        [SerializeField] private Slider RelativeZ;

        private Vector3 Vector = Vector3.zero;

        public event VectorControlValueChanged ValueChanged;

        private void Awake()
        {
            AbsoluteX.onValueChanged.AddListener(OnAbsoluteValueChanged);
            AbsoluteY.onValueChanged.AddListener(OnAbsoluteValueChanged);
            AbsoluteZ.onValueChanged.AddListener(OnAbsoluteValueChanged);

            AbsoluteX.onEndEdit.AddListener(OnAbsoluteEndEdit);
            AbsoluteY.onEndEdit.AddListener(OnAbsoluteEndEdit);
            AbsoluteZ.onEndEdit.AddListener(OnAbsoluteEndEdit);
        }

        private void Update()
        {
            if (RelativeX.value != 0)
                AbsoluteX.text = (Vector.x += RelativeX.value * Speed * Time.deltaTime).ToString();
            if (RelativeY.value != 0)
                AbsoluteY.text = (Vector.y += RelativeY.value * Speed * Time.deltaTime).ToString();
            if (RelativeZ.value != 0)
                AbsoluteZ.text = (Vector.z += RelativeZ.value * Speed * Time.deltaTime).ToString();
        }

        private void OnAbsoluteValueChanged(string _)
        {
            ValueChanged?.Invoke(Vector);
        }

        private void OnAbsoluteEndEdit(string _)
        {
            Vector.x = float.Parse(AbsoluteX.text);
            Vector.y = float.Parse(AbsoluteY.text);
            Vector.z = float.Parse(AbsoluteZ.text);
            ValueChanged?.Invoke(Vector);
        }

        public void LoadVector(Vector3 vector)
        {
            Vector = vector;

            AbsoluteX.text = Vector.x.ToString();
            AbsoluteY.text = Vector.y.ToString();
            AbsoluteZ.text = Vector.z.ToString();
        }

    }

    public delegate void VectorControlValueChanged(Vector3 vector);
}
