using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float animScaleMultiplier;
    private Vector3 defaultScale;
    private RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOScale(defaultScale*animScaleMultiplier, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOScale(defaultScale, 0.2f);
    }
}
