using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;
    //確認是否為玩家單位
    public bool IsPlayerUnit { get { return isPlayerUnit; } }

    Image image;
    Vector3 orginalPosition;
    Color orginalColor;
    public Pokemon Pokemon { get; set; }

    public BattleHud Hud { get { return hud; } }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        image = GetComponent<Image>();
        orginalPosition = image.transform.localPosition;
        orginalColor = image.color;
    }

    /*color在每一場後重製否則戰鬥結束後顏色會一直透明*/
    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;
        if (isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);
        //捕捉後還原圖片大小
        transform.localScale = new Vector3(1, 1, 1);

        image.color = orginalColor;
        EnterAnimation();
    }
    /*開場進場import DG函數 讓開場進場有緩慢的效果*/
    public void EnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, orginalPosition.y);
        else
            image.transform.localPosition = new Vector3(500f, orginalPosition.y);

        image.transform.DOLocalMoveX(orginalPosition.x, 1f);
    }
    /**/
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orginalPosition.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPosition.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orginalPosition.x, 0.25f));
    }
    /*pokemon 受傷時快速閃爍效果*/
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(orginalColor, 0.1f));
    }
    /*擊敗動畫讓image 移動Y軸並且讓圖片變透明*/
    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orginalPosition.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    /*捕捉動畫,因為要同時做需要使用sequence.join ,
      DOMoveY 根據圖片元座標致BALL的座標Y軸移動距離
    */
    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        /*淡化圖向*/
        sequence.Append(image.DOFade(0, 0.5f));
        /*Pokemon Sprite Y軸移動向上*/
        sequence.Join(transform.DOLocalMoveY(orginalPosition.y + 50f, 0.5f));
        /*Pokemon Sprite 圖片縮小*/
        sequence.Join(transform.DOScale(new Vector3(0.2f, 0.2f, 1f), 0.5f));
        /*等待動畫結束*/
        yield return sequence.WaitForCompletion();
    }
    /*球破開動畫*/
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        /*淡化圖向*/
        sequence.Append(image.DOFade(1, 0.5f));
        /*Pokemon Sprite Y軸移動向上*/
        sequence.Join(transform.DOLocalMoveY(orginalPosition.y, 0.5f));
        /*Pokemon Sprite 圖片縮小*/
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        /*等待動畫結束*/
        yield return sequence.WaitForCompletion();
    }

}
