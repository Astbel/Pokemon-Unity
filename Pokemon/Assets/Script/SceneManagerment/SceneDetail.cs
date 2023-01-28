using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class SceneDetail : MonoBehaviour
{
    [SerializeField] List<SceneDetail> connectedScenes;

    public bool isLoaded { get; private set; }
    List<SavableEntity> savableEntities;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log($"Entetred {gameObject.name}");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);
            /*顯示連接旁邊的地圖*/
            foreach (var scene in connectedScenes)
            {
                scene.LoadScene();
            }

            /*卸載沒有連結的地圖*/
            var preScene = GameController.Instance.PrevScene;
            if (preScene != null)
            {
                var previoslyLoadedScenes = preScene.connectedScenes;
                foreach (var scene in previoslyLoadedScenes)
                {
                    /*如果場景不是連結場景以及當前場景就取消loading*/
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnLoadScene();
                }
                /*卸載非當前連結場景*/
                if (!connectedScenes.Contains(preScene))  
                    preScene.UnLoadScene();
            }
        }
    }
    /*異步函式會在加載過程繼續執行後續的程式*/
    public void LoadScene()
    {
        if (!isLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);

            isLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitiesInScene();

                SavingSystem.i.RestoreEntityStates(savableEntities);
            };

        }
    }
    public void UnLoadScene()
    {
        if (isLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }

    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currScene = SceneManager.GetSceneByName(gameObject.name);

        var saveAbleEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();

        return saveAbleEntities;
    }
}
