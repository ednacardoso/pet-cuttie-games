#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PetCuttieGames.Rhythm.Editor
{
    /// <summary>
    /// Gera a cena de gameplay com todos os objetos necessarios para teste rapido.
    /// Acesse via menu: Tools > Pet Cuttie Games > Setup Gameplay Scene
    /// </summary>
    public static class GameplaySceneSetup
    {
        private const string GameplayScenePath = "Assets/Scenes/Gameplay.unity";
        private const string NotePrefabPath = "Assets/Prefabs/Note.prefab";

        [MenuItem("Tools/Pet Cuttie Games/Setup Gameplay Scene")]
        public static void CreateGameplayScene()
        {
            // Garante que o sprite branco exista antes de criar objetos
            Sprite whiteSprite = EnsureWhiteSprite();
            Debug.Log($"White sprite: {(whiteSprite != null ? whiteSprite.name : "NULL")}");

            // Cria a cena nova ou carrega a existente
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            var cam = camera.AddComponent<Camera>();
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.18f);
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.transform.position = new Vector3(0f, 1f, -10f);
            camera.AddComponent<AudioListener>();

            // Luz (apenas para nao ficar totalmente escuro se usar materiais default)
            var lightGO = new GameObject("Directional Light");
            var light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Prefab da nota (cria, salva e carrega a referencia do disco)
            GameObject notePrefabInstance = CreateNotePrefab(whiteSprite);
            PrefabUtility.SaveAsPrefabAsset(notePrefabInstance, NotePrefabPath);
            Object.DestroyImmediate(notePrefabInstance);
            AssetDatabase.Refresh();
            GameObject notePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(NotePrefabPath);
            Debug.Log($"Note prefab: {(notePrefab != null ? notePrefab.name : "NULL")}");

            // Lanes
            Transform[] laneSpawnPoints = CreateLanes(whiteSprite);

            // NoteSpawner
            var noteSpawnerGO = new GameObject("NoteSpawner");
            var noteSpawner = noteSpawnerGO.AddComponent<NoteSpawner>();
            noteSpawnerGO.transform.position = new Vector3(0f, 6f, 0f);

            // Usar reflection para setar campos privados [SerializeField]
            SetPrivateField(noteSpawner, "notePrefab", notePrefab);
            SetPrivateField(noteSpawner, "laneSpawnPoints", laneSpawnPoints);
            SetPrivateField(noteSpawner, "fallSpeed", 3f);
            SetPrivateField(noteSpawner, "spawnAnticipation", 3f);
            SetPrivateField(noteSpawner, "songStartDelay", 2f);

            // AudioManager
            var audioManagerGO = new GameObject("AudioManager");
            var audioManager = audioManagerGO.AddComponent<AudioManager>();
            audioManagerGO.AddComponent<AudioSource>();

            // ScoreManager + Canvas UI
            var scoreManagerGO = new GameObject("ScoreManager");
            var scoreManager = scoreManagerGO.AddComponent<ScoreManager>();
            SetupScoreUI(scoreManager);

            // GameManager
            var gameManagerGO = new GameObject("GameManager");
            var gameManager = gameManagerGO.AddComponent<GameManager>();
            SetPrivateField(gameManager, "noteSpawner", noteSpawner);
            SetPrivateField(gameManager, "scoreManager", scoreManager);
            SetPrivateField(gameManager, "audioManager", audioManager);
            SetPrivateField(gameManager, "autoStartFromMenu", true);

            // Salva cena
            EditorSceneManager.SaveScene(scene, GameplayScenePath);
            AssetDatabase.Refresh();

            Debug.Log($"Cena de gameplay criada em: {GameplayScenePath}");
        }

        private static GameObject CreateNotePrefab(Sprite whiteSprite)
        {
            var noteGO = new GameObject("Note");
            var sr = noteGO.AddComponent<SpriteRenderer>();
            sr.sprite = whiteSprite;
            sr.color = new Color(1f, 0.9f, 0.2f);
            sr.drawMode = SpriteDrawMode.Simple;
            noteGO.transform.localScale = new Vector3(0.8f, 0.3f, 1f);

            var collider = noteGO.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.8f, 0.5f);

            var rb = noteGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.isKinematic = true;

            noteGO.AddComponent<Note>();

            return noteGO;
        }

        private static Transform[] CreateLanes(Sprite whiteSprite)
        {
            var lanesParent = new GameObject("Lanes").transform;
            Transform[] spawnPoints = new Transform[5];

            float startX = -4f;
            float spacing = 2f;
            float yPos = -3.5f;

            KeyCode[] keys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G };

            for (int i = 0; i < 5; i++)
            {
                var laneGO = new GameObject($"Lane_{i}");
                laneGO.transform.parent = lanesParent;
                laneGO.transform.position = new Vector3(startX + i * spacing, yPos, 0f);

                var sr = laneGO.AddComponent<SpriteRenderer>();
                sr.sprite = whiteSprite;
                sr.color = new Color(0.4f, 0.4f, 0.6f);
                sr.drawMode = SpriteDrawMode.Simple;
                laneGO.transform.localScale = new Vector3(1f, 7f, 1f);

                var collider = laneGO.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = new Vector2(1f, 0.15f);

                var lane = laneGO.AddComponent<Lane>();
                lane.laneIndex = i;
                lane.keyboardKey = keys[i];

                // Criar ponto de spawn acima da lane
                var spawnPoint = new GameObject($"SpawnPoint_{i}").transform;
                spawnPoint.parent = laneGO.transform;
                spawnPoint.localPosition = new Vector3(0f, 9f, 0f);
                spawnPoints[i] = spawnPoint;
            }

            return spawnPoints;
        }

        private static void SetupScoreUI(ScoreManager scoreManager)
        {
            var canvasGO = new GameObject("ScoreCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var scoreText = CreateText(canvasGO.transform, "ScoreText", new Vector2(0f, 0.9f), "Score: 000000");
            var comboText = CreateText(canvasGO.transform, "ComboText", new Vector2(0f, 0.8f), "");
            var feedbackText = CreateText(canvasGO.transform, "FeedbackText", new Vector2(0f, 0.7f), "");

            SetPrivateField(scoreManager, "scoreText", scoreText);
            SetPrivateField(scoreManager, "comboText", comboText);
            SetPrivateField(scoreManager, "feedbackText", feedbackText);
        }

        private static TextMeshProUGUI CreateText(Transform parent, string name, Vector2 anchoredPosition, string initialText)
        {
            var go = new GameObject(name);
            go.transform.parent = parent;
            go.transform.localScale = Vector3.one;

            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = initialText;
            text.fontSize = 36;
            text.alignment = TextAlignmentOptions.Center;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition * new Vector2(Screen.width, Screen.height);
            rect.sizeDelta = new Vector2(400f, 60f);

            return text;
        }

        private static Sprite EnsureWhiteSprite()
        {
            string directory = "Assets/Resources/Sprites";
            string texturePath = $"{directory}/WhiteSquareTexture.asset";
            string spritePath = $"{directory}/WhiteSquareSprite.sprite";

            Directory.CreateDirectory(directory);

            var existingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (existingSprite != null)
            {
                return existingSprite;
            }

            var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            var pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();
            texture.name = "WhiteSquareTexture";

            AssetDatabase.CreateAsset(texture, texturePath);
            AssetDatabase.SaveAssets();

            var sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
            sprite.name = "WhiteSquareSprite";
            AssetDatabase.CreateAsset(sprite, spritePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return sprite;
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
