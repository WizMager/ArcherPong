using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
        public static RoomManager Instance;

        private void Awake()
        {
            Debug.Log("Try create room manager");
                if (Instance)
                {
                     Destroy(gameObject);
                     return;
                }
                
                DontDestroyOnLoad(gameObject);
                Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 1)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Playermanager"), Vector3.zero, Quaternion.identity);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ArrowManager"), Vector3.zero, Quaternion.identity);
            }
        }
}