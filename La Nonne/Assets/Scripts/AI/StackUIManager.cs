using AI.Boss;
using Shop;
using UnityEngine;

namespace AI
{
    public class StackUIManager : MonoBehaviour
    {
        private Component entityController;
        private SpriteRenderer[] squareSpriteRenderers = new SpriteRenderer[3];
        
        private void Start()
        {
            entityController = transform.parent.CompareTag("Boss") ? transform.parent.GetComponent<BossStateManager>() : transform.parent.GetComponent<EnemyController>();
            squareSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
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
            // FROM
           // squareSpriteRenderers[i].enabled = (entityController is BossStateManager boss ? boss.stacks[i].effect : ((EnemyController)entityController).stacks[i].effect) != EffectManager.Effect.None;

           
           // INTO
           
            bool isActive = false;
            
            if (entityController is BossStateManager boss && boss.stacks[i].effect != EffectManager.Effect.None)
            {
                isActive = true;
            }
            else if (entityController is EnemyController en && en.stacks[i].effect != EffectManager.Effect.None)
            {
                isActive = true;
            }

            squareSpriteRenderers[i].enabled = isActive;
            
            //
            
            if (squareSpriteRenderers[i].enabled)
            {
                squareSpriteRenderers[i].color = (entityController is BossStateManager boss2 ? boss2.stacks[i].effect : ((EnemyController)entityController).stacks[i].effect) switch
                {
                    EffectManager.Effect.Bleed => Color.red,
                    EffectManager.Effect.Chill => Color.cyan,
                    EffectManager.Effect.Target => Color.green,
                    EffectManager.Effect.Wealth => Color.yellow,
                    _ => Color.white
                };
            }
        }
    }
}
