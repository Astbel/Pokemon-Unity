using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    private Character character;
    /*產生一個委派 事件同於C# delegate*/
    /*Delegate就是 C++ pointer function*/
    public Character Character => character;
    private Vector2 input;

    public Sprite Sprite { get => sprite; }

    public string Name { get => name; }

    private void Awake()
    {
        character = GetComponent<Character>();
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
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            /*避免腳色移動對角線*/
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }
        character.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interacPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interacPos, Color.red, 0.5f);
        /*檢測附近半圓內是否有InteractableLayer*/
        var collider = Physics2D.OverlapCircle(interacPos, 0.3f, GameLayer.Instance.InteractLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);//輸入腳色的方向
        }
    }
    /*所有觸發事件*/
    private void OnMoveOver()
    {
        /*OverlapCircle只能回傳第一個數組,OverlapCircleAll能分別回傳對應的OPP*/
        var triggerables =
        Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.offsetY), 0.2f, GameLayer.Instance.TriggerAbleLayers);

        foreach (var collider in triggerables)
        {
            var ColliderTriggerAble = collider.GetComponent<IPlayerTriggerAble>();
            if (ColliderTriggerAble != null)
            {
                ColliderTriggerAble.OnPlayerTriggered(this);
                break;
            }
        }

    }
}
