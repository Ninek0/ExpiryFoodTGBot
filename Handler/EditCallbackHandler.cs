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
    public class EditCallbackHandler
    {
        private readonly ProductService _productService;
        private readonly MessageService _messageService;
        private readonly UserService _userService;
        public EditCallbackHandler(ProductService productService, MessageService messageService, UserService userService)
        {
            _productService = productService;
            _messageService = messageService;
            _userService = userService;
        }
        public async Task Edit(int productId, long chatId)
        {
            _userService.UpdateUserState(chatId, CurrentState.Editing);
            await GetProductById(chatId, productId);
            await _messageService.SendEditableProductMessage(chatId, _userService.GetProduct(chatId));
        }
        private async Task GetProductById(ChatId chatId, int productId)
        {
            ProductModel? product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return;
            _userService.UpdateProduct(chatId, product);
        }
    }
}
