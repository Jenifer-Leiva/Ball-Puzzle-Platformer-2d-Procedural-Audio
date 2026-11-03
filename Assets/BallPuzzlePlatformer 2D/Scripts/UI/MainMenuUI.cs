using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace BallPuzzlePlatformer2D
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField, Space]
        private DataStorage m_DataStorage;

        [SerializeField, Space]
        private GameplayData m_GameplayData;


        public void BtnExit()
        {
            ProceduralSfx.Play(Sfx.UIBack);
            Application.Quit();
        }

        public void BtnLevel(int num)
        {
            ProceduralSfx.Play(Sfx.UIClick);
            m_GameplayData.LevelNumber = num;
            SceneManager.LoadScene("Scene_1");
        }
    }
}