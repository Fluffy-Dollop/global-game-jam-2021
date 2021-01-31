using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpDisplay : MonoBehaviour
{
    public TMPro.TMP_Text instructions;
    public TMPro.TMP_Text help;

    public void Toggle()
    {
        if (instructions.gameObject.activeSelf)
        {
            instructions.gameObject.SetActive(false);
            help.gameObject.SetActive(true);
        } else
        {
            instructions.gameObject.SetActive(true);
            help.gameObject.SetActive(false);
        }
    }
}
