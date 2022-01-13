using AutoMapper;
using Notifications.DAL.Models;
using Notifications.DTO.DTOs;

namespace Notifications.DTO.Configurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Event, EventDTO>().ReverseMap();
            CreateMap<Event, CreateEventDTO>().ReverseMap();
            CreateMap<NotificationType, NotificationTypeDTO>().ReverseMap();
            CreateMap<NotificationType, CreateNotificationTypeDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }
}
