namespace GK.Abstractions
{
    public interface ISpecification<T>
    {
        public bool IsSatisifiedBy(T value);
    }
}
