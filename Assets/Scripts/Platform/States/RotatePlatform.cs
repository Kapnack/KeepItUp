using System;
using UnityEngine;

namespace Platform.States
{
    public class RotatePlatform : BasePlatformState
    {
        private bool _increase;
        private readonly float _maxAngle;

        float duration = 0.5f;
        float t = 0f;

        private Quaternion startRot;
        Quaternion targetRot = Quaternion.identity;

        public RotatePlatform(float maxAngle, float speed)
        {
            _maxAngle = maxAngle;
            Speed = speed;
        }

        protected override void NormalUpdate()
        {
            base.NormalUpdate();

            Owner.transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0, 0, -_maxAngle),
                Quaternion.Euler(0, 0, _maxAngle),
                Progress
            );
        }

        protected override void ExitRoutine()
        {
            if (t < 1f)
            {
                t += Time.deltaTime / duration;
                Owner.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            }
            else
                ExitAction?.Invoke(this);
        }

        public override void Exit(Action<IPlatformState> callback)
        {
            base.Exit(callback);

            startRot = Owner.transform.rotation;
        }
    }
}