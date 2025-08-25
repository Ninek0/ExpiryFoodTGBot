using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpiryFoodTGBot.Models;
using ExpiryFoodTGBot.Service;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExpiryFoodTGBot.Services
{
    public class MessageService
    {
        private TelegramBotClient _botClient;
        public MessageService(TelegramBotClient telegramBotClient)
        {
            _botClient = telegramBotClient;
        }
        public async void AnswerRemoveProduct(string callbackQueryId, int productId, bool isRemoved)
        {
            if (isRemoved)
                await _botClient.AnswerCallbackQuery(callbackQueryId, text: $"Продукт с ID {productId} успешно удален");
            else
                await _botClient.AnswerCallbackQuery(callbackQueryId, text: $"Продукт с ID {productId} не удален");
        }
        public async void DeleteMessage(ChatId chatId, int messageId)
        {
            await _botClient.DeleteMessage(chatId, messageId);
        }
        public async Task SendEditableProductMessage(ChatId chatId, ProductModel? product)
        {
            var messageText = "";
            if (product == null)
                messageText = $"📦 *Отсутствует*\n" +
                              $"📝: Отсутствует\n" +
                              $"📅: Не указано";
            else
                messageText = $"📦 *{product.Name}*\n" +
                              $"📝: {product.Description}\n" +
                              $"📅: {product.ExpireAt:dd.MM.yyyy}";

            await _botClient.SendMessage(
                chatId: chatId,
                text: messageText,
                parseMode: ParseMode.Markdown,
                replyMarkup: GetEditButtons());
        }
        public async void AskForEditProductField(ChatId chatId, ProductFields field) 
        {
            switch (field) 
            {
                case ProductFields.Name:
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Введите новое название",
                        replyMarkup: GetEditButtons()
                        );
                    break;
                case ProductFields.Description:
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Введите новое описание",
                        replyMarkup: GetEditButtons()
                        );
                    break;
                case ProductFields.Date:
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Введите когда истечет срок годности",
                        replyMarkup: GetEditButtons()
                        );
                    break;
            }  
        }
        public async void SendMainMenu(ChatId chatId)
        {
            await _botClient.SendMessage(
                       chatId: chatId,
                       text: "Выбери пункт из меню",
                       replyMarkup: GetMainButtons()
                       );
        }
        public async void AskForEdit(ChatId chatId)
        {
            await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Что редактируем?",
                        replyMarkup: GetEditButtons()
                        );
        }
        public async void SendAllProducts(ChatId chatId, IReadOnlyList<ProductModel> productList)
        {
            try
            {
                if (productList.Count == 0)
                {
                    await _botClient.SendMessage(chatId, text: "🍃 Список продуктов пуст", replyMarkup: GetMainButtons());
                    return;
                }

                var sb = new StringBuilder();

                foreach (var product in productList!.OrderBy(f => f.ExpireAt))
                {
                    var expiresIn = (product.ExpireAt - DateTime.Now).Days;
                    var statusIcon = expiresIn switch
                    {
                        < 0 => "🚨 ПРОСРОЧЕНО",
                        0 => "⚠️ Сегодня!",
                        1 => "🔸 Завтра",
                        <= 3 => $"🔹 Осталось {expiresIn} дн.",
                        _ => $"✅ Осталось {expiresIn} дн."
                    };

                    sb.AppendLine($"🆔 {product.Id} | 🏷 {product.Name}");
                    sb.AppendLine($"📝 Описание: {product.Description ?? "—"}");
                    sb.AppendLine($"📅 Срок: {product.ExpireAt:dd.MM.yyyy} ({statusIcon})");

                    await _botClient.SendMessage(
                        chatId,
                        sb.ToString(),
                        replyMarkup: new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton> {
                                InlineKeyboardButton.WithCallbackData("Удалить", $"{ProductCallbackActions.Delete}_{product.Id}"),
                                InlineKeyboardButton.WithCallbackData("Редактировать", $"{ProductCallbackActions.Edit}_{product.Id}")
                            }));
                    sb.Clear();
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, text: "🍃 Список продуктов пуст", replyMarkup: GetMainButtons());
            }
        }
        private ReplyMarkup GetMainButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
            {
            new List<KeyboardButton>
            {
                new KeyboardButton { Text = "Посмотреть все продукты" }
            },
            new List<KeyboardButton>
            {
                new KeyboardButton { Text = "Добавить продукт" }
            }
        }
            };
        }
        private ReplyMarkup GetEditButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
        {
            new List<KeyboardButton>
            {
                new KeyboardButton { Text = "Редактировать название" }
            },
            new List<KeyboardButton>
            {
                new KeyboardButton { Text = "Редактировать описание" }
            },
            new List<KeyboardButton>
            {
                new KeyboardButton { Text = "Когда испортится" }
            },
            new List<KeyboardButton>
            {
                new KeyboardButton { Text = "Готово" }
            }
        }
            };
        }
    }
}
