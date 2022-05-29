using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Nivara.Data.Models.NivaraDbContext
{
    public class NivaraDbContext : IdentityDbContext
    {
        private readonly DbContextOptions _options;

        public NivaraDbContext()
        {
        }
       

        public NivaraDbContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyRoles> CompanyRoles { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<CompanyRolesToModule> CompanyRolesToModules { get; set; }
        public virtual DbSet<UsersTask> UsersTask { get; set; }
        public virtual DbSet<UsersTaskDocument> UsersTaskDocument { get; set; }
        public virtual DbSet<EndUsers> EndUsers { get; set; }
        public virtual DbSet<EmployeesTask> EmployeesTasks { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<TaskStatus> TaskStatus { get; set; }
        public virtual DbSet<TaskComments> TaskComments { get; set; }
        public virtual DbSet<PreDefinedTask> PreDefinedTasks { get; set; }
    }
}
