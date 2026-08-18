#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PetCuttieGames.Rhythm.Editor
{
    /// <summary>
    /// Gera a cena de menu com paineis, botoes e scripts configurados.
    /// Acesse via menu: Tools > Pet Cuttie Games > Setup Menu Scene
    /// </summary>
    public static class MenuSceneSetup
    {
        private const string MenuScenePath = "Assets/Scenes/Menu.unity";

        [MenuItem("Tools/Pet Cuttie Games/Setup Menu Scene")]
        public static void CreateMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera basica
            var camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            var cam = camera.AddComponent<Camera>();
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.18f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);

            // GameSession
            var gameSessionGO = new GameObject("GameSession");
            gameSessionGO.AddComponent<GameSession>();

            // Canvas
            var canvasGO = new GameObject("MenuCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasGO.AddComponent<GraphicRaycaster>();

            // Painels
            var splashPanel = CreatePanel(canvasGO.transform, "SplashPanel");
            var mainMenuPanel = CreatePanel(canvasGO.transform, "MainMenuPanel");
            var instrumentPanel = CreatePanel(canvasGO.transform, "InstrumentPanel");
            var songPanel = CreatePanel(canvasGO.transform, "SongPanel");
            var difficultyPanel = CreatePanel(canvasGO.transform, "DifficultyPanel");

            // MenuManager
            var menuManagerGO = new GameObject("MenuManager");
            var menuManager = menuManagerGO.AddComponent<MenuManager>();
            SetPrivateField(menuManager, "splashPanel", splashPanel);
            SetPrivateField(menuManager, "mainMenuPanel", mainMenuPanel);
            SetPrivateField(menuManager, "instrumentPanel", instrumentPanel);
            SetPrivateField(menuManager, "songPanel", songPanel);
            SetPrivateField(menuManager, "difficultyPanel", difficultyPanel);
            SetPrivateField(menuManager, "splashDuration", 2f);
            SetPrivateField(menuManager, "waitForLoading", true);

            // Splash
            CreateText(splashPanel.transform, "Carregando...", new Vector2(0f, 0f), 48f);

            // Main Menu
            CreateText(mainMenuPanel.transform, "Pet Cuttie Games", new Vector2(0f, 150f), 64f);
            var playButton = CreateButton(mainMenuPanel.transform, "Jogar", new Vector2(0f, 0f));
            var settingsButton = CreateButton(mainMenuPanel.transform, "Configuracoes", new Vector2(0f, -80f));
            var creditsButton = CreateButton(mainMenuPanel.transform, "Creditos", new Vector2(0f, -160f));

            playButton.onClick.AddListener(menuManager.ShowInstrumentSelection);
            settingsButton.onClick.AddListener(() => Debug.Log("Configuracoes em breve."));
            creditsButton.onClick.AddListener(() => Debug.Log("Creditos em breve."));

            // Instrument Selection
            CreateText(instrumentPanel.transform, "Escolha seu Instrumento", new Vector2(0f, 350f), 48f);
            var instrumentContainer = CreateContainer(instrumentPanel.transform, "InstrumentContainer", new Vector2(0f, 100f));
            var instrumentButtonPrefab = CreateButtonPrefab("InstrumentButtonPrefab");
            var instrumentSelection = instrumentPanel.AddComponent<InstrumentSelection>();
            var instrumentConfirmButton = CreateButton(instrumentPanel.transform, "Proximo", new Vector2(200f, -350f));
            var instrumentBackButton = CreateButton(instrumentPanel.transform, "Voltar", new Vector2(-200f, -350f));

            SetPrivateField(instrumentSelection, "instruments", LoadInstruments());
            SetPrivateField(instrumentSelection, "buttonContainer", instrumentContainer);
            SetPrivateField(instrumentSelection, "instrumentButtonPrefab", instrumentButtonPrefab);
            SetPrivateField(instrumentSelection, "confirmButton", instrumentConfirmButton);
            SetPrivateField(instrumentSelection, "backButton", instrumentBackButton);

            // Song Selection
            CreateText(songPanel.transform, "Escolha a Musica", new Vector2(0f, 350f), 48f);
            var songContainer = CreateContainer(songPanel.transform, "SongContainer", new Vector2(0f, 100f));
            var songItemPrefab = CreateButtonPrefab("SongItemPrefab");
            var songSelection = songPanel.AddComponent<SongSelection>();
            var songConfirmButton = CreateButton(songPanel.transform, "Proximo", new Vector2(200f, -350f));
            var songBackButton = CreateButton(songPanel.transform, "Voltar", new Vector2(-200f, -350f));

            SetPrivateField(songSelection, "listContainer", songContainer);
            SetPrivateField(songSelection, "songItemPrefab", songItemPrefab);
            SetPrivateField(songSelection, "confirmButton", songConfirmButton);
            SetPrivateField(songSelection, "backButton", songBackButton);

            // Difficulty Selection
            CreateText(difficultyPanel.transform, "Escolha a Dificuldade", new Vector2(0f, 350f), 48f);
            var difficultyContainer = CreateContainer(difficultyPanel.transform, "DifficultyContainer", new Vector2(0f, 100f));
            var difficultyButtonPrefab = CreateButtonPrefab("DifficultyButtonPrefab");
            var difficultySelection = difficultyPanel.AddComponent<DifficultySelection>();
            var difficultyStartButton = CreateButton(difficultyPanel.transform, "Iniciar", new Vector2(200f, -350f));
            var difficultyBackButton = CreateButton(difficultyPanel.transform, "Voltar", new Vector2(-200f, -350f));

            SetPrivateField(difficultySelection, "difficulties", LoadDifficulties());
            SetPrivateField(difficultySelection, "buttonContainer", difficultyContainer);
            SetPrivateField(difficultySelection, "difficultyButtonPrefab", difficultyButtonPrefab);
            SetPrivateField(difficultySelection, "startButton", difficultyStartButton);
            SetPrivateField(difficultySelection, "backButton", difficultyBackButton);
            SetPrivateField(difficultySelection, "gameplaySceneName", "Gameplay");

            EditorSceneManager.SaveScene(scene, MenuScenePath);
            AssetDatabase.Refresh();

            Debug.Log($"Cena de menu criada em: {MenuScenePath}");
        }

        private static GameObject CreatePanel(Transform parent, string name)
        {
            var panel = new GameObject(name);
            panel.transform.parent = parent;
            panel.transform.localScale = Vector3.one;

            var image = panel.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.18f, 1f);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return panel;
        }

        private static Transform CreateContainer(Transform parent, string name, Vector2 anchoredPosition)
        {
            var go = new GameObject(name);
            go.transform.parent = parent;
            go.transform.localScale = Vector3.one;

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(800f, 500f);

            return go.transform;
        }

        private static GameObject CreateButtonPrefab(string name)
        {
            var go = new GameObject(name);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200f, 60f);

            var image = go.AddComponent<Image>();
            image.color = Color.gray;

            var button = go.AddComponent<Button>();

            var textGO = new GameObject("Text");
            textGO.transform.parent = go.transform;
            textGO.transform.localScale = Vector3.one;

            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = "Button";
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            PrefabUtility.SaveAsPrefabAsset(go, $"Assets/Prefabs/{name}.prefab");
            Object.DestroyImmediate(go);

            return AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/{name}.prefab");
        }

        private static Button CreateButton(Transform parent, string text, Vector2 anchoredPosition)
        {
            var go = new GameObject($"Button_{text}");
            go.transform.parent = parent;
            go.transform.localScale = Vector3.one;

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(200f, 60f);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.5f, 0.8f);

            var button = go.AddComponent<Button>();

            var textGO = new GameObject("Text");
            textGO.transform.parent = go.transform;
            textGO.transform.localScale = Vector3.one;

            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return button;
        }

        private static TextMeshProUGUI CreateText(Transform parent, string text, Vector2 anchoredPosition, float fontSize)
        {
            var go = new GameObject($"Text_{text}");
            go.transform.parent = parent;
            go.transform.localScale = Vector3.one;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(800f, 80f);

            return tmp;
        }

        private static List<InstrumentData> LoadInstruments()
        {
            var instruments = new List<InstrumentData>();
            var guids = AssetDatabase.FindAssets("t:InstrumentData", new[] { "Assets/Resources/Instruments" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var instrument = AssetDatabase.LoadAssetAtPath<InstrumentData>(path);
                if (instrument != null) instruments.Add(instrument);
            }
            return instruments;
        }

        private static List<DifficultyConfig> LoadDifficulties()
        {
            var difficulties = new List<DifficultyConfig>();
            var guids = AssetDatabase.FindAssets("t:DifficultyConfig", new[] { "Assets/Resources/Difficulties" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var difficulty = AssetDatabase.LoadAssetAtPath<DifficultyConfig>(path);
                if (difficulty != null) difficulties.Add(difficulty);
            }
            return difficulties;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                Debug.LogWarning($"Campo '{fieldName}' nao encontrado em {target.GetType().Name}");
            }
        }
    }
}
#endif
