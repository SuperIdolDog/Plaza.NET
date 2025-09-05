using AutoMapper;
using Plaza.Net.Model.Entities;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.ViewModels.DTO;
using System.Reflection;


namespace Plaza.Net.Mapping
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
           
            // 递归映射 SysMenuEntity 到 SysMenuDTO
            CreateMap<SysMenuEntity, SysMenuDTO>()
                .ForMember(dto => dto.Children, opt => opt.MapFrom(entity => entity.Children));

            // 递归映射 SysMenuDTO 到 SysMenuEntity
            CreateMap<SysMenuDTO, SysMenuEntity>()
                .ForMember(entity => entity.Children, opt => opt.MapFrom(dto => dto.Children));
            // 获取当前程序集中所有的类型
            var types = Assembly.GetExecutingAssembly().GetTypes();

            // 找到所有的实体类和对应的DTO类并进行映射配置
            var entityTypes = types.Where(t => t.Namespace?.EndsWith("Entity") == true).ToList();
            var dtoTypes = types.Where(t => t.Namespace?.EndsWith("DTO") == true).ToList();

            foreach (var entityType in entityTypes)
            {
                var dtoType = dtoTypes.FirstOrDefault(d => d.Name == entityType.Name.Replace("Entity", "DTO"));
                if (dtoType != null)
                {
                    CreateMap(entityType, dtoType);
                    CreateMap(dtoType, entityType); // 双向映射
                }
            }
        }
    }

}

