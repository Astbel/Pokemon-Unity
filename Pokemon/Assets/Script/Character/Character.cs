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
    }

    public CharacterAnim Animator { get => animator; }


    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        /*如果不能走過從協程break出來*/
        if (!IsWalkable(targetPos))
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
}
