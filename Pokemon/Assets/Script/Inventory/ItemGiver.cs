using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] itemBase item;
    [SerializeField] int count=1;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManger.Instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;

        string dialogText = $"{player.Name} received {item.Name}";
        if (count > 1)
            dialogText = $"{player.Name} received {count} {item.Name}";

        yield return DialogManger.Instance.ShowDialogText(dialogText);
    }
    /*確認是否可以使用以及是否存在*/
    public bool CanBeGiven()
    {
        return item != null && count > 0 && !used;
    }

}