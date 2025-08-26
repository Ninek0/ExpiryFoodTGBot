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
    public class EditingHandler
    {
        private readonly UserService _userService;
        private readonly ProductService _productService;
        private readonly MessageService _messageService;
        public EditingHandler(UserService userService, ProductService productService, MessageService messageService) 
        {
            _userService = userService;
            _productService = productService;
            _messageService = messageService;
        }
        public async Task HandleNonIdleState(ChatId chatId, string message)
        {
            switch (message)
            {
                case "Редактировать название":
                    _userService.UpdateProductField(chatId, ProductFields.Name);
                    _messageService.AskForEditProductField(chatId, ProductFields.Name);
                    break;
                case "Редактировать описание":
                    _userService.UpdateProductField(chatId, ProductFields.Description);
                    _messageService.AskForEditProductField(chatId, ProductFields.Description);
                    break;
                case "Когда испортится":
                    _userService.UpdateProductField(chatId, ProductFields.Date);
                    _messageService.AskForEditProductField(chatId, ProductFields.Date);
                    break;
                case "Готово":
                    switch (_userService.GetState(chatId))
                    {
                        case CurrentState.Adding:
                            await _productService.CreateProductAsync(_userService.GetProduct(chatId)!);
                            break;
                        case CurrentState.Editing:
                            await _productService.UpdateProductAsync(_userService.GetProduct(chatId)!);
                            break;
                    }
                    _userService.UpdateUserState(chatId, CurrentState.Idle);
                    _userService.UpdateProduct(chatId, new());
                    _messageService.SendMainMenu(chatId);
                    break;
                default:
                    EditProductParametr(chatId, message);
                    _messageService.AskForEdit(chatId);
                    await _messageService.SendEditableProductMessage(chatId, _userService.GetProduct(chatId));
                    break;
            }
        }
        private void EditProductParametr(ChatId chatId, string newValue)
        {
            var currentProduct = _userService.GetProduct(chatId)!;

            switch (_userService.GetProductField(chatId))
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

            _userService.UpdateProduct(chatId, currentProduct);
        }
    }
}
