using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
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
            StartCoroutine(Interact());
    }

    IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interacPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interacPos, Color.red, 0.5f);
        /*檢測附近半圓內是否有InteractableLayer*/
        var collider = Physics2D.OverlapCircle(interacPos, 0.3f, GameLayer.Instance.InteractLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);//輸入腳色的方向
        }
    }
    IPlayerTriggerAble currentlyInTrigger;
    /*所有觸發事件Layer 腳本中設定是否可以重複觸發,如果是可重複觸發則再次觸發否則break出去*/
    private void OnMoveOver()
    {
        /*OverlapCircle只能回傳第一個數組,OverlapCircleAll能分別回傳對應的OPP*/
        var triggerables =
        Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.offsetY), 0.2f, GameLayer.Instance.TriggerAbleLayers);

        IPlayerTriggerAble ColliderTriggerAble = null;

        foreach (var collider in triggerables)
        {
            ColliderTriggerAble = collider.GetComponent<IPlayerTriggerAble>();
            if (ColliderTriggerAble != null)
            {
                /*部分事件觸發避免重複確認boolean,如果設定上false則break出去不在次觸發*/
                if (ColliderTriggerAble == currentlyInTrigger && !ColliderTriggerAble.TriggerRepeatedly)
                    break;

                ColliderTriggerAble.OnPlayerTriggered(this);
                currentlyInTrigger = ColliderTriggerAble;
                break;
            }
        }
        /*count當前沒有可以觸發對象*/
        if (triggerables.Count() == 0 || ColliderTriggerAble != currentlyInTrigger)
            currentlyInTrigger = null;

    }

    /*Save 紀錄需要有SerializeField才能被序列化,object在C#泛型可以回傳任意type*/
    public object CaptureState()
    {
        /*將要儲存的放進一個類別*/
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            /*用Linq的select將Party中所有的元素再轉成GetSaveData 類別最後再轉成List*/
            pokemons = GetComponent<PokemonParty>().Pokemons.Select(p => p.GetSaveData()).ToList()
        };

        return saveData;
    }
    /*Load data*/
    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);
        /*讀取player隊伍*/
        GetComponent<PokemonParty>().Pokemons = saveData.pokemons.Select(s => new Pokemon(s)).ToList();
    }
}


/*Class for save player data*/
[Serializable]
public class PlayerSaveData
{
    public float[] position;

    public List<PokemonSaveData> pokemons;
}