using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
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
