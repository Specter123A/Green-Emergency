using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnswerData : MonoBehaviour
{
    [Header ("UI Elements")]

    [SerializeField]
    TextMeshProUGUI infoTextObject;

    [SerializeField]
    Image toggle;

    [Header("Textures")]
    [SerializeField]
    Sprite uncheckToggle;

    [SerializeField]
    Sprite checkToggle;

    [Header("References")]
    [SerializeField]
    GameEvents events;

    private RectTransform rect;
    public RectTransform Rect
    {
        get 
        {
            if (rect ==null)
            {
                rect = GetComponent<RectTransform> () ?? gameObject.AddComponent<RectTransform>();
            }
            
            return rect;
            }
    }

    private int answerIndex =-1;
    public int AnswerIndex {get { return answerIndex; }}

    private bool Checked = false; 

    public void UpdateData(string info, int index)
    {
      infoTextObject.text = info;
      answerIndex = index;
    }

    public void Reset()
    {
        Checked = false;
        UpdateUI();
    }

    public void SwitchState()
    {
        Checked = !Checked;
        UpdateUI();

        if (events.UpdateAnswerUI !=null)
        {
            events.UpdateAnswerUI(this);
        }
    }

    void UpdateUI()
    {
        toggle.sprite = (Checked) ? checkToggle : uncheckToggle;
    }
}
