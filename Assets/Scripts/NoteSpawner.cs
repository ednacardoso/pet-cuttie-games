using System.Collections.Generic;
using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Estrutura que representa uma unica nota da musica.
    /// </summary>
    [System.Serializable]
    public struct SongNote
    {
        public int laneIndex;
        public float hitTime; // tempo em segundos desde o inicio da musica
    }

    /// <summary>
    /// Spawna as notas da musica no tempo correto.
    /// </summary>
    public class NoteSpawner : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private GameObject notePrefab;
        [SerializeField] private Transform[] laneSpawnPoints;

        [Header("Configuracao")]
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private float spawnAnticipation = 2f; // quanto tempo antes a nota aparece
        [SerializeField] private float songStartDelay = 2f;

        [Header("Musica")]
        [SerializeField] private List<SongNote> songNotes = new List<SongNote>();

        private float songTimer;
        private int nextNoteIndex;
        private bool isPlaying;

        private void Awake()
        {
            // Carrega Ode to Joy como musica padrao para prototipo
            LoadOdeToJoy();
        }

        private void Update()
        {
            if (!isPlaying) return;

            songTimer += Time.deltaTime;

            while (nextNoteIndex < songNotes.Count &&
                   songNotes[nextNoteIndex].hitTime - spawnAnticipation <= songTimer)
            {
                SpawnNote(songNotes[nextNoteIndex]);
                nextNoteIndex++;
            }

            if (nextNoteIndex >= songNotes.Count && songTimer > songNotes[songNotes.Count - 1].hitTime + 3f)
            {
                EndSong();
            }
        }

        /// <summary>
        /// Inicia o spawn das notas.
        /// </summary>
        public void StartSong()
        {
            songTimer = -songStartDelay;
            nextNoteIndex = 0;
            isPlaying = true;
        }

        private void SpawnNote(SongNote songNote)
        {
            if (songNote.laneIndex < 0 || songNote.laneIndex >= laneSpawnPoints.Length)
            {
                Debug.LogWarning($"Lane invalida: {songNote.laneIndex}");
                return;
            }

            Transform spawnPoint = laneSpawnPoints[songNote.laneIndex];
            GameObject noteObject = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity, transform);

            Note note = noteObject.GetComponent<Note>();
            if (note != null)
            {
                note.Initialize(songNote.laneIndex, songTimer, songNote.hitTime, fallSpeed);
            }
        }

        private void EndSong()
        {
            isPlaying = false;
            Debug.Log($"Musica finalizada! Score: {ScoreManager.Instance?.Score ?? 0}");
        }

        /// <summary>
        /// Carrega uma musica a partir dos dados selecionados.
        /// </summary>
        public void LoadSong(SongData song)
        {
            if (song == null)
            {
                Debug.LogWarning("NoteSpawner: SongData nulo. Mantendo musica atual.");
                return;
            }

            songNotes = song.ToSongNotes();
            Debug.Log($"Musica carregada: {song.title} ({songNotes.Count} notas)");
        }

        /// <summary>
        /// Aplica configuracoes de dificuldade na velocidade das notas.
        /// </summary>
        public void ApplyDifficulty(DifficultyConfig difficulty)
        {
            if (difficulty == null)
            {
                Debug.LogWarning("NoteSpawner: DifficultyConfig nulo. Mantendo velocidade padrao.");
                return;
            }

            fallSpeed *= difficulty.speedMultiplier;
            Debug.Log($"Dificuldade aplicada: {difficulty.displayName} (velocidade x{difficulty.speedMultiplier})");
        }

        /// <summary>
        /// Carrega a sequencia de Ode to Joy (An die Freude) simplificada.
        /// Mapeamento de lanes:
        /// 0 = Do (C), 1 = Re (D), 2 = Mi (E), 3 = Fa (F), 4 = Sol (G)
        /// </summary>
        private void LoadOdeToJoy()
        {
            songNotes = new List<SongNote>
            {
                // E E F G | G F E D | C C D E | E D D D |
                new SongNote { laneIndex = 2, hitTime = 2.0f },
                new SongNote { laneIndex = 2, hitTime = 2.5f },
                new SongNote { laneIndex = 3, hitTime = 3.0f },
                new SongNote { laneIndex = 4, hitTime = 3.5f },

                new SongNote { laneIndex = 4, hitTime = 4.5f },
                new SongNote { laneIndex = 3, hitTime = 5.0f },
                new SongNote { laneIndex = 2, hitTime = 5.5f },
                new SongNote { laneIndex = 1, hitTime = 6.0f },

                new SongNote { laneIndex = 0, hitTime = 7.0f },
                new SongNote { laneIndex = 0, hitTime = 7.5f },
                new SongNote { laneIndex = 1, hitTime = 8.0f },
                new SongNote { laneIndex = 2, hitTime = 8.5f },

                new SongNote { laneIndex = 2, hitTime = 9.5f },
                new SongNote { laneIndex = 1, hitTime = 10.0f },
                new SongNote { laneIndex = 1, hitTime = 10.5f },
                new SongNote { laneIndex = 1, hitTime = 11.0f },

                // E E F G | G F E D | C C D E | D C C C |
                new SongNote { laneIndex = 2, hitTime = 12.0f },
                new SongNote { laneIndex = 2, hitTime = 12.5f },
                new SongNote { laneIndex = 3, hitTime = 13.0f },
                new SongNote { laneIndex = 4, hitTime = 13.5f },

                new SongNote { laneIndex = 4, hitTime = 14.5f },
                new SongNote { laneIndex = 3, hitTime = 15.0f },
                new SongNote { laneIndex = 2, hitTime = 15.5f },
                new SongNote { laneIndex = 1, hitTime = 16.0f },

                new SongNote { laneIndex = 0, hitTime = 17.0f },
                new SongNote { laneIndex = 0, hitTime = 17.5f },
                new SongNote { laneIndex = 1, hitTime = 18.0f },
                new SongNote { laneIndex = 2, hitTime = 18.5f },

                new SongNote { laneIndex = 1, hitTime = 19.5f },
                new SongNote { laneIndex = 0, hitTime = 20.0f },
                new SongNote { laneIndex = 0, hitTime = 20.5f },
                new SongNote { laneIndex = 0, hitTime = 21.0f }
            };
        }
    }
}
