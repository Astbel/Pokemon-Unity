using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetail : MonoBehaviour
{
    [SerializeField] List<SceneDetail> connectedScenes;

    public bool isLoaded { get; private set; }
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
            if (GameController.Instance.PrevScene != null)
            {
                var previoslyLoadedScenes = GameController.Instance.PrevScene.connectedScenes;
                foreach (var scene in previoslyLoadedScenes)
                {
                    /*如果場景不是連結場景以及當前場景就取消loading*/
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnLoadScene();
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!isLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);

            isLoaded = true;
        }
    }
    public void UnLoadScene()
    {
        if (isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);

            isLoaded = false;
        }
    }
}
