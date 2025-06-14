namespace Earnings.Advance.Platform.WebApi.Utils
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            return value == null ? null : value.ToString().ToLowerInvariant();
        }
    }
}
