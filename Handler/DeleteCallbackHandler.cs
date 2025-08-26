using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpiryFoodTGBot.Service;
using ExpiryFoodTGBot.Services;
using Telegram.Bot.Types;

namespace ExpiryFoodTGBot.Handler
{
    public class DeleteCallbackHandler
    {
        private readonly ProductService _productService;
        private readonly MessageService _messageService;
        public DeleteCallbackHandler(ProductService productService, MessageService messageService) 
        {
            _productService = productService;
            _messageService = messageService;
        }
        public async Task Delete(string callbackQueryId, int productId, long chatId, int messageId)
        {
            var result = await _productService.DeleteProductAsync(productId);
            _messageService.AnswerRemoveProduct(callbackQueryId, productId, result);
            if (result)
                _messageService.DeleteMessage(chatId, messageId);
        }
    }
}
