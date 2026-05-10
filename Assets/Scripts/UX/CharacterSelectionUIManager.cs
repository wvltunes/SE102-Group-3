using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // STRUCTS
    // ─────────────────────────────────────────────

    [System.Serializable]
    public class SwappableImage
    {
        public Image image;
        public Sprite[] spritesPerSelection = new Sprite[5];
    }

    [System.Serializable]
    public class TwoStateButton
    {
        public Image image;
        public Button button;
        public Sprite[] selectedSprites = new Sprite[5];
        public Sprite[] unselectedSprites = new Sprite[5];
    }

    [System.Serializable]
    public class ToggleButton
    {
        public Image image;
        public Button button;
        public Sprite[] onSprites = new Sprite[5];
        public Sprite[] offSprites = new Sprite[5];
        [HideInInspector] public bool isOn = false;
    }

    [System.Serializable]
    public class ChampionData
    {
        [Header("TEXT")]
        public string characterName;
        public string traits;

        [Header("COLORS")]
        public Color nameColor = Color.white;
        public Color titleColor = Color.white;
        public Color traitsColor = Color.white;

        [Header("BACKGROUND")]
        public Sprite backgroundSprite;
        public Sprite rotatingCircleSprite;
    }

    // ─────────────────────────────────────────────
    // INSPECTOR
    // ─────────────────────────────────────────────

    [Header("CHAMPIONS")]
    public ChampionData[] champions;

    [Header("LEFT PANEL")]
    public GameObject leftPanel;
    public TextMeshProUGUI textPickYourChampion;
    public SwappableImage belowTitle;
    public Button[] charButtons = new Button[5];
    public SwappableImage[] charButtonImages = new SwappableImage[5];

    [Header("BACKGROUND")]
    public Image imgBackground;
    public Image imgRotatingCircle;

    [Header("CENTER PANEL")]
    public TextMeshProUGUI textCharacterName;
    public TextMeshProUGUI textTraits;
    public SwappableImage deathEffectButton;
    public ToggleButton toggleViewButton;

    [Header("RIGHT PANEL")]
    public GameObject rightPanel;
    public TwoStateButton[] tabButtons = new TwoStateButton[3];
    public SwappableImage[] infoPanels = new SwappableImage[3];

    [Header("PANEL SLIDE ANIMATION")]
    [Range(0.1f, 0.5f)] public float panelSlideDuration = 0.25f;
    [Tooltip("Số pixel panel trái slide ra ngoài (âm = sang trái)")]
    public float leftPanelSlideOffset = -350f;
    [Tooltip("Số pixel panel phải slide ra ngoài (dương = sang phải)")]
    public float rightPanelSlideOffset = 350f;

    [Header("ANIMATION")]
    [Range(0.1f, 0.8f)] public float fadeDuration = 0.3f;
    [Range(5f, 180f)] public float rotationSpeed = 30f;

    [Header("BUTTON SCALE")]
    public float selectedScale = 1.08f;   // button được chọn to hơn
    public float unselectedScale = 1f;
    [Range(0.05f, 0.3f)] public float scaleDuration = 0.12f;

    // ─────────────────────────────────────────────
    // PRIVATE
    // ─────────────────────────────────────────────

    private int currentChampion = -1;
    private int currentTab = 0;
    private bool panelsHidden = false;

    private Coroutine transitionCoroutine;
    private Coroutine[] scaleCoroutines;
    private Coroutine leftPanelCoroutine;
    private Coroutine rightPanelCoroutine;
    private Vector2 leftPanelOrigin;
    private Vector2 rightPanelOrigin; // 1 coroutine per button

    // Crossfade layers cho background
    private Image bgLayerA, bgLayerB;
    private Image circleLayerA, circleLayerB;
    private bool usingLayerA = true;

    // ─────────────────────────────────────────────
    // LIFECYCLE
    // ─────────────────────────────────────────────

    void Start()
    {
        scaleCoroutines = new Coroutine[charButtons.Length];

        BuildCrossfadeLayers();
        SavePanelOrigins();

        for (int i = 0; i < charButtons.Length; i++)
        {
            int idx = i;
            charButtons[i]?.onClick.AddListener(() => SelectChampion(idx));
        }

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int idx = i;
            tabButtons[i]?.button?.onClick.AddListener(() => SelectTab(idx));
        }

        toggleViewButton?.button?.onClick.AddListener(OnToggleViewClicked);

        if (champions != null && champions.Length > 0)
            ApplyImmediate(0);

        SelectTab(0);
    }

    void Update()
    {
        // Xoay cả 2 layer circle cùng lúc để không bị giật khi crossfade
        float rot = -rotationSpeed * Time.deltaTime;
        if (circleLayerA != null) circleLayerA.rectTransform.Rotate(0f, 0f, rot);
        if (circleLayerB != null) circleLayerB.rectTransform.Rotate(0f, 0f, rot);

        // Keyboard navigation UP / DOWN
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SelectChampion(Mathf.Max(0, currentChampion - 1));
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            SelectChampion(Mathf.Min(champions.Length - 1, currentChampion + 1));
    }

    // ─────────────────────────────────────────────
    // CROSSFADE LAYER SETUP
    // ─────────────────────────────────────────────

    void BuildCrossfadeLayers()
    {
        bgLayerA = imgBackground;
        bgLayerB = CloneImage(imgBackground, "_B");
        circleLayerA = imgRotatingCircle;
        circleLayerB = CloneImage(imgRotatingCircle, "_B");

        SetAlpha(bgLayerB, 0f);
        SetAlpha(circleLayerB, 0f);
    }

    Image CloneImage(Image src, string suffix)
    {
        if (src == null) return null;
        var go = new GameObject(src.gameObject.name + suffix);
        go.transform.SetParent(src.transform.parent, false);
        var srt = src.rectTransform;
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = srt.anchorMin; rt.anchorMax = srt.anchorMax;
        rt.pivot = srt.pivot; rt.sizeDelta = srt.sizeDelta;
        rt.anchoredPosition = srt.anchoredPosition;
        go.transform.SetSiblingIndex(src.transform.GetSiblingIndex() + 1);
        var img = go.AddComponent<Image>();
        img.sprite = src.sprite; img.color = src.color;
        img.raycastTarget = false;
        return img;
    }

    // ─────────────────────────────────────────────
    // SELECT CHAMPION
    // ─────────────────────────────────────────────

    public void SelectChampion(int index)
    {
        if (champions == null) return;
        if (index < 0 || index >= champions.Length) return;
        if (index == currentChampion) return;

        int prev = currentChampion;
        currentChampion = index;

        // Đổi sprite ngay
        SwapAllImages(index);

        // Scale button
        AnimateButtonScale(prev, false);
        AnimateButtonScale(index, true);

        // Crossfade background + lerp text
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(CrossfadeTransition(champions[index]));
    }

    void ApplyImmediate(int index)
    {
        currentChampion = index;
        var d = champions[index];

        if (bgLayerA != null && d.backgroundSprite != null) bgLayerA.sprite = d.backgroundSprite;
        if (circleLayerA != null && d.rotatingCircleSprite != null) circleLayerA.sprite = d.rotatingCircleSprite;

        ApplyTexts(d);
        SwapAllImages(index);

        // Scale tất cả button về đúng trạng thái
        for (int i = 0; i < charButtons.Length; i++)
        {
            if (charButtons[i] == null) continue;
            float s = (i == index) ? selectedScale : unselectedScale;
            charButtons[i].transform.localScale = Vector3.one * s;
        }
    }

    void ApplyTexts(ChampionData d)
    {
        if (textCharacterName != null) { textCharacterName.text = d.characterName; textCharacterName.color = ForceAlpha(d.nameColor); }
        if (textTraits != null) { textTraits.text = d.traits; textTraits.color = ForceAlpha(d.traitsColor); }
        if (textPickYourChampion != null) textPickYourChampion.color = ForceAlpha(d.titleColor);
    }

    // ─────────────────────────────────────────────
    // SMOOTH CROSSFADE TRANSITION
    // Không fade to black — crossfade trực tiếp giữa 2 layer
    // + lerp màu text đồng thời
    // ─────────────────────────────────────────────

    IEnumerator CrossfadeTransition(ChampionData data)
    {
        Image bgIn = usingLayerA ? bgLayerB : bgLayerA;
        Image bgOut = usingLayerA ? bgLayerA : bgLayerB;
        Image circIn = usingLayerA ? circleLayerB : circleLayerA;
        Image circOut = usingLayerA ? circleLayerA : circleLayerB;

        // Gán sprite mới vào layer sắp hiện (đang ẩn)
        if (bgIn != null && data.backgroundSprite != null) bgIn.sprite = data.backgroundSprite;
        if (circIn != null && data.rotatingCircleSprite != null) circIn.sprite = data.rotatingCircleSprite;

        SetAlpha(bgIn, 0f); SetAlpha(bgOut, 1f);
        SetAlpha(circIn, 0f); SetAlpha(circOut, 1f);

        // Lưu màu text ban đầu để lerp
        Color startName = textCharacterName != null ? textCharacterName.color : Color.white;
        Color startTraits = textTraits != null ? textTraits.color : Color.white;
        Color startTitle = textPickYourChampion != null ? textPickYourChampion.color : Color.white;

        // Cập nhật text content ngay (người dùng sẽ thấy chữ mờ dần ra)
        if (textCharacterName != null) textCharacterName.text = data.characterName;
        if (textTraits != null) textTraits.text = data.traits;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / fadeDuration));

            // Crossfade background
            SetAlpha(bgIn, t);
            SetAlpha(bgOut, 1f - t);
            SetAlpha(circIn, t);
            SetAlpha(circOut, 1f - t);

            // Lerp màu text
            if (textCharacterName != null) textCharacterName.color = Color.Lerp(startName, ForceAlpha(data.nameColor), t);
            if (textTraits != null) textTraits.color = Color.Lerp(startTraits, ForceAlpha(data.traitsColor), t);
            if (textPickYourChampion != null) textPickYourChampion.color = Color.Lerp(startTitle, ForceAlpha(data.titleColor), t);

            yield return null;
        }

        // Chốt giá trị cuối
        SetAlpha(bgIn, 1f); SetAlpha(bgOut, 0f);
        SetAlpha(circIn, 1f); SetAlpha(circOut, 0f);
        ApplyTexts(data);

        usingLayerA = !usingLayerA;
    }

    // ─────────────────────────────────────────────
    // BUTTON SCALE ANIMATION
    // ─────────────────────────────────────────────

    void AnimateButtonScale(int index, bool grow)
    {
        if (index < 0 || index >= charButtons.Length) return;
        if (charButtons[index] == null) return;

        if (scaleCoroutines[index] != null) StopCoroutine(scaleCoroutines[index]);
        scaleCoroutines[index] = StartCoroutine(ScaleButton(charButtons[index].transform, grow ? selectedScale : unselectedScale));
    }

    IEnumerator ScaleButton(Transform t, float targetScale)
    {
        Vector3 startScale = t.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float s = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / scaleDuration));
            t.localScale = Vector3.Lerp(startScale, endScale, s);
            yield return null;
        }

        t.localScale = endScale;
    }

    // ─────────────────────────────────────────────
    // TOGGLE HIDE LEFT + RIGHT PANEL
    // ─────────────────────────────────────────────

    void SavePanelOrigins()
    {
        if (leftPanel != null) leftPanelOrigin = leftPanel.GetComponent<RectTransform>().anchoredPosition;
        if (rightPanel != null) rightPanelOrigin = rightPanel.GetComponent<RectTransform>().anchoredPosition;
    }

    void OnToggleViewClicked()
    {
        if (toggleViewButton == null) return;

        toggleViewButton.isOn = !toggleViewButton.isOn;
        panelsHidden = toggleViewButton.isOn;

        // Slide animation cho left panel
        if (leftPanel != null)
        {
            if (leftPanelCoroutine != null) StopCoroutine(leftPanelCoroutine);
            Vector2 target = panelsHidden
                ? leftPanelOrigin + new Vector2(leftPanelSlideOffset, 0f)
                : leftPanelOrigin;
            leftPanelCoroutine = StartCoroutine(SlidePanel(leftPanel, target, panelsHidden));
        }

        // Slide animation cho right panel
        if (rightPanel != null)
        {
            if (rightPanelCoroutine != null) StopCoroutine(rightPanelCoroutine);
            Vector2 target = panelsHidden
                ? rightPanelOrigin + new Vector2(rightPanelSlideOffset, 0f)
                : rightPanelOrigin;
            rightPanelCoroutine = StartCoroutine(SlidePanel(rightPanel, target, false));
        }

        RefreshToggleSprite();
    }

    IEnumerator SlidePanel(GameObject panel, Vector2 targetPos, bool deactivateOnEnd)
    {
        panel.SetActive(true); // Đảm bảo hiện trong lúc animate
        var rt = panel.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector2 startPos = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < panelSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / panelSlideDuration));
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rt.anchoredPosition = targetPos;

        // Nếu là ẩn thì deactivate sau khi slide xong
        if (deactivateOnEnd)
            panel.SetActive(false);
    }

    void RefreshToggleSprite()
    {
        if (toggleViewButton == null || toggleViewButton.image == null) return;
        if (currentChampion < 0) return;

        var sprites = toggleViewButton.isOn ? toggleViewButton.onSprites : toggleViewButton.offSprites;
        if (sprites == null || currentChampion >= sprites.Length) return;
        var sprite = sprites[currentChampion];
        if (sprite != null) toggleViewButton.image.sprite = sprite;
    }

    // ─────────────────────────────────────────────
    // TAB (Profile / Story / Skill)
    // ─────────────────────────────────────────────

    public void SelectTab(int tabIndex)
    {
        currentTab = tabIndex;

        for (int i = 0; i < infoPanels.Length; i++)
        {
            var panel = infoPanels[i];
            if (panel == null || panel.image == null) continue;
            bool active = (i == tabIndex);
            panel.image.gameObject.SetActive(active);
            if (active && panel.spritesPerSelection != null
                && currentChampion >= 0 && currentChampion < panel.spritesPerSelection.Length)
            {
                var sprite = panel.spritesPerSelection[currentChampion];
                if (sprite != null) panel.image.sprite = sprite;
            }
        }

        RefreshTabSprites();
    }

    void RefreshTabSprites()
    {
        if (currentChampion < 0) return;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            var tb = tabButtons[i];
            if (tb == null || tb.image == null) continue;
            var sprites = (i == currentTab) ? tb.selectedSprites : tb.unselectedSprites;
            if (sprites == null || currentChampion >= sprites.Length) continue;
            var sprite = sprites[currentChampion];
            if (sprite != null) tb.image.sprite = sprite;
        }
    }

    void RefreshActivePanelSprite()
    {
        if (currentChampion < 0) return;
        for (int i = 0; i < infoPanels.Length; i++)
        {
            if (i != currentTab) continue;
            var panel = infoPanels[i];
            if (panel == null || panel.image == null) continue;
            if (panel.spritesPerSelection == null || currentChampion >= panel.spritesPerSelection.Length) continue;
            var sprite = panel.spritesPerSelection[currentChampion];
            if (sprite != null) panel.image.sprite = sprite;
        }
    }

    // ─────────────────────────────────────────────
    // SWAP ALL IMAGES
    // ─────────────────────────────────────────────

    void SwapAllImages(int idx)
    {
        SwapSingle(belowTitle, idx);
        SwapGroup(charButtonImages, idx);
        SwapSingle(deathEffectButton, idx);
        RefreshToggleSprite();
        RefreshTabSprites();
        RefreshActivePanelSprite();
    }

    void SwapGroup(SwappableImage[] group, int idx)
    {
        if (group == null) return;
        foreach (var item in group) SwapSingle(item, idx);
    }

    void SwapSingle(SwappableImage item, int idx)
    {
        if (item == null || item.image == null) return;
        if (item.spritesPerSelection == null || idx >= item.spritesPerSelection.Length) return;
        var sprite = item.spritesPerSelection[idx];
        if (sprite != null) item.image.sprite = sprite;
    }

    // ─────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color; c.a = a; img.color = c;
    }

    Color ForceAlpha(Color c) { c.a = 1f; return c; }

    // ─────────────────────────────────────────────
    // EDITOR TEST
    // ─────────────────────────────────────────────
#if UNITY_EDITOR
    [ContextMenu("PulseRunner")] void T0() => SelectChampion(0);
    [ContextMenu("Riff")]        void T1() => SelectChampion(1);
    [ContextMenu("Aria")]        void T2() => SelectChampion(2);
    [ContextMenu("Baze")]        void T3() => SelectChampion(3);
    [ContextMenu("Clef")]        void T4() => SelectChampion(4);
    [ContextMenu("Tab: Profile")] void Tab0() => SelectTab(0);
    [ContextMenu("Tab: Story")]   void Tab1() => SelectTab(1);
    [ContextMenu("Tab: Skill")]   void Tab2() => SelectTab(2);
#endif
}