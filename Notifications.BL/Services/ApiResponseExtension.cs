using Microsoft.AspNetCore.Mvc;

namespace Notifications.BL.Services
{
    public static class ApiResponseExtension
    {
        public static ActionResult ToResult(this IApiResponse response) => new ObjectResult(response)
        {
            StatusCode = (int)response.StatusCode
        };

        public static ActionResult<T> ToResult<T>(this IApiResponse response) => new ObjectResult(response)
        {
            StatusCode = (int)response.StatusCode
        };
    }
}