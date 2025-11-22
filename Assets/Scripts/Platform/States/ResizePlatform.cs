using System;
using UnityEngine;

namespace Platform.States
{
    public class ResizePlatform : BasePlatformState
    {
        private readonly Vector2 _minSize;
        private readonly Vector2 _maxSize;

        private Vector2 _initialSize;
        private Vector2 _exitStartSize;

        float _exitProgress = 0f;

        public ResizePlatform(Vector2 minSize, Vector2 maxSize, float speed)
        {
            _minSize = minSize;
            _maxSize = maxSize;
            Speed = speed;
        }

        public override void Enter(GameObject owner)
        {
            base.Enter(owner);
            _initialSize = Owner.transform.localScale;
        }

        protected override void NormalUpdate()
        {
            base.NormalUpdate();

            Owner.transform.localScale = Vector3.Lerp(_minSize, _maxSize, Progress);
        }

        protected override void ExitRoutine()
        {
            _exitProgress += Time.deltaTime * Speed;

            Owner.transform.localScale =
                Vector3.Lerp(_exitStartSize, _initialSize, _exitProgress);
            
            if (_exitProgress >= 1f)
            {
                Owner.transform.localScale = _initialSize;
                ExitAction?.Invoke(this);
            }
        }

        public override void Exit(Action<IPlatformState> callback)
        {
            base.Exit(callback);
            
            _exitStartSize = Owner.transform.localScale;
        }
    }
}