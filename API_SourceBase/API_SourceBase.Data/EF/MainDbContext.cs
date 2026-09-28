using API_SourceBase.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_SourceBase.Data.EF
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Position> Positions { get; set; } = null!;
        public virtual DbSet<Account> Accounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Seed dữ liệu mẫu ban đầu để test ngay lập tức trên In-Memory DB
        /// </summary>
        public void SeedData()
        {
            if (!Accounts.Any())
            {
                Accounts.AddRange(
                    new Account { Id = 1, UserName = "admin", FirstName = "Quản trị", LastName = "Hệ thống" },
                    new Account { Id = 2, UserName = "quocthi", FirstName = "Quốc", LastName = "Thi" }
                );
            }

            if (!Departments.Any())
            {
                Departments.AddRange(
                    new Department
                    {
                        Id = 1,
                        Code = "PB_KT",
                        Name = "Phòng Kế Toán",
                        FarmId = 1,
                        FactoryId = 1,
                        Sort = 1,
                        Status = 1,
                        CreatedAt = DateTime.Now,
                        CreatedBy = 1
                    },
                    new Department
                    {
                        Id = 2,
                        Code = "PB_NS",
                        Name = "Phòng Nhân Sự",
                        FarmId = 1,
                        FactoryId = 1,
                        Sort = 2,
                        Status = 1,
                        CreatedAt = DateTime.Now,
                        CreatedBy = 1
                    }
                );
            }

            if (!Positions.Any())
            {
                Positions.AddRange(
                    new Position
                    {
                        Id = 1,
                        DepartmentId = 1,
                        Code = "KTT",
                        Name = "Kế toán trưởng",
                        Type = 3,
                        Sort = 1,
                        Status = 1,
                        CreatedAt = DateTime.Now,
                        CreatedBy = 1
                    },
                    new Position
                    {
                        Id = 2,
                        DepartmentId = 2,
                        Code = "TPNS",
                        Name = "Trưởng phòng nhân sự",
                        Type = 3,
                        Sort = 1,
                        Status = 1,
                        CreatedAt = DateTime.Now,
                        CreatedBy = 1
                    }
                );
            }

            SaveChanges();
        }
    }
}
