using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectUI : MonoBehaviour
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Color highlightedColor;
    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMove, MoveBase newMove)
    {
        for (int i = 0; i < currentMove.Count; ++i)
        {
            moveTexts[i].text = currentMove[i].Name;
        }

        moveTexts[currentMove.Count].text = newMove.Name;
    }

    public void HandleForgetMoves(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, PokemonBase.MaxNumOfMoves);

        UpdateMoveSelection(currentSelection);

        /*學則要忘記的技能,MoveSelected不知道 newmove OPP,直接invoke調用battleSystem內的method去執行*/
        if (Input.GetKeyDown(KeyCode.Z))
            onSelected?.Invoke(currentSelection);
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < PokemonBase.MaxNumOfMoves + 1; i++)
        {
            if (i == selection)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
    }

}
