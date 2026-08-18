using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Representa uma nota que cai pela tela em direcao a zona de acerto.
    /// </summary>
    public class Note : MonoBehaviour
    {
        [Header("Configuracao")]
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hitColor = Color.green;
        [SerializeField] private Color missColor = Color.red;

        private SpriteRenderer spriteRenderer;
        private bool isActive = true;

        public int LaneIndex { get; private set; }
        public float SpawnTime { get; private set; }
        public float HitTime { get; private set; }
        public float FallSpeed => fallSpeed;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;
            }
        }

        private void Update()
        {
            if (!isActive) return;

            // Move a nota para baixo
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Se passar muito da zona de acerto, considera erro
            if (transform.position.y < -6f)
            {
                Miss();
            }
        }

        /// <summary>
        /// Inicializa a nota com seus dados de gameplay.
        /// </summary>
        public void Initialize(int laneIndex, float spawnTime, float hitTime, float speed)
        {
            LaneIndex = laneIndex;
            SpawnTime = spawnTime;
            HitTime = hitTime;
            fallSpeed = speed;
        }

        /// <summary>
        /// Marca a nota como acertada.
        /// </summary>
        public void Hit()
        {
            if (!isActive) return;

            isActive = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = hitColor;
            }

            // Efeito visual simples: encolhe e some
            LeanTweenScaleAndDestroy();
        }

        /// <summary>
        /// Marca a nota como perdida.
        /// </summary>
        public void Miss()
        {
            if (!isActive) return;

            isActive = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = missColor;
            }

            ScoreManager.Instance?.RegisterMiss();
            AudioManager.Instance?.PlayMissSound();

            LeanTweenScaleAndDestroy();
        }

        private void LeanTweenScaleAndDestroy()
        {
            // Sem LeanTween no projeto ainda, entao destroi direto
            // Futuramente podemos adicionar um pequeno tween
            Destroy(gameObject, 0.1f);
        }
    }
}
