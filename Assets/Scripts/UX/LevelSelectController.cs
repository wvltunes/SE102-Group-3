using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives a grid-style level-select screen with a fixed set of level slots.
///
/// For each slot it reads the shared unlock progress (see <see cref="LevelProgress"/>)
/// and:
///   - shows a lock icon and disables the button while the level is locked,
///   - hides the lock and enables the button once it is unlocked.
/// Clicking an unlocked slot loads its scene through <see cref="SceneTransitionManager"/>.
///
/// This shares the "UnlockedLevel" PlayerPrefs convention with the existing
/// DiscMenuController, so progress made on either screen carries over to the other.
/// </summary>
public class LevelSelectController : MonoBehaviour
{
    [System.Serializable]
    public class LevelSlot
    {
        [Tooltip("The clickable button for this level slot.")]
        public Button button;

        [Tooltip("Lock overlay shown while the level is locked (hidden once unlocked).")]
        public GameObject lockIcon;

        [Tooltip("Scene to load for this slot. Leave empty to default to \"Level<n>\" " +
                 "derived from the slot order. Loading by name avoids the build-index " +
                 "vs level-number mismatch (e.g. Level1 is build index 5).")]
        public string sceneName;
    }

    [Tooltip("One entry per level slot, in order (slot 0 = Level 1, slot 1 = Level 2, ...).")]
    [SerializeField] private LevelSlot[] slots = new LevelSlot[5];

    private void OnEnable()
    {
        // Re-evaluate locks whenever the screen is (re)shown - e.g. returning here
        // after unlocking the next level - so newly unlocked slots light up.
        RefreshSlots();
    }

    /// <summary>
    /// Applies the current unlock state to every slot: lock-icon visibility, button
    /// interactivity, and the click handler that loads the right level.
    /// </summary>
    public void RefreshSlots()
    {
        if (slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            LevelSlot slot = slots[i];
            if (slot == null) continue;

            int levelNumber = i + 1;                       // slot 0 -> Level 1
            bool unlocked = LevelProgress.IsUnlocked(levelNumber);

            if (slot.lockIcon != null)
            {
                slot.lockIcon.SetActive(!unlocked);
            }

            if (slot.button != null)
            {
                slot.button.interactable = unlocked;

                // Rebuild the runtime click wiring so each button captures the correct
                // level. RemoveAllListeners clears only listeners added from code (it
                // leaves any Inspector-wired calls untouched), which also stops the
                // handler stacking up across repeated RefreshSlots calls.
                slot.button.onClick.RemoveAllListeners();

                if (unlocked)
                {
                    // Resolve the scene name once; a fresh local is captured per slot
                    // so the lambda never loads the wrong (last-iteration) level.
                    string sceneToLoad = string.IsNullOrEmpty(slot.sceneName)
                        ? "Level" + levelNumber
                        : slot.sceneName;
                    slot.button.onClick.AddListener(
                        () => SceneTransitionManager.LoadLevel(sceneToLoad));
                }
            }
        }
    }
}
