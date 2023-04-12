// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;

namespace Fireasy.Data.Schema
{
    /// <summary>
    /// OleDb 驱动的架构信息的获取方法。
    /// </summary>
    public class OdbcSchema : SchemaBase
    {
        private DataTable? _cachedTables = null;
        private DataTable? _cachedViews = null;

        /// <summary>
        /// 初始化数据类型映射。
        /// </summary>
        protected override void InitializeDataTypes()
        {
            AddDataType(nameof(OdbcType.BigInt), DbType.Int64, typeof(long));
            AddDataType(nameof(OdbcType.Binary), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.Bit), DbType.Boolean, typeof(bool));
            AddDataType(nameof(OdbcType.Char), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.Date), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OdbcType.Decimal), DbType.Decimal, typeof(decimal));
            AddDataType(nameof(OdbcType.Double), DbType.Double, typeof(double));
            AddDataType(nameof(OdbcType.Image), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.Int), DbType.Int32, typeof(int));
            AddDataType(nameof(OdbcType.Numeric), DbType.Decimal, typeof(decimal));
            AddDataType(nameof(OdbcType.NChar), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.NText), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.NVarChar), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.Real), DbType.Single, typeof(float));
            AddDataType(nameof(OdbcType.SmallDateTime), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OdbcType.SmallInt), DbType.Int16, typeof(short));
            AddDataType(nameof(OdbcType.Text), DbType.String, typeof(string));
            AddDataType(nameof(OdbcType.Time), DbType.DateTime, typeof(DateTime));
            AddDataType(nameof(OdbcType.Timestamp), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.TinyInt), DbType.Byte, typeof(byte));
            AddDataType(nameof(OdbcType.UniqueIdentifier), DbType.Guid, typeof(Guid));
            AddDataType(nameof(OdbcType.VarBinary), DbType.Binary, typeof(byte[]));
            AddDataType(nameof(OdbcType.VarChar), DbType.String, typeof(string));
        }

        /// <summary>
        /// 初始化约定查询限制。
        /// </summary>
        protected override void InitializeRestrictions()
        {
            AddRestriction<Table>(s => s.Name, s => s.Type);
            AddRestriction<Column>(s => s.TableName, s => s.Name);
            AddRestriction<View>(s => s.Name);
            AddRestriction<ViewColumn>(s => s.ViewName, s => s.Name);
            AddRestriction<ForeignKey>(s => s.TableName, s => s.Name);
        }

        /// <summary>
        /// 获取是否提供同一限制的集合查询。
        /// </summary>
        public override bool RestrictionMultipleQuerySupport => false;
    }
}
