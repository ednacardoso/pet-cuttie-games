using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Dados de um instrumento/timbre jogavel.
    /// ScriptableObject para facilitar a criacao de assets reutilizaveis.
    /// </summary>
    [CreateAssetMenu(fileName = "NewInstrument", menuName = "Pet Cuttie Games/Instrument Data")]
    public class InstrumentData : ScriptableObject
    {
        [Header("Identificacao")]
        [Tooltip("ID unico usado no codigo e no GameSession.")]
        public string id;

        [Tooltip("Nome exibido para o jogador.")]
        public string displayName;

        [Tooltip("Icone representativo do instrumento.")]
        public Sprite icon;

        [Header("Configuracao de Audio")]
        [Tooltip("Duracao total de cada nota sintetizada.")]
        public float noteDuration = 0.6f;

        [Tooltip("Tempo de ataque do envelope ADSR.")]
        public float attackTime = 0.01f;

        [Tooltip("Tempo de decay do envelope ADSR.")]
        public float decayTime = 0.3f;

        [Tooltip("Nivel de sustain do envelope ADSR.")]
        [Range(0f, 1f)]
        public float sustainLevel = 0.3f;

        [Tooltip("Tempo de release do envelope ADSR.")]
        public float releaseTime = 0.2f;

        [Tooltip("Pesos dos harmonicos usados na sintese. X = indice do harmonico, Y = peso.")]
        public AnimationCurve harmonicWeights;

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = name;
            }

            if (harmonicWeights == null || harmonicWeights.length == 0)
            {
                harmonicWeights = new AnimationCurve(
                    new Keyframe(1f, 1f),
                    new Keyframe(2f, 0.25f),
                    new Keyframe(3f, 0.125f),
                    new Keyframe(4f, 0.0625f)
                );
            }
        }

        /// <summary>
        /// Aplica as configuracoes deste instrumento no AudioManager ativo.
        /// </summary>
        public void ApplyTo(AudioManager audioManager)
        {
            if (audioManager == null)
            {
                Debug.LogWarning($"InstrumentData '{id}': AudioManager nao encontrado para aplicar.");
                return;
            }

            audioManager.ApplyInstrument(this);
        }
    }
}
