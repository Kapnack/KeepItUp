using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Platform.States
{
    public abstract class BasePlatformState : IPlatformState
    {
        protected GameObject Owner;

        protected float Progress;

        private bool _increase;

        protected float Speed;

        private bool _deactivating;

        protected Action<IPlatformState> ExitAction;

        public virtual void Enter(GameObject owner)
        {
            Owner = owner;
        }

        public void Update()
        {
            if (!Owner)
                return;

            if (!_deactivating)
                NormalUpdate();
            else
                ExitRoutine();
            
        }

        protected virtual void NormalUpdate()
        {
            if (!Owner)
                return;

            if (!_deactivating)
            {
                if (_increase)
                {
                    Progress += Speed * Time.deltaTime;
                    if (Progress > 1.0f)
                    {
                        _increase = false;
                        Progress = 1f;
                    }
                }
                else
                {
                    Progress -= Speed * Time.deltaTime;
                    if (Progress < 0.0f)
                    {
                        _increase = true;
                        Progress = 0f;
                    }
                }
            }
        }

        public virtual void Exit(Action<IPlatformState> callback)
        {
            ExitAction = callback;
            _deactivating = true;
        }
        
        protected abstract void ExitRoutine();
    }
}