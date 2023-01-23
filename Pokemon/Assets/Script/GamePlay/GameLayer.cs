using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;//Trainer 觀測用BOX
    public static GameLayer Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public LayerMask SolidLayer { get => solidObjectsLayer; }
    public LayerMask InteractLayer { get => interactableLayer; }
    public LayerMask GrassLayer { get => grassLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask FovLayer { get => fovLayer; }
}
