using System.Data;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using StreamProcessing.SqlExecutor.Interfaces;
#pragma warning disable CS8767

namespace StreamProcessing.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed class StreamDbConnection : IStreamDbConnection
{
    private readonly OdbcConnection _connection;

    public StreamDbConnection(OdbcConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public IDbTransaction BeginTransaction()
    {
        return _connection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return _connection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
        _connection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        _connection.Close();
    }

    public IDbCommand CreateCommand()
    {
        return CreateStreamDbCommand();
    }
    
    public IStreamDbCommand CreateStreamDbCommand()
    {
        var command = new StreamDbCommand(_connection.CreateCommand());
        command.Connection = this;
        return command;
    }
    
    public void Open()
    {
        _connection.Open();
    }

    public string ConnectionString
    {
        get => _connection.ConnectionString;
        set => _connection.ConnectionString = value;
    }

    public int ConnectionTimeout => _connection.ConnectionTimeout;
    public string Database => _connection.Database;
    public ConnectionState State => _connection.State;

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }

    public Task OpenAsync()
    {
        return _connection.OpenAsync();
    }

    public Task OpenAsync(CancellationToken cancellationToken)
    {
        return _connection.OpenAsync(cancellationToken);
    }

    public IDbConnection BaseConnection => _connection;
}