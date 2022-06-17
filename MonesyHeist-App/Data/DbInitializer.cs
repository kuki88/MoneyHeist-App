using MonesyHeist_App.Data.Model;

namespace MonesyHeist_App.Data
{
    public class DbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                //if (!context.Skills.Any())
                //{
                //    context.Skills.AddRange(
                //    new Skill()
                //}

                //if (!context.Members.Any())
                //{
                //    context.Members.AddRange(
                //    new Member()
                //    {
                //        Email = "markomarkovic@gmail.com",
                //        Name = "Marko Markovic",
                         
                //    },
                //    new Member()
                //    {
                //    });
                //    context.SaveChanges();
                //}
            }
        }
    }
}
