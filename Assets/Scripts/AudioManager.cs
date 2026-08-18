using System.Collections.Generic;
using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Gera e toca sons proceduralmente para o jogo musical.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Configuracao de Audio")]
        [SerializeField] private int sampleRate = 44100;
        [SerializeField] private float noteDuration = 0.6f;
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 0.5f;

        [Header("Sintese do Instrumento")]
        [SerializeField] private float attackTime = 0.01f;
        [SerializeField] private float decayTime = 0.3f;
        [SerializeField] private float sustainLevel = 0.3f;
        [SerializeField] private float releaseTime = 0.2f;

        private AudioSource audioSource;
        private Dictionary<int, AudioClip> noteClips = new Dictionary<int, AudioClip>();

        [Header("Timbre")]
        [SerializeField] private AnimationCurve harmonicWeights = new AnimationCurve(
            new Keyframe(1f, 1f),
            new Keyframe(2f, 0.25f),
            new Keyframe(3f, 0.125f),
            new Keyframe(4f, 0.0625f)
        );

        // Frequencias das notas na oitava 4 (Hz)
        private readonly Dictionary<int, float> laneFrequencies = new Dictionary<int, float>
        {
            { 0, 261.63f }, // C4 - Do
            { 1, 293.66f }, // D4 - Re
            { 2, 329.63f }, // E4 - Mi
            { 3, 349.23f }, // F4 - Fa
            { 4, 392.00f }  // G4 - Sol
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;

            GenerateNoteClips();
        }

        /// <summary>
        /// Gera os AudioClips para cada lane/instrumento.
        /// </summary>
        private void GenerateNoteClips()
        {
            foreach (var pair in laneFrequencies)
            {
                AudioClip clip = GenerateNoteClip(pair.Key, pair.Value);
                noteClips[pair.Key] = clip;
            }
        }

        /// <summary>
        /// Gera um AudioClip com uma nota sintetizada.
        /// </summary>
        private AudioClip GenerateNoteClip(int laneIndex, float frequency)
        {
            int sampleCount = Mathf.RoundToInt(sampleRate * noteDuration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / sampleRate;

                // Onda senoidal base
                float sample = Mathf.Sin(2f * Mathf.PI * frequency * time);

                // Harmonicos configuraveis pelo instrumento selecionado
                if (harmonicWeights != null)
                {
                    foreach (Keyframe key in harmonicWeights.keys)
                    {
                        int harmonic = Mathf.RoundToInt(key.time);
                        if (harmonic <= 0) continue;
                        sample += key.value * Mathf.Sin(2f * Mathf.PI * frequency * harmonic * time);
                    }
                }

                // Envelope ADSR simplificado
                float envelope = CalculateEnvelope(time, noteDuration);

                samples[i] = sample * envelope * masterVolume;
            }

            AudioClip clip = AudioClip.Create($"Note_Lane{laneIndex}_{frequency}Hz", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }

        /// <summary>
        /// Calcula o envelope ADSR simples para o som.
        /// </summary>
        private float CalculateEnvelope(float time, float totalDuration)
        {
            // Attack
            if (time < attackTime)
            {
                return time / attackTime;
            }

            // Decay
            if (time < attackTime + decayTime)
            {
                float decayProgress = (time - attackTime) / decayTime;
                return Mathf.Lerp(1f, sustainLevel, decayProgress);
            }

            // Sustain
            if (time < totalDuration - releaseTime)
            {
                return sustainLevel;
            }

            // Release
            float releaseProgress = (time - (totalDuration - releaseTime)) / releaseTime;
            return Mathf.Lerp(sustainLevel, 0f, releaseProgress);
        }

        /// <summary>
        /// Aplica as configuracoes de um instrumento e regenera os clips.
        /// </summary>
        public void ApplyInstrument(InstrumentData instrument)
        {
            if (instrument == null) return;

            noteDuration = instrument.noteDuration;
            attackTime = instrument.attackTime;
            decayTime = instrument.decayTime;
            sustainLevel = instrument.sustainLevel;
            releaseTime = instrument.releaseTime;
            harmonicWeights = instrument.harmonicWeights;

            GenerateNoteClips();
        }

        /// <summary>
        /// Toca o som da nota correspondente a uma lane.
        /// </summary>
        public void PlayNote(int laneIndex)
        {
            if (noteClips.TryGetValue(laneIndex, out AudioClip clip))
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"AudioManager: nao encontrou clip para lane {laneIndex}");
            }
        }

        /// <summary>
        /// Toca som de erro quando uma nota e perdida.
        /// </summary>
        public void PlayMissSound()
        {
            AudioClip missClip = GenerateNoiseClip(0.2f, 0.2f);
            audioSource.PlayOneShot(missClip);
        }

        /// <summary>
        /// Gera um ruido curto para feedback negativo.
        /// </summary>
        private AudioClip GenerateNoiseClip(float duration, float volume)
        {
            int sampleCount = Mathf.RoundToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / sampleRate;
                float noise = Random.Range(-1f, 1f);
                float envelope = Mathf.Max(0f, 1f - (time / duration));
                samples[i] = noise * envelope * volume * masterVolume;
            }

            AudioClip clip = AudioClip.Create("MissSound", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }

        /// <summary>
        /// Retorna a frequencia de uma lane (util para afinacao futura).
        /// </summary>
        public float GetFrequency(int laneIndex)
        {
            return laneFrequencies.TryGetValue(laneIndex, out float frequency) ? frequency : 440f;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
