using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonImage;
    public Sprite normalImage;
    public Sprite hoverImage;

    void Start()
    {
        // 초기 이미지 설정
        buttonImage.sprite = normalImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 커서가 버튼에 들어오면 이미지 변경
        buttonImage.sprite = hoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 커서가 버튼을 벗어나면 이미지 원래대로 변경
        buttonImage.sprite = normalImage;
    }
}