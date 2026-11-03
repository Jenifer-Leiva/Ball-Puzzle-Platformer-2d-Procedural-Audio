
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BallPuzzlePlatformer2D
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField, Space]
        private GameplayData m_GameplayData;

        [SerializeField, Space]
        private Contents m_Contents;

        [SerializeField, Space]
        private DataStorage m_DataStorage;

        [SerializeField]
        private Text Text_LevelNum;

        [SerializeField, Space]
        public RectTransform MainCanvas;

        public static InGameUI Current;

        public Image[] m_SelectionFrames;

        [HideInInspector]
        public int m_SelectedItem = -1;

        void Awake()
        {
            Current = this;
        }

        void Start()
        {
            // Cursor.visible = false;
        }

        void Update()
        {
            Text_LevelNum.text = "Level " +
                (m_GameplayData.LevelNumber + 1).ToString();
        }

        public void BtnExit()
        {
            // Conservar el sonido de la compañera.
            ProceduralSfx.Play(Sfx.UIBack);

            // Ejecutar tu salida con fade out.
            StartCoroutine(Co_BtnExit());
        }

        private IEnumerator Co_BtnExit()
        {
            MusicDirector director = MusicDirector.Instancia;

            if (director != null)
            {
                director.FadeOutYDetener(1.5f);

                // Esperar a que termine el desvanecimiento.
                yield return new WaitForSecondsRealtime(1.6f);
            }

            SceneManager.LoadScene("MainMenu");
        }
    }
}
