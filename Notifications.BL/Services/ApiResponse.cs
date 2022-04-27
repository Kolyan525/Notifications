using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;

namespace Notifications.BL.Services
{
    public static class ApiResponse
    {
        public static IApiResponse<TData> Ok<TData>(TData data)
        {
            return new ApiResponse<TData> { Data = data, StatusCode = HttpStatusCode.OK };
        }

        public static IApiResponse Ok()
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.OK };
        }

        public static IApiResponse NotFound(params string[] messages)
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.NotFound, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse<TData> NotFound<TData>(params string[] messages)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.NotFound, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse<TData> NotFound<TData>(TData value, params string[] messages)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.NotFound, Message = string.Join(Environment.NewLine, messages), Data = value };
        }

        public static IApiResponse BadRequest(params string[] messages)
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.BadRequest, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse<TData> BadRequest<TData>(params string[] messages)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.BadRequest, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse<TData> BadRequest<TData>(TData value, params string[] messages)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.BadRequest, Message = string.Join(Environment.NewLine, messages), Data = value };
        }

        public static IApiResponse<TData> BadRequest<TData>(IdentityResult result)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.BadRequest, Message = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) };
        }

        public static IApiResponse BadRequest(IdentityResult result)
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.BadRequest, Message = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)) };
        }

        public static IApiResponse Conflict(params string[] messages)
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.Conflict, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse<TData> Conflict<TData>(params string[] messages)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.Conflict, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse Forbidden(params string[] messages)
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.Forbidden, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse<TData> Forbidden<TData>(params string[] messages)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.Forbidden, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse Unauthorized(params string[] messages)
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.Unauthorized, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse<TData> Unauthorized<TData>(params string[] messages)
        {
            return new ApiResponse<TData> { StatusCode = HttpStatusCode.Unauthorized, Message = string.Join(Environment.NewLine, messages) };
        }

        public static IApiResponse NoContent(params string[] messages)
        {
            return new ApiResponse<object> { StatusCode = HttpStatusCode.NoContent, Message = string.Join(Environment.NewLine, messages) };
        }
    }

    public class ApiResponse<TData> : IApiResponse<TData>
    {
        public TData Data { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}