using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BoxCollider2D itemCollider;

    /*[SerializeField]
    int health = 3;
    [SerializeField]
    bool nonDestructible;*/

    /*[SerializeField]
    private GameObject hitFeedback, destoyFeedback;*/

    /*public UnityEvent OnGetHit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }*/

    public void Initialize(ItemData itemData)
    {
        //set sprite
        spriteRenderer.sprite = itemData.sprite;
        //set sprite offset
        spriteRenderer.transform.localPosition = new Vector2(0.5f * itemData.size.x, 0.5f * itemData.size.y);
        
        itemCollider.size = itemData.size;
        itemCollider.offset = spriteRenderer.transform.localPosition;
        
        //spawn Prefabs
        //itemData.prefab = Instantiate(itemData.prefab, transform.position, Quaternion.identity);

        /*if (itemData.nonDestructible)
            nonDestructible = true;

        this.health = itemData.health;*/

    }

    /*public void GetHit(int damage, GameObject damageDealer)
    {
        if (nonDestructible)
            return;
        if(health>1)
            Instantiate(hitFeedback, spriteRenderer.transform.position, Quaternion.identity);
        else
            Instantiate(destoyFeedback, spriteRenderer.transform.position, Quaternion.identity);
        spriteRenderer.transform.DOShakePosition(0.2f, 0.3f, 75, 1, false, true).OnComplete(ReduceHealth);
    }*/

    /*private void ReduceHealth()
    {
        health--;
        if (health <= 0)
        {
            spriteRenderer.transform.DOComplete();
            Destroy(gameObject);
        }
            
    }*/
}
