using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public GameObject toolTipPanel;
    public TMP_Text toolTipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTipPanel.gameObject.SetActive(true);
        toolTipText.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTipPanel.gameObject.SetActive(false);
    }
}