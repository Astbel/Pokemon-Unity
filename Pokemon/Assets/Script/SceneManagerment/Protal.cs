using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Protal : MonoBehaviour, IPlayerTriggerAble
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;  /*腳色重生位置*/

    // public object First { get; private set; }
    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }
    /*切換場景*/
    IEnumerator SwitchScene()
    {
        /*確保重生點不被破壞*/
        DontDestroyOnLoad(gameObject);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        /*重新把player座標定位*/
        var destPortal = FindObjectsOfType<Protal>().First(x => x != this);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);
        Destroy(gameObject);

    }

    public Transform SpawnPoint => spawnPoint;


}
