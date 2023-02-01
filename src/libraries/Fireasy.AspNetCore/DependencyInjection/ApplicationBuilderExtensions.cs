using Microsoft.AspNetCore.Builder;

namespace Fireasy.AspNetCore.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFireasy(this IApplicationBuilder builder)
        {
            return builder;
        }
    }
}
