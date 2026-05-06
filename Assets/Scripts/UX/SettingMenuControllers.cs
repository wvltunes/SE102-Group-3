using System.Collections;
using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    private SettingRow[] rows;
    private int currentIndex = 0;

    public void SetRows(SettingRow[] newRows)
    {
        // Deselect rows cũ
        if (rows != null)
            foreach (var row in rows)
                if (row != null) row.SetSelected(false);

        rows = newRows;
        currentIndex = 0;

        // ✅ Delay 1 frame để panel SetActive xong rồi mới selection
        StopAllCoroutines();
        StartCoroutine(SelectFirstNextFrame());
    }

    IEnumerator SelectFirstNextFrame()
    {
        yield return null; // Đợi 1 frame
        UpdateSelection();
    }

    void OnEnable()
    {
        currentIndex = 0;
        if (rows != null && rows.Length > 0)
            UpdateSelection();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (rows != null)
            foreach (var row in rows)
                if (row != null) row.SetSelected(false);
    }

    void Update()
    {
        if (rows == null || rows.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex++;
            if (currentIndex >= rows.Length) currentIndex = 0;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = rows.Length - 1;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            bool isRight = Input.GetKeyDown(KeyCode.RightArrow);
            HandleRowInput(currentIndex, isRight);
        }
    }

    void HandleRowInput(int index, bool isRight)
    {
        if (index < 0 || index >= rows.Length) return;

        Transform rowTransform = rows[index].transform;

        SliderClickControl slider = rowTransform
            .GetComponentInChildren<SliderClickControl>(true);
        if (slider != null)
        {
            if (isRight) slider.Increase();
            else slider.Decrease();
            return;
        }

        TwoButtonToggle toggle = rowTransform
            .GetComponentInChildren<TwoButtonToggle>(true);
        if (toggle != null)
        {
            if (isRight) toggle.Next();
            else toggle.Previous();
            return;
        }
    }

    public void SelectRow(int index)
    {
        currentIndex = index;
        UpdateSelection();
    }

    void UpdateSelection()
    {
        if (rows == null) return;
        for (int i = 0; i < rows.Length; i++)
            rows[i].SetSelected(i == currentIndex);
    }
}