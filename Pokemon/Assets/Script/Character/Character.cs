using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
//    public  IEnumerator Move(Vector2 moveVec)
//     {
//         animator.MoveX = moveVec.x;
//         animator.MoveY = moveVec.y;
//         var targetPos = transform.position;
//         targetPos.x += moveVec.x;
//         targetPos.y += moveVec.y;

//         if(!IsWalkable(targetPos))
//            yield break;

//         isMoving = true;

//         while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
//         {
//             transform.position = Vector3.MoveTowards(transform.position, targetPos, move_speed * Time.deltaTime);
//             yield return null;
//         }
//         transform.position = targetPos;

//         isMoving = false;

//         checkForEncounters();
//     }

//     private bool IsWalkable(Vector3 targetPos)
//     {

//         if (Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | InteractableLayer) != null)
//         {
//             return false;
//         }
//         return true;
//     }
}
