using System;
using System.IO;
using UnityEngine;

namespace View.Control
{
    public class ProceduralEffects : MonoBehaviour
    {
        private const double TwoPi = 2.0 * System.Math.PI;

        private static readonly float[] Saw5 = { 1.00f, 0.48f, 0.30f, 0.23f, 0.19f };
        private static readonly float[] Two = { 1.00f, 0.48f };
        private static readonly float[] Pure = { 1.00f };
        private static readonly float[] Saw8 =
            { 1f, 1f / 2, 1f / 3, 1f / 4, 1f / 5, 1f / 6, 1f / 7, 1f / 8 };

        private AudioClip _gameStart;
        private AudioClip _win;
        private AudioClip _invalidRotate;
        private AudioClip _effect1;
        private AudioClip _effect2;
        private AudioClip _effect3;
        private AudioClip _effect4;
        private AudioClip _effect5;
        private bool _generated;

        public AudioClip Effect1Clip { get { Generate(); return _effect1; } }
        public AudioClip Effect2Clip { get { Generate(); return _effect2; } }
        public AudioClip Effect3Clip { get { Generate(); return _effect3; } }
        public AudioClip Effect4Clip { get { Generate(); return _effect4; } }
        public AudioClip Effect5Clip { get { Generate(); return _effect5; } }

        public void FillClips(AudioClip[] clips)
        {
            Generate();

            Set(clips, GameClip.GameStart, _gameStart);
            Set(clips, GameClip.WinBoard, _win);
            Set(clips, GameClip.InvalidRotate, _invalidRotate);

            Set(clips, GameClip.LevelEnable, _effect1);
            Set(clips, GameClip.GameEnd, _effect2);
            Set(clips, GameClip.NodeRotate90, _effect3);
            Set(clips, GameClip.MenuSelect, _effect4);
            Set(clips, GameClip.MovePushHigh, _effect5);
            Set(clips, GameClip.MovePullHigh, _effect5);
            Set(clips, GameClip.MovePullMid, _effect5);
            Set(clips, GameClip.MovePushMid, _effect5);
            Set(clips, GameClip.MovePullLow, _effect5);
            Set(clips, GameClip.MovePushLow, _effect5);
        }

        private static void Set(AudioClip[] clips, GameClip index, AudioClip clip)
        {
            if ((int)index < clips.Length)
                clips[(int)index] = clip;
        }

        private void Generate()
        {
            if (_generated) return;
            _generated = true;

            int sampleRate = AudioSettings.outputSampleRate;
            _gameStart = MakeClip("GameStart", RenderGameStart(sampleRate), sampleRate);
            _win = MakeClip("WinBoard", RenderWin(sampleRate), sampleRate);
            _invalidRotate = MakeClip("InvalidRotate", RenderInvalidRotate(sampleRate), sampleRate);
            _effect1 = MakeClip("Effect1", RenderEffect1(sampleRate), sampleRate);
            _effect2 = MakeClip("Effect2", RenderEffect2(sampleRate), sampleRate);
            _effect3 = MakeClip("Effect3", RenderEffect3(sampleRate), sampleRate);
            _effect4 = MakeClip("Effect4", RenderEffect4(sampleRate), sampleRate);
            _effect5 = MakeClip("Effect5", RenderEffect5(sampleRate), sampleRate);
        }

        private static float[] RenderGameStart(int sampleRate)
        {
            float[] data = new float[sampleRate * 1];

            AddNote(data, sampleRate, 130.81f, Saw5, 0.00, 0.65, 0.30f, 0.20f, 0.30f, 0.10f, 0.37f);
            AddNote(data, sampleRate, 164.81f, Saw5, 0.62, 0.25, 0.08f, 0.05f, 0.30f, 0.10f, 0.37f);

            Limit(data);
            return data;
        }

        private static float[] RenderWin(int sampleRate)
        {
            float[] data = new float[sampleRate * 2];

            AddNote(data, sampleRate, 587.33f, Two, 0.25, 0.80, 0.05f, 0.40f, 0.70f, 0.30f, 0.26f);   // Re5
            AddNote(data, sampleRate, 659.26f, Two, 0.45, 0.80, 0.05f, 0.40f, 0.70f, 0.30f, 0.26f);   // Mi5
            AddNote(data, sampleRate, 698.46f, Two, 0.80, 0.90, 0.05f, 0.40f, 0.70f, 0.30f, 0.26f);   // Fa5

            Tremolo(data, sampleRate, 9.33f, 0.5f);

            Limit(data);
            return data;
        }

        private static float[] RenderInvalidRotate(int sampleRate)
        {
            float[] data = new float[sampleRate * 1];

            AddNote(data, sampleRate, 185.00f, Pure, 0.00, 0.95, 0.20f, 0.55f, 0.15f, 0.20f, 0.20f);
            AddNote(data, sampleRate, 349.23f, Pure, 0.00, 0.95, 0.20f, 0.55f, 0.15f, 0.20f, 0.15f);
            AddNote(data, sampleRate, 392.00f, Pure, 0.00, 0.95, 0.20f, 0.55f, 0.15f, 0.20f, 0.08f);

            RingSquare(data, sampleRate, 5f, 6f);

            Limit(data);
            return data;
        }

