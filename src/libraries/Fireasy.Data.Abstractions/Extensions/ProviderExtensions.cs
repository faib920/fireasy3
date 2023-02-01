using Fireasy.Common;
using Fireasy.Data.Provider;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Fireasy.Data.Extensions
{
    public static class ProviderExtensions
    {
        public static DbCommand CreateCommand(this IProvider provider, DbConnection connection, DbTransaction? transaction, string? commandText, CommandType commandType = CommandType.Text, IEnumerable<Parameter> parameters = null)
        {
            Guard.ArgumentNull(provider, nameof(provider));
            Guard.NullReference(provider.DbProviderFactory);

            var command = provider.DbProviderFactory.CreateCommand();
            command.Connection = connection;
            command.CommandType = commandType;
            command.CommandText = commandText;
            command.Transaction = transaction;

            if (parameters != null)
            {
                command.PrepareParameters(provider, parameters);
            }
            return command;
        }
    }
}
