using System.Threading.Tasks;
using UnityEngine;

namespace Systems.Pool
{
    public interface IPool<T>
    {
        public void InitializeRandom(int amount, int offset = 0);
        public void InitializeAll(int repeat = 0, int offset = 0);
        public Task<T> Get(int offset = 0);
        public void Return(T data);
        public void Clear();
    }

    public class PoolData<T>
    {
        public PoolData(GameObject obj)
        {
            Obj = obj;

            Component = obj ? obj.gameObject.GetComponent<T>() : default;
        }

        public GameObject Obj { get; private set; }
        public T Component { get; private set; }

        public static implicit operator bool(PoolData<T> data)
        {
            return data != null;
        }
    }
}