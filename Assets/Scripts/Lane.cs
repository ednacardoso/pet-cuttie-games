using UnityEngine;

namespace PetCuttieGames.Rhythm
{
    /// <summary>
    /// Representa uma pista/tecla do jogo. Detecta input e verifica acertos.
    /// </summary>
    public class Lane : MonoBehaviour
    {
        [Header("Configuracao")]
        public int laneIndex;
        public KeyCode keyboardKey = KeyCode.A;

        [Header("Zona de Acerto")]
        [SerializeField] private Transform hitZone;
        [SerializeField] private float perfectThreshold = 0.05f;
        [SerializeField] private float goodThreshold = 0.15f;

        private float basePerfectThreshold;
        private float baseGoodThreshold;
        private Note currentNote;

        private void Awake()
        {
            basePerfectThreshold = perfectThreshold;
            baseGoodThreshold = goodThreshold;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // Suporte a teclado (para testes no editor/web)
            // e touch (para mobile)
            bool pressed = Input.GetKeyDown(keyboardKey) || WasTouched();

            if (pressed)
            {
                TryHit();
            }
        }

        private bool WasTouched()
        {
            if (Input.touchCount == 0) return false;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);
                    Collider2D col = Physics2D.OverlapPoint(worldPoint);

                    if (col != null && col.transform == transform)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Aplica o multiplicador de janela de acerto da dificuldade selecionada.
        /// </summary>
        public void ApplyDifficulty(float hitWindowMultiplier)
        {
            perfectThreshold = basePerfectThreshold * hitWindowMultiplier;
            goodThreshold = baseGoodThreshold * hitWindowMultiplier;
        }

        private void TryHit()
        {
            if (currentNote == null)
            {
                return;
            }

            float hitY = hitZone != null ? hitZone.position.y : transform.position.y;
            float distance = Mathf.Abs(currentNote.transform.position.y - hitY);

            if (distance <= goodThreshold)
            {
                // Converte distancia em erro de tempo (segundos) usando a velocidade da nota
                float accuracy = distance / Mathf.Max(currentNote.FallSpeed, 0.001f);

                ScoreManager.Instance?.RegisterHit(accuracy);
                AudioManager.Instance?.PlayNote(laneIndex);
                currentNote.Hit();
                currentNote = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Note note = other.GetComponent<Note>();
            if (note != null && note.LaneIndex == laneIndex)
            {
                currentNote = note;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Note note = other.GetComponent<Note>();
            if (note != null && note == currentNote)
            {
                currentNote = null;
            }
        }
    }
}
