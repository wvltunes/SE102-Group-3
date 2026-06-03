using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RhythmRush.UI
{
    /// <summary>Segmented control (e.g. NOTE SPEED · SLOW / MID / FAST). Selected = purple fill + white text.</summary>
    [DisallowMultipleComponent]
    public sealed class RRSegmentedFX : MonoBehaviour
    {
        readonly List<Image> _bgs = new List<Image>();
        readonly List<TextMeshProUGUI> _labels = new List<TextMeshProUGUI>();
        int _selected;

        public Action<int> OnChanged;
        public int Selected => _selected;

        public void Register(int index, Image bg, TextMeshProUGUI label, Button btn)
        {
            while (_bgs.Count <= index) { _bgs.Add(null); _labels.Add(null); }
            _bgs[index] = bg; _labels[index] = label;
            int captured = index;
            btn.onClick.AddListener(() => Select(captured));
        }

        public void Select(int index, bool notify = true)
        {
            _selected = index;
            for (int i = 0; i < _bgs.Count; i++)
            {
                bool on = i == index;
                if (_bgs[i] != null) _bgs[i].color = on ? RRTheme.Purple : new Color(1, 1, 1, 0.05f);
                if (_labels[i] != null) _labels[i].color = on ? RRTheme.Fg1 : RRTheme.Fg2;
            }
            if (notify) OnChanged?.Invoke(index);
        }
    }
}
