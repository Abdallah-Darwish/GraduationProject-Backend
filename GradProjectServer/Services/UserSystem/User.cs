using GradProjectServer.Controllers;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.Resources;
using SkiaSharp;

namespace GradProjectServer.Services.UserSystem
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public string? Token { get; set; }
        public string PasswordHash { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }
        public ICollection<Exam> VolunteeredExams { get; set; }
        public ICollection<Question> VolunteeredQuestions { get; set; }
        public ICollection<Resource> VolunteeredResources { get; set; }

        public static void ConfigureEntity(EntityTypeBuilder<User> b)
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Email)
                .IsRequired()
                .IsUnicode();
            b.Property(u => u.Name)
                .IsRequired()
                .IsUnicode();
            b.Property(u => u.Token)
                .IsUnicode();
            b.Property(u => u.IsAdmin);
            b.HasOne(u => u.StudyPlan)
                .WithMany()
                .HasForeignKey(u => u.StudyPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        //todo: can volunteer
        //todo: points

        public static async Task CreateSeedFiles()
        {
            using SKPaint paint = new()
            {
                Color = SKColors.Red, Style = SKPaintStyle.Fill, HintingLevel = SKPaintHinting.Full,
                IsAntialias = true, TextAlign = SKTextAlign.Center, TextSize = 10, StrokeWidth = 10
            };
            Random rand = new();

//todo: fix me
            async Task GenerateProfilePicture(User u)
            {
                using SKBitmap bmp = new(200, 200);
                using (SKCanvas can = new(bmp))
                {
                    can.Clear(new SKColor((uint) rand.Next(100, int.MaxValue)));
                    can.DrawText(u.Name, new SKPoint(0, 0), paint);
                    can.Flush();
                }

                using var bmpData = bmp.Encode(SKEncodedImageFormat.Jpeg, 100);
                await using var userImageFileStream = new FileStream(UserManager.GetProfilePicturePath(u.Id),
                    FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
                bmpData.SaveTo(userImageFileStream);
                await userImageFileStream.FlushAsync();
            }


            foreach (var user in Seed)
            {
                if (rand.NextBool())
                {
                    await GenerateProfilePicture(user).ConfigureAwait(false);
                }
            }
        }

        private static User[]? _seed = null;

        public static User[] Seed
        {
            get
            {
                if (_seed != null)
                {
                    return _seed;
                }

                var rand = new Random();
                var studyPlans = StudyPlan.Seed;
                var firstNames = new string[]
                {
                    "Abdallah", "Hashim", "Shatha", "Jannah", "Malik", "Basel", "Al-Bara", "Mohammad", "Aya", "Issra",
                    "Huda", "Tuqa", "Deema"
                };
                var lastNames = new string[]
                {
                    "Darwish", "Al-Mansour", "Shreim", "Barqawi", "Arabiat", "Azaizeh", "Zeer", "Faroun", "Abu-Rumman",
                    "Allan", "Odeh"
                };
                var seed = new List<User>();
                int id = 1;
                for (char i = 'a'; i <= 'z'; i++)
                {
                    var sp = rand.NextElement(studyPlans);
                    var user = new User
                    {
                        Id = id++,
                        Email = $"{i}@{i}.com",
                        IsAdmin = true,
                        Name = $"{rand.NextElement(firstNames)} {rand.NextElement(lastNames)}",
                        StudyPlanId = sp.Id,
                        PasswordHash = UserManager.HashPassword($"{i}123456789{i}"),
                        Token = null
                    };

                    seed.Add(user);
                }

                _seed = seed.ToArray();
                return _seed;
            }
        }
    }
}