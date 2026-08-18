using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Controla a exibicao dos paineis de menu.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("Telas")]
        [SerializeField] private GameObject splashPanel;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject instrumentPanel;
        [SerializeField] private GameObject songPanel;
        [SerializeField] private GameObject difficultyPanel;

        [Header("Configuracao")]
        [SerializeField] private float splashDuration = 2f;
        [SerializeField] private bool waitForLoading = true;

        private float splashTimer;
        private bool loadingComplete;

        private void Start()
        {
            ShowPanel(splashPanel);
            splashTimer = 0f;
            loadingComplete = false;

            // Aqui pode ser carregado dados assincronos no futuro.
            loadingComplete = true;
        }

        private void Update()
        {
            if (splashPanel != null && splashPanel.activeSelf)
            {
                splashTimer += Time.deltaTime;
                if (splashTimer >= splashDuration && (!waitForLoading || loadingComplete))
                {
                    ShowMainMenu();
                }
            }
        }

        public void ShowMainMenu() => ShowPanel(mainMenuPanel);
        public void ShowInstrumentSelection() => ShowPanel(instrumentPanel);
        public void ShowSongSelection() => ShowPanel(songPanel);
        public void ShowDifficultySelection() => ShowPanel(difficultyPanel);

        private void ShowPanel(GameObject panel)
        {
            if (splashPanel != null) splashPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (instrumentPanel != null) instrumentPanel.SetActive(false);
            if (songPanel != null) songPanel.SetActive(false);
            if (difficultyPanel != null) difficultyPanel.SetActive(false);

            if (panel != null) panel.SetActive(true);
        }
    }
}
