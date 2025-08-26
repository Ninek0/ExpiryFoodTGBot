using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpiryFoodTGBot.Models;
using ExpiryFoodTGBot.Service;
using ExpiryFoodTGBot.Services;
using Telegram.Bot.Types;

namespace ExpiryFoodTGBot.Handler
{
    public class IdleHandler
    {
        readonly private UserService _userService;
        readonly private ProductService _productService;
        readonly private MessageService _messageService;
        public IdleHandler(UserService userService, ProductService productService, MessageService messageService)
        {
            _userService = userService;
            _productService = productService;
            _messageService = messageService;
        }
        public async Task HandleIdleState(ChatId chatId, string message)
        {
            switch (message)
            {
                case "Посмотреть все продукты":
                    _userService.UpdateUserState(chatId, CurrentState.Idle);
                    _messageService.SendAllProducts(chatId, await _productService.GetExpiringProductsAsync());
                    break;
                case "Добавить продукт":
                    _userService.UpdateUserState(chatId, CurrentState.Adding);
                    await _messageService.SendEditableProductMessage(chatId, _userService.GetProduct(chatId));
                    break;
                default:
                    _messageService.SendMainMenu(chatId);
                    break;
            }
        }
    }
}
