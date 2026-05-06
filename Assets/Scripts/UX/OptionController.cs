using System.Collections;
using UnityEngine;

public class OptionController : MonoBehaviour
{
    public MenuButton[] buttons;
    public MenuButton btnDefault;

    private MenuButton current;

    void OnEnable()
    {
        // ✅ Chạy mỗi khi panel được mở — không dùng Start nữa
        foreach (var btn in buttons)
            if (btn != null) btn.Deselect();

        current = null;
        StartCoroutine(SelectDefaultNextFrame());
    }

    void OnDisable()
    {
        // ✅ Deselect tất cả khi đóng panel
        if (current != null)
        {
            current.Deselect();
            current = null;
        }
    }

    IEnumerator SelectDefaultNextFrame()
    {
        yield return null;

        if (btnDefault != null)
            SelectButton(btnDefault);
    }

    public void SelectButton(MenuButton selected)
    {
        if (current == selected) return;

        if (current != null)
            current.Deselect();

        selected.Select();
        current = selected;
    }
}