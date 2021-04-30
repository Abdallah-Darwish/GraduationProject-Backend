using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GradProjectServer.Common;
using GradProjectServer.Controllers;
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
        public ResourceType Type { get; set; }
        public Course Course { get; set; }
        //todo: connect with Instructor if you ever add it
        public int CreationYear { get; set; }
        public Semester CreationSemester { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Not prefixed with dot.
        /// </summary>
        public string FileExtension { get; set; }

        public bool IsApproved { get; set; }
        public int VolunteerId { get; set; }
        public User Volunteer { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<Resource> b)
        {
            b.HasKey(r => r.Id);
            b.Property(r => r.Name)
                .IsRequired()
                .IsUnicode();
            b.Property(r => r.FileExtension)
                .IsRequired()
                .IsUnicode();
            b.Property(r => r.Type)
                .IsRequired()
                .HasConversion<byte>();
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

            b.HasCheckConstraint("CK_RESOURCE_CREATIONYEAR", $"\"{nameof(CreationYear)}\" >= 1900");
        }

        public static async Task CreateSeedFiles()
        {
            Random rand = new();
            foreach (var resource in Seed)
            {
                await using var resourceFileStream = new FileStream(ResourceController.GetResourceFilePath(resource),
                    FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
                await using var resourceWriter = new StreamWriter(resourceFileStream);
                await resourceWriter.WriteLineAsync($"Id: {resource.Id}").ConfigureAwait(false);
                await resourceWriter.WriteLineAsync($"Name: {resource.Name}").ConfigureAwait(false);
                await resourceWriter.WriteLineAsync(rand.NextText()).ConfigureAwait(false);
                await resourceWriter.FlushAsync().ConfigureAwait(false);
            }
        }

        private static Resource[]? _seed = null;

        public static Resource[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                var rand = new Random();
                var courses = Course.Seed;
                var users = User.Seed;
                var seed = new List<Resource>();
                var semesters = Enum.GetValues<Semester>();
                int id = 1;
                foreach (var course in courses)
                {
                    int resourcesCount = rand.Next(5);
                    for (int i = 0; i < resourcesCount; i++)
                    {
                        var volunteer = rand.NextElement(users);
                        Resource resource = new()
                        {
                            Id = id++,
                            Name =
                                $"Course {course.Id}, User {volunteer.Id}, resource {rand.NextText(rand.Next(1, 10))}",
                            CourseId = course.Id,
                            VolunteerId = volunteer.Id,
                            FileExtension = "txt",
                            CreationSemester = rand.NextElement(semesters),
                            CreationYear = rand.Next(2015, DateTime.Now.Year),
                            IsApproved = rand.NextBool()
                        };
                        seed.Add(resource);
                    }
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}