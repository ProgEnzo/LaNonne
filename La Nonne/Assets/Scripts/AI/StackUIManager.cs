using System.Collections.Generic;
using AI.Boss;
using Shop;
using UnityEngine;

namespace AI
{
    public class StackUIManager : MonoBehaviour
    {
        private Component entityController;
        private readonly SpriteRenderer[] squareSpriteRenderers = new SpriteRenderer[3];
        private readonly SpriteRenderer[] otherSpriteRenderers = new SpriteRenderer[3];
        [SerializeField] private List<Sprite> stackSprites = new();
        
        private void Start()
        {
            entityController = transform.parent.CompareTag("Boss") ? transform.parent.GetComponent<BossStateManager>() : GetComponentInParent<EnemyController>();
            for (var i = 0; i < transform.childCount; i++)
            {
                squareSpriteRenderers[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
                otherSpriteRenderers[i] = transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>();
            }
        }

        private void Update()
        {
            for (var i = 0; i < squareSpriteRenderers.Length; i++)
            {
                StackSwitch(i);
            }
        }
        
        private void StackSwitch(int i)
        {
            var isActive = false;
            
            switch (entityController)
            {
                case BossStateManager boss when boss.stacks[i].effect != EffectManager.Effect.None:
                case EnemyController en when en.stacks[i].effect != EffectManager.Effect.None:
                    isActive = true;
                    break;
            }

            squareSpriteRenderers[i].enabled = isActive;
            otherSpriteRenderers[i].enabled = isActive;
            
            if (squareSpriteRenderers[i].enabled)
            {
                squareSpriteRenderers[i].sprite = (entityController is BossStateManager boss2 ? boss2.stacks[i].effect : ((EnemyController)entityController).stacks[i].effect) switch
                {
                    EffectManager.Effect.Bleed => stackSprites[0],
                    EffectManager.Effect.Chill => stackSprites[1],
                    EffectManager.Effect.Target => stackSprites[2],
                    EffectManager.Effect.Wealth => stackSprites[3],
                    _ => null
                };
            }
        }
    }
}
