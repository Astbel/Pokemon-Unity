using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Ledge : MonoBehaviour
{
    [SerializeField] int xDir;
    [SerializeField] int yDir;

    /// <summary>
    /// 遊戲開始時提示柵欄跳躍顏色關閉
    /// </summary>
    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled=false;
    }

    public bool TryToJump(Character character, Vector2 moveDir)
    {
        if (moveDir.x == xDir && moveDir.y == yDir)
        {
            StartCoroutine(Jump(character));
            return true;
        }
        return false;
    }
    //dotween jump 函數(跳要距離,跳的力量,移動距離,間格時間)
    IEnumerator Jump(Character character)
    {
        //在跳躍時靜止玩家控制,當結束切回控制權
        GameController.Instance.PausedGame(true);

        var jumpDest =character.transform.position+new Vector3(xDir,yDir)*2;
        yield return character.transform.DOJump(jumpDest,0.3f,1,0.5f).WaitForCompletion();

        GameController.Instance.PausedGame(false);
    }

}
