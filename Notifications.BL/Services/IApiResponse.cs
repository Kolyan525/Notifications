using System.Net;
using System.Text.Json.Serialization;

namespace Notifications.BL.Services
{
    public interface IApiResponse
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; }

        public string Message { get; }
    }

    public interface IApiResponse<out TData> : IApiResponse
    {
        public TData Data { get; }
    }
}