using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*Teleports the player to a different position without switching scenes*/
public class LocationPortal : MonoBehaviour, IPlayerTriggerAble
{
    [SerializeField] DestinationIdentifier destinationIdentifier;
    [SerializeField] Transform spawnPoint;  /*腳色重生位置*/
    Fader fader; /*切換場景的動畫*/
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        /*停止腳色移動動畫*/
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());

    }
    /*切換場景  PauseGame的作用是在切換場景時不能讓玩家繼續操作腳色,由於會一直計算下一個位置會導致單地圖不一樣時在切換時腳色一樣去走到該座標*/
    IEnumerator Teleport()
    {


        GameController.Instance.PausedGame(true);

        yield return fader.FaderIn(0.5f);


        /*重新把player座標定位,目標位置需要一致再多窗口使用*/
        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);

        yield return fader.FaderOut(0.5f);

        GameController.Instance.PausedGame(false);


    }
    public Transform SpawnPoint => spawnPoint;


}
