# Level Trading Webhook Receiver

.NET минимальный API для приема вебхуков от TradingView с интеграцией Bybit и уведомлениями в Telegram.

## Возможности

- ✅ Прием вебхуков от TradingView
- ✅ Открытие позиций на Bybit через API
- ✅ Отправка уведомлений в Telegram о событиях TradingView

## Endpoints

### 1. `/webhook/notify` - Уведомления в Telegram
Принимает вебхук от TradingView и отправляет уведомление в Telegram с поддержкой динамических шаблонов.

**Пример запроса (базовый):**
```json
POST /webhook/notify
Content-Type: application/json

{
  "instrument": "BTCUSDT",
  "timeframe": "1D",
  "price": 100,
  "comment": "Затухание волатильности"
}
```

**Пример запроса (с кастомным шаблоном):**
```json
POST /webhook/notify
Content-Type: application/json

{
  "instrument": "ETHUSDT",
  "timeframe": "4H",
  "price": 2500.50,
  "comment": "Пробой уровня сопротивления",
  "template": "🚨 *АЛЕРТ*\n\n📊 *Инструмент:* {instrument}\n⏰ *Таймфрейм:* {timeframe}\n💵 *Цена:* ${price}\n\n💬 *Комментарий:*\n{comment}"
}
```

**Обязательные поля:**
- `instrument` - торговый инструмент (например, BTCUSDT)
- `timeframe` - таймфрейм (например, 1H, 15m, 4H, 1D)

**Опциональные поля:**
- `price` - цена
- `comment` - комментарий к сигналу
- `template` - кастомный Markdown-шаблон для форматирования сообщения
- `data` - дополнительные пользовательские поля в JSON формате

**Шаблонизация:**

Поле `template` поддерживает интерполяцию полей из тела запроса. Используйте фигурные скобки для вставки значений:
- `{instrument}` - инструмент
- `{timeframe}` - таймфрейм
- `{price}` - цена
- `{comment}` - комментарий

**Стандартный шаблон (если `template` не указан):**
```
📊 *TradingView Alert*

🔹 *Instrument:* {instrument}
🕒 *Timeframe:* {timeframe}
💰 *Price:* {price}
💬 {comment}
```

**Форматирование Markdown:**
- `*жирный текст*` - жирный
- `_курсив_` - курсив
- `` `код` `` - моноширинный шрифт
- `\n` - перенос строки

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

**Message (JSON) - базовый вариант:**
```json
{
  "instrument": "{{ticker}}",
  "timeframe": "{{interval}}",
  "price": {{close}},
  "comment": "Сигнал: {{strategy.order.action}}"
}
```

**Message (JSON) - с кастомным шаблоном:**
```json
{
  "instrument": "{{ticker}}",
  "timeframe": "{{interval}}",
  "price": {{close}},
  "comment": "Цена закрытия",
  "template": "🔔 *Алерт*\n\n📈 {instrument} | {timeframe}\n💵 Цена: ${price}\n📝 {comment}"
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
    "timeframe": "1D",
    "price": 100,
    "comment": "Затухание волатильности"
  }'
```

## Пример уведомления в Telegram

**С базовым шаблоном:**
```
📊 *TradingView Alert*

🔹 *Instrument:* BTCUSDT
🕒 *Timeframe:* 1D
💰 *Price:* 100
💬 Затухание волатильности
```

**С кастомным шаблоном:**
```
🚨 *АЛЕРТ*

📊 *Инструмент:* ETHUSDT
⏰ *Таймфрейм:* 4H
� *Цена:* $2500.50

� *Комментарий:*
Пробой уровня сопротивления
```

## Технологии

- .NET 10.0 (Minimal API)
- Bybit.NET
- Telegram Bot API
- OpenAPI/Swagger
