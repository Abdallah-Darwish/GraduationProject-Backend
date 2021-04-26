using System;
using GradProjectServer.Common;
using GradProjectServer.Services.Infrastructure;
using GradProjectServer.Services.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GradProjectServer.Services.Resources
{
    public class Resource
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        //todo: connect with Instructor if you ever add it
        public int CreationYear { get; set; }
        public Semester CreationSemester { get; set; }
        public string Name { get; set; }
        //needed for extension
        public string FileName { get; set; }
        public  bool IsApproved { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<Resource> b)
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Name)
                .IsRequired()
                .IsUnicode();
            b.Property(r => r.FileName)
                .IsRequired()
                .IsUnicode();
            b.Property(r => r.IsApproved)
                .IsRequired();
            b.Property(r => r.CreationYear)
                .IsRequired();
            b.Property(r => r.CreationSemester)
                .HasConversion<byte>()
                .IsRequired();
            b.HasOne(r => r.Course)
                .WithMany(c => c.Resources)
                .HasForeignKey(r => r.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(r => r.Volunteer)
                .WithMany(u => u.VolunteeredResources)
                .HasForeignKey(r => r.VolunteerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasCheckConstraint("CK_RESOURCE_CREATIONYEAR", $"\"{nameof(CreationYear)}\" >= 1900;");
        }
        //todo: seed
    }
}