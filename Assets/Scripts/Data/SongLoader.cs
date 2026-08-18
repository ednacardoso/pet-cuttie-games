using System.Collections.Generic;
using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Carrega dados de musicas a partir de arquivos JSON na pasta Resources/Songs.
    /// </summary>
    public static class SongLoader
    {
        private const string SongsFolder = "Songs";

        /// <summary>
        /// Carrega todas as musicas disponiveis.
        /// </summary>
        public static List<SongData> LoadAll()
        {
            var songs = new List<SongData>();
            TextAsset[] songAssets = Resources.LoadAll<TextAsset>(SongsFolder);

            foreach (var asset in songAssets)
            {
                var song = LoadFromText(asset.text);
                if (song != null)
                {
                    songs.Add(song);
                }
            }

            return songs;
        }

        /// <summary>
        /// Carrega uma musica pelo ID (titulo sem espacos) ou pelo titulo exato.
        /// </summary>
        public static SongData Load(string songId)
        {
            if (string.IsNullOrEmpty(songId))
            {
                Debug.LogWarning("SongLoader: songId vazio. Usando musica padrao.");
                return LoadDefault();
            }

            foreach (var song in LoadAll())
            {
                if (song.Id.Equals(songId, System.StringComparison.OrdinalIgnoreCase) ||
                    song.title.Equals(songId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return song;
                }
            }

            Debug.LogWarning($"SongLoader: musica '{songId}' nao encontrada. Usando musica padrao.");
            return LoadDefault();
        }

        /// <summary>
        /// Carrega a primeira musica disponivel como fallback.
        /// </summary>
        public static SongData LoadDefault()
        {
            List<SongData> all = LoadAll();
            return all.Count > 0 ? all[0] : null;
        }

        /// <summary>
        /// Desserializa um texto JSON em SongData.
        /// </summary>
        public static SongData LoadFromText(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                return JsonUtility.FromJson<SongData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"SongLoader: falha ao carregar musica. {e.Message}");
                return null;
            }
        }
    }
}
