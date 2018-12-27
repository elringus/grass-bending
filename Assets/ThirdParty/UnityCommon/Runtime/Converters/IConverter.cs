using System.Threading.Tasks;

namespace UnityCommon
{
    /// <summary>
    /// Implentation is able to convert objects.
    /// </summary>
    public interface IConverter
    {
        object Convert (object obj);
        Task<object> ConvertAsync (object obj);
    }

    /// <summary>
    /// Implentation is able to convert <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
    /// </summary>
    public interface IConverter<TSource, TResult> : IConverter
    {
        TResult Convert (TSource obj);
        Task<TResult> ConvertAsync (TSource obj);
    }
}
