using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class SurfableWater : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
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

                var animator = initiator.GetComponent<CharacterAnim>();
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                animator.IsSurfing = true;
            }
        }

    }
}
