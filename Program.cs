using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"))//.AddAttributes(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("mykey", new string('a', 6000)) }))
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("https://staging-otlp.nr-data.net");
            });
    })
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"))
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("https://staging-otlp.nr-data.net");
            });
    });

    //builder.Logging.ClearProviders();
    builder.Logging.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"))
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("https://staging-otlp.nr-data.net");
            });
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
