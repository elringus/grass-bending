using UnityEngine.EventSystems;

namespace UnityCommon
{
    public abstract class ScriptableUIComponent<T> : ScriptableUIBehaviour where T : UIBehaviour
    {
        public T UIComponent { get { return uiComponent ?? (uiComponent = GetComponent<T>()); } }

        private T uiComponent;
    }
}
