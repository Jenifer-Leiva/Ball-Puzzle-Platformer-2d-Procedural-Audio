using System;
using UnityEngine;

namespace View.Control
{
    public class GameAudio : MonoBehaviour
    {
        private const string MusicStatusKey = "music.status";
        private const string SfxStatusKey = "sfx.status";

        public AudioClip[] SfxClips;

        private AudioSource _musicSource;
        private int _musicVolumeTweenId;
        private ProceduralMusic _proceduralMusic;
        private ProceduralEffects _proceduralEffects;
        private bool _musicEnabled = true;
        public bool MusicEnabled
        {
            get { return _musicEnabled; }
            set
            {
                if (_musicEnabled != value)
                {
                    LeanTween.cancel(_musicVolumeTweenId);
                }

                _musicEnabled = value;

                if (_musicSource == null)
                    _musicSource = GetComponent<AudioSource>();

                PlayerPrefs.SetInt(MusicStatusKey, value ? 0 : 1);
            }
        }

        private bool _sfxEnabled = true;
        public bool SfxEnabled
        {
            get { return _sfxEnabled; }
            set
            {
                _sfxEnabled = value;
                PlayerPrefs.SetInt(SfxStatusKey, value ? 0 : 1);
            }
        }

        private void Awake()
        {
            StartEffects();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartMusic();

            if (!PlayerPrefs.HasKey(MusicStatusKey))
            {
                PlayerPrefs.SetInt(MusicStatusKey, 0);
            }
            if (!PlayerPrefs.HasKey(SfxStatusKey))
            {
                PlayerPrefs.SetInt(SfxStatusKey, 0);
            }

            MusicEnabled = PlayerPrefs.GetInt(MusicStatusKey) == 0;
            SfxEnabled = PlayerPrefs.GetInt(SfxStatusKey) == 0;
        }

       public void Play(GameClip clip, float delay = 0f, float volume = 1.5f, float startTime = 0f)
        {
            if (!enabled || !SfxEnabled) return;

            AudioClip audioClip = SfxClips[(uint)clip];

            if (audioClip == null) return;

            //  objeto independiente  efecto de sonido
            GameObject sfxObject = new GameObject("SFX_" + clip);

            AudioSource source = sfxObject.AddComponent<AudioSource>();

            source.clip = audioClip;
            source.volume = volume;
            source.spatialBlend = 0f;
            source.time = Mathf.Clamp(startTime, 0f, audioClip.length);

            source.PlayDelayed(delay);

            
            Destroy(sfxObject, delay + audioClip.length - source.time + 0.1f);
        }

       

        private void StartEffects()
        {
            _proceduralEffects = GetComponent<ProceduralEffects>();

            if (_proceduralEffects == null)
                _proceduralEffects = gameObject.AddComponent<ProceduralEffects>();

            int count = Enum.GetValues(typeof(GameClip)).Length;

            if (SfxClips == null)
                SfxClips = new AudioClip[count];
            else if (SfxClips.Length < count)
                Array.Resize(ref SfxClips, count);

            _proceduralEffects.FillClips(SfxClips);
        }

        private void StartMusic()
        {
            _proceduralMusic = GetComponent<ProceduralMusic>();

            if (_proceduralMusic == null)
            {
                Debug.LogError("NO SE ENCONTRÓ PROCEDURAL MUSIC");
                return;
            }

            if (_proceduralMusic != null)
            {
            
                _musicSource = GetComponent<AudioSource>();

                if (_musicSource == null)
                    _musicSource = gameObject.AddComponent<AudioSource>();

                _proceduralMusic.enabled = true;
                return;
            }
        }

    }

    public enum GameClip
    {
        GameStart,
        WinBoard,
        NodeEnter,
        NodeLeave,
        MovePushHigh,
        MovePullHigh,
        MovePullMid,
        MovePushMid,
        MovePullLow,
        MovePushLow,
        ArcMoveHigh,
        NodeRotate90,
        InvalidRotate,
        MenuSelect,
        GameEnd,
        LevelEnable,
        ArcMoveMid,
        ArcMoveLow
    }

    public enum MusicClip
    {
        Ambient01,
        Ambient02
    }
}