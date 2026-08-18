using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Carrega as musicas disponiveis e registra a escolha no GameSession.
    /// </summary>
    public class SongSelection : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform listContainer;
        [SerializeField] private GameObject songItemPrefab;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;

        [Header("Feedback")]
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color normalColor = Color.white;

        private readonly List<SongData> songs = new List<SongData>();
        private SongData selectedSong;
        private readonly List<GameObject> spawnedItems = new List<GameObject>();

        private void Start()
        {
            LoadSongs();
            BuildSongList();

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
        /// Carrega todos os JSONs da pasta Resources/Songs.
        /// </summary>
        private void LoadSongs()
        {
            songs.Clear();
            songs.AddRange(SongLoader.LoadAll());

            if (songs.Count == 0)
            {
                Debug.LogWarning("SongSelection: nenhuma musica encontrada em Resources/Songs.");
            }
        }

        /// <summary>
        /// Cria os itens da lista de musicas.
        /// </summary>
        private void BuildSongList()
        {
            if (listContainer == null || songItemPrefab == null)
            {
                Debug.LogWarning("SongSelection: listContainer ou prefab nao configurado.");
                return;
            }

            ClearItems();

            foreach (var song in songs)
            {
                if (song == null) continue;

                GameObject itemObject = Instantiate(songItemPrefab, listContainer);
                spawnedItems.Add(itemObject);

                Button button = itemObject.GetComponentInChildren<Button>();
                TMP_Text label = itemObject.GetComponentInChildren<TMP_Text>();

                if (label != null)
                {
                    label.text = $"{song.title} - {song.composer}\n{song.bpm} BPM";
                }

                if (button != null)
                {
                    SongData captured = song;
                    button.onClick.AddListener(() => SelectSong(captured));
                }
            }

            RestoreSelection();
        }

        /// <summary>
        /// Registra a musica escolhida no GameSession.
        /// </summary>
        public void SelectSong(SongData song)
        {
            selectedSong = song;

            if (GameSession.Instance != null)
            {
                GameSession.Instance.SelectedSongId = song.Id;
            }

            HighlightSelection();
            Debug.Log($"Musica selecionada: {song.title}");
        }

        /// <summary>
        /// Avanca para a proxima tela (deve ser ligado ao MenuManager).
        /// </summary>
        public void OnConfirm()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null)
            {
                menuManager.ShowDifficultySelection();
            }
            else
            {
                Debug.LogWarning("SongSelection: MenuManager nao encontrado na cena.");
            }
        }

        /// <summary>
        /// Volta para a selecao de instrumento.
        /// </summary>
        public void OnBack()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null)
            {
                menuManager.ShowInstrumentSelection();
            }
            else
            {
                Debug.LogWarning("SongSelection: MenuManager nao encontrado na cena.");
            }
        }

        private void RestoreSelection()
        {
            string savedId = GameSession.Instance?.SelectedSongId;
            if (string.IsNullOrEmpty(savedId)) return;

            foreach (var song in songs)
            {
                if (song != null && song.Id == savedId)
                {
                    SelectSong(song);
                    return;
                }
            }
        }

        private void HighlightSelection()
        {
            if (selectedSong == null) return;

            for (int i = 0; i < spawnedItems.Count && i < songs.Count; i++)
            {
                Image image = spawnedItems[i].GetComponentInChildren<Image>();
                if (image != null)
                {
                    image.color = songs[i] == selectedSong ? selectedColor : normalColor;
                }
            }
        }

        private void ClearItems()
        {
            foreach (var item in spawnedItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }

            spawnedItems.Clear();
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
