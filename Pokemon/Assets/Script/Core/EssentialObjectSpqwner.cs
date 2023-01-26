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
            Instantiate(essentialObjectsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }


}
