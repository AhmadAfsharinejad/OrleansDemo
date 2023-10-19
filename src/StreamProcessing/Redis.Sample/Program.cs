// See https://aka.ms/new-console-template for more information

using System;
using System.Linq;
using StackExchange.Redis;

//redis-cli flushall

Console.WriteLine("Start");

var redis = ConnectionMultiplexer.Connect("localhost");
var db = redis.GetDatabase(0);
var endPoint = redis.GetEndPoints().First();
var keys = redis.GetServer(endPoint).Keys(pattern: "*").ToArray();
foreach (var key in keys)
{
    Console.WriteLine(key);
}

Console.WriteLine("Finish");
