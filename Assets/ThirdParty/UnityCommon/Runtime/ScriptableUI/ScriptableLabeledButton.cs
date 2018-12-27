using System;

namespace UnityCommon
{
    public class ScriptableLabeledButton : ScriptableUIControl<LabeledButton>
    {
        public event Action OnButtonClicked;

        public override bool IsInteractable => CanvasGroup ? base.IsInteractable : UIComponent.interactable;

        public override void SetIsInteractable (bool isInteractable)
        {
            if (CanvasGroup) base.SetIsInteractable(isInteractable);
            else UIComponent.interactable = isInteractable;
        }

        protected override void BindUIEvents ()
        {
            UIComponent.onClick.AddListener(OnButtonClick);
            UIComponent.onClick.AddListener(InvokeOnButtonClicked);
        }

        protected override void UnbindUIEvents ()
        {
            UIComponent.onClick.RemoveListener(OnButtonClick);
            UIComponent.onClick.RemoveListener(InvokeOnButtonClicked);
        }

        protected virtual void OnButtonClick () { }

        private void InvokeOnButtonClicked ()
        {
            if (OnButtonClicked != null)
                OnButtonClicked.Invoke();
        }
    }
}
