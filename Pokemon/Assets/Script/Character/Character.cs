using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnim animator;
    public float move_speed;

    public bool IsMoving { get; private set; }
    private void Awake()
    {
        animator = GetComponent<CharacterAnim>();
        SetPositionAndSnapToTile(transform.position);
    }
    public float offsetY { get; private set; } = 0.0f;
    public CharacterAnim Animator { get => animator; }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        /*為了讓圖片對其中心 Ex當前座標2.3 ->floor後->2+0.5*/
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.x) + 0.5f + offsetY;

        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        /*如果不能走過從協程break出來*/
        if (!IsPathClear(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, move_speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    /*檢測腳色走過是否為合法能走的區域*/
    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;/*計算距離*/
        var direction = diff.normalized;/*向量位置*/

        if (Physics2D.BoxCast(transform.position + direction, new Vector2(0.2f, 0.2f), 0f, direction, diff.magnitude - 1,
            GameLayer.Instance.SolidLayer | GameLayer.Instance.InteractLayer | GameLayer.Instance.PlayerLayer) == true)
            return false;
        else
            return true;
    }

    /*確認是否為可走動區域*/
    /*OverlapCircle 三組參數1.Vector座標 2.Radius 3.LayerMask  根據Vector位置創建Radius一樣大的圓如果再layer範圍內則回傳*/
    private bool IsWalkable(Vector3 targetPos)
    {

        if (Physics2D.OverlapCircle(targetPos, 0.1f, GameLayer.Instance.SolidLayer | GameLayer.Instance.InteractLayer) != null)
        {
            return false;
        }
        return true;
    }

    /*NPC 對話轉向玩家檢測X和Y軸*/
    /*                                1      */
    /*NPC方向只能看四個軸其中一種  -1   0   1  */
    /*                               -1      */
    public void LookTowards(Vector3 target)
    {
        var xdiff = Mathf.Floor(target.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(target.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)  //當腳色面向其中一個X或是Y一定不會變動
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.Log("Error in look Towards:You cant ask the character to look diagonally. ");

    }

}
