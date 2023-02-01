// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Fireasy.DistributedLocking
{
    /// <summary>
    /// 提供分布式锁。
    /// </summary>
    public interface IDistributedLocker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockKey"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task LockAsync(string lockKey, TimeSpan timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lockKey"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<T> LockAsync<T>(string lockKey, TimeSpan timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockKey"></param>
        /// <param name="timeout"></param>
        /// <param name="onFailure"></param>
        /// <returns></returns>
        Task TryLockAsync(string lockKey, TimeSpan timeout, Func<Task>? onFailure = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lockKey"></param>
        /// <param name="timeout"></param>
        /// <param name="onFailure"></param>
        /// <returns></returns>
        Task<T> TryLockAsync<T>(string lockKey, TimeSpan timeout, Func<Task<T>>? onFailure = null);
    }
}
