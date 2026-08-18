using UnityEngine;
using TMPro;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Gerencia pontuacao, combo e feedback visual de acertos/erros.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Pontuacao")]
        [SerializeField] private int pointsPerHit = 100;
        [SerializeField] private int pointsPerPerfect = 200;
        [SerializeField] private float comboMultiplierStep = 0.1f;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI feedbackText;

        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int MaxCombo { get; private set; }
        public int Hits { get; private set; }
        public int Misses { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            UpdateUI();
        }

        /// <summary>
        /// Registra um acerto com base na precisao do timing.
        /// </summary>
        /// <param name="accuracy">Diferenca de tempo entre o acerto e o momento ideal (em segundos).</param>
        public void RegisterHit(float accuracy)
        {
            Hits++;
            Combo++;

            if (Combo > MaxCombo)
            {
                MaxCombo = Combo;
            }

            float multiplier = 1f + (Combo * comboMultiplierStep);
            int basePoints = Mathf.Abs(accuracy) < 0.05f ? pointsPerPerfect : pointsPerHit;
            int points = Mathf.RoundToInt(basePoints * multiplier);

            Score += points;

            ShowFeedback(Mathf.Abs(accuracy) < 0.05f ? "PERFECT!" : "GOOD!");
            UpdateUI();
        }

        /// <summary>
        /// Registra um erro (nota perdida ou tecla errada).
        /// </summary>
        public void RegisterMiss()
        {
            Misses++;
            Combo = 0;

            ShowFeedback("MISS");
            UpdateUI();
        }

        private void ShowFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
                CancelInvoke(nameof(ClearFeedback));
                Invoke(nameof(ClearFeedback), 0.5f);
            }
        }

        private void ClearFeedback()
        {
            if (feedbackText != null)
            {
                feedbackText.text = string.Empty;
            }
        }

        private void UpdateUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {Score:D6}";
            }

            if (comboText != null)
            {
                comboText.text = Combo > 1 ? $"Combo x{Combo}" : string.Empty;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
