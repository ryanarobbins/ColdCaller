namespace ColdCaller.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ColdCaller.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ColdCaller.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ColdCaller.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //  TODO: Uncomment to initially seed the database.
            //
            /*context.Students.AddOrUpdate(
              p => p.Name,
              new Student { Name = "Luke Skywalker", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Han Solo", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "The Doctor", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Rose", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Clara", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Bill", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Ted", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Paul Atreides", StudentClass = "1st Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Jar Jar Binks", StudentClass = "2nd Period", TeacherId = "DemoTeacher" },
              new Student { Name = "Admiral Ackbar", StudentClass = "2nd Period", TeacherId = "DemoTeacher" }
            );*/
        }
    }
}
