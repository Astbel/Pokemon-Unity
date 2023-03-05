using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    Text text;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void SetSelected(bool selected)
    {
        text.color = (selected) ? GlobalSettings.i.HighhlightColor : Color.black;
    }

    public Text TextField => text;
}
