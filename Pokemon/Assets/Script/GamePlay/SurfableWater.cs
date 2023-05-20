using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class SurfableWater : MonoBehaviour, Interactable,IPlayerTriggerAble
{
    bool isJumpingToWater = false;
    //在水中類似草叢也是連續觸發模式
    public bool TriggerRepeatedly => true;

    
    // 用來避免遇敵過於頻繁的時間間隔
    private const float EncounterCooldown = 5.0f;
    private float lastEncounterTime;

    public IEnumerator Interact(Transform initiator)
    {
        //檢測玩家是否還是衝浪狀態
        var animator = initiator.GetComponent<CharacterAnim>();
        if (animator.IsSurfing || isJumpingToWater)
            yield break;
        yield return DialogManger.Instance.ShowDialogText("This water is deep blue!");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Surf"));

        if (pokemonWithSurf != null)
        {
            int selectedChoice = 0;
            yield return DialogManger.Instance.ShowDialogText($"Should{pokemonWithSurf.Base.Name} use surf?",
                choices: new List<string>() { "YES", "NO" },
                onChoiceSelected: (selection) => selectedChoice = selection);
            if (selectedChoice == 0)
            {
                //YES
                yield return DialogManger.Instance.ShowDialogText($"{pokemonWithSurf.Base.Name} use surf!");

                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                isJumpingToWater = true;
                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                isJumpingToWater = false;
                animator.IsSurfing = true;
            }
        }

    }

    public void OnPlayerTriggered(PlayerController player)
    {
         if (Time.time - lastEncounterTime >= EncounterCooldown && UnityEngine.Random.Range(1, 101) <= 10)
        {
            GameController.Instance.StartBattle(BattleTrigger.Water);

            lastEncounterTime = Time.time;
        }
    }
}
