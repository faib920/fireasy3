﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Syntax;

namespace Fireasy.Data.Identity
{
    /// <summary>
    /// 使用 SYS_SEQUENCE 表作为序列值的生成器。无法继承此类。
    /// </summary>
    public sealed class BaseSequenceGenerator : IGeneratorProvider
    {
        /// <summary>
        /// 自动生成列的值。
        /// </summary>
        /// <param name="database">提供给当前插件的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="tableName">表的名称。</param>
        /// <param name="columnName">列的名称。</param>
        /// <returns>用于标识唯一性的值。</returns>
        public async ValueTask<long> GenerateValueAsync(IDatabase database, string tableName, string? columnName = null)
        {
            //查询下一个值
            var value = 0L;
            var syntax = database.Provider.GetService<ISyntaxProvider>();

            await database.WithTransactionAsync(async db =>
                {
                    if (GeneratorCache.IsSequenceCreated(tableName, columnName, async () =>
                    {
                        if (await db.ExecuteScalarAsync<long>((SqlCommand)syntax!.ExistsTable("SYS_SEQUENCE")) == 0)
                        {
                            await db.ExecuteNonQueryAsync((SqlCommand)string.Format("CREATE TABLE SYS_SEQUENCE(TABLE_NAME {0}, COLUMN_NAME {0}, VALUE {1})",
                                syntax.Column(DbType.String, 100),
                                syntax.Column(DbType.Int32)));
                            await db.ExecuteNonQueryAsync((SqlCommand)string.Format("INSERT INTO SYS_SEQUENCE(TABLE_NAME, COLUMN_NAME, VALUE) VALUES('{0}', '{1}', {2})", tableName, columnName, ++value));
                        }

                        return true;
                    }))
                    {
                        var o = await db.ExecuteScalarAsync((SqlCommand)string.Format("SELECT VALUE FROM SYS_SEQUENCE WHERE TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'", tableName, columnName));
                        if (o == DBNull.Value || o == null)
                        {
                            await db.ExecuteNonQueryAsync((SqlCommand)string.Format("INSERT INTO SYS_SEQUENCE(TABLE_NAME, COLUMN_NAME, VALUE) VALUES('{0}', '{1}', {2})", tableName, columnName, ++value));
                        }
                        else
                        {
                            value = Convert.ToInt32(o) + 1;
                            await db.ExecuteNonQueryAsync((SqlCommand)string.Format("UPDATE SYS_SEQUENCE SET VALUE = {0} WHERE TABLE_NAME = '{1}' AND COLUMN_NAME = '{2}'", value, tableName, columnName));
                        }
                    }
                });

            return value;
        }
    }
}
