using System.Data;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using StreamProcessing.SqlExecutor.Interfaces;

#pragma warning disable CS8767

namespace StreamProcessing.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed class StreamDbCommand : IStreamDbCommand
{
    private readonly OdbcCommand _odbcCommand;

    public StreamDbCommand(OdbcCommand odbcCommand)
    {
        _odbcCommand = odbcCommand ?? throw new ArgumentNullException(nameof(odbcCommand));
    }

    public void Dispose()
    {
        _odbcCommand.Dispose();
    }

    public void Cancel()
    {
        _odbcCommand.Cancel();
    }

    public IDbDataParameter CreateParameter()
    {
        return _odbcCommand.CreateParameter();
    }

    public int ExecuteNonQuery()
    {
        return _odbcCommand.ExecuteNonQuery();
    }

    public IDataReader ExecuteReader()
    {
        return _odbcCommand.ExecuteReader();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        return _odbcCommand.ExecuteReader(behavior);
    }

    public object? ExecuteScalar()
    {
        return _odbcCommand.ExecuteScalar();
    }

    public void Prepare()
    {
        _odbcCommand.Prepare();
    }

    public string CommandText
    {
        get => _odbcCommand.CommandText;
        set => _odbcCommand.CommandText = value;
    }

    public int CommandTimeout
    {
        get => _odbcCommand.CommandTimeout;
        set => _odbcCommand.CommandTimeout = value;
    }

    public CommandType CommandType
    {
        get => _odbcCommand.CommandType;
        set => _odbcCommand.CommandType = value;
    }

    private IDbConnection? _connection;
    public IDbConnection? Connection
    {
        get => _connection;
        set
        {
            _connection = value;
            _odbcCommand.Connection = (_connection as IStreamDbConnection)?.BaseConnection as OdbcConnection;
        }
    }

    public IDataParameterCollection Parameters => _odbcCommand.Parameters;

    public IDbTransaction? Transaction
    {
        get => _odbcCommand.Transaction;
        set => _odbcCommand.Transaction = (OdbcTransaction?)value;
    }

    public UpdateRowSource UpdatedRowSource
    {
        get => _odbcCommand.UpdatedRowSource;
        set => _odbcCommand.UpdatedRowSource = value;
    }

    public void AddParameterWithValue(object? value)
    {
        _odbcCommand.Parameters.AddWithValue(null, value);
    }
}