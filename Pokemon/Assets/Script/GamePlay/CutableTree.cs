using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogManger.Instance.ShowDialogText("This tree looks like it can be cut");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(m => m.Base.Name == "Cut"));

        if (pokemonWithCut != null)
        {
            int selectedChoice = 0;
            yield return DialogManger.Instance.ShowDialogText($"Should{pokemonWithCut.Base.Name} use cut?",
                choices: new List<string>() { "YES", "NO" },
                onChoiceSelected: (selection) => selectedChoice = selection);
            if (selectedChoice == 0)
            {
                //YES
                yield return DialogManger.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} use cut!");
                gameObject.SetActive(false);
            }
        }

    }
}
