using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] List<itemBase> availableItems;

    public IEnumerator Trade()
    {
        yield return ShopController.i.StartTrading(this);
    }

    public List<itemBase> AvailableItems => availableItems;
}
