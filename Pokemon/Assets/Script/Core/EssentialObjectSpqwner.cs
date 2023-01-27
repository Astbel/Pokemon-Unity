using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectSpqwner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        /*確認遊戲基本元素不存在*/
        if (existingObjects.Length == 0)
        {
            //If there is a grid then spawn at it's center
            /*調整玩家起點位置*/
            var spawnPos = new Vector3(0, -70, 0);

            var grid = FindObjectOfType<Grid>();
            if (grid != null)
                spawnPos = grid.transform.position;

            Instantiate(essentialObjectsPrefab, spawnPos, Quaternion.identity);
        }
        /*調整玩家起點位置*/
    }


}
