using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Dados de um nivel de dificuldade.
    /// ScriptableObject para facilitar a criacao de assets reutilizaveis.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDifficulty", menuName = "Pet Cuttie Games/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        [Header("Identificacao")]
        [Tooltip("ID unico usado no codigo e no GameSession.")]
        public string id;

        [Tooltip("Nome exibido para o jogador.")]
        public string displayName;

        [Header("Efeitos na Partida")]
        [Tooltip("Multiplicador aplicado a velocidade de queda das notas.")]
        public float speedMultiplier = 1f;

        [Tooltip("Multiplicador aplicado a janela de acerto das lanes.")]
        public float hitWindowMultiplier = 1f;

        [Tooltip("Multiplicador aplicado a pontuacao base.")]
        public float scoreMultiplier = 1f;

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = name;
            }
        }
    }
}
