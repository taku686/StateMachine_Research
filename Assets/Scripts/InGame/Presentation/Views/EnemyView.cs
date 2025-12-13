using InGame.Constants;
using UnityEngine;

namespace InGame.Presentation.Views
{
    /// <summary>
    /// 敵のView
    /// </summary>
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform healthBarAnchor;

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

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }
    }
}

