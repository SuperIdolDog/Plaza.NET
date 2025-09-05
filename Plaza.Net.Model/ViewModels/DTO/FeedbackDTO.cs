using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
   public class FeedbackDTO
    {
       public string Content { get; set; } = null!;
        public string? ImageUrls { get; set; }
        public string? Contact { get; set; }
        public int UserId { get; set; }
    }
}
