using UnityEngine;

namespace Platform.States
{
    public class MovePlatform : BasePlatformState
    {
        private Vector2 _startPosition;
        private Vector2 _targetPosition;

        private readonly float _maxDistance;
        private bool _movingToTarget = true;

        public MovePlatform(float speed, float maxDistance = 5f)
        {
            Speed = speed;
            _maxDistance = maxDistance;
        }

        public override void Enter(GameObject owner)
        {
            base.Enter(owner);
            _startPosition = owner.transform.localPosition;
            _targetPosition = GetNewRandomTarget();
        }

        public override void Update()
        {
            Owner.transform.localPosition = Vector2.MoveTowards(
                Owner.transform.localPosition,
                _targetPosition,
                Speed * Time.deltaTime
            );


            if (Vector2.Distance(Owner.transform.localPosition, _targetPosition) < 0.01f)
            {
                if (_movingToTarget)
                {
                    _targetPosition = _startPosition;
                    _movingToTarget = false;
                }
                else
                {
                    _targetPosition = GetNewRandomTarget();
                    _movingToTarget = true;
                }
            }
        }

        private Vector2 GetNewRandomTarget()
        {
            Vector2 randomOffset;

            do
            {
                randomOffset = Random.insideUnitCircle * _maxDistance;
            } while (randomOffset.y < 0);

            return _startPosition + randomOffset;
        }

        protected override void ExitRoutine()
        {
            Owner.transform.localPosition = Vector2.MoveTowards(
                Owner.transform.localPosition,
                _startPosition,
                Speed * Time.deltaTime
            );

            if (Vector2.Distance(Owner.transform.localPosition, _startPosition) < 0.01f)
            {
                ExitAction?.Invoke(this);
            }
        }
    }
}