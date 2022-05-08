using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Animator : MonoBehaviour
{
    public Animator yearAnimator;
    public TextMeshProUGUI yearText;

    public void AnimateYearSuccession()
    {
        yearText.SetText("Year {0}", FindObjectOfType<GameController>().currentYear);
        yearAnimator.SetTrigger("NextYear");
    }
}
