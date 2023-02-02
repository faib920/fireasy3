// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fireasy.Data.Identity
{
    /// <summary>
    /// 雪花算法生成器。
    /// </summary>
    public abstract class SnowflakeGenerator : IGeneratorProvider, IServiceProviderAccessor
    {
        private readonly object _locker = new object();
        private long _lastTimestamp = -1L;
        private long _sequence;

        /// <summary>
        /// 获取或设置避免重复的随机量。
        /// </summary>
        public virtual long Twepoch { get; set; } = 697848001128L;

        /// <summary>
        /// 获取或设置机器码字节数。
        /// </summary>
        public virtual int WorkerIdBits { get; set; } = 5;

        /// <summary>
        /// 获取或设置计数器字节数。
        /// </summary>
        public virtual int SequenceBits { get; set; } = 12;

        private long MaxWorkerId => -1L ^ (-1L << WorkerIdBits);

        private int WorkerIdShift => SequenceBits;
        private int TimestampLeftShift => SequenceBits + WorkerIdBits;
        private long SequenceMask => -1L ^ (-1L << SequenceBits);

        IServiceProvider IServiceProviderAccessor.ServiceProvider { get; set; }

        /// <summary>
        /// 自动生成列的值。
        /// </summary>
        /// <param name="database">提供给当前插件的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="tableName">表的名称。</param>
        /// <param name="columnName">列的名称。</param>
        /// <returns>用于标识唯一性的值。</returns>
        public ValueTask<long> GenerateValueAsync(IDatabase database, string tableName, string? columnName = null)
        {
            var options = ((IServiceProviderAccessor)this).ServiceProvider?.GetRequiredService<IOptionsMonitor<SnowflakeOptions>>();
            if (options == null)
            {
                throw new NotSupportedException();
            }

            var workerId = options.CurrentValue.WorkerId;

            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0 ");
            }

            lock (_locker)
            {
                var timestamp = ToTimeStamp();

                if (timestamp < _lastTimestamp)
                {
                    throw new Exception($"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                var value = ((timestamp - Twepoch) << TimestampLeftShift) |
                         (workerId << WorkerIdShift) | _sequence;

                return new ValueTask<long>(value);
            }
        }

        /// <summary>
        /// 获取下一微秒时间戳。
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long TilNextMillis(long lastTimestamp)
        {
            var timestamp = ToTimeStamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = ToTimeStamp();
            }

            return timestamp;
        }

        /// <summary>
        /// 生成当前时间戳。
        /// </summary>
        /// <returns></returns>
        private long ToTimeStamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
