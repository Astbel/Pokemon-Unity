using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text ShowHPText;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;
    [SerializeField] Text statusText;
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;
    public int MaxLevel { get; private set; } = 100;
    Dictionary<ConditionID, Color> statusColors;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        /*切換時後壁面沿用之前的狀態先取消訂閱事件*/
        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHpChanged -= UpdateHP;
        }
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        /*顯示Hp*/
        ShowHPText.text = pokemon.HP.ToString() + "/" + pokemon.MaxHp.ToString();
        SetExp();
        /*配置status color*/
        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.slp, slpColor},
            {ConditionID.par, parColor},
            {ConditionID.frz, frzColor},
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnHpChanged += UpdateHP;
    }
    /*設定升級*/
    public void SetLevel()
    {
        levelText.text = "Lv:" + _pokemon.Level;
    }
    //狀態異常標示
    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public void SetExp()
    {
        if (expBar == null) return; /*敵人使用因為敵人物件為null*/
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);

    }
    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break; /*敵人使用因為敵人物件為null*/
        /*如果有升等則清空EXP Bar*/
        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }
    float GetNormalizedExp()
    {
        int currLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);
        /*pokemon滿等時經驗條不跑動*/
        if (_pokemon.Level == MaxLevel) return 0f;
        /*標準化EXP*/
        float normalizedExp = (float)(_pokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);/*Exp Bar 最大是1所以限制Max為1*/
    }
    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    //HP Bar Update
    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
        /*跟新顯示*/
        ShowHPText.text = _pokemon.HP.ToString() + "/" + _pokemon.MaxHp.ToString();

    }

    public IEnumerator WaitForHpUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

}
