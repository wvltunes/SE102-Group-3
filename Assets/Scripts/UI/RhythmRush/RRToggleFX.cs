using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RhythmRush.UI
{
    /// <summary>Pill toggle (Settings). Cyan when on; knob slides; click flips state.</summary>
    [DisallowMultipleComponent]
    public sealed class RRToggleFX : MonoBehaviour, IPointerClickHandler
    {
        Image _bg;
        RectTransform _knob;
        float _onX, _offX;
        Color _onColor, _offColor;
        bool _on;

        public Action<bool> OnChanged;
        public bool IsOn => _on;

        public void Init(Image bg, RectTransform knob, float offX, float onX, Color offColor, Color onColor, bool on)
        {
            _bg = bg; _knob = knob; _offX = offX; _onX = onX; _offColor = offColor; _onColor = onColor;
            _on = on;
            _bg.color = on ? onColor : offColor;
            _knob.anchoredPosition = new Vector2(on ? onX : offX, _knob.anchoredPosition.y);
        }

        public void Set(bool on, bool notify = true)
        {
            _on = on;
            if (notify) OnChanged?.Invoke(_on);
        }

        public void OnPointerClick(PointerEventData e) => Set(!_on);

        void Update()
        {
            if (_knob == null) return;
            float k = 1f - Mathf.Exp(-18f * Time.unscaledDeltaTime);
            float targetX = _on ? _onX : _offX;
            var p = _knob.anchoredPosition;
            _knob.anchoredPosition = new Vector2(Mathf.Lerp(p.x, targetX, k), p.y);
            _bg.color = Color.Lerp(_bg.color, _on ? _onColor : _offColor, k);
        }
    }
}
