

namespace Plaza.Net.Model.ViewModels.DTO
{
    public class SysMenuDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public string Type { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Order { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public List<SysMenuDTO> Children { get; set; } = new List<SysMenuDTO>();
        public int? RequiredPermissionId { get; set; }

      
    }
}
