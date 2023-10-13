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
        // �ʱ� �̹��� ����
        buttonImage.sprite = normalImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺 Ŀ���� ��ư�� ������ �̹��� ����
        buttonImage.sprite = hoverImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���콺 Ŀ���� ��ư�� ����� �̹��� ������� ����
        buttonImage.sprite = normalImage;
    }
}