using HR.Data.Models;
using HR.Data.Models.Advances;
using HR.Data.Models.Auth;
using HR.Data.Models.BANKING;
using HR.Data.Models.Departments;
using HR.Data.Models.Employees;
using HR.Data.Models.Leaves;


using Microsoft.EntityFrameworkCore;

namespace HR.Data
{
	public class HRDbContext : DbContext
	{
		public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
		{
		}

		// DbSets
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<EducationHistory> EducationHistories { get; set; }
		public DbSet<WorkHistory> WorkHistories { get; set; }
		public DbSet<NextOfKin> NextOfKins { get; set; }
		public DbSet<Hobby> Hobbies { get; set; }
		public DbSet<Skill> Skills { get; set; }
		public DbSet<LeaveRequest> LeaveRequests { get; set; }
		public DbSet<SalaryAdvance> SalaryAdvances { get; set; }
		public DbSet<PaymentData> PaymentData { get; set; }
		public DbSet<BankDetail> BankDetails { get; set; }

		// 2. Add DbSets for Auth models
		public DbSet<User> Users { get; set; } 
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserPermission> UserPermissions { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<HR.Data.Models.Auth.User>(entity =>
			{
				// This is the line that captures the unique constraint
				entity.HasIndex(u => u.Email).IsUnique();
			});

			// -----------------------------
			// Departments
			modelBuilder.Entity<Department>(entity =>
			{
				entity.ToTable("Departments");
				entity.HasKey(d => d.Id);
				entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
				entity.Property(d => d.Description).HasMaxLength(250);
			});

			// -----------------------------
			// Employees
			modelBuilder.Entity<Employee>(entity =>
			{
				entity.ToTable("Employees");
				entity.HasKey(e => e.Id);

				entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
				entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
				entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
				entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
				entity.Property(e => e.Address).IsRequired().HasMaxLength(250);
				entity.Property(e => e.Gender).HasMaxLength(20);
				entity.Property(e => e.JobTitle).HasMaxLength(100);
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.Disability).HasMaxLength(100);
				entity.Property(e => e.Ethnicity).HasMaxLength(100);

				// Department FK
				entity.HasOne(e => e.Department)
					  .WithMany(d => d.Employees)
					  .HasForeignKey(e => e.DepartmentId)
					  .OnDelete(DeleteBehavior.Restrict);

				// PaymentData 1:1
				entity.HasOne(e => e.PaymentData)
					  .WithOne(p => p.Employee)
					  .HasForeignKey<PaymentData>(p => p.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// EducationHistory
			modelBuilder.Entity<EducationHistory>(entity =>
			{
				entity.ToTable("EducationHistories");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.SchoolName).IsRequired().HasMaxLength(150);
				entity.Property(e => e.Country).HasMaxLength(100);
				entity.Property(e => e.Field).HasMaxLength(100);
				entity.Property(e => e.Level).HasMaxLength(50);

				entity.HasOne(e => e.Employee)
					  .WithMany(emp => emp.Education)
					  .HasForeignKey(e => e.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// WorkHistory
			modelBuilder.Entity<WorkHistory>(entity =>
			{
				entity.ToTable("WorkHistories");
				entity.HasKey(w => w.Id);
				entity.Property(w => w.JobTitle).HasMaxLength(100);
				entity.Property(w => w.CompanyName).HasMaxLength(150);
				entity.Property(w => w.CompanyCity).HasMaxLength(100);
				entity.Property(w => w.CompanyCountry).HasMaxLength(100);
				entity.Property(w => w.JobDuties).HasMaxLength(500);

				entity.HasOne(w => w.Employee)
					  .WithMany(e => e.WorkHistory)
					  .HasForeignKey(w => w.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// NextOfKin
			modelBuilder.Entity<NextOfKin>(entity =>
			{
				entity.ToTable("NextOfKins");
				entity.HasKey(n => n.Id);
				entity.Property(n => n.FullName).IsRequired().HasMaxLength(150);
				entity.Property(n => n.Relationship).HasMaxLength(50);
				entity.Property(n => n.Phone).HasMaxLength(20);
				entity.Property(n => n.Address).HasMaxLength(250);

				entity.HasOne(n => n.Employee)
					  .WithMany(e => e.NextOfKin)
					  .HasForeignKey(n => n.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// Hobby
			modelBuilder.Entity<Hobby>(entity =>
			{
				entity.ToTable("Hobbies");
				entity.HasKey(h => h.Id);
				entity.Property(h => h.Name).HasMaxLength(100);

				entity.HasOne(h => h.Employee)
					  .WithMany(e => e.Hobbies)
					  .HasForeignKey(h => h.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// Skill
			modelBuilder.Entity<Skill>(entity =>
			{
				entity.ToTable("Skills");
				entity.HasKey(s => s.Id);
				entity.Property(s => s.Name).HasMaxLength(100);

				entity.HasOne(s => s.Employee)
					  .WithMany(e => e.Skills)
					  .HasForeignKey(s => s.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// LeaveRequest
			modelBuilder.Entity<LeaveRequest>(entity =>
			{
				entity.ToTable("LeaveRequests");
				entity.HasKey(l => l.Id);
				entity.Property(l => l.LeaveType).HasMaxLength(50);
				entity.Property(l => l.Status).HasMaxLength(50);

				entity.HasOne(l => l.Employee)
					  .WithMany(e => e.LeaveRequests)
					  .HasForeignKey(l => l.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// SalaryAdvance
			modelBuilder.Entity<SalaryAdvance>(entity =>
			{
				entity.ToTable("SalaryAdvances");
				entity.HasKey(s => s.Id);
				entity.Property(s => s.Amount).HasColumnType("decimal(18,2)");
				entity.Property(s => s.Reason).HasMaxLength(250);
				entity.Property(s => s.Status).HasMaxLength(50);

				entity.HasOne(s => s.Employee)
					  .WithMany(e => e.SalaryAdvances)
					  .HasForeignKey(s => s.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// PaymentData
			modelBuilder.Entity<PaymentData>(entity =>
			{
				entity.ToTable("PaymentData");
				entity.HasKey(p => p.Id);

				entity.Property(p => p.KRA_PIN).HasMaxLength(50);
				entity.Property(p => p.NSSF_Number).HasMaxLength(50);
				entity.Property(p => p.NHIF_Number).HasMaxLength(50);

				// PaymentData ↔ BankDetail 1:1
				entity.HasOne(p => p.BankDetail) // PaymentData has one BankDetail
					.WithOne(b => b.PaymentData) // BankDetail relates back to one PaymentData
					.HasForeignKey<PaymentData>(p => p.BankDetailId) // The FK is located on the PaymentData model
					.OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// BankDetail
			modelBuilder.Entity<BankDetail>(entity =>
			{
				entity.ToTable("BankDetails");
				entity.HasKey(b => b.Id);

				entity.Property(b => b.BankName).HasMaxLength(100);
				entity.Property(b => b.Branch).HasMaxLength(100);
				entity.Property(b => b.AccountName).HasMaxLength(150);
				entity.Property(b => b.AccountNumber).HasMaxLength(50);
			});

			// UserPermission
			modelBuilder.Entity<UserPermission>(entity =>
			{
				entity.ToTable("UserPermissions");
				entity.HasKey(up => up.Id);

				// Foreign Key setup
				entity.HasOne(up => up.User)
					  .WithMany() // User can have many permissions, but we don't need a collection on the User model
					  .HasForeignKey(up => up.UserId)
					  .OnDelete(DeleteBehavior.Cascade);

				// Ensure no duplicate permission codes for the same user
				entity.HasIndex(up => new { up.UserId, up.PermissionCode }).IsUnique();
			});
		}
	}
}
