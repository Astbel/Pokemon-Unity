using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text countText;
    /*包包座標Y軸*/
    RectTransform rectTransform;

    private void Awake()
    {
        
    }
    public Text NameText =>nameText;
    public Text CountText =>countText;
    /*取得包包height*/
    public float Height=>rectTransform.rect.height;
    public void SetData(ItemSlot itemSlot)
    {
         rectTransform=GetComponent<RectTransform>();
        nameText.text = itemSlot.Item.Name;
        countText.text = $"X {itemSlot.Count}";
    }
    /*商店販賣*/
    public void SetNameAndPrice(itemBase item)
    {
         rectTransform=GetComponent<RectTransform>();
        nameText.text = item.Name;
        countText.text = $"$ {item.Price}";
    }



}
