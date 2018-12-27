using System;
using UnityEngine.UI;

namespace UnityCommon
{
    public class ScriptableDropdown : ScriptableUIControl<Dropdown>
    {
        public event Action<int> OnDropdownValueChanged;

        protected override void BindUIEvents ()
        {
            UIComponent.onValueChanged.AddListener(OnValueChanged);
            UIComponent.onValueChanged.AddListener(InvokeOnDropdownValueChanged);
        }

        protected override void UnbindUIEvents ()
        {
            UIComponent.onValueChanged.RemoveListener(OnValueChanged);
            UIComponent.onValueChanged.RemoveListener(InvokeOnDropdownValueChanged);
        }

        protected virtual void OnValueChanged (int value) { }

        private void InvokeOnDropdownValueChanged (int value)
        {
            if (OnDropdownValueChanged != null)
                OnDropdownValueChanged.Invoke(value);
        }
    }
}
