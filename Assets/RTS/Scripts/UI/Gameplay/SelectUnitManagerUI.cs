using UnityEngine;

public class SelectUnitManagerUI : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private RectTransform selectUnitArea;

    private bool _isSelected;

    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectAreaStart += OnSelectAreaStart;
        UnitSelectionManager.Instance.OnSelectAreaEnd += OnSelectAreaEnd;

        _isSelected = false;
        selectUnitArea.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isSelected)
            UpdateSelectAreaVisual();
    }

    private void OnSelectAreaStart(object sender, System.EventArgs e)
    {
        _isSelected = true;
        selectUnitArea.gameObject.SetActive(true);
        UpdateSelectAreaVisual();
    }

    private void OnSelectAreaEnd(object sender, System.EventArgs e)
    {
        _isSelected = false;
        selectUnitArea.gameObject.SetActive(false);
    }

    private void UpdateSelectAreaVisual()
    {
        // Use canvas scale to execute select area correctly
        float areaScale = mainCanvas.transform.localScale.x;
        Rect selectAreaRect = UnitSelectionManager.Instance.GetSelectArea();
        selectUnitArea.anchoredPosition = selectAreaRect.position / areaScale;
        selectUnitArea.sizeDelta = selectAreaRect.size / areaScale;
    }
}
