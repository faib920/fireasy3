using System;

namespace Fireasy.Caching
{
    public class SlidingExpired : ICacheItemExpiration
    {
        public TimeSpan? GetExpirationTime()
        {
            throw new NotImplementedException();
        }

        public bool HasExpired()
        {
            throw new NotImplementedException();
        }
    }
}
