using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.Model.Entities;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Device;
using Plaza.Net.Model.Entities.Order;

using Plaza.Net.Model.Entities.Store;
using Plaza.Net.Model.Entities.Sys;
using Plaza.Net.Model.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Plaza.Net.Model
{
    public class EFDbContext : IdentityDbContext<UserEntity, UserRoleEntity, int>
    {
        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder EntityBuilder)
        {
            base.OnModelCreating(EntityBuilder);

            var softDeleteEntities = EntityBuilder.Model.GetEntityTypes()
           .Where(e => typeof(IBaseEntity).IsAssignableFrom(e.ClrType) &&
                        e.ClrType.GetProperty(nameof(IBaseEntity.IsDeleted)) != null);
            foreach (var entity in softDeleteEntities)
            {
                // 使用强类型方式创建查询过滤器
                var parameter = Expression.Parameter(entity.ClrType, "e");
                var property = Expression.Property(parameter, nameof(IBaseEntity.IsDeleted));
                var condition = Expression.MakeUnary(ExpressionType.Not, property, typeof(bool));
                var lambda = Expression.Lambda(condition, parameter);

                // 应用查询过滤器
                EntityBuilder.Entity(entity.ClrType).HasQueryFilter(lambda);
            }
            // 禁用自动外键发现约定
            // 或者更直接地移除所有导航属性的外键自动发现
            //foreach (var entityType in EntityBuilder.Model.GetEntityTypes())
            //{
            //    foreach (var navigation in entityType.GetNavigations())
            //    {
            //        navigation.SetIsEagerLoaded(false);
            //    }
            //}

            //EntityBuilder.UseCollation("Chinese_PRC_CI_AS");
            //从程序集加载所有的IEntityguration
            EntityBuilder.ApplyConfigurationsFromAssembly(typeof(EFDbContext).Assembly);
            EntityBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoleMapping");
            EntityBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaim");
            EntityBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogin");
            EntityBuilder.Entity<IdentityUserToken<int>>().ToTable("UserToken");
            EntityBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaim");

        }
        // DbSets
        public DbSet<PlazaEntity> Plaza { get; set; }
        public DbSet<FloorEntity> Floor { get; set; }
        public DbSet<ParkingAreaEntity> ParkingArea { get; set; }
        public DbSet<ParkingSpotEntity> ParkingSpot { get; set; }
        public DbSet<StoreEntity> Store { get; set; }
        public DbSet<StoreTypeEntity> StoreType { get; set; }
        public DbSet<DeviceEntity> Device { get; set; }
        public DbSet<DeviceTypeEntity> DeviceType { get; set; }
        public DbSet<DeviceDataEntity> DeviceData { get; set; }
        public DbSet<ParkingRecordEntity> ParkingRecord { get; set; }
        public DbSet<OrderEntity> Order { get; set; }
        public DbSet<OrderItemEntity> OrderItem { get; set; }
        public DbSet<PaymentRecordEntity> PaymentRecord { get; set; }
        public DbSet<ReviewEntity> Review { get; set; }
        public DbSet<EmployeeEntity> Employee { get; set; }
        public DbSet<EmployeeRoleEntity> EmployeeRole { get; set; }
        public DbSet<ProductEntity> Product { get; set; }
        public DbSet<ProductTypeEntity> ProductType { get; set; }
        public DbSet<ShoppingCartItemEntity> ShoppingCartItem { get; set; }
        public DbSet<UserFeedbackEntity> UserFeedback { get; set; }
        public DbSet<NotificationEntity> Notification { get; set; }
        public DbSet<SysMenuEntity> SysMenu { get; set; }
        public DbSet<PermissionEntity> Permission { get; set; }
        public DbSet<MenuPermissionEntity> MenuPermission { get; set; }
        public DbSet<LoginLogEntity> LoginLog { get; set; }
        public DbSet<OperationLogEntity> OperationLog { get; set; }
        public DbSet<DictionaryEntity> Dictionary { get; set; }
        public DbSet<DictionaryItemEntity> DictionaryItem { get; set; }
        public DbSet<AddressEntity> Address { get; set; }

        public DbSet<ProductSkuEntity> ProductSkus { get; set; }

        public DbSet<ProductSkuSpecValueEntity> ProductSkuSpecs { get; set; }

        public DbSet<ProductSpecEntity> ProductSpec { get; set; }

        public DbSet<ProductSpecValueEntity> ProductSpecValue { get; set; }

    }
}
