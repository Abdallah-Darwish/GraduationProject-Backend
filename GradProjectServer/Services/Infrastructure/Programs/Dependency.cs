using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Infrastructure
{
    //todo: delete
    public class Dependency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static void ConfigureEntity(EntityTypeBuilder<Dependency> b)
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name)
                .IsRequired()
                .IsUnicode();
        }

    }
}
