using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Protal : MonoBehaviour, IPlayerTriggerAble
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifier destinationIdentifier;
    [SerializeField] Transform spawnPoint;  /*腳色重生位置*/

    // public object First { get; private set; }
    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }
    /*切換場景  PauseGame的作用是在切換場景時不能讓玩家繼續操作腳色,由於會一直計算下一個位置會導致單地圖不一樣時在切換時腳色一樣去走到該座標*/
    IEnumerator SwitchScene()
    {
        /*確保重生點不被破壞*/
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PausedGame(true);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        /*重新把player座標定位,目標位置需要一致再多窗口使用*/
        var destPortal = FindObjectsOfType<Protal>().First(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);

        GameController.Instance.PausedGame(false);

        Destroy(gameObject);

    }

    public Transform SpawnPoint => spawnPoint;


}


public enum DestinationIdentifier
{
    A, B, C, D, E
}
