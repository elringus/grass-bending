using UnityEngine;
using UnityEngine.UI;

namespace UnityCommon
{
    public class LabeledButton : Button
    {
        public Text Label => labelText;
        public ColorBlock LabelColorBlock => labelColors;
        public Color LabelColorMultiplier
        {
            get { return labelColorMultiplier; }
            set { labelColorMultiplier = value; DoStateTransition(currentSelectionState, false); }
        }

        [SerializeField] private Text labelText;
        [SerializeField] private ColorBlock labelColors = ColorBlock.defaultColorBlock;

        private Color labelColorMultiplier = Color.white;
        private Tweener<ColorTween> tintTweener;

        protected override void Awake ()
        {
            base.Awake();

            tintTweener = new Tweener<ColorTween>(this);
        }

        #if UNITY_EDITOR
        protected override void OnValidate ()
        {
            base.OnValidate();

            if (!labelText) labelText = GetComponentInChildren<Text>();
        }
        #endif

        protected override void Start ()
        {
            base.Start();

            if (!labelText) labelText = GetComponentInChildren<Text>();
        }

        protected override void DoStateTransition (SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (!Label) return;

            Color tintColor;
            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = LabelColorBlock.normalColor;
                    break;
                case SelectionState.Highlighted:
                    tintColor = LabelColorBlock.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    tintColor = LabelColorBlock.pressedColor;
                    break;
                case SelectionState.Disabled:
                    tintColor = LabelColorBlock.disabledColor;
                    break;
                default:
                    tintColor = Color.black;
                    break;
            }

            if (instant)
            {
                if (tintTweener != null && tintTweener.IsRunning) tintTweener.CompleteInstantly();
                Label.color = tintColor * LabelColorBlock.colorMultiplier * LabelColorMultiplier;
            }
            else if (tintTweener != null)
            {
                var tween = new ColorTween(Label.color, tintColor * LabelColorBlock.colorMultiplier * LabelColorMultiplier, ColorTweenMode.All, LabelColorBlock.fadeDuration, c => Label.color = c);
                tintTweener.Run(tween);
            }
        }
    }
}
