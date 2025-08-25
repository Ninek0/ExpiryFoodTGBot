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

        public BotHandler(TelegramBotClient telegramBotClient)
        {
            var bot = telegramBotClient;
            bot.OnMessage += OnMessageHandler;
            bot.OnUpdate += OnUpdateHandeler;
            productService = new ProductService(new HttpClient());
            userService = new UserService();
            messageService = new MessageService(telegramBotClient);
        }
        private async Task OnUpdateHandeler(Update update)
        {
            if (update.CallbackQuery is { } callbackQuery && callbackQuery != null) 
            {
                var chatId = callbackQuery.Message!.Chat.Id;
                var messageId = callbackQuery.Message!.MessageId;

                ProductCallbackActions action = (ProductCallbackActions)int.Parse(callbackQuery.Data!.Split('_')[0]);

                var productId = int.Parse(callbackQuery.Data.Split('_')[1]);
                
                string param = callbackQuery.Data.Split("_").Last();
                
                switch (action)
                {
                    case ProductCallbackActions.Delete:
                        var result = await productService.DeleteProductAsync(productId);
                        messageService.AnswerRemoveProduct(callbackQuery.Id, productId, result);
                        if(result)
                            messageService.DeleteMessage(chatId, messageId);
                        break;
                    case ProductCallbackActions.Edit:
                        userService.UpdateUserState(chatId, CurrentState.Editing);
                        await GetProductById(chatId, productId); 
                        await messageService.SendEditableProductMessage(chatId, userService.GetProduct(chatId));    
                        break;
                }
            }
        }
        public async Task GetProductById(ChatId chatId, int productId)
        {
            ProductModel? product = await productService.GetProductByIdAsync(productId);
            if (product == null) return;
            userService.UpdateProduct(chatId, product);
        }
        public async void HandleNonIdleState(ChatId chatId, string message)
        {
            switch (message)
            {
                case "Редактировать название":
                    userService.UpdateProductField(chatId, ProductFields.Name);
                    messageService.AskForEditProductField(chatId, ProductFields.Name);
                    break;
                case "Редактировать описание":
                    userService.UpdateProductField(chatId, ProductFields.Description);
                    messageService.AskForEditProductField(chatId, ProductFields.Description);
                    break;
                case "Когда испортится":
                    userService.UpdateProductField(chatId, ProductFields.Date);
                    messageService.AskForEditProductField(chatId, ProductFields.Date);
                    break;
                case "Готово":
                    switch (userService.GetState(chatId))
                    {
                        case CurrentState.Adding:
                            await productService.CreateProductAsync(userService.GetProduct(chatId)!);
                            break;
                        case CurrentState.Editing:
                            await productService.UpdateProductAsync(userService.GetProduct(chatId)!);
                            break;
                    }
                    userService.UpdateUserState(chatId, CurrentState.Idle);
                    userService.UpdateProduct(chatId, new());
                    messageService.SendMainMenu(chatId);
                    break;
                default:
                    EditProductParametr(chatId, message);
                    messageService.AskForEdit(chatId);
                    await messageService.SendEditableProductMessage(chatId, userService.GetProduct(chatId));
                    break;
            }
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
                    HandleIdleState(chatId, message.Text);
                    break;
                default:
                    HandleNonIdleState(chatId, message.Text);
                    break;
            }

            return Task.CompletedTask;
        }
        public async void HandleIdleState(ChatId chatId, string message)
        {
            switch (message)
            {
                case "Посмотреть все продукты":
                    userService.UpdateUserState(chatId, CurrentState.Idle);
                    messageService.SendAllProducts(chatId, await productService.GetExpiringProductsAsync());
                    break;
                case "Добавить продукт":
                    userService.UpdateUserState(chatId, CurrentState.Adding);
                    await messageService.SendEditableProductMessage(chatId, userService.GetProduct(chatId));
                    break;
                default:
                    messageService.SendMainMenu(chatId);
                    break;
            }
        }
        private void EditProductParametr(ChatId chatId, string newValue)
        {
            var currentProduct = userService.GetProduct(chatId)!;
            
            switch (userService.GetProductField(chatId))
            {
                case ProductFields.Name:
                    currentProduct.Name = newValue;
                    break;
                case ProductFields.Description:
                    currentProduct.Description = newValue;
                    break;
                case ProductFields.Date:
                    currentProduct.ExpireAt = Convert.ToDateTime(newValue);
                    break;
            }

            userService.UpdateProduct(chatId, currentProduct);
        }   
    }
}
