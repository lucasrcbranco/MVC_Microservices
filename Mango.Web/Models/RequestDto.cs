﻿using static Mango.Web.Utility.SD;

namespace Mango.Web.Models;

public class RequestDto
{
    public ApiType ApiType { get; set; } = ApiType.GET;
    public string URL { get; set; } = null!;
    public object? Data { get; set; }
    public string? AccessToken { get; set; }

    public ContentType ContentType { get; set; } = ContentType.Json;
}
