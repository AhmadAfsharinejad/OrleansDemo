using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

HttpListener _listener;

Console.WriteLine("Start");

Task.Run(Run);
Task.Run(Start);

Console.WriteLine("Finish");

Console.ReadLine();

async Task Run()
{
    Console.WriteLine($"{DateTime.Now}");
    
    var client = new HttpClient();
   
    for (int i = 0; i < 100000; i++)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:1280/index/");
        request.Headers.Add("id", "123");
        var content = new StringContent("myContent", null, "text/plain");
        request.Content = content;
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    Console.WriteLine($"{DateTime.Now}");
}

void Start()
{
    _listener = new HttpListener();
    _listener.Prefixes.Add("http://localhost:1280/index/");
    _listener.Start();
    Listen();
}

void Stop()
{
    _listener.Stop();
}

void Listen()
{
    while (_listener.IsListening)
    {
        var context = _listener.GetContext();
        Handle(context);
    }
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