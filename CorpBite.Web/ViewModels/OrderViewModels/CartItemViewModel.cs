﻿namespace CorpBite.ViewModels.OrderViewModels
{
    public class CartItemViewModel
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}