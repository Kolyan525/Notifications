using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notifications.DAL.Models;

namespace Notifications.DTO.DTOs
{
    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(maximumLength: 20, ErrorMessage = "Category Name is too long")]
        public string CategoryName { get; set; }
    }
    public class CategoryDTO : CreateCategoryDTO
    {
        public long CategoryId { get; set; }

        // This should refer to DTO
        public ICollection<EventCategory> EventCategories { get; set; }
    }


}
