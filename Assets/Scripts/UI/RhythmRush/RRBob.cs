using UnityEngine;

namespace RhythmRush.UI
{
    /// <summary>Gentle vertical bob for the running character / floating orbs. Unscaled so it survives a pause.</summary>
    [DisallowMultipleComponent]
    public sealed class RRBob : MonoBehaviour
    {
        public float amplitude = 10f;
        public float speed = 2f;
        public float phase = 0f;
        RectTransform _rt;
        Vector2 _base;

        public void Configure(float amplitude, float speed, float phase)
        {
            this.amplitude = amplitude; this.speed = speed; this.phase = phase;
            _rt = (RectTransform)transform; _base = _rt.anchoredPosition;
        }

        void Awake() { if (_rt == null) { _rt = (RectTransform)transform; _base = _rt.anchoredPosition; } }

        void Update()
        {
            float y = Mathf.Sin(Time.unscaledTime * speed + phase) * amplitude;
            _rt.anchoredPosition = new Vector2(_base.x, _base.y + y);
        }
    }
}
