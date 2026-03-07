using HR.Core.Enums;
using HR.Data.Models;
using HR.Data.Models.Advances;
using HR.Data.Models.Auth;
using HR.Data.Models.BANKING;
using HR.Data.Models.Country;
using HR.Data.Models.County;
using HR.Data.Models.Departments;
using HR.Data.Models.Employees;
using HR.Data.Models.EmployestatusLog;
using HR.Data.Models.Leaves;
using HR.Data.Models.PAYROLL;
using HR.Data.Models.Recruitment;

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
		public DbSet<Country> Countries { get; set; }
		public DbSet<County> Counties { get; set; }
		public DbSet<SubCounty> SubCounties { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Ethnicity> Ethnicities { get; set; }
		public DbSet<EducationHistory> EducationHistories { get; set; }
		public DbSet<WorkHistory> WorkHistories { get; set; }
		public DbSet<NextOfKin> NextOfKins { get; set; }
		public DbSet<Hobby> Hobbies { get; set; }
		public DbSet<Skill> Skills { get; set; }
		public DbSet<LeaveRequest> LeaveRequests { get; set; }
		public DbSet<SalaryAdvance> SalaryAdvances { get; set; }
		public DbSet<PaymentData> PaymentData { get; set; }
		public DbSet<BankDetail> BankDetails { get; set; }
		public DbSet<DisabilityDetail> DisabilityDetails { get; set; }
		// 2. Add DbSets for Auth models
		public DbSet<User> Users { get; set; } 
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserPermission> UserPermissions { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; } = default!;
		public DbSet<EmployeeStatusLog> EmployeeStatusLogs { get; set; }
		public DbSet<JobRequisition> JobRequisition { get; set; }
		public DbSet<RequisitionApproval> RequisitionApprovals { get; set; }
		public DbSet<JobListing> JobListings { get; set; }
		public DbSet<JobApplication> JobApplications { get; set; }
		public DbSet<ApplicantEducation> ApplicantEducations { get; set; }
		public DbSet<ApplicantExperience> ApplicantExperiences { get; set; }
		public DbSet<Interview> Interviews { get; set; } = default!;
		public DbSet<Onboarding> Onboardings { get; set; }
		public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<HR.Data.Models.Auth.User>(entity =>
			{
				// This is the line that captures the unique constraint
				entity.HasIndex(u => u.Email).IsUnique();
				entity.Property(u => u.IsActive).HasDefaultValue(true);
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

			// Ethnicities Table Configuration
			modelBuilder.Entity<Ethnicity>(entity =>
			{
				entity.ToTable("Ethnicities");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
			});

			// -----------------------------
			//Countries
			modelBuilder.Entity<Country>(entity =>
			{
				entity.ToTable("Countries");
				entity.HasKey(c => c.Id);
				entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
				entity.Property(c => c.IsoCode).IsRequired().HasMaxLength(5);
				entity.Property(c => c.DialCode).IsRequired().HasMaxLength(10);
			});

			// -----------------------------
			// Counties
			modelBuilder.Entity<County>(entity =>
			{
				entity.ToTable("Counties");
				entity.HasKey(c => c.Id);
				entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
				entity.Property(c => c.Code).IsRequired().HasMaxLength(10);
			});
			// -----------------------------
			// SubCounties
			// Define the One-to-Many Relationship
			modelBuilder.Entity<SubCounty>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

				// Link SubCounty to County
				entity.HasOne(d => d.County)
					  .WithMany(p => p.SubCounties)
					  .HasForeignKey(d => d.CountyId)
					  .OnDelete(DeleteBehavior.Restrict);
			});
			// Link Employee to Country
			modelBuilder.Entity<Employee>()
				.HasOne(e => e.Country)
				.WithMany()
				.HasForeignKey(e => e.CountryId)
				.OnDelete(DeleteBehavior.Restrict);
			// -----------------------------
			// Employees
			modelBuilder.Entity<Employee>(entity =>
			{
				entity.ToTable("Employees");
				entity.HasKey(e => e.Id);

				entity.Property(e => e.Status)
					  .IsRequired()
					  .HasColumnType("int");

				entity.Property(e => e.ContractType)
					  .IsRequired()
					  .HasColumnType("int")
					  .HasDefaultValue(ContractType.Permanent);

				entity.Property(e => e.ContractEndDate)
	                  .IsRequired(false); // Nullable because Permanent staff don't have an end date
				entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
				entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
				entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
				entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
				entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
				//entity.Property(e => e.Address).IsRequired().HasMaxLength(250);
				// Link Employee to County
				entity.HasOne(e => e.County)
					  .WithMany() // 
					  .HasForeignKey(e => e.CountyId)
					  .IsRequired(false); // 

				// Link Employee to SubCounty
				entity.HasOne(e => e.SubCounty)
					  .WithMany()
					  .HasForeignKey(e => e.SubCountyId)
					  .IsRequired(false); //

				entity.Property(e => e.Estate).IsRequired(false).HasMaxLength(250);
				entity.Property(e => e.POBox).IsRequired(false).HasMaxLength(50); 
				entity.Property(e => e.NationalID).HasMaxLength(50);
				entity.Property(e => e.Gender).HasMaxLength(20);
				entity.Property(e => e.JobTitle).HasMaxLength(100);
				entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
				entity.Property(e => e.Disability).HasMaxLength(100);



				// Department FK
				entity.HasOne(e => e.Department)
					  .WithMany(d => d.Employees)
					  .HasForeignKey(e => e.DepartmentId)
					  .OnDelete(DeleteBehavior.Restrict);
				// Ethnicirty FK
				entity.HasOne(e => e.Ethnicity)
			  .WithMany() // Ethnicity doesn't need a list of Employees
			  .HasForeignKey(e => e.EthnicityId)
			  .OnDelete(DeleteBehavior.Restrict);

				// PaymentData 1:1
				entity.HasOne(e => e.PaymentData)
					  .WithOne(p => p.Employee)
					  .HasForeignKey<PaymentData>(p => p.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// DISABILITYDETAILS
			modelBuilder.Entity<DisabilityDetail>(entity =>
			{
				entity.HasKey(d => d.Id);

				// Nature can be long since it's a manual description
				entity.Property(d => d.DisabilityNature)
					.HasMaxLength(500)
					.IsRequired();

				entity.Property(d => d.NCPWD_Number)
					.HasMaxLength(50)
					.IsRequired();

				entity.Property(d => d.KRA_ExemptionNumber)
					.HasMaxLength(50);

				// Paths can be long
				entity.Property(d => d.CertificateFilePath)
					.HasMaxLength(1000);

				// Configure the One-to-One relationship
				entity.HasOne(d => d.Employee)
					  .WithOne(e => e.DisabilityDetail)
					  .HasForeignKey<DisabilityDetail>(d => d.EmployeeId)
					  .OnDelete(DeleteBehavior.Cascade);
			});
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
			// 3. EmployeeStatusLog Configuration (The Vital Table)
			modelBuilder.Entity<EmployeeStatusLog>(entity =>
			{
				entity.ToTable("EmployeeStatusLogs");
				entity.HasKey(e => e.Id);

				// Explicitly match the ASCII setting found in your Employees table
				entity.Property(e => e.Id)
					  .HasColumnType("char(36)")
					  .HasCharSet("ascii")
					  .UseCollation("ascii_general_ci");

				entity.Property(e => e.EmployeeId)
					  .HasColumnType("char(36)")
					  .HasCharSet("ascii")
					  .UseCollation("ascii_general_ci");

				entity.Property(e => e.NewStatus).IsRequired();
				entity.Property(e => e.AuthorizedBy).IsRequired().HasMaxLength(150);
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
					  .WithMany(u => u.UserPermissions) // User can have many permissions, but we don't need a collection on the User model
					  .HasForeignKey(up => up.UserId)
					  .OnDelete(DeleteBehavior.Cascade);

				// Ensure no duplicate permission codes for the same user
				entity.HasIndex(up => new { up.UserId, up.PermissionCode }).IsUnique();
			});

			// -----------------------------
			// JobApplications Location Mapping
			modelBuilder.Entity<JobApplication>(entity =>
			{
				entity.ToTable("JobApplications");
				entity.HasKey(a => a.Id);

				// Link Application to Country
				entity.HasOne(a => a.Country)
					.WithMany()
					.HasForeignKey(a => a.CountryId)
					.OnDelete(DeleteBehavior.Restrict);

				// Link Application to County
				entity.HasOne(a => a.County)
					.WithMany()
					.HasForeignKey(a => a.CountyId)
					.OnDelete(DeleteBehavior.Restrict);

				// Link Application to SubCounty
				entity.HasOne(a => a.SubCounty)
					.WithMany()
					.HasForeignKey(a => a.SubCountyId)
					.OnDelete(DeleteBehavior.Restrict);

				// Character limits for the new fields
				entity.Property(a => a.Estate).HasMaxLength(250);
				entity.Property(a => a.POBox).HasMaxLength(50);
			});
			// -----------------------------
			// Applicant Education Mapping
			modelBuilder.Entity<ApplicantEducation>(entity =>
			{
				entity.ToTable("ApplicantEducations");
				entity.HasKey(e => e.Id);

				entity.Property(e => e.Institution).IsRequired().HasMaxLength(200);
				entity.Property(e => e.Field).HasMaxLength(150);

				// If an application is deleted, remove its education records automatically
				entity.HasOne(e => e.JobApplication)
					.WithMany(a => a.Education)
					.HasForeignKey(e => e.JobApplicationId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// -----------------------------
			// Applicant Experience Mapping
			modelBuilder.Entity<ApplicantExperience>(entity =>
			{
				entity.ToTable("ApplicantExperiences");
				entity.HasKey(e => e.Id);

				entity.Property(e => e.Company).IsRequired().HasMaxLength(200);
				entity.Property(e => e.JobTitle).HasMaxLength(150);

				// If an application is deleted, remove its experience records automatically
				entity.HasOne(e => e.JobApplication)
					.WithMany(a => a.Experience)
					.HasForeignKey(e => e.JobApplicationId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
