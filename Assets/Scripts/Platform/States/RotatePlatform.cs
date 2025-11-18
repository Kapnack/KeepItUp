using UnityEngine;

namespace Platform.States
{
    public class RotatePlatform : BasePlatformState
    {
        private bool _increase;
        private readonly float _maxAngle;

        public RotatePlatform(float maxAngle, float speed)
        {
            _maxAngle = maxAngle;
            Speed = speed;
        }

        public override void Update()
        {
            base.Update();

            Owner.transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0, 0, -_maxAngle),
                Quaternion.Euler(0, 0, _maxAngle),
                Progress
            );
        }

        protected override void ExitRoutine()
        {
            ExitAction?.Invoke(this);
        }
    }
}