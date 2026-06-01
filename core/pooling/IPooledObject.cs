

namespace WeWereDumb.Core.Pooling
{
    public interface IPooledObject
    {
        void SetPool(ObjectPool pool);
        ObjectPool GetPool();
    }
}