using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    RectTransform rt;
    RectTransform canvasRect;
    public Transform target;

    public void Init(RectTransform canvas, Transform _target)
    {
        rt = GetComponent<RectTransform>();
        canvasRect = canvas;
        target = _target;
    }

    void Update()
    {

        if (target.parent != null && !target.parent.gameObject.activeSelf) Destroy(gameObject);

        Vector2 view = Camera.main.WorldToViewportPoint(target.position);
        Vector2 world = new Vector2(
            (view.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (view.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));

        rt.anchoredPosition = world;
    }
}
