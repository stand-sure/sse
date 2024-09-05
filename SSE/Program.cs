using System.Buffers;
using System.Text;
using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

WebApplication app = builder.Build();

app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
});

app.MapGet("/",
    (CancellationToken cancellationToken) =>
    {
        const int lastIndexToSend = 10;

        Console.WriteLine($"Request at {DateTime.UtcNow:s}");
        return Results.Stream(StreamWriterCallback, "text/event-stream");

        async Task StreamWriterCallback(Stream stream)
        {
            for (var i = 0; i <= lastIndexToSend; i += 1)
            {
                var writer = new ArrayBufferWriter<byte>(8192);

                Payload payload = new(DateTime.Now, i);
                var text = $"event: message\ndata: {JsonSerializer.Serialize(payload)}\n\n";

                Encoding.UTF8.GetBytes(text, writer);

                ReadOnlyMemory<byte> memory = writer.WrittenMemory;

                await stream.WriteAsync(memory, cancellationToken).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                Console.WriteLine(text);

                await Task.Delay(1_000).ConfigureAwait(false);
            }

            stream.Close();
        }
    });

app.Run();

public record struct Payload(DateTime When, int Index);