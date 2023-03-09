using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _YabuGames.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        public int sceneID;
        public int levelID;

        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null) 
            {
                Destroy(this);
                return;
            }

            Instance = this;

            #endregion
            
            GetValues();
        }
        
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        #region Subscribtons

        private void Subscribe()
        {
            CoreGameSignals.Instance.OnSave += Save;
            CoreGameSignals.Instance.OnLevelWin += LevelWin;
            CoreGameSignals.Instance.OnLevelLoad += LoadScene;
        }
        
        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnSave -= Save;
            CoreGameSignals.Instance.OnLevelWin -= LevelWin;
            CoreGameSignals.Instance.OnLevelLoad -= LoadScene;
        }

        #endregion
        
        private void GetValues()
        {
            sceneID = PlayerPrefs.GetInt("sceneID", 0);
            levelID = PlayerPrefs.GetInt("levelID", 1);
        }

        private void LevelWin()
        {
           // if(false) return;
            levelID++;
        }

        private void Save()
        {
            PlayerPrefs.SetInt("sceneID",sceneID);
            PlayerPrefs.SetInt("levelID",levelID);
        }

        private void LoadScene()
        {
            SceneManager.LoadScene(sceneID);
        }
    }
}