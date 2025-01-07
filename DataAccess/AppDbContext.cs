using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagmentModule.Models;

namespace UserManagmentModule.DataAccess
{
    public class AppDbContext : IdentityDbContext<User>
    {
        //Bu constructor, DbContext sınıfına ayarları geçirir.
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        //NOT: ASP.NET Identity'nin oluşturduğu tablolar için DbSet eklemeye gerek yok. Çünkü Identity tabloları zaten IdentityDbContext tarafından yönetiliyor. Yani IdentityDbContext sınıfını genişlettiğinde, AspNetUsers, AspNetRoles, AspNetUserRoles gibi tablolar otomatik olarak oluşturulur ve yönetilir.
    }
}
/*public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
*/