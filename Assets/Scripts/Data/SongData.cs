using System.Collections.Generic;
using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Representacao de uma nota dentro do arquivo JSON de uma musica.
    /// </summary>
    [System.Serializable]
    public class SongNoteData
    {
        public int lane;
        public float time;
    }

    /// <summary>
    /// Dados de uma musica carregada de JSON.
    /// </summary>
    [System.Serializable]
    public class SongData
    {
        public string title;
        public string composer;
        public float bpm;
        public string difficulty;
        public List<SongNoteData> notes = new List<SongNoteData>();

        /// <summary>
        /// ID da musica, derivado do titulo.
        /// </summary>
        public string Id => string.IsNullOrEmpty(title) ? string.Empty : title.Replace(" ", "");

        /// <summary>
        /// Converte as notas do JSON para o formato usado pelo NoteSpawner.
        /// </summary>
        public List<SongNote> ToSongNotes()
        {
            var result = new List<SongNote>(notes.Count);

            foreach (var note in notes)
            {
                result.Add(new SongNote
                {
                    laneIndex = note.lane,
                    hitTime = note.time
                });
            }

            return result;
        }
    }
}
