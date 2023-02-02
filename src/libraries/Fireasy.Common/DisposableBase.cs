﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace Fireasy.Common
{
    /// <summary>
    /// 实现了标准的 <see cref="IDisposable"/> 模式的抽象类。
    /// </summary>
    public abstract class DisposableBase : IDisposable, ISpecificDisposable
        , IAsyncDisposable, ISpecificAsyncDisposable
    {
        protected bool IsDisposed = false;

        /// <summary>
        /// 获取是否检验是否已经释放，当为 true 时，重复 Dispose 会引发 <see cref="ObjectDisposedException"/> 异常。默认为 false。
        /// </summary>
        public virtual bool VerifyDisposed { get; }

        /// <summary>
        /// 
        /// </summary>
        [SuppressMessage("Design", "CA1063")]
        ~DisposableBase()
        {
            DoDispose(false);
        }

        /// <summary>
        /// 释放对象所占用的非托管和托管资源。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
        /// <returns></returns>
        protected virtual bool Dispose(bool disposing)
        {
            return DisposeAsync(disposing).AsTask().GetAwaiter().GetResult();
        }

        private void DoDispose(bool disposing)
        {
            if (VerifyDisposed && IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (!IsDisposed)
            {
                if (Dispose(disposing))
                {
                    IsDisposed = true;
                }
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 手动释放非托管资源。
        /// </summary>
        [SuppressMessage("Design", "CA1063")]
        public void Dispose()
        {
            DoDispose(true);
        }

        void ISpecificDisposable.Dispose(bool disposing)
        {
            DoDispose(disposing);
        }

        ValueTask ISpecificAsyncDisposable.DisposeAsync(bool disposing)
        {
            return DoDisposeAsync(disposing);
        }

        /// <summary>
        /// 异步的，释放对象所占用的非托管和托管资源。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
        /// <returns></returns>
        protected virtual ValueTask<bool> DisposeAsync(bool disposing)
        {
            return new ValueTask<bool>(true);
        }

        private async ValueTask DoDisposeAsync(bool disposing)
        {
            if (VerifyDisposed && IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (!IsDisposed)
            {
                if (await DisposeAsync(disposing))
                {
                    IsDisposed = true;
                }
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 异步的，手动释放非托管资源。
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DoDisposeAsync(true);
        }
    }
}
