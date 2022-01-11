using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Users

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Universal",
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Quarantine",
                }
            );

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    EventId = 1,
                    Title = "Online Learning in NaU OA Starts",
                    Description = "Dear students! for the next three weeks we need all together (students and teachers) to unite so as not to lose precious time of the second semester. Therefore, an online learning will be established.",
                    ShortDesc = "Very short description for online learning",
                    EventLink = "https://docs.google.com/document/d/1X7SwM3uUyATgTzd6XIfqop1moM26FsjXfiMxfZqQCZA/edit",
                    StartAt = DateTime.Today,
                },
                new Event
                {
                    EventId = 2,
                    Title = "International rating",
                    Description = "Congratulations, My name is Natalia, I deal with international rankings and NaUAA membership in them. This year, U - Multirank is conducting a survey among students majoring in Computer Science. Please contribute to the high place of NaUAA in this ranking by filling out a small survey.I quote the letter",
                    ShortDesc = "Very short description for international rating",
                    EventLink = "https://che-survey.de/uc/umr2022/ ",
                    StartAt = new DateTime(2021, 12, 20, 11, 24, 00),
                }
            );
        }
    }
}