        private static float[] RenderEffect1(int sampleRate)
        {
            float[] data = new float[sampleRate * 2];

            AddNote(data, sampleRate, 654.3f, Two, 0.25, 1.15, 0.14f, 0.85f, 0.05f, 0.15f, 0.11f);
            AddNote(data, sampleRate, 664.3f, Two, 0.25, 1.15, 0.14f, 0.85f, 0.05f, 0.15f, 0.11f);

            Limit(data);
            return data;
        }

        private static float[] RenderEffect2(int sampleRate)
        {
            float[] data = new float[sampleRate * 2];

            AddNote(data, sampleRate, 349.23f, Two, 0.20, 1.75, 0.20f, 1.20f, 0.10f, 0.35f, 0.16f);

            Tremolo(data, sampleRate, 5.6f, 0.95f);

            Limit(data);
            return data;
        }

        private static float[] RenderEffect3(int sampleRate)
        {
            float[] data = new float[(int)(sampleRate * 2.5)];

            const float low = 87.31f;
            const float high = 98.00f;
            const float lfoRate = 3.0f;
            const double start = 0.45;
            const double length = 1.90;

            double phase = 0.0;

            for (int i = 0; i < data.Length; i++)
            {
                double t = i / (double)sampleRate;

                float square = 0.5f + 0.5f * (float)System.Math.Tanh(8.0 * System.Math.Sin(TwoPi * lfoRate * t));
                float frequency = Mathf.Lerp(low, high, square);

                phase += TwoPi * frequency / sampleRate;
                phase %= TwoPi;

                float sample = 0f;
                for (int h = 0; h < Saw8.Length; h++)
                    sample += Saw8[h] * (float)System.Math.Sin(phase * (h + 1));

                float envelope = Adsr(t - start, length, 0.20f, 1.00f, 0.35f, 0.60f);
                data[i] = 0.13f * envelope * sample;
            }

            Limit(data);
            return data;
        }

        private static float[] RenderEffect4(int sampleRate)
        {
            float[] data = new float[sampleRate * 1];

            AddNote(data, sampleRate, 130.81f, Saw5, 0.28, 0.60, 0.08f, 0.45f, 0.05f, 0.12f, 0.20f); 
            AddNote(data, sampleRate, 146.83f, Saw5, 0.28, 0.60, 0.08f, 0.45f, 0.05f, 0.12f, 0.20f);

            Limit(data);
            return data;
        }

        private static float[] RenderEffect5(int sampleRate)
        {
            float[] data = new float[sampleRate * 2];

            AddNote(data, sampleRate, 659.26f, Two, 0.32, 1.05, 0.10f, 0.90f, 0.15f, 0.10f, 0.23f);   // Mi5

            Tremolo(data, sampleRate, 10f, 0.8f);

            Limit(data);
            return data;
        }

        private static void AddNote(float[] data, int sampleRate, float frequency, float[] harmonics,
                                    double start, double duration,
                                    float attack, float decay, float sustain, float release, float level)
        {
            int first = (int)(start * sampleRate);
            int last = System.Math.Min(data.Length, (int)((start + duration) * sampleRate));

            for (int i = first; i < last; i++)
            {
                double t = i / (double)sampleRate;
                float envelope = Adsr(t - start, duration, attack, decay, sustain, release);

                double w = TwoPi * frequency * t;
                float sample = 0f;
                for (int h = 0; h < harmonics.Length; h++)
                    sample += harmonics[h] * (float)System.Math.Sin(w * (h + 1));

                data[i] += level * envelope * sample;
            }
        }

        private static float Adsr(double t, double duration, float a, float d, float s, float r)
        {
            if (t < 0.0 || t >= duration) return 0f;

            double total = a + d + r;
            double k = total > duration ? duration / total : 1.0;
            double aa = a * k, dd = d * k, rr = r * k;

            if (t < aa) return (float)(t / aa);
            t -= aa;

            if (t < dd) return Mathf.Lerp(1f, s, (float)(t / dd));
            t -= dd;

            double sustainLength = duration - aa - dd - rr;
            if (t < sustainLength) return s;
            t -= sustainLength;

            return rr > 0.0 ? s * (1f - (float)(t / rr)) : 0f;
        }

        private static void Tremolo(float[] data, int sampleRate, float rate, float depth)
        {
            for (int i = 0; i < data.Length; i++)
            {
                double t = i / (double)sampleRate;
                float lfo = 0.5f + 0.5f * (float)System.Math.Sin(TwoPi * rate * t);
                data[i] *= 1f - depth * lfo;
            }
        }

        private static void RingSquare(float[] data, int sampleRate, float rate, float sharpness)
        {
            double norm = System.Math.Tanh(sharpness);
            for (int i = 0; i < data.Length; i++)
            {
                double t = i / (double)sampleRate;
                double square = System.Math.Tanh(sharpness * System.Math.Sin(TwoPi * rate * t)) / norm;
                data[i] *= (float)square;
            }
        }

        private static void Limit(float[] data)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = Mathf.Clamp(data[i], -1f, 1f);
        }

        private static AudioClip MakeClip(string name, float[] samples, int sampleRate)
        {
            AudioClip clip = AudioClip.Create(name, samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}