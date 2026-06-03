using UnityEngine;
using UnityEngine.EventSystems;

namespace RhythmRush.UI
{
    /// <summary>
    /// Capsule-button "juice": lifts on hover (−2px), sinks into its shadow on press
    /// (+4/+5px, shadow collapses). Mirrors the CSS hover/active transitions. Runs on
    /// unscaled time so it stays alive on the paused / game-over screens.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RRButtonFX : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        RectTransform _body, _shadow;
        Vector2 _restBody, _hoverBody, _pressBody;
        Vector2 _restShadow, _hoverShadow, _pressShadow;
        Vector2 _targetBody, _targetShadow;
        bool _hover, _down;
        const float Speed = 16f;

        public void Init(RectTransform body, RectTransform shadow, Vector2 drop)
        {
            _body = body; _shadow = shadow;

            _restBody  = Vector2.zero;
            _hoverBody = new Vector2(0f, 2f);
            _pressBody = new Vector2(4f, -5f);

            _restShadow  = new Vector2(drop.x, -drop.y);
            _hoverShadow = new Vector2(drop.x + 2f, -(drop.y + 2f));
            _pressShadow = new Vector2(1f, -2f);

            _targetBody = _restBody; _targetShadow = _restShadow;
            _body.anchoredPosition = _restBody;
            _shadow.anchoredPosition = _restShadow;
        }

        void Resolve()
        {
            if (_down)      { _targetBody = _pressBody; _targetShadow = _pressShadow; }
            else if (_hover){ _targetBody = _hoverBody; _targetShadow = _hoverShadow; }
            else            { _targetBody = _restBody;  _targetShadow = _restShadow; }
        }

        void Update()
        {
            if (_body == null) return;
            float k = 1f - Mathf.Exp(-Speed * Time.unscaledDeltaTime);
            _body.anchoredPosition   = Vector2.Lerp(_body.anchoredPosition,   _targetBody,   k);
            _shadow.anchoredPosition = Vector2.Lerp(_shadow.anchoredPosition, _targetShadow, k);
        }

        public void OnPointerEnter(PointerEventData e) { _hover = true;  Resolve(); }
        public void OnPointerExit(PointerEventData e)  { _hover = false; _down = false; Resolve(); }
        public void OnPointerDown(PointerEventData e)  { _down = true;   Resolve(); }
        public void OnPointerUp(PointerEventData e)    { _down = false;  Resolve(); }
    }
}
