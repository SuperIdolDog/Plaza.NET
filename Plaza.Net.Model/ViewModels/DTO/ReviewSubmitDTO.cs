using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class ReviewSubmitDTO
    {
        public int ItemId { get; set; }
        public int Rate{ get; set; } // 45-49
        public string Content { get; set; } = "";
        public string ImageUrls { get; set; } = ""; // 竖线分隔多张图
        public int UserId { get; set; }
    }


}
