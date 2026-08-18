using UnityEngine;
using UnityEngine.SceneManagement;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Controla o fluxo geral do jogo: inicio, reinicio e fim da musica.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private NoteSpawner noteSpawner;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private AudioManager audioManager;

        [Header("Telas")]
        [SerializeField] private GameObject startScreen;
        [SerializeField] private GameObject gameplayScreen;
        [SerializeField] private GameObject resultScreen;

        [Header("Configuracao")]
        [SerializeField] private bool autoStartFromMenu = true;

        private bool hasStarted;

        private void Start()
        {
            ApplySessionChoices();

            if (autoStartFromMenu && GameSession.Instance != null)
            {
                StartGame();
            }
            else if (startScreen == null)
            {
                // Quando nao ha tela inicial configurada, inicia automaticamente
                // (util para testar a cena Gameplay isoladamente).
                StartGame();
            }
            else
            {
                ShowStartScreen();
            }
        }

        /// <summary>
        /// Carrega as escolhas do GameSession e aplica nos sistemas.
        /// </summary>
        private void ApplySessionChoices()
        {
            if (GameSession.Instance == null)
            {
                Debug.Log("GameManager: GameSession nao encontrado. Usando valores padrao.");
                return;
            }

            SongData song = SongLoader.Load(GameSession.Instance.SelectedSongId);
            noteSpawner?.LoadSong(song);

            DifficultyConfig difficulty = Resources.Load<DifficultyConfig>($"Difficulties/{GameSession.Instance.SelectedDifficultyId}");
            if (difficulty == null)
            {
                difficulty = Resources.Load<DifficultyConfig>($"Difficulties/{GameSession.Instance.SelectedDifficultyId}Config");
            }

            if (difficulty != null)
            {
                noteSpawner?.ApplyDifficulty(difficulty);
                ApplyDifficultyToLanes(difficulty.hitWindowMultiplier);
            }

            InstrumentData instrument = Resources.Load<InstrumentData>($"Instruments/{GameSession.Instance.SelectedInstrumentId}");
            if (instrument == null)
            {
                instrument = Resources.Load<InstrumentData>($"Instruments/{GameSession.Instance.SelectedInstrumentId}Data");
            }

            if (instrument != null)
            {
                audioManager?.ApplyInstrument(instrument);
            }
        }

        /// <summary>
        /// Aplica o multiplicador de janela de acerto em todas as lanes.
        /// </summary>
        private void ApplyDifficultyToLanes(float hitWindowMultiplier)
        {
            Lane[] lanes = FindObjectsOfType<Lane>();
            foreach (Lane lane in lanes)
            {
                lane.ApplyDifficulty(hitWindowMultiplier);
            }
        }

        /// <summary>
        /// Inicia o jogo. Pode ser chamada por um botao ou tecla.
        /// </summary>
        public void StartGame()
        {
            if (hasStarted) return;

            hasStarted = true;
            ShowGameplayScreen();
            scoreManager?.gameObject.SetActive(true);
            noteSpawner?.StartSong();
        }

        /// <summary>
        /// Reinicia a cena atual.
        /// </summary>
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ShowStartScreen()
        {
            if (startScreen != null) startScreen.SetActive(true);
            if (gameplayScreen != null) gameplayScreen.SetActive(false);
            if (resultScreen != null) resultScreen.SetActive(false);
        }

        private void ShowGameplayScreen()
        {
            if (startScreen != null) startScreen.SetActive(false);
            if (gameplayScreen != null) gameplayScreen.SetActive(true);
            if (resultScreen != null) resultScreen.SetActive(false);
        }

        private void Update()
        {
            // Teste global de input
            if (Input.anyKeyDown)
            {
                Debug.Log($"Tecla pressionada: {Input.inputString}");
            }

            // Atalho para iniciar no editor/teclado
            if (!hasStarted && Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }

            // Atalho para reiniciar
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
}
