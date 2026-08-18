using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Constrói a lista de instrumentos e registra a escolha no GameSession.
    /// </summary>
    public class InstrumentSelection : MonoBehaviour
    {
        [Header("Dados")]
        [SerializeField] private List<InstrumentData> instruments;

        [Header("UI")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject instrumentButtonPrefab;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;

        [Header("Feedback")]
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color normalColor = Color.white;

        private InstrumentData selectedInstrument;
        private readonly List<GameObject> spawnedButtons = new List<GameObject>();

        private void Start()
        {
            BuildInstrumentList();
            HighlightSelection();

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirm);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBack);
            }
        }

        /// <summary>
        /// Cria os botoes de instrumento no container configurado.
        /// </summary>
        private void BuildInstrumentList()
        {
            if (buttonContainer == null || instrumentButtonPrefab == null)
            {
                Debug.LogWarning("InstrumentSelection: buttonContainer ou prefab nao configurado.");
                return;
            }

            ClearButtons();

            foreach (var instrument in instruments)
            {
                if (instrument == null) continue;

                GameObject buttonObject = Instantiate(instrumentButtonPrefab, buttonContainer);
                spawnedButtons.Add(buttonObject);

                Button button = buttonObject.GetComponentInChildren<Button>();
                TMP_Text label = buttonObject.GetComponentInChildren<TMP_Text>();
                Image icon = buttonObject.GetComponentInChildren<Image>();

                if (label != null)
                {
                    label.text = instrument.displayName;
                }

                if (icon != null && instrument.icon != null)
                {
                    icon.sprite = instrument.icon;
                }

                if (button != null)
                {
                    InstrumentData captured = instrument;
                    button.onClick.AddListener(() => SelectInstrument(captured));
                }
            }

            // Tenta restaurar selecao anterior da sessao
            RestoreSelection();
        }

        /// <summary>
        /// Registra o instrumento escolhido e aplica no AudioManager.
        /// </summary>
        public void SelectInstrument(InstrumentData instrument)
        {
            selectedInstrument = instrument;

            if (GameSession.Instance != null)
            {
                GameSession.Instance.SelectedInstrumentId = instrument.id;
            }

            if (AudioManager.Instance != null)
            {
                instrument.ApplyTo(AudioManager.Instance);
            }

            HighlightSelection();
            Debug.Log($"Instrumento selecionado: {instrument.displayName}");
        }

        /// <summary>
        /// Avanca para a proxima tela (deve ser ligado ao MenuManager).
        /// </summary>
        public void OnConfirm()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null)
            {
                menuManager.ShowSongSelection();
            }
            else
            {
                Debug.LogWarning("InstrumentSelection: MenuManager nao encontrado na cena.");
            }
        }

        /// <summary>
        /// Volta para a tela inicial.
        /// </summary>
        public void OnBack()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null)
            {
                menuManager.ShowMainMenu();
            }
            else
            {
                Debug.LogWarning("InstrumentSelection: MenuManager nao encontrado na cena.");
            }
        }

        private void RestoreSelection()
        {
            string savedId = GameSession.Instance?.SelectedInstrumentId;
            if (string.IsNullOrEmpty(savedId)) return;

            foreach (var instrument in instruments)
            {
                if (instrument != null && instrument.id == savedId)
                {
                    SelectInstrument(instrument);
                    return;
                }
            }
        }

        private void HighlightSelection()
        {
            if (selectedInstrument == null) return;

            for (int i = 0; i < spawnedButtons.Count && i < instruments.Count; i++)
            {
                Image image = spawnedButtons[i].GetComponentInChildren<Image>();
                if (image != null)
                {
                    image.color = instruments[i] == selectedInstrument ? selectedColor : normalColor;
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
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(OnConfirm);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(OnBack);
            }
        }
    }
}
