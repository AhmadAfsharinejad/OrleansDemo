using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

string Url = "http://localhost:1380/index/";
HttpListener _listener;

Console.WriteLine("Start");

await Run();

//Task.Run(Run);
//Task.Run(Start);

Console.WriteLine("Finish");

Console.ReadLine();

async Task Run()
{
    Console.WriteLine($"{DateTime.Now}");
    
    var client = new HttpClient();
    var sw = new Stopwatch();
   
    for (int i = 0; i < 100000; i++)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Url);
        request.Headers.Add("id", i.ToString());
        var content = new StringContent("myContent", null, "text/plain");
        request.Content = content;
        
        sw.Start();
        var response = await client.SendAsync(request);
        sw.Stop();
        if (response.Headers.GetValues("resId").First() != i.ToString())
        {
            throw new Exception();
        }
    }

    Console.WriteLine(sw.Elapsed.TotalMilliseconds);
    
    Console.WriteLine($"{DateTime.Now}");
}

void Start()
{
    _listener = new HttpListener();
    _listener.Prefixes.Add(Url);
    _listener.Start();
   
    while (_listener.IsListening)
    {
        var context = _listener.GetContext();
        Handle(context);
    }
}

void Stop()
{
    _listener.Stop();
}

async Task Handle(HttpListenerContext context)
{
    var request = context.Request;

    //Console.WriteLine($"{request.Url}");

    var queryParameter = request.QueryString["id"];
    //Console.WriteLine(queryParameter);

    using var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
    var content = reader.ReadToEnd();
    //Console.WriteLine(content);


    //await Task.Delay(1000);

    var response = context.Response;

    var responseString = "Ur response";
    var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
    response.ContentLength64 = buffer.Length;

    await using var output = response.OutputStream;
    output.Write(buffer, 0, buffer.Length);
}