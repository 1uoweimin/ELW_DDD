using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using Zack.DomainCommons.Models;

namespace Microsoft.EntityFrameworkCore
{
    public static class EFCoreExtensions
    {
        /// <summary>
        /// set global 'IsDeleted=false' queryfilter for every entity
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void EnableSoftDeletionGlobalFilter(this ModelBuilder modelBuilder)
        {
            var entityTypesHasSoftDeletion = modelBuilder.Model.GetEntityTypes()
                .Where(e => e.ClrType.IsAssignableTo(typeof(ISoftDelete)));

            foreach (var entityType in entityTypesHasSoftDeletion)
            {
                var isDeletedProperty = entityType.FindProperty(nameof(ISoftDelete.IsDeleted));
                var parameter = Expression.Parameter(entityType.ClrType, "p");
                var filter = Expression.Lambda(Expression.Not(Expression.Property(parameter, isDeletedProperty.PropertyInfo)), parameter);
                entityType.SetQueryFilter(filter);
            }
        }

        /// <summary>
        /// 为所有实体的DateTime属性设置全局的UTC转换
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void EnableConvertToUTCGlobal(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(
                        new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(), // Convert to UTC when writing to the database  
                            v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified), // Convert to Unspecified when reading from the database (or you can set it to Local if you prefer)  
                            new ConverterMappingHints(size: 8) // Specify size if needed  
                            )
                        );
                        continue;
                    }

                    // For nullable DateTime?  
                    property.SetValueConverter(
                        new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null, // Convert to UTC when writing to the database  
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) : (DateTime?)null, // Convert to Unspecified when reading from the database (or you can set it to Local if you prefer)  
                            new ConverterMappingHints(size: 8) // Specify size if needed  
                        )
                    );
                }
            }
        }

        public static IQueryable<T> Query<T>(this DbContext ctx) where T : class, IEntity
        {
            return ctx.Set<T>().AsNoTracking();
        }
    }
}
