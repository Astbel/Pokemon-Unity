using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour,Interactable
{
    [SerializeField] Dialog dialog;

    public void Interact()
    {
      //  DialogManger.Instance.ShowDialog(dialog);
        StartCoroutine(DialogManger.Instance.ShowDialog(dialog));
    }


}
