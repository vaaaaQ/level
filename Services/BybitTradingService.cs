using Bybit.Net.Enums;
using Bybit.Net.Interfaces.Clients;
using level.Models;
using level.Options;
using Microsoft.Extensions.Options;

namespace level.Services;

public interface IBybitTradingService
{
    Task<TradingActionResult> OpenPositionAsync(TradingViewOrderRequest request, CancellationToken cancellationToken = default);
}


public sealed class BybitTradingService : IBybitTradingService
{
    private readonly IBybitRestClient _client;
    private readonly BybitOptions _options;

    public BybitTradingService(IBybitRestClient client, IOptions<BybitOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<TradingActionResult> OpenPositionAsync(TradingViewOrderRequest request, CancellationToken cancellationToken = default)
    {
        var orderSide = request.Type.Equals("long", StringComparison.OrdinalIgnoreCase)
            ? OrderSide.Buy
            : OrderSide.Sell;

        var priceResult = await _client.V5Api.ExchangeData.GetLinearInverseTickersAsync(Category.Linear, request.Asset, ct: cancellationToken);
        if (!priceResult.Success)
        {
            var errorMessage = priceResult.Error?.Message ?? "Failed to retrieve ticker information. Unknown error.";
            return new TradingActionResult(false, errorMessage, null);
        }

        var ticker = priceResult.Data?.List.FirstOrDefault();
        var currentPrice = ticker?.LastPrice;

        if (currentPrice == null)
        {
            return new TradingActionResult(false, "Не удалось получить текущую цену.", null);
        }

        // Ожидается, что request.Take и request.Stop приходят в процентах (например 1.5 = 1.5%)
        var takePercent = (decimal)request.Take;
        var stopPercent = (decimal)request.Stop;

        if (takePercent <= 0 || stopPercent <= 0)
        {
            return new TradingActionResult(false, "Проценты для тейк и стоп должны быть больше нуля.", null);
        }

        decimal takePrice;
        decimal stopPrice;

        if (orderSide == OrderSide.Buy) // LONG
        {
            takePrice = currentPrice.Value * (1 + takePercent / 100m);
            stopPrice = currentPrice.Value * (1 - stopPercent / 100m);
        }
        else // SHORT
        {
            takePrice = currentPrice.Value * (1 - takePercent / 100m);
            stopPrice = currentPrice.Value * (1 + stopPercent / 100m);
        }

        // Округление — при необходимости подкорректируйте количество знаков по тиксайзу инструмента
        takePrice = Math.Round(takePrice, 8);
        stopPrice = Math.Round(stopPrice, 8);


        var placeOrderResult = await _client.V5Api.Trading.PlaceOrderAsync(
            category: _options.Category,
            symbol: request.Asset,
            side: orderSide,
            type: NewOrderType.Market,
            quantity: request.Size,
            timeInForce: TimeInForce.GoodTillCanceled,
            reduceOnly: false,
            takeProfit: takePrice,
            stopLoss: stopPrice,
            ct: cancellationToken);

        if (!placeOrderResult.Success)
        {
            var errorMessage = placeOrderResult.Error?.Message ?? "Failed to open position. Unknown error.";
            return new TradingActionResult(false, errorMessage, null);
        }

        var orderId = placeOrderResult.Data?.OrderId;
        return new TradingActionResult(true, "Successfully opened position.", orderId);
    }
}

public sealed record TradingActionResult(bool Success, string Message, string? OrderId);
