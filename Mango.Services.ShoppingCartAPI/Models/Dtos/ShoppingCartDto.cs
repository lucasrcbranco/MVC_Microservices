﻿namespace Mango.Services.ShoppingCartAPI.Models.Dtos;

public class ShoppingCartDto
{
    public ShoppingCartHeaderDto ShoppingCartHeader { get; set; } = null!;
    public IEnumerable<ShoppingCartDetailDto>? ShoppingCartDetails { get; set; }

}
