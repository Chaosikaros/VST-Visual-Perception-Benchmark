using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class DraggableImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public List<GameObject> allImages; // 存储所有图片的静态列表，应该在游戏开始时被初始化
    public List<Vector2> snapPositions; // 存储所有可能的位置点
    public int index;
    private int originalIndex; // 拖动前的原始索引
    public Vector2 originalPosition;

    private void Start()
    {
        index = GetComponent<Attribute>().index;
        if (index > 0 && index <= 22)
        {
            allImages= GetComponentInParent<GridGenerator>().team1;
            snapPositions = GetComponentInParent<GridGenerator>().position1;
        }
        if (index > 22 && index <= 44)
        {
            allImages = GetComponentInParent<GridGenerator>().team2;
            snapPositions = GetComponentInParent<GridGenerator>().position2;
        }
        if (index > 44 && index <= 66)
        {
            allImages = GetComponentInParent<GridGenerator>().team3;
            snapPositions = GetComponentInParent<GridGenerator>().position3;
        }
        if (index > 66 && index <= 88)
        {
            allImages = GetComponentInParent<GridGenerator>().team4;
            snapPositions = GetComponentInParent<GridGenerator>().position4;
        }
        // 确定当前图片的初始索引
        //originalIndex = allImages.IndexOf(gameObject);
    }

    public void getAllImage()
    {
        
        index = GetComponent<Attribute>().index;
        if (index > 0 && index <= 22)
        {
            allImages= GetComponentInParent<GridGenerator>().team1;
            snapPositions = GetComponentInParent<GridGenerator>().position1;
        }
        if (index > 22 && index <= 44)
        {
            allImages = GetComponentInParent<GridGenerator>().team2;
            snapPositions = GetComponentInParent<GridGenerator>().position2;
        }
        if (index > 44 && index <= 66)
        {
            allImages = GetComponentInParent<GridGenerator>().team3;
            snapPositions = GetComponentInParent<GridGenerator>().position3;
        }
        if (index > 66 && index <= 88)
        {
            allImages = GetComponentInParent<GridGenerator>().team4;
            snapPositions = GetComponentInParent<GridGenerator>().position4;
        }
        // 确定当前图片的初始索引
        //originalIndex = allImages.IndexOf(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 myAnchoredPosition = rectTransform.anchoredPosition;
        originalPosition = myAnchoredPosition;
        // 可以在这里实现一些拖动开始时的逻辑，如高亮显示等
        if (index > 0 && index <= 22)
        {
            allImages = GetComponentInParent<GridGenerator>().team1;
            //snapPositions = GetComponentInParent<GridGenerator>().position1;
        }
        if (index > 22 && index <= 44)
        {
            allImages = GetComponentInParent<GridGenerator>().team2;
            //snapPositions = GetComponentInParent<GridGenerator>().position2;
        }
        if (index > 44 && index <= 66)
        {
            allImages = GetComponentInParent<GridGenerator>().team3;
            //snapPositions = GetComponentInParent<GridGenerator>().position3;
        }
        if (index > 66 && index <= 88)
        {
            allImages = GetComponentInParent<GridGenerator>().team4;
            //snapPositions = GetComponentInParent<GridGenerator>().position4;
        }
        originalIndex = allImages.IndexOf(gameObject);
        //Debug.Log(originalIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 更新当前图片的位置，使其跟随鼠标或触摸
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int finalIndex = FindClosestSnapPositionIndex(GetComponent<RectTransform>().anchoredPosition);

        // 检查是否需要移动（即最终索引与原始索引不同）
        if (finalIndex != originalIndex)
        {
            Debug.Log(originalIndex);
            if (finalIndex == 0 || finalIndex == 21)
            {
                // 直接使用原始位置恢复图片位置
                GetComponent<RectTransform>().anchoredPosition = originalPosition;
            }
            else
            {
                //Debug.Log(originalIndex);
                // 获取被拖动的图片
                GameObject draggedImage = allImages[originalIndex];

                // 更新allImages数组：首先移除原始位置的图片，然后在新位置插入
                allImages.RemoveAt(originalIndex);
                allImages.Insert(finalIndex, draggedImage);

                // 此时allImages数组已经被更新以反映新的顺序
                // 你可以在这里调用自定义的方法来处理UI上的位置更新或其他操作
                UpdateUIPositions(); // 假设你有一个方法来更新UI
            }
        }
    }
    public void UpdateUIPositions()
    {
        for (int i = 0; i < allImages.Count; i++)
        {
            allImages[i].GetComponent<RectTransform>().anchoredPosition = snapPositions[i];
        }
        if (index <= 22)
        {
            GetComponentInParent<GridGenerator>().team1 = allImages;
        }
        if (index>22&&index <= 44)
        {
            GetComponentInParent<GridGenerator>().team2 = allImages;
        }
        if (index > 44 && index <= 66)
        {
            GetComponentInParent<GridGenerator>().team3 = allImages;
        }
        if (index > 66 && index <= 88)
        {
            GetComponentInParent<GridGenerator>().team4 = allImages;
        }

    }

    // 寻找最接近当前位置的snapPosition索引
    private int FindClosestSnapPositionIndex(Vector3 currentPosition)
    {
        float closestDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < snapPositions.Count; i++)
        {
            float distance = Vector2.Distance(currentPosition, snapPositions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
    
}
