// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Collections;
using Fireasy.Common.Extensions;
using Fireasy.Data.RecordWrapper;
using Fireasy.Data.Schema.Linq;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// 一个抽象类，提供获取数据库架构的方法。
    /// </summary>
    public abstract class SchemaBase : ISchemaProvider
    {
        private readonly Dictionary<Type, List<MemberInfo>> _dicRestrMbrs = new Dictionary<Type, List<MemberInfo>>();
        private readonly List<DataType> _dataTypes = new List<DataType>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        protected ConnectionParameter GetConnectionParameter(IDatabase database)
        {
            return database.Provider.GetConnectionParameter(database!.ConnectionString);
        }

        /// <summary>
        /// 获取指定类型的数据库架构信息。
        /// </summary>
        /// <typeparam name="T">架构信息的类型。</typeparam>
        /// <param name="database">提供给当前插件的 <see cref="IDatabase"/> 对象。</param>
        /// <param name="predicate">用于测试架构信息是否满足条件的函数。</param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<T> GetSchemasAsync<T>(IDatabase database, Expression<Func<T, bool>>? predicate = null) where T : ISchemaMetadata
        {
            var restrictionValues = SchemaQueryTranslator.GetRestrictions<T>(predicate, _dicRestrMbrs);

            if (typeof(Database) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetDatabasesAsync(database, restrictionValues);
            }
            else if (typeof(DataType) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetDataTypesAsync(database, restrictionValues);
            }
            else if (typeof(MetadataCollection) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetMetadataCollectionsAsync(database, restrictionValues);
            }
            else if (typeof(ReservedWord) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetReservedWordsAsync(database, restrictionValues);
            }
            else if (typeof(Table) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetTablesAsync(database, restrictionValues);
            }
            else if (typeof(Column) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetColumnsAsync(database, restrictionValues);
            }
            else if (typeof(View) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetViewsAsync(database, restrictionValues);
            }
            else if (typeof(ViewColumn) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetViewColumnsAsync(database, restrictionValues);
            }
            else if (typeof(Index) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetIndexsAsync(database, restrictionValues);
            }
            else if (typeof(IndexColumn) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetIndexColumnsAsync(database, restrictionValues);
            }
            else if (typeof(Procedure) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetProceduresAsync(database, restrictionValues);
            }
            else if (typeof(ProcedureParameter) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetProcedureParametersAsync(database, restrictionValues);
            }
            else if (typeof(ForeignKey) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetForeignKeysAsync(database, restrictionValues);
            }
            else if (typeof(User) == typeof(T))
            {
                return (IAsyncEnumerable<T>)GetUsersAsync(database, restrictionValues);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// 为 <typeparamref name="T"/> 类型添加约定限定。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restrs"></param>
        protected void AddRestriction<T>(params Expression<Func<T, object>>[] restrs) where T : ISchemaMetadata
        {
            Guard.ArgumentNull(restrs, nameof(restrs));

            var members = _dicRestrMbrs.TryGetValue(typeof(T), () => new List<MemberInfo>());
            members.Clear();

            for (int i = 0, n = restrs.Length; i < n; i++)
            {
                var mbrExp = restrs[i].Body as MemberExpression;
                if (mbrExp == null && restrs[i].Body.NodeType == ExpressionType.Convert && restrs[i].Body is UnaryExpression unaryExp)
                {
                    mbrExp = unaryExp!.Operand as MemberExpression;
                }

                if (mbrExp?.Member is PropertyInfo property)
                {
                    members.Add(property);
                }
            }
        }

        /// <summary>
        /// 添加数据类型。
        /// </summary>
        /// <param name="name">类型名称。</param>
        /// <param name="dbType"></param>
        /// <param name="systemType"></param>
        protected void AddDataType(string name, DbType dbType, Type systemType)
        {
            _dataTypes.Add(new DataType { Name = name, DbType = dbType, SystemType = systemType });
        }

        /// <summary>
        /// 添加数据类型。
        /// </summary>
        /// <param name="names">类型名称数组。</param>
        /// <param name="dbType"></param>
        /// <param name="systemType"></param>
        protected void AddDataType(string[] names, DbType dbType, Type systemType)
        {
            foreach (var name in names)
            {
                _dataTypes.Add(new DataType { Name = name, DbType = dbType, SystemType = systemType });
            }
        }

        /// <summary>
        /// 获取 <see cref="Database"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<Database> GetDatabasesAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<Database>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="ReservedWord"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<ReservedWord> GetReservedWordsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<ReservedWord>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="Restriction"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<Restriction> GetRestrictionsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<Restriction>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="User"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<User> GetUsersAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<User>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="Table"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<Table> GetTablesAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<Table>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="Column"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<Column> GetColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<Column>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="View"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<View> GetViewsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<View>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="ViewColumn"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<ViewColumn> GetViewColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<ViewColumn>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="Procedure"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<Procedure> GetProceduresAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<Procedure>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="ProcedureParameter"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<ProcedureParameter> GetProcedureParametersAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<ProcedureParameter>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="ForeignKey"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<ForeignKey> GetForeignKeysAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<ForeignKey>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="DataType"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<DataType> GetDataTypesAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            using var enumerator = _dataTypes.GetEnumerator();
            return new Sync2AsyncEnumerable<DataType>(enumerator);
        }

        /// <summary>
        /// 获取 <see cref="Index"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<Index> GetIndexsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<Index>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="IndexColumn"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<IndexColumn> GetIndexColumnsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<IndexColumn>.Empty;
        }

        /// <summary>
        /// 获取 <see cref="MetadataCollection"/> 元数据序列。
        /// </summary>
        /// <param name="database"></param>
        /// <param name="restrictionValues"></param>
        /// <returns></returns>
        protected virtual IAsyncEnumerable<MetadataCollection> GetMetadataCollectionsAsync(IDatabase database, RestrictionDictionary restrictionValues)
        {
            return AsyncEmptyEnumerable<MetadataCollection>.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        protected async IAsyncEnumerable<T> ExecuteAndParseMetadataAsync<T>(IDatabase database, SpecialCommand sql, ParameterCollection? parameters, Func<IRecordWrapper?, IDataReader, T> parser)
        {
            using var reader = (IAsyncIDataReader)await database.ExecuteReaderAsync(sql, parameters: parameters);
            var wrapper = database.Provider.GetService<IRecordWrapper>();
            while (await reader.ReadAsync())
            {
                yield return parser(wrapper, reader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        protected virtual Column SetColumnType(Column column)
        {
            if (column.NumericPrecision > 0 && column.NumericScale != null && column.DataType.IndexOf("int", StringComparison.CurrentCultureIgnoreCase) == -1)
            {
                column.ColumnType = $"{column.DataType}({column.NumericPrecision},{column.NumericScale})";
            }
            else if (column.NumericPrecision > 0)
            {
                column.ColumnType = $"{column.DataType}({column.NumericPrecision})";
            }
            else if (column.Length > 0)
            {
                column.ColumnType = $"{column.DataType}({column.Length})";
            }
            else
            {
                column.ColumnType = column.DataType;
            }

            return column;
        }
    }
}
