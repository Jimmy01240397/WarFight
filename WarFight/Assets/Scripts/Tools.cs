using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static Vector2 GetSize(this RectTransform rectTransform)
    {
        RectTransform parentrect = rectTransform.parent == null ? null : rectTransform.parent.GetComponent<RectTransform>();
        bool parentgetbefore = false;
        Vector2 parentsize = new Vector2();
        float width;
        if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
            width = rectTransform.sizeDelta.x;
        else
        {
            if (!parentgetbefore)
            {
                parentsize = parentrect.GetSize();
                parentgetbefore = true;
            }
            width = (rectTransform.anchorMax.x - rectTransform.anchorMin.x) * parentsize.x - rectTransform.offsetMin.x + rectTransform.offsetMax.x;
        }

        float height;
        if (rectTransform.anchorMin.y == rectTransform.anchorMax.y)
            height = rectTransform.sizeDelta.y;
        else
        {
            if (!parentgetbefore)
            {
                parentsize = parentrect.GetSize();
                parentgetbefore = true;
            }
            height = (rectTransform.anchorMax.y - rectTransform.anchorMin.y) * parentsize.y - rectTransform.offsetMin.y + rectTransform.offsetMax.y;
        }
        return new Vector2(width, height);
    }
}
