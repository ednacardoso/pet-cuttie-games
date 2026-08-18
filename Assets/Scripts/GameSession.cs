using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Mantem as escolhas do jogador entre as cenas de Menu e Gameplay.
    /// Sobrevive a mudancas de cena via DontDestroyOnLoad.
    /// </summary>
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        public string SelectedInstrumentId { get; set; } = "Piano";
        public string SelectedSongId { get; set; } = "OdeToJoy";
        public string SelectedDifficultyId { get; set; } = "Easy";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
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
