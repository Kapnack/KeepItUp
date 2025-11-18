using UnityEngine;

namespace Platform.States
{
    public class ResizePlatform : BasePlatformState
    {
        private readonly Vector2 _minSize;
        private readonly Vector2 _maxSize;
        
        private Vector2 _initialSize;
        private Vector2 _exitSize;
        
        float t = 0f;
        public ResizePlatform(Vector2 minSize, Vector2 maxSize, float speed)
        {
            _minSize = minSize;
            _maxSize = maxSize;
            Speed = speed;
        }
        
        public override void Enter(GameObject owner)
        {
            base.Enter(owner);
            _initialSize =  Owner.transform.localScale;
        }

        public override void Update()
        {
           base.Update();

            Owner.transform.localScale = Vector3.Lerp(_minSize, _maxSize, Progress);
        }

        protected override void ExitRoutine()
        {
            _exitSize = Owner.transform.localScale;
            
            while (t < 1f)
            {
                t += Time.deltaTime * Speed;
                Owner.transform.localScale = Vector3.Lerp(_exitSize, _initialSize, t);
            }
            
            ExitAction?.Invoke(this);
        }
    }
}