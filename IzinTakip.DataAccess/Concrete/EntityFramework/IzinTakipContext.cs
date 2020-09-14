using IzinTakip.Entities.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IzinTakip.DataAccess.Concrete.EntityFramework
{
    public class IzinTakipContext : IdentityDbContext<User, Role, string>
    {
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAnnualDetails> EmployeeAnnualDetails { get; set; }
        public virtual DbSet<EmployeeSpecialLeave> EmployeeSpecialLeaves { get; set; }

        // Parameterless contsractor
        public IzinTakipContext() { }
        public IzinTakipContext(DbContextOptions<IzinTakipContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(@"Server=DESKTOP-6MBABS7\SQLEXPRESS;Database=IzinTakip; User=sa; Password=ikikerekiki69!;");
                //optionsBuilder.UseSqlServer(@"Server=DESKTOP-6MBABS7\SQLEXPRESS;Database=IzinTakipTest; User=sa; Password=ikikerekiki69!;");
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-UEVCS24\SQLEXPRESS;Database=IzinTakipTest;Trusted_Connection=True;Integrated Security=true;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            BuildEmployeeModel(modelBuilder);
            BuildEmployeeAnnualRightsModel(modelBuilder);
            BuildUsersModel(modelBuilder);
            BuildEmployeeSpecialLeaveModel(modelBuilder);
        }
        private void BuildEmployeeSpecialLeaveModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeSpecialLeave>(entity =>
            {
                entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd();

                entity.Property(e => e.Text)
                     .HasMaxLength(256);

                entity.Property(e => e.StartDate)
                     .HasColumnType("datetime2");

                entity.Property(e => e.EndDate)
                     .HasColumnType("datetime2");

                entity.Property(e => e.CreatedAt)
                     .HasColumnType("datetime2");

                entity.Property(e => e.UpdatedAt)
                     .HasColumnType("datetime2");

                entity.HasOne(e => e.Employee)
                     .WithMany(e => e.EmployeeSpecialLeaves)
                     .HasForeignKey(e => e.EmployeeId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_EmployeeSpecialLeaves_Employee");
            });
        }

        #region utils
        private void BuildEmployeeModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                      .HasMaxLength(128);

                entity.Property(e => e.Surname)
                      .HasMaxLength(128);

                entity.Property(e => e.Position)
                      .HasMaxLength(128);

                entity.Property(e => e.PhoneNum)
                      .HasMaxLength(64);

                entity.Property(e => e.Image)
                      .HasMaxLength(255);

                entity.Property(e => e.RecruitmentDate)
                     .HasColumnType("datetime2");

                entity.Property(e => e.CreatedAt)
                     .HasColumnType("datetime2");

                entity.Property(e => e.UpdatedAt)
                     .HasColumnType("datetime2");

                entity.HasOne(e => e.User)
                     .WithMany(e => e.Employees)
                     .HasForeignKey(e => e.UserId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_Employees_User");
            });
        }
        private void BuildEmployeeAnnualRightsModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeAnnualDetails>(entity =>
            {
                entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd();

                entity.Property(e => e.StartDate)
                     .HasColumnType("datetime2");

                entity.Property(e => e.EndDate)
                     .HasColumnType("datetime2");

                entity.Property(e => e.CreatedAt)
                     .HasColumnType("datetime2");

                entity.Property(e => e.UpdatedAt)
                     .HasColumnType("datetime2");

                entity.HasOne(e => e.Employees)
                     .WithMany(e => e.EmployeeAnnualDetails)
                     .HasForeignKey(e => e.EmployeesId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_EmployeeAnnualDetails_Employee");
            });
        }
        private void BuildUsersModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                     .ValueGeneratedOnAdd();

                entity.Property(e => e.UserName)
                      .HasMaxLength(32)
                      .HasColumnType("text");

                entity.Property(e => e.Email)
                      .HasMaxLength(32)
                      .HasColumnType("text");

                entity.Property(e => e.Biography)
                      .HasMaxLength(300)
                      .HasColumnType("text");

                entity.Property(e => e.CreatedAt)
                                     .HasColumnType("datetime2");

                entity.Property(e => e.UpdatedAt)
                                      .HasColumnType("datetime2");
            });
        }
        #endregion
    }
}
