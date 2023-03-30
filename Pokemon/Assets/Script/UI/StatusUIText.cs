using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatusUIText : MonoBehaviour
{
    // Start is called before the first frame update

    Text text;
    private void Awake()
    {
        text = GetComponent<Text>();


    }    // Update is called once per frame
    public void SetSelected(bool selected)
    {
        text.color = (selected) ? GlobalSettings.i.HighhlightColor : Color.black;
    }

    public Text TextField => text;
}
