using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandDisplay : MonoBehaviour
{
    public string hand;
    public TMPro.TMP_Text NameField;
    public TMPro.TMP_Text HelpField;

    private void Start()
    {
        ResetText();
    }

    public void ResetText()
    {
        NameField.text = "";
        HelpField.text = "";
    }

    public void SetText(string itemName, string itemHelp)
    {
        NameField.text = hand + " Click: " + itemName;
        HelpField.text = itemHelp;
    }

}
