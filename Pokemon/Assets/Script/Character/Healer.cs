using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        yield return DialogManger.Instance.ShowDialog(dialog);

        yield return Fader.i.FaderIn(0.5f);

        var playerParty = player.GetComponent<PokemonParty>();

        playerParty.Pokemons.ForEach(p => p.Heal());

        playerParty.PartyUpdated();

        yield return Fader.i.FaderOut(0.5f);
    }
}
