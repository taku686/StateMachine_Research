using InGame.Constants;
using UnityEngine;

namespace InGame.Presentation.Views
{
    /// <summary>
    /// プレイヤーのView
    /// </summary>
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform healthBarAnchor;

        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        private static readonly int DeathTriggerHash = Animator.StringToHash("Death");

        public Transform HealthBarAnchor => healthBarAnchor;

        private void Awake()
        {
            // SpriteRendererがない場合は自動で追加
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }

            // HealthBarアンカーがない場合は作成
            if (healthBarAnchor == null)
            {
                var anchorObj = new GameObject("HealthBarAnchor");
                anchorObj.transform.SetParent(transform);
                anchorObj.transform.localPosition = new Vector3(0, InGameConstants.UI.HealthBarAnchorHeight, 0);
                healthBarAnchor = anchorObj.transform;
            }
        }

        public void UpdatePosition(Vector3 position)
        {
            transform.position = position;
        }

        public void UpdateMoveAnimation(float speed)
        {
            if (animator != null)
            {
                animator.SetFloat(MoveSpeedHash, speed);
            }
        }

        public void PlayAttackAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(AttackTriggerHash);
            }
        }

        public void PlayDeathAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(DeathTriggerHash);
            }
        }

        public void SetSprite(Sprite sprite)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        public void SetColor(Color color)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
    }
}

