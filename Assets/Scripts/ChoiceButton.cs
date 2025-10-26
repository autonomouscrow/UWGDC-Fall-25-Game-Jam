using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ChoiceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string nextLineID;
    private string choiceText;

    public TMP_Text label;

    public Button button;

    public void Init(string text, string nextLine)
    {
        choiceText = text;
        nextLineID = nextLine;

        if (label != null)
            label.text = text;

        button.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DialogueManager.Instance.PreviewChoice(nextLineID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DialogueManager.Instance.EndPreview();
    }

    public void OnClick()
    {
        Debug.Log("clicked" + nextLineID);
        DialogueManager.Instance.SelectedChoice(nextLineID);
    }
}
