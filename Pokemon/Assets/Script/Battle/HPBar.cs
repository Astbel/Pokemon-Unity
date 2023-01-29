using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool IsUpdating { get; private set; }

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }
    /*讓HP條更滑順 當前hp減掉傷害值 要大於0便免寫調出現負數*/
    public IEnumerator SetHPSmooth(float newHP)
    {
        IsUpdating = true;

        float curHp = health.transform.localPosition.x;

        float changeAmt = curHp - newHP;

        while (curHp - newHP > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.fixedDeltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;  //當本次IEnumerator 結束停止corouite
        }
        health.transform.localScale = new Vector3(newHP, 1f);

        IsUpdating = false;
    }

}
