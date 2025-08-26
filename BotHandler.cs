using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net.Http.Json;
using ExpiryFoodTGBot.Models;
using ExpiryFoodTGBot.Service;
using ExpiryFoodTGBot.Services;
using Microsoft.Extensions.Configuration;
using ExpiryFoodTGBot.Handler;

namespace ExpiryFoodTGBot
{
    public enum ProductCallbackActions 
    {
        Delete,
        Edit
    }
    public class BotHandler
    {
        
        ProductService productService;
        UserService userService;
        MessageService messageService;
        IdleHandler idleHandler;
        EditingHandler editingHandler;
        DeleteCallbackHandler deleteCallbackHandler;
        EditCallbackHandler editCallbackHandler;

        public BotHandler(TelegramBotClient telegramBotClient)
        {
            var bot = telegramBotClient;
            bot.OnMessage += OnMessageHandler;
            bot.OnUpdate += OnUpdateHandeler;
            productService = new ProductService(new HttpClient());
            userService = new UserService();
            messageService = new MessageService(telegramBotClient);
            idleHandler = new IdleHandler(userService, productService, messageService);
            editingHandler = new EditingHandler(userService, productService, messageService);
            deleteCallbackHandler = new DeleteCallbackHandler(productService, messageService);
            editCallbackHandler = new EditCallbackHandler(productService, messageService, userService);
        }
        private Task OnUpdateHandeler(Update update)
        {
            if (update.CallbackQuery is { } callbackQuery && callbackQuery != null) 
            {
                string callbackQueryId = update.CallbackQuery.Id;
                
                var chatId = callbackQuery.Message!.Chat.Id;
                
                var messageId = callbackQuery.Message!.MessageId;
                
                var productId = int.Parse(callbackQuery.Data!.Split('_')[1]);
                
                string param = callbackQuery.Data.Split("_").Last();
                
                ProductCallbackActions action = (ProductCallbackActions)int.Parse(callbackQuery.Data!.Split('_')[0]);

                switch (action)
                {
                    case ProductCallbackActions.Delete:
                        _ = deleteCallbackHandler.Delete(callbackQueryId, productId, chatId, messageId);
                        break;
                    case ProductCallbackActions.Edit:
                        _ = editCallbackHandler.Edit(productId, chatId);
                        break;
                }
            }

            return Task.CompletedTask;
        }

        public Task OnMessageHandler(Message message, UpdateType type)
        {
            var chatId = message.Chat.Id;
            
            if (message.Date < DateTime.UtcNow.AddMinutes(-1)) {
                return Task.CompletedTask;
            }
            
            if (message.Text == null) {
                return Task.CompletedTask;
            }

            switch (userService.GetState(chatId))
            {
                case CurrentState.Idle:
                    _ = idleHandler.HandleIdleState(chatId, message.Text);
                    break;
                default:
                    _ = editingHandler.HandleNonIdleState(chatId, message.Text);
                    break;
            }
            return Task.CompletedTask;
        }   
    }
}
