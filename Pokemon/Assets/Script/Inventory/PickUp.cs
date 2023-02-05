using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable
{
    [SerializeField] itemBase item;
    public bool Used { get; set; } = false;

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
        }

        Used = true;
        /*當撿起道具時不使用destroy因為要保存時如果是destory無法存取該狀態,所以直接關閉sprite以及boxCoilder*/
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        /*訊息顯示玩家撿起道具*/
        yield return DialogManger.Instance.ShowDialogText($"Player found {item.Name}");
    }
}
