# 🍕 ExpiryFood Bot - Твой умный помощник по продуктам

[![Telegram](https://img.shields.io/badge/Telegram-Bot-blue?logo=telegram)](https://t.me/YourBotName)
[![.NET](https://img.shields.io/badge/.NET-6.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**Никогда не забывай о просроченных продуктах!** ExpiryFood Bot помогает отслеживать сроки годности и управлять твоими продуктами через Telegram.

---

## ✨ Возможности

- 🛍 **Добавление продуктов** с указанием срока годности
- 📋 **Просмотр всего списка** с сортировкой по свежести
- ✏️ **Редактирование** названия, описания и даты
- 🗑 **Удаление** продуктов одним кликом
- ⏰ **Умные статусы** (все хорошо, скоро истечет, просрочено)
- 💬 **Интуитивный интерфейс** с кнопками и меню

---

## 🛠 Технологический стек

- **.NET 6.0** - современная платформа
- **Telegram.Bot** - официальная библиотека Telegram API
- **ASP.NET Core** - хостинг через Webhooks
- **Clean Architecture** - чистая и масштабируемая структура
- **Dependency Injection** - внедрение зависимостей

---

## 🚀 Быстрый старт

### Предварительные требования

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download)
- Telegram Bot Token от [@BotFather](https://t.me/BotFather)

### Установка

1. **Клонируй репозиторий**
   ```bash
   git clone https://github.com/Ninek0/ExpiryFoodTGBot.git
   cd ExpiryFoodTGBot
   ```
2. **Настрой конфигурацию**
   ```bash
   # Скопируй и настрой файл настроек
   cp appsettings.example.json appsettings.json
   ```
   ### Пример настроек  
   ```
   {
     "BotConfig": {
        "BotToken": "YOUR_TELEGRAM_BOT_TOKEN"
     },
     "ApiSettings": {
        "BaseUrl": "YOUR_API_BASE_URL"
     }
   }
   ```
3. **Запусти бота**
   ```bash
   dotnet run
   ```
### 🏗 Архитектура проекта
ExpiryFoodTGBot/
├── Handlers/           # Обработчики команд и состояний
│   ├── IdleHandler.cs
│   ├── EditingHandler.cs
│   ├── DeleteCallbackHandler.cs
│   └── EditCallbackHandler.cs
├── Services/           # Бизнес-логика
│   ├── MessageService.cs
│   ├── UserService.cs
│   └── ProductService.cs
├── Models/             # Модели данных
│   ├── ProductModel.cs
│   └── UserModel.cs
├── Settings/           # Конфигурация
│   ├── BotSettings.cs
│   └── appsettings.json
└── BotHandler.cs       # Главный координатор
### 🚧 Roadmap

    Уведомления о скором истечении срока

    Категории продуктов

    Мультиязычность

    Экспорт данных в CSV

    Интеграция с облачными хранилищами

    Веб-панель управления

### 📝 Лицензия

Этот проект распространяется под лицензией MIT. Подробнее в файле LICENSE.

### 👨‍💻 Автор

tg: @Ninek0

💡 Нашли баг?
🚀 Есть идея?

С любовью к свежим продуктам и чистому коду! 🍎💻
