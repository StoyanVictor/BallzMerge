using System;
using System.Collections.Generic;
using CodeBase.Grid;
using DG.Tweening;
using UnityEngine;

public class MatchMerger : MonoBehaviour
{
    private GridAnimator gridAnimator = new GridAnimator();
    public void Merge(List<GridCell> matches, float duration, Action onComplete)
    {
        List<GameObject> objs = new List<GameObject>();
        Vector3 centerPoint = Vector3.zero;

        foreach (var cell in matches)
        {
            if (cell.content != null)
            {
                objs.Add(cell.content);
                centerPoint += cell.content.transform.position;
            }
        }
        centerPoint /= objs.Count;

        Sequence mergeSequence = DOTween.Sequence();

        foreach (var cell in matches)
        {
            GameObject obj = cell.content;
            if (obj == null) continue;

            cell.Clear();
            var collider = obj.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;

            GameEvents.BlockDestroyed();
            gridAnimator.MatchAnimation(duration, onComplete, mergeSequence, obj, centerPoint, objs);
        }
    }

   
}
