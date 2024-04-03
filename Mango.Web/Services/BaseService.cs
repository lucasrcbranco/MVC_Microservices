using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Services;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenProvider _tokenProvider;

    public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
    }

    public async Task<ResponseDto?> SendAsync(RequestDto request, bool requiresAuthentication = true)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
            HttpRequestMessage message = new();
            message.RequestUri = new Uri(request.URL);

            if (request.ContentType == ContentType.Json)
            {
                message.Headers.Add("Accept", "application/json");
            }
            else
            {
                message.Headers.Add("Accept", "*/*");
            }

            if (requiresAuthentication)
            {
                message.Headers.Add("Authorization", $"Bearer {_tokenProvider.GetToken()}");
            }

            if (request.ContentType == ContentType.MultipartFormData
                    && request.Data != null)
            {
                var content = new MultipartFormDataContent();

                foreach (var prop in request.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(request.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                        {
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? string.Empty : value.ToString()), prop.Name);
                    }

                    message.Content = content;
                }
            }
            else if (request.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage? apiResponse = null;

            switch (request.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                case ApiType.GET:
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await client.SendAsync(message);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new() { IsSuccess = false, Message = "Not Found" };
                case HttpStatusCode.Forbidden:
                    return new() { IsSuccess = false, Message = "Forbidden" };
                case HttpStatusCode.Unauthorized:
                    return new() { IsSuccess = false, Message = "Unauthorized" };
                case HttpStatusCode.InternalServerError:
                    return new() { IsSuccess = false, Message = "Internal Server Error" };
                default:
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            }
        }
        catch (Exception ex)
        {
            return new() { IsSuccess = false, Message = ex.Message };
        }
    }
}
