using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BoxCollider2D itemCollider;

    public void Initialize(ItemData itemData)
    {
        //set sprite
        spriteRenderer.sprite = itemData.sprite;
        //set sprite offset
        spriteRenderer.transform.localPosition = new Vector2(0.5f * itemData.size.x, 0.5f * itemData.size.y);
        
        itemCollider.size = itemData.size;
        itemCollider.offset = spriteRenderer.transform.localPosition;
        
        //spawn Prefabs
        Instantiate(itemData.prefab, transform.position, Quaternion.identity);
    }
}
