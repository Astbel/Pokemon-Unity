using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;

    /*產生一個委派 事件同於C# delegate*/
    /*Delegate就是 C++ pointer function*/
    public event Action OnEncountered;

    public float move_speed;
    private bool isMoving;
    private Vector2 input;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    /*
    Describe: Moving method
    確認使否可以移動GetAxisRaw會回傳-1,0,1且由Horizontal&Vertical來判斷上下左右
    當輸入區域不為0,0且Vector2.zero 為Vector2(0,0),Animator 配置BlendTree隨著 input來控制移動動畫
    IsWalkable 傳入Player Position 到Physics2D.OverlapCircle確認是否可走入該區域
    */
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            /*避免腳色移動對角線*/
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, move_speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        checkForEncounters();
    }

    /*OverlapCircle 三組參數1.Vector座標 2.Radius 3.LayerMask  根據Vector位置創建Radius一樣大的圓如果再layer範圍內則回傳*/
    private bool IsWalkable(Vector3 targetPos)
    {

        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }
        return true;
    }
    //遇敵 Random 1~100 當小於10則遇到敵人,新增在遇敵後取消腳色動畫
    private void checkForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }

    }
}
