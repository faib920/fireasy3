﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Syntax;

namespace Fireasy.Data
{
    /// <summary>
    /// 以尝试下一页作为评估依据，它并不计算数据的总量及总页数，始终只判断下一页是否有效，该评估器一般用于数据量非常大的环境中。无法继承此类。
    /// </summary>
    public sealed class TryNextEvaluator : IDataPageEvaluator
    {
        async Task IDataPageEvaluator.EvaluateAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (!(context.Segment is IPager dataPager))
            {
                return;
            }

            var nextPage = CreateNextPager(dataPager);
            var sql = ChangeCommand(context, nextPage);

            //查询下一页是否有数据
            var result = await context.Database.ExecuteScalarAsync<int>(sql);
            if (result == 0)
            {
                //查询当前页剩余的记录
                nextPage.CurrentPageIndex--;
                sql = ChangeCommand(context, nextPage);
                result = await context.Database.ExecuteScalarAsync<int>(sql);

                HandleNoNextPage(dataPager, result);
            }
            else
            {
                HandleHasNextPage(dataPager);
            }
        }

        /// <summary>
        /// 获取是否还有下一页。
        /// </summary>
        public bool HasNextPage { get; private set; }

        private SqlCommand ChangeCommand(CommandContext context, DataPager pager)
        {
            var sql = context.TryChangeSegment(pager, () =>
            {
                var syntax = context.Database.Provider.GetService<ISyntaxProvider>();
                syntax.Segment(context);
                return context.Command.CommandText;
            });

            return $"SELECT COUNT(1) FROM ({sql}) TEMP";
        }

        private DataPager CreateNextPager(IPager pager)
        {
            return new DataPager(pager.PageSize, pager.CurrentPageIndex + 1);
        }

        private void HandleNoNextPage(IPager pager, int result)
        {
            pager.RecordCount = pager.PageSize * pager.CurrentPageIndex + result;
            HasNextPage = false;
        }

        private void HandleHasNextPage(IPager pager)
        {
            pager.RecordCount = pager.PageSize * (pager.CurrentPageIndex + 1) + 1;
            HasNextPage = true;
        }
    }
}
