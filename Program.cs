using System.ComponentModel.DataAnnotations;
using CryptoExchange.Net.Authentication;
using level.Models;
using level.Options;
using level.Services;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI (kept from template)
builder.Services.AddOpenApi();

// Bind Bybit options from configuration (appsettings.json)
builder.Services.Configure<BybitOptions>(builder.Configuration.GetSection(BybitOptions.SectionName));
builder.Services.AddBybit(options =>
{
    var bybitOptions = builder.Configuration.GetSection(BybitOptions.SectionName).Get<BybitOptions>();
    if (bybitOptions != null)
    {
        options.ApiCredentials = new ApiCredentials(bybitOptions.ApiKey, bybitOptions.ApiSecret);
    }
});

// Bind Telegram options from configuration
builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection(TelegramOptions.SectionName));

// Register trading service (implementation uses Bybit.Net internally)
builder.Services.AddScoped<IBybitTradingService, BybitTradingService>();

// Register Telegram notification service
builder.Services.AddHttpClient<ITelegramNotificationService, TelegramNotificationService>();

// Register template service
builder.Services.AddSingleton<ITemplateService, TemplateService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Root info
app.MapGet("/", () => Results.Ok(new { message = "Level trading webhook receiver" }));

// GET-based webhook example: /open?type=long&asset=BTCUSDT&size=0.001&stop=0.2&take=0.8
app.MapPost("/open", async (string type, string asset, decimal size, decimal? stop, decimal? take, IBybitTradingService tradingService) =>
{
    var req = new TradingViewOrderRequest
    {
        Type = type,
        Asset = asset,
        Size = size,
        Stop = stop,
        Take = take
    };

    // Validate request
    var ctx = new ValidationContext(req);
    var results = new List<ValidationResult>();
    if (!Validator.TryValidateObject(req, ctx, results, true))
        return Results.BadRequest(results);

    var result = await tradingService.OpenPositionAsync(req);
    if (!result.Success)
        return Results.Problem(result.Message);

    return Results.Ok(new { result.Message, result.OrderId });
}).WithName("OpenPositionGet");

// POST webhook endpoint (TradingView can POST JSON). Accepts JSON body.
app.MapPost("/webhook/open", async (TradingViewOrderRequest req, IBybitTradingService tradingService) =>
{
    var ctx = new ValidationContext(req);
    var results = new List<ValidationResult>();
    if (!Validator.TryValidateObject(req, ctx, results, true))
        return Results.BadRequest(results);

    var result = await tradingService.OpenPositionAsync(req);
    if (!result.Success)
        return Results.Problem(result.Message);

    return Results.Ok(new { result.Message, result.OrderId });
}).WithName("OpenPositionPost");

// Webhook endpoint for TradingView notifications with Telegram alerts
app.MapPost("/webhook/notify", async (
    TradingViewWebhookRequest req, 
    ITelegramNotificationService telegramService,
    ITemplateService templateService) =>
{
    var ctx = new ValidationContext(req);
    var results = new List<ValidationResult>();
    if (!Validator.TryValidateObject(req, ctx, results, true))
        return Results.BadRequest(results);

    // Рендерим шаблон с данными из запроса
    var message = templateService.RenderTemplate(req.Template, req);

    var sent = await telegramService.SendNotificationAsync(message);
    
    if (!sent)
        return Results.Problem("Failed to send Telegram notification");

    return Results.Ok(new { message = "Notification sent successfully" });
}).WithName("TradingViewNotify");

app.Run();
