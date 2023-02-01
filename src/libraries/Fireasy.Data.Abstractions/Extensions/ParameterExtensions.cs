// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Data.Provider;
using System;
using System.Data;
using System.Data.Common;

namespace Fireasy.Data.Extensions
{
    /// <summary>
    /// <see cref="Parameter"/> 扩展类。
    /// </summary>
    public static class ParameterExtensions
    {
        /// <summary>
        /// 将自定的Parameter转换到IDataParameter接口
        /// </summary>
        /// <param name="sourcePar"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DbParameter ToDbParameter(this Parameter sourcePar, IProvider provider)
        {
            Guard.ArgumentNull(sourcePar, nameof(sourcePar));

            var parameter = provider.DbProviderFactory.CreateParameter();
            Guard.NullReference(parameter);

            parameter.ParameterName = sourcePar.ParameterName;
            parameter.SourceColumn = sourcePar.SourceColumn;
            parameter.Direction = sourcePar.Direction;
            parameter.DbType = sourcePar.DbType;

            //处理空值
            if (sourcePar.IsInput() &&
                sourcePar.Value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = sourcePar.Value;
            }

            var dbParameter = parameter as IDbDataParameter;
            if (sourcePar.Size != 0)
            {
                dbParameter.Size = sourcePar.Size;
            }

            if (sourcePar.Precision != 0)
            {
                dbParameter.Precision = sourcePar.Precision;
            }

            if (sourcePar.Scale != 0)
            {
                dbParameter.Scale = sourcePar.Scale;
            }

            if (!string.IsNullOrEmpty(sourcePar.SourceColumn))
            {
                dbParameter.SourceColumn = sourcePar.SourceColumn;
            }

            dbParameter.SourceVersion = sourcePar.SourceVersion;

            return parameter;
        }

        /// <summary>
        /// 判断参数是否为输入型参数。
        /// </summary>
        /// <param name="parameter">存储参数。</param>
        /// <returns></returns>
        public static bool IsInput(this Parameter parameter)
        {
            return parameter.Direction == ParameterDirection.Input ||
                parameter.Direction == ParameterDirection.InputOutput;
        }

        /// <summary>
        /// 判断参数是否为输出型参数。
        /// </summary>
        /// <param name="parameter">存储参数。</param>
        /// <returns></returns>
        public static bool IsOutput(this Parameter parameter)
        {
            return parameter.Direction != ParameterDirection.Input;
        }
    }
}
