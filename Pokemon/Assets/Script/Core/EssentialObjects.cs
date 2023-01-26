using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    /*切換場景時不要摧毀遊戲基本物件*/
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
