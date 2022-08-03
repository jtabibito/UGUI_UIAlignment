using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAlignment
{
    public RectTransform m_rtRoot;

    public enum Axis
    {
        Horizontal,
        Vertical
    }
    public Axis m_axis;

    private TextAnchor m_anchor;
    public TextAnchor Anchor
    {
        get
        {
            return m_anchor;
        }
        set
        {
            m_anchor = value;
            AdjustAlignment();
        }
    }

    private bool m_ReverseArrangement;
    public bool ReverseArrangement
    {
        get
        {
            return m_ReverseArrangement;
        }
        set
        {
            m_ReverseArrangement = value;
            Reverse();
        }
    }

    public Vector2 spacing = Vector2.zero;
    public Vector2 childSpacing = Vector2.zero;

    public List<RectTransform> listRectTransforms;

    public UIAlignment()
    {
        m_axis = Axis.Horizontal;
        m_ReverseArrangement = false;
        m_anchor = TextAnchor.UpperLeft;

        listRectTransforms = new List<RectTransform>(4);
    }

    public void Init(RectTransform rtRoot)
    {
        m_rtRoot = rtRoot;

        foreach (RectTransform child in rtRoot)
        {
            if (child.gameObject.activeSelf)
            {
                listRectTransforms.Add(child);
            }
        }
    }

    public float GetAlignmentPivot(int axis)
    {
        if (axis == 0)
            return 0.5f*((int)m_anchor % 3);
        else
            return 0.5f*((8 - (int)m_anchor)/3);
    }

    public void AdjustAlignment()
    {
        Vector2 alignment = new Vector2(GetAlignmentPivot(0), GetAlignmentPivot(1));
        // Vector2 requireSpace = new Vector2(x, y) + spacing;
        Vector2 requireSpace = m_rtRoot.rect.size * (alignment - m_rtRoot.pivot);

        Vector2[] listChildSpacing = new Vector2[listRectTransforms.Count];
        listChildSpacing[0] = Vector2.zero;

        Vector2 realLocalPositionAtStart = listRectTransforms[0].localPosition;
        Vector2 realLocalPosition;
        for (int i = 0; i < listRectTransforms.Count; ++i)
        {
            realLocalPosition = listRectTransforms[i].localPosition;
            listChildSpacing[i] = realLocalPosition - realLocalPositionAtStart;
        }

        requireSpace += (listRectTransforms[0].pivot - alignment) * listRectTransforms[0].rect.size;
        for (int i = 0; i < listRectTransforms.Count; ++i)
        {
            listRectTransforms[i].localPosition = requireSpace + listChildSpacing[i];
        }
    }

    public void Reverse(int axis)
    {
        float[] listChildSpacing = new float[listRectTransforms.Count];
        listChildSpacing[0] = 0;

        float realLocalPositionAtStart = listRectTransforms[0].localPosition[axis];
        for (int next = 1; next < listRectTransforms.Count; ++next)
        {
            float realLocalPosition = listRectTransforms[next].localPosition[axis];
            listChildSpacing[next] = realLocalPositionAtStart - realLocalPosition + listRectTransforms[next].rect.size[axis]*(-0.5f + listRectTransforms[next].pivot[axis]);
        }

        float requireSpace = listRectTransforms[listRectTransforms.Count - 1].localPosition[axis];
        Vector3 position;
        for (int i = 0; i < listRectTransforms.Count; ++i)
        {
            position = listRectTransforms[i].localPosition;
            position[axis] = requireSpace + listChildSpacing[i] + listRectTransforms[i].rect.size[axis]*(-0.5f + listRectTransforms[i].pivot[axis]);
            listRectTransforms[i].localPosition = position;
        }
    }

    public void Reverse()
    {
        Reverse((int)m_axis);
    }

    public void RevertArrangement()
    {
        if (m_ReverseArrangement)
        {
            ReverseArrangement = false;
        }
    }
}