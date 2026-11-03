using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace BallPuzzlePlatformer2D
{
    public class LoseUI : MonoBehaviour
    {

        void OnEnable()
        {
            ProceduralSfx.Play(Sfx.Lose);
        }

        public void BtnRestart()
        {
            ProceduralSfx.Play(Sfx.UIClick);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BtnExit()
        {
            ProceduralSfx.Play(Sfx.UIBack);
            SceneManager.LoadScene("MainMenu");
        }

    }

}