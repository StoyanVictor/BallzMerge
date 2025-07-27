using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Grid
{
    public class GridAnimator
    {
        public void MoveAnimation(GameObject obj,Vector3 targetPos,float duration)
        {
            obj.transform.DOMove(targetPos, duration).SetEase(Ease.OutQuad);
        }
        public  void MatchAnimation(float duration, Action onComplete, Sequence mergeSequence, GameObject obj,
            Vector3 centerPoint, List<GameObject> objs)
        {
            mergeSequence.Join(obj.transform.DOMove(centerPoint, duration * 0.7f).SetEase(Ease.InQuad));
            mergeSequence.Join(obj.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));

            var renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                mergeSequence.Join(renderer.DOColor(Color.white * 1.5f, duration * 0.3f).SetLoops(2, LoopType.Yoyo));
            }

            mergeSequence.OnComplete(() =>
            {
                foreach (var o in objs)
                {
                    if (o != null) Object.Destroy(o);
                }
                onComplete?.Invoke();
            });
        }
    }
}