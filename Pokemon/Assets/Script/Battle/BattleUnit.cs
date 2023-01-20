using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    Image image;
    Vector3 orginalPosition;
    Color orginalColor;
    public Pokemon Pokemon { get; set; }

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
    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        image.color =orginalColor;
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
        var sequence =DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orginalPosition.y -150f, 0.5f));
        sequence.Join(image.DOFade(0f,0.5f));
    }



}
