
#nullable enable
using System;
using System.Collections.Generic;

namespace Systems.EventSystem
{
    public class CentralizedEventSystem
    {
        private readonly Dictionary<Type, Delegate?> _events = new();

        public void Register<TDelegate>(TDelegate del) where TDelegate : Delegate
        {
            Type type = typeof(TDelegate);

            if (!_events.TryAdd(type, del))
                _events[type] = Delegate.Combine(_events[type], del);
        }

        public void Unregister<TDelegate>() where TDelegate : Delegate
        {
            Type type = typeof(TDelegate);
            _events.Remove(type);
        }
        
        public TDelegate? Get<TDelegate>() where TDelegate : Delegate
        {
            Type type = typeof(TDelegate);

            if (_events.TryGetValue(type, out Delegate? del))
                return del as TDelegate;

            _events[type] = null;
            return null;
        }

        public void AddListener<TDelegate>(TDelegate listener) where TDelegate : Delegate
        {
            Type type = typeof(TDelegate);

            if (_events.TryGetValue(type, out Delegate? del))
                _events[type] = Delegate.Combine(del, listener);
            else
                _events[type] = listener;
        }
        
        public void RemoveListener<TDelegate>(TDelegate listener) where TDelegate : Delegate
        {
            Type type = typeof(TDelegate);

            if (_events.TryGetValue(type, out Delegate? existing))
            {
                Delegate? newDelegate = Delegate.Remove(existing, listener);

                // If no more listeners remain, remove the key to clean up
                if (newDelegate == null)
                    _events.Remove(type);
                else
                    _events[type] = newDelegate;
            }
        }
    }

    public class Event<T> where T : Delegate
    {
        private T? _eventDelegate;

        public void AddListener(T listener)
        {
            _eventDelegate = (T)Delegate.Combine(_eventDelegate, listener);
        }
    
        public void RemoveListener(T listener)
        {
            _eventDelegate = (T?)Delegate.Remove(_eventDelegate, listener);
        }
    
        public void Invoke(params object[] args)
        {
            _eventDelegate?.DynamicInvoke();
        }
    }
}