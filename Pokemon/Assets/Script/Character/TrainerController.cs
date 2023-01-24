using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    bool battleLost = false;
    Character character;

    public Sprite Sprite { get => sprite; }

    public string Name { get => name; }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }
    /*腳色移動動畫*/
    private void Update()
    {
        character.HandleUpdate();
    }

    //對話戰鬥
    public void Interact(Transform initiator)
    {
        /*面相玩家*/
        character.LookTowards(initiator.position);

        if (!battleLost)
        {
            //show dialog
            StartCoroutine(DialogManger.Instance.ShowDialog(dialog, () =>
             {
                 GameController.Instance.StartTrainerBattle(this);
             }));
        }
        else
        {
            StartCoroutine(DialogManger.Instance.ShowDialog(dialogAfterBattle));
        }

    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);
        /*訓練家跟玩家距離要移動的距離*/
        var diff = player.transform.position - transform.position;

        var moveVac = diff - diff.normalized;
        /*moveVactor保持為int型別*/
        moveVac = new Vector2(Mathf.Round(moveVac.x), Mathf.Round(moveVac.y));

        yield return character.Move(moveVac);

        //show dialog
        StartCoroutine(DialogManger.Instance.ShowDialog(dialog, () =>
         {
             GameController.Instance.StartTrainerBattle(this);
         }));

    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    /*Tranier Colider隨著面向轉向*/
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }


}
