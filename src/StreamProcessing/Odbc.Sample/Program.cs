// See https://aka.ms/new-console-template for more information

using System;
using System.Data.Odbc;

#pragma warning disable CS8321

const string ConnectionString = @"Driver={ClickHouse ODBC Driver (Unicode)};Host=localhost;PORT=8123;Timeout=500;Username=admin;Password=admin";

Console.WriteLine($"Start {DateTime.Now}");

//InsertRow();
//InsertRowWithParameter();
ReadRow();

Console.WriteLine($"Finish {DateTime.Now}");

void InsertRowWithParameter()
{
    const string query = "INSERT INTO test Values(?, ?)";
    using var command = new OdbcCommand(query);
    command.Parameters.AddWithValue(null, 2);
    command.Parameters.AddWithValue(null, "param");

    ExecuteNonQuery(command);
}

void InsertRow()
{
    const string query = "INSERT INTO test Values(1, 'hi')";
    using var command = new OdbcCommand(query);
    ExecuteNonQuery(command);
}

void CreateTable()
{
    const string query = "CREATE TABLE test (id Int64, name String ) ENGINE = MergeTree() ORDER BY id";
    using var command = new OdbcCommand(query);
    ExecuteNonQuery(command);
}

void ExecuteNonQuery(OdbcCommand command)
{
    using var connection = new OdbcConnection(ConnectionString);
    command.Connection = connection;
    connection.Open();
    command.ExecuteNonQuery();
}

void ReadRow()
{
    const string query = "select id, name from test";
    using var connection = new OdbcConnection(ConnectionString);
    using var command = new OdbcCommand(query, connection);
    connection.Open();
    var reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine(reader["id"]);
    }
}