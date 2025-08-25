using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpiryFoodTGBot.Models
{
    public enum CurrentState
    {
        Idle,
        Editing,
        Adding
    }
    public enum ProductFields
    {
        Name,
        Description,
        Date
    }
    public class UserModel
    {
        public CurrentState currentState = CurrentState.Idle;
        public ProductFields productFields { get; set; }
        public ProductModel? currentProduct = null;
    }
}
