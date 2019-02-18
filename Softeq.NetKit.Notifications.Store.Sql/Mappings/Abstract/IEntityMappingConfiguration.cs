// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Softeq.NetKit.Notifications.Store.Sql.Mappings.Abstract
{
    internal interface IEntityMappingConfiguration
    {
        void Map(ModelBuilder builder);
    }

    internal interface IEntityMappingConfiguration<T> : IEntityMappingConfiguration where T : class
    {
        void Map(EntityTypeBuilder<T> builder);
    }
}
