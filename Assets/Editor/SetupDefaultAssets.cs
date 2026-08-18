#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PetCuttieGames.Rhythm.Editor
{
    /// <summary>
    /// Cria os assets iniciais de instrumento e dificuldade para o prototipo.
    /// Acesse via menu: Tools > Pet Cuttie Games > Setup Default Assets
    /// </summary>
    public static class SetupDefaultAssets
    {
        private const string InstrumentsPath = "Assets/Resources/Instruments";
        private const string DifficultiesPath = "Assets/Resources/Difficulties";

        [MenuItem("Tools/Pet Cuttie Games/Setup Default Assets")]
        public static void CreateDefaultAssets()
        {
            Directory.CreateDirectory(InstrumentsPath);
            Directory.CreateDirectory(DifficultiesPath);

            CreateInstrument("Piano", "Piano", 0.6f, 0.01f, 0.3f, 0.3f, 0.2f);
            CreateInstrument("Synth", "Synth", 0.5f, 0.05f, 0.1f, 0.5f, 0.1f);
            CreateInstrument("Pluck", "Pluck", 0.25f, 0.005f, 0.15f, 0.1f, 0.05f);

            CreateDifficulty("Beginner", "Iniciante", 0.6f, 1.3f, 0.8f);
            CreateDifficulty("Easy", "Facil", 0.8f, 1.15f, 1f);
            CreateDifficulty("Medium", "Medio", 1f, 1f, 1.2f);
            CreateDifficulty("Hard", "Dificil", 1.3f, 0.7f, 1.5f);
            CreateDifficulty("Expert", "Expert", 1.6f, 0.5f, 2f);

            AssetDatabase.Refresh();
            Debug.Log("Assets iniciais criados com sucesso.");
        }

        private static void CreateInstrument(
            string id, string displayName, float noteDuration,
            float attackTime, float decayTime, float sustainLevel, float releaseTime)
        {
            string path = $"{InstrumentsPath}/{id}.asset";
            if (AssetDatabase.LoadAssetAtPath<InstrumentData>(path) != null)
            {
                return;
            }

            var instrument = ScriptableObject.CreateInstance<InstrumentData>();
            instrument.id = id;
            instrument.displayName = displayName;
            instrument.noteDuration = noteDuration;
            instrument.attackTime = attackTime;
            instrument.decayTime = decayTime;
            instrument.sustainLevel = sustainLevel;
            instrument.releaseTime = releaseTime;

            AssetDatabase.CreateAsset(instrument, path);
        }

        private static void CreateDifficulty(
            string id, string displayName, float speedMultiplier,
            float hitWindowMultiplier, float scoreMultiplier)
        {
            string path = $"{DifficultiesPath}/{id}.asset";
            if (AssetDatabase.LoadAssetAtPath<DifficultyConfig>(path) != null)
            {
                return;
            }

            var difficulty = ScriptableObject.CreateInstance<DifficultyConfig>();
            difficulty.id = id;
            difficulty.displayName = displayName;
            difficulty.speedMultiplier = speedMultiplier;
            difficulty.hitWindowMultiplier = hitWindowMultiplier;
            difficulty.scoreMultiplier = scoreMultiplier;

            AssetDatabase.CreateAsset(difficulty, path);
        }
    }
}
#endif
