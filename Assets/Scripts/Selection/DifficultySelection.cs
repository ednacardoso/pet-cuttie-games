using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Constrói a lista de dificuldades e inicia a partida.
    /// </summary>
    public class DifficultySelection : MonoBehaviour
    {
        [Header("Dados")]
        [SerializeField] private List<DifficultyConfig> difficulties;

        [Header("UI")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject difficultyButtonPrefab;
        [SerializeField] private Button startButton;
        [SerializeField] private Button backButton;

        [Header("Feedback")]
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color normalColor = Color.white;

        [Header("Cenas")]
        [SerializeField] private string gameplaySceneName = "Gameplay";

        private DifficultyConfig selectedDifficulty;
        private readonly List<GameObject> spawnedButtons = new List<GameObject>();

        private void Start()
        {
            BuildDifficultyList();

            if (startButton != null)
            {
                startButton.onClick.AddListener(StartGame);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBack);
            }
        }

        /// <summary>
        /// Cria os botoes de dificuldade no container configurado.
        /// </summary>
        private void BuildDifficultyList()
        {
            if (buttonContainer == null || difficultyButtonPrefab == null)
            {
                Debug.LogWarning("DifficultySelection: buttonContainer ou prefab nao configurado.");
                return;
            }

            ClearButtons();

            foreach (var difficulty in difficulties)
            {
                if (difficulty == null) continue;

                GameObject buttonObject = Instantiate(difficultyButtonPrefab, buttonContainer);
                spawnedButtons.Add(buttonObject);

                Button button = buttonObject.GetComponentInChildren<Button>();
                TMP_Text label = buttonObject.GetComponentInChildren<TMP_Text>();

                if (label != null)
                {
                    label.text = difficulty.displayName;
                }

                if (button != null)
                {
                    DifficultyConfig captured = difficulty;
                    button.onClick.AddListener(() => SelectDifficulty(captured));
                }
            }

            RestoreSelection();
        }

        /// <summary>
        /// Registra a dificuldade escolhida no GameSession.
        /// </summary>
        public void SelectDifficulty(DifficultyConfig difficulty)
        {
            selectedDifficulty = difficulty;

            if (GameSession.Instance != null)
            {
                GameSession.Instance.SelectedDifficultyId = difficulty.id;
            }

            HighlightSelection();
            Debug.Log($"Dificuldade selecionada: {difficulty.displayName}");
        }

        /// <summary>
        /// Carrega a cena de gameplay com as escolhas do jogador.
        /// </summary>
        public void StartGame()
        {
            if (selectedDifficulty == null)
            {
                Debug.LogWarning("DifficultySelection: nenhuma dificuldade selecionada.");
                return;
            }

            if (string.IsNullOrEmpty(gameplaySceneName))
            {
                Debug.LogError("DifficultySelection: nome da cena de gameplay nao configurado.");
                return;
            }

            SceneManager.LoadScene(gameplaySceneName);
        }

        /// <summary>
        /// Volta para a selecao de musica.
        /// </summary>
        public void OnBack()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null)
            {
                menuManager.ShowSongSelection();
            }
            else
            {
                Debug.LogWarning("DifficultySelection: MenuManager nao encontrado na cena.");
            }
        }

        private void RestoreSelection()
        {
            string savedId = GameSession.Instance?.SelectedDifficultyId;
            if (string.IsNullOrEmpty(savedId)) return;

            foreach (var difficulty in difficulties)
            {
                if (difficulty != null && difficulty.id == savedId)
                {
                    SelectDifficulty(difficulty);
                    return;
                }
            }
        }

        private void HighlightSelection()
        {
            if (selectedDifficulty == null) return;

            for (int i = 0; i < spawnedButtons.Count && i < difficulties.Count; i++)
            {
                Image image = spawnedButtons[i].GetComponentInChildren<Image>();
                if (image != null)
                {
                    image.color = difficulties[i] == selectedDifficulty ? selectedColor : normalColor;
                }
            }
        }

        private void ClearButtons()
        {
            foreach (var button in spawnedButtons)
            {
                if (button != null)
                {
                    Destroy(button);
                }
            }

            spawnedButtons.Clear();
        }

        private void OnDestroy()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(StartGame);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBack);
            }
        }
    }
}
