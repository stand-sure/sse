## .NET 9 ServerSideEvent (SSE) Publisher

This is a simple proof-of-concept for publishing SSEs.

There is an HTML client for in-browser verification.

With the new APIs in .net 9, the following could be used as a starting point for figuring out a server client:

```c#
var uriBuilder = new UriBuilder
{
    Scheme = "http",
    Host = "localhost",
    Port = 5101,
};

var client = new HttpClient
{
    BaseAddress = uriBuilder.Uri,
};

client.DefaultRequestHeaders.Add("Accepts", "*");

Stream stream = await client.GetStreamAsync("/").ConfigureAwait(false);

await foreach (SseItem<string> item in SseParser.Create(stream).EnumerateAsync())
{
    Console.WriteLine(item.Data);
}
```