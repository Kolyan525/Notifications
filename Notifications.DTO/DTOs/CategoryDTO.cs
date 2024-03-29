﻿using Notifications.DAL.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notifications.DTO.DTOs
{
    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(maximumLength: 20, ErrorMessage = "Category Name is too long")]
        public string CategoryName { get; set; }
    }
    public class UpdateCategoryDTO : CreateCategoryDTO
    {

    }
    public class CategoryDTO : CreateCategoryDTO
    {
        public long CategoryId { get; set; }

        // This should refer to DTO
        public ICollection<EventCategory> EventCategories { get; set; }
    }
}
