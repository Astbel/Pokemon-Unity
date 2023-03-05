using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;

        yield return DialogManger.Instance.ShowDialog(dialog,
        new List<string>() { "YES", "NO" },
        (choiceIndex) => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //Yes
            yield return Fader.i.FaderIn(0.5f);

            var playerParty = player.GetComponent<PokemonParty>();

            playerParty.Pokemons.ForEach(p => p.Heal());

            playerParty.PartyUpdated();

            yield return Fader.i.FaderOut(0.5f);

            yield return DialogManger.Instance.ShowDialogText($"Your pokemon should be fully heal now ");
        }
        else
        {
            //No
            yield return DialogManger.Instance.ShowDialogText($"Okay! Come Back when ever you need ! ");

        }

    }
}
