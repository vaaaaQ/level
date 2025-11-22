# Level Trading Webhook Receiver

.NET минимальный API для приема вебхуков от TradingView с интеграцией Bybit и уведомлениями в Telegram.

## Возможности

- ✅ Прием вебхуков от TradingView
- ✅ Открытие позиций на Bybit через API
- ✅ Отправка уведомлений в Telegram о событиях TradingView

## Endpoints

### 1. `/webhook/notify` - Уведомления в Telegram
Принимает вебхук от TradingView и отправляет уведомление в Telegram.

**Пример запроса:**
```json
POST /webhook/notify
Content-Type: application/json

{
  "instrument": "BTCUSDT",
  "timeframe": "1H",
  "action": "BUY",
  "price": 42150.50,
  "signal": "Golden Cross"
}
```

**Обязательные поля:**
- `instrument` - торговый инструмент (например, BTCUSDT)
- `timeframe` - таймфрейм (например, 1H, 15m, 4H)

**Опциональные поля:**
- `action` - действие (BUY, SELL и т.д.)
- `price` - цена
- `signal` - название сигнала

### 2. `/webhook/open` - Открытие позиции
Принимает вебхук и открывает позицию на Bybit.

**Пример запроса:**
```json
POST /webhook/open
Content-Type: application/json

{
  "type": "long",
  "asset": "BTCUSDT",
  "size": 0.001,
  "stop": 0.02,
  "take": 0.05
}
```

## Настройка

### 1. Настройка Telegram бота

1. Создайте бота через [@BotFather](https://t.me/BotFather):
   - Отправьте `/newbot`
   - Укажите имя и username бота
   - Сохраните полученный **Bot Token**

2. Получите Chat ID:
   - Отправьте сообщение вашему боту
   - Откройте в браузере: `https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getUpdates`
   - Найдите значение `chat.id` в ответе

### 2. Настройка appsettings.json

Обновите файл `appsettings.json`:

```json
{
  "Telegram": {
    "BotToken": "1234567890:ABCdefGHIjklMNOpqrsTUVwxyz",
    "ChatId": "123456789"
  },
  "Bybit": {
    "ApiKey": "ваш_api_key",
    "ApiSecret": "ваш_api_secret",
    "Category": "Linear"
  }
}
```

### 3. Настройка TradingView

В TradingView создайте Alert со следующим webhook:

**URL:** `https://ваш-домен.com/webhook/notify`

**Message (JSON):**
```json
{
  "instrument": "{{ticker}}",
  "timeframe": "{{interval}}",
  "action": "{{strategy.order.action}}",
  "price": {{close}},
  "signal": "Ваш сигнал"
}
```

## Запуск проекта

```bash
dotnet run
```

Приложение будет доступно по адресу: `http://localhost:5280`

## Тестирование

Используйте файл `level.http` для тестирования endpoints или выполните:

```bash
curl -X POST http://localhost:5280/webhook/notify \
  -H "Content-Type: application/json" \
  -d '{
    "instrument": "BTCUSDT",
    "timeframe": "1H",
    "action": "BUY",
    "price": 42150.50,
    "signal": "Test Signal"
  }'
```

## Пример уведомления в Telegram

```
📊 TradingView Alert

🔹 Instrument: BTCUSDT
🕒 Timeframe: 1H
⚡ Action: BUY
💰 Price: 42150.5
📡 Signal: Golden Cross
```

## Технологии

- .NET 10.0 (Minimal API)
- Bybit.NET
- Telegram Bot API
- OpenAPI/Swagger
