// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Threading.Tasks;

#pragma warning disable CS8321

//const string ConnectionString = @"Driver={ClickHouse ODBC Driver (Unicode)};Host=localhost;PORT=8123;Timeout=500;Username=admin;Password=admin";
string ConnectionString = "Driver={SQL Server};Server=192.168.30.171;Uid=sa;Pwd=!QAZ2wsx;";


Console.WriteLine($"Start {DateTime.Now}");

//InsertRow();
//InsertRowWithParameter();

var list = new List<Task>();
for (int i = 0; i < 100; i++)
{
    list.Add(Task.Run(ReadRow));
}

await Task.WhenAll(list);

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
    const string query = "SELECT 1 as dateTime";
    using var connection = new OdbcConnection(ConnectionString);
    using var command = new OdbcCommand(query, connection);
    connection.Open();
    for (int i = 0; i < 5000; i++)
    {
        try
        {
            using var reader = command.ExecuteReader();
            //Console.WriteLine("haha");
        }
        catch (Exception e)
        {
            Console.WriteLine($" Error Time: {DateTime.Now}\n{e.Message}");
        }
        
        // while (reader.Read())
        // {
        //     if (i % 500 == 0)
        //         Console.WriteLine(reader["dateTime"]);
        // }
    }

    Console.WriteLine("finish");
}