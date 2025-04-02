using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StudyProj.SeedConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                   Id = "348e2a0b-18c6-4ba3-8426-31bb84eb0eb2",
                   Name = "Chief",
                   NormalizedName = "CHIEF",
                   Description = "Роль старосты для пользователя",
                },
                new Role
                {
                    Id = "51bf09e1-d994-434f-8dd2-0e32f2341c10",
                    Name = "Dean",
                    NormalizedName = "DEAN",
                    Description = "Роль декана",
                },
                new Role
                {
                    Id = "51bf09e1-d994-434f-8dd2-0e32f2341c19",
                    Name = "Deputy Dean",
                    NormalizedName = "DEPUTY DEAN",
                    Description = "Роль зам. декана",
                },
                new Role
                {
                    Id = "51bf09e1-d994-434f-8dd2-0e32f2341c18",
                    Name = "Teacher",
                    NormalizedName = "TEACHER",
                    Description = "Роль преподавателя",
                });
        }
    }
}
