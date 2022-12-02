using Shop;
using UnityEngine;

namespace AI
{
    public class StackUIManager : MonoBehaviour
    {
        private EnemyController enemyController;
        private SpriteRenderer[] squareSpriteRenderers = new SpriteRenderer[3];
        
        private void Start()
        {
            enemyController = transform.parent.GetComponent<EnemyController>();
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
            Debug.Log(enemyController.stacks[i].effect != EffectManager.Effect.None);
            squareSpriteRenderers[i].enabled = enemyController.stacks[i].effect != EffectManager.Effect.None;

            squareSpriteRenderers[i].color = enemyController.stacks[i].effect switch
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
