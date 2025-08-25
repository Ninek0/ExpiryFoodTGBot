using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpiryFoodTGBot.Models;
using Telegram.Bot.Types;

namespace ExpiryFoodTGBot.Services
{
    public class UserService
    {
        ConcurrentDictionary<ChatId, UserModel> cache = new();
        private UserModel GetOrCreateUser(ChatId chatId)
        {
            return cache.GetOrAdd(chatId, _ => new UserModel());
        }
        public void UpdateUserState(ChatId chatId, CurrentState newState) 
            => GetOrCreateUser(chatId).currentState = newState;
        public CurrentState GetState(ChatId chatId)
            => GetOrCreateUser(chatId).currentState;
        public void UpdateProduct(ChatId chatId, ProductModel product)
            => GetOrCreateUser(chatId).currentProduct = product;
        public ProductModel? GetProduct(ChatId chatId)
            => GetOrCreateUser(chatId).currentProduct;
        public void UpdateProductField(ChatId chatId, ProductFields field)
            => GetOrCreateUser(chatId).productFields = field;
        public ProductFields GetProductField(ChatId chatId)
            => GetOrCreateUser(chatId).productFields;
    }
}
