using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // ═══════════════════════════════════════════════
    // STRUCTS
    // ═══════════════════════════════════════════════

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
        public TextMeshProUGUI label;        // Text trên button
        public string textWhenOn = "SHOW";  // isOn = true  → panels ẩn
        public string textWhenOff = "HIDE";  // isOn = false → panels hiện
        public Sprite[] onSprites = new Sprite[5];
        public Sprite[] offSprites = new Sprite[5];
        [HideInInspector] public bool isOn = false;
    }

    // ───────────────────────────────────────────────
    // PANEL DATA STRUCTS
    // ───────────────────────────────────────────────

    [System.Serializable]
    public class ProfilePanelData
    {
        [Header("Speed")]
        [Range(0, 100)] public int speedValue = 80;
        public Color speedTextColor = Color.white;

        [Header("Progress Bar")]
        [Tooltip("Image dạng fill — fillAmount sẽ = speedValue / 100")]
        public Image progressFill;
        [Tooltip("Màu của fill bar theo char")]
        public Color fillColor = Color.cyan;
    }

    [System.Serializable]
    public class StoryPanelData
    {
        [TextArea(3, 6)]
        public string storyText;
        [TextArea(2, 4)]
        public string quoteText;
        public Color quoteColor = Color.white;
    }

    [System.Serializable]
    public class SkillPanelData
    {
        public string skillName;

        [Header("Energy (mỗi unit = 25%, max 4)")]
        [Range(1, 4)] public int energyCost = 2;
        [Tooltip("Image dạng fill cho energy bar")]
        public Image energyFill;
        [Tooltip("Màu fill energy theo char")]
        public Color energyFillColor = Color.yellow;
        

        [Header("Texts")]
        [TextArea(2, 4)] public string howToUse;
        [TextArea(2, 3)] public string noteText;
        public Color noteColor = Color.white;
    }

    // ───────────────────────────────────────────────
    // CHAMPION DATA
    // ───────────────────────────────────────────────

    [System.Serializable]
    public class ChampionData
    {
        [Header("IDENTITY")]
        public string characterName;
        public string traits;

        [Header("COLORS")]
        public Color nameColor = Color.white;
        public Color titleColor = Color.white;
        public Color traitsColor = Color.white;

        [Header("BACKGROUND")]
        public Sprite backgroundSprite;
        public Sprite rotatingCircleSprite;

        [Header("PANELS")]
        public ProfilePanelData profile;
        public StoryPanelData story;
        public SkillPanelData skill;
    }

    // ═══════════════════════════════════════════════
    // INSPECTOR
    // ═══════════════════════════════════════════════

    [Header("── CHAMPIONS ──────────────────────────")]
    public ChampionData[] champions;

    [Header("── LEFT PANEL ──────────────────────────")]
    public GameObject leftPanel;
    public TextMeshProUGUI textPickYourChampion;
    public SwappableImage belowTitle;
    public Button[] charButtons = new Button[5];
    public SwappableImage[] charButtonImages = new SwappableImage[5];

    [Header("── BACKGROUND ──────────────────────────")]
    public Image imgBackground;
    public Image imgRotatingCircle;

    [Header("── CENTER PANEL ─────────────────────────")]
    public TextMeshProUGUI textCharacterName;
    public TextMeshProUGUI textTraits;
    public SwappableImage deathEffectButton;
    public ToggleButton toggleViewButton;

    [Header("── RIGHT PANEL ──────────────────────────")]
    public GameObject rightPanel;
    public TwoStateButton[] tabButtons = new TwoStateButton[3];
    public SwappableImage[] infoPanelBgs = new SwappableImage[3]; // background của 3 panel

    [Header("── PROFILE PANEL UI ────────────────────")]
    [Tooltip("Text hiển thị số tốc độ, VD: '80'")]
    public TextMeshProUGUI profileSpeedText;
    [Tooltip("Image fill của progress bar tốc độ")]
    public Image profileProgressFill;
    [Tooltip("Image nền progress bar (không fill)")]
    public Image profileProgressBg;

    [Header("── STORY PANEL UI ──────────────────────")]
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI quoteText;

    [Header("── SKILL PANEL UI ──────────────────────")]
    public TextMeshProUGUI skillNameText;
    [Tooltip("Image fill của energy bar (fillAmount = energyCost / 4)")]
    public Image skillEnergyFill;
    [Tooltip("Image nền energy bar")]
    public Image skillEnergyBg;
    public TextMeshProUGUI skillHowToUseText;
    public TextMeshProUGUI skillNoteText;

    [Header("── SLIDE ANIMATION ─────────────────────")]
    [Range(0.1f, 0.5f)] public float panelSlideDuration = 0.25f;
    public float leftPanelSlideOffset = -350f;
    public float rightPanelSlideOffset = 350f;

    [Header("── TRANSITION ───────────────────────────")]
    [Range(0.1f, 0.8f)] public float fadeDuration = 0.3f;
    [Range(5f, 180f)] public float rotationSpeed = 30f;

    [Header("── BUTTON SCALE ─────────────────────────")]
    public float selectedScale = 1.08f;
    public float unselectedScale = 1f;
    [Range(0.05f, 0.3f)] public float scaleDuration = 0.12f;

    // ═══════════════════════════════════════════════
    // PRIVATE
    // ═══════════════════════════════════════════════

    private int currentChampion = -1;
    private int currentTab = 0;
    private bool panelsHidden = false;

    private Coroutine transitionCoroutine;
    private Coroutine[] scaleCoroutines;
    private Coroutine leftPanelCoroutine;
    private Coroutine rightPanelCoroutine;
    private Vector2 leftPanelOrigin;
    private Vector2 rightPanelOrigin;

    private Image bgLayerA, bgLayerB;
    private Image circleLayerA, circleLayerB;
    private bool usingLayerA = true;

    // ═══════════════════════════════════════════════
    // LIFECYCLE
    // ═══════════════════════════════════════════════

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
        float rot = -rotationSpeed * Time.deltaTime;
        if (circleLayerA != null) circleLayerA.rectTransform.Rotate(0f, 0f, rot);
        if (circleLayerB != null) circleLayerB.rectTransform.Rotate(0f, 0f, rot);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            SelectChampion(Mathf.Max(0, currentChampion - 1));
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            SelectChampion(Mathf.Min(champions.Length - 1, currentChampion + 1));
    }

    // ═══════════════════════════════════════════════
    // CROSSFADE LAYERS
    // ═══════════════════════════════════════════════

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

    // ═══════════════════════════════════════════════
    // SELECT CHAMPION
    // ═══════════════════════════════════════════════

    public void SelectChampion(int index)
    {
        if (champions == null || index < 0 || index >= champions.Length) return;
        if (index == currentChampion) return;

        int prev = currentChampion;
        currentChampion = index;

        SwapAllImages(index);
        AnimateButtonScale(prev, false);
        AnimateButtonScale(index, true);

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
        ApplyPanelData(d);
        SwapAllImages(index);

        for (int i = 0; i < charButtons.Length; i++)
        {
            if (charButtons[i] == null) continue;
            charButtons[i].transform.localScale = Vector3.one * (i == index ? selectedScale : unselectedScale);
        }
    }

    void ApplyTexts(ChampionData d)
    {
        if (textCharacterName != null) { textCharacterName.text = d.characterName; textCharacterName.color = ForceAlpha(d.nameColor); }
        if (textTraits != null) { textTraits.text = d.traits; textTraits.color = ForceAlpha(d.traitsColor); }
        if (textPickYourChampion != null) textPickYourChampion.color = ForceAlpha(d.titleColor);
    }

    // ═══════════════════════════════════════════════
    // APPLY PANEL DATA
    // ═══════════════════════════════════════════════

    void ApplyPanelData(ChampionData d)
    {
        ApplyProfilePanel(d.profile);
        ApplyStoryPanel(d.story);
        ApplySkillPanel(d.skill);
    }

    void ApplyProfilePanel(ProfilePanelData p)
    {
        if (p == null) return;

        // Speed text
        if (profileSpeedText != null)
        {
            profileSpeedText.text = p.speedValue.ToString();
            profileSpeedText.color = ForceAlpha(p.speedTextColor);
        }

        // Progress fill
        if (profileProgressFill != null)
        {
            profileProgressFill.fillAmount = p.speedValue / 100f;
            profileProgressFill.color = ForceAlpha(p.fillColor);
        }
    }

    void ApplyStoryPanel(StoryPanelData s)
    {
        if (s == null) return;

        if (storyText != null) storyText.text = s.storyText;

        if (quoteText != null)
        {
            quoteText.text = s.quoteText;
            quoteText.color = ForceAlpha(s.quoteColor);
        }
    }

    void ApplySkillPanel(SkillPanelData sk)
    {
        if (sk == null) return;

        if (skillNameText != null) skillNameText.text = sk.skillName;
        if (skillHowToUseText != null) skillHowToUseText.text = sk.howToUse;
        if (skillNoteText != null)
        {
            skillNoteText.text = sk.noteText;
            skillNoteText.color = ForceAlpha(sk.noteColor);
        }

        // Energy fill: mỗi unit = 25%, max 4 units
        if (skillEnergyFill != null)
        {
            skillEnergyFill.fillAmount = Mathf.Clamp(sk.energyCost, 0, 4) / 4f;
            skillEnergyFill.color = ForceAlpha(sk.energyFillColor);
        }
    }

    // ═══════════════════════════════════════════════
    // CROSSFADE TRANSITION
    // ═══════════════════════════════════════════════

    IEnumerator CrossfadeTransition(ChampionData data)
    {
        Image bgIn = usingLayerA ? bgLayerB : bgLayerA;
        Image bgOut = usingLayerA ? bgLayerA : bgLayerB;
        Image circIn = usingLayerA ? circleLayerB : circleLayerA;
        Image circOut = usingLayerA ? circleLayerA : circleLayerB;

        if (bgIn != null && data.backgroundSprite != null) bgIn.sprite = data.backgroundSprite;
        if (circIn != null && data.rotatingCircleSprite != null) circIn.sprite = data.rotatingCircleSprite;

        SetAlpha(bgIn, 0f); SetAlpha(bgOut, 1f);
        SetAlpha(circIn, 0f); SetAlpha(circOut, 1f);

        Color startName = textCharacterName != null ? textCharacterName.color : Color.white;
        Color startTraits = textTraits != null ? textTraits.color : Color.white;
        Color startTitle = textPickYourChampion != null ? textPickYourChampion.color : Color.white;

        if (textCharacterName != null) textCharacterName.text = data.characterName;
        if (textTraits != null) textTraits.text = data.traits;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / fadeDuration));

            SetAlpha(bgIn, t); SetAlpha(bgOut, 1f - t);
            SetAlpha(circIn, t); SetAlpha(circOut, 1f - t);

            if (textCharacterName != null) textCharacterName.color = Color.Lerp(startName, ForceAlpha(data.nameColor), t);
            if (textTraits != null) textTraits.color = Color.Lerp(startTraits, ForceAlpha(data.traitsColor), t);
            if (textPickYourChampion != null) textPickYourChampion.color = Color.Lerp(startTitle, ForceAlpha(data.titleColor), t);

            yield return null;
        }

        SetAlpha(bgIn, 1f); SetAlpha(bgOut, 0f);
        SetAlpha(circIn, 1f); SetAlpha(circOut, 0f);

        ApplyTexts(data);
        ApplyPanelData(data); // Cập nhật panel sau khi transition xong

        usingLayerA = !usingLayerA;
    }

    // ═══════════════════════════════════════════════
    // BUTTON SCALE
    // ═══════════════════════════════════════════════

    void AnimateButtonScale(int index, bool grow)
    {
        if (index < 0 || index >= charButtons.Length || charButtons[index] == null) return;
        if (scaleCoroutines[index] != null) StopCoroutine(scaleCoroutines[index]);
        scaleCoroutines[index] = StartCoroutine(ScaleButton(charButtons[index].transform, grow ? selectedScale : unselectedScale));
    }

    IEnumerator ScaleButton(Transform t, float target)
    {
        Vector3 start = t.localScale;
        Vector3 end = Vector3.one * target;
        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / scaleDuration)));
            yield return null;
        }
        t.localScale = end;
    }

    // ═══════════════════════════════════════════════
    // TOGGLE HIDE PANELS (slide animation)
    // ═══════════════════════════════════════════════

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

        if (leftPanel != null)
        {
            if (leftPanelCoroutine != null) StopCoroutine(leftPanelCoroutine);
            Vector2 target = panelsHidden ? leftPanelOrigin + new Vector2(leftPanelSlideOffset, 0f) : leftPanelOrigin;
            leftPanelCoroutine = StartCoroutine(SlidePanel(leftPanel, target, panelsHidden));
        }

        if (rightPanel != null)
        {
            if (rightPanelCoroutine != null) StopCoroutine(rightPanelCoroutine);
            Vector2 target = panelsHidden ? rightPanelOrigin + new Vector2(rightPanelSlideOffset, 0f) : rightPanelOrigin;
            rightPanelCoroutine = StartCoroutine(SlidePanel(rightPanel, target, panelsHidden));
        }

        RefreshToggleSprite();
    }

    IEnumerator SlidePanel(GameObject panel, Vector2 targetPos, bool deactivateOnEnd)
    {
        panel.SetActive(true);
        var rt = panel.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector2 startPos = rt.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < panelSlideDuration)
        {
            elapsed += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / panelSlideDuration)));
            yield return null;
        }
        rt.anchoredPosition = targetPos;
        if (deactivateOnEnd) panel.SetActive(false);
    }

    void RefreshToggleSprite()
    {
        if (toggleViewButton == null) return;

        // Đổi text HIDE ↔ SHOW
        if (toggleViewButton.label != null)
            toggleViewButton.label.text = toggleViewButton.isOn
                ? toggleViewButton.textWhenOn
                : toggleViewButton.textWhenOff;

        // Đổi sprite
        if (toggleViewButton.image == null || currentChampion < 0) return;
        var sprites = toggleViewButton.isOn ? toggleViewButton.onSprites : toggleViewButton.offSprites;
        if (sprites == null || currentChampion >= sprites.Length) return;
        var sprite = sprites[currentChampion];
        if (sprite != null) toggleViewButton.image.sprite = sprite;
    }

    // ═══════════════════════════════════════════════
    // TAB BUTTONS
    // ═══════════════════════════════════════════════

    public void SelectTab(int tabIndex)
    {
        currentTab = tabIndex;

        for (int i = 0; i < infoPanelBgs.Length; i++)
        {
            var panel = infoPanelBgs[i];
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
        for (int i = 0; i < infoPanelBgs.Length; i++)
        {
            if (i != currentTab) continue;
            var panel = infoPanelBgs[i];
            if (panel == null || panel.image == null) continue;
            if (panel.spritesPerSelection == null || currentChampion >= panel.spritesPerSelection.Length) continue;
            var sprite = panel.spritesPerSelection[currentChampion];
            if (sprite != null) panel.image.sprite = sprite;
        }
    }

    // ═══════════════════════════════════════════════
    // SWAP ALL IMAGES
    // ═══════════════════════════════════════════════

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

    // ═══════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color; c.a = a; img.color = c;
    }

    Color ForceAlpha(Color c) { c.a = 1f; return c; }

    // ═══════════════════════════════════════════════
    // EDITOR TEST
    // ═══════════════════════════════════════════════
#if UNITY_EDITOR
    [ContextMenu("PulseRunner")]  void T0()   => SelectChampion(0);
    [ContextMenu("Riff")]         void T1()   => SelectChampion(1);
    [ContextMenu("Aria")]         void T2()   => SelectChampion(2);
    [ContextMenu("Baze")]         void T3()   => SelectChampion(3);
    [ContextMenu("Clef")]         void T4()   => SelectChampion(4);
    [ContextMenu("Tab: Profile")] void Tab0() => SelectTab(0);
    [ContextMenu("Tab: Story")]   void Tab1() => SelectTab(1);
    [ContextMenu("Tab: Skill")]   void Tab2() => SelectTab(2);
#endif
}