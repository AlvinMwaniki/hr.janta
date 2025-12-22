using HR.Data;
using HR.Data.Models.Departments;

public static class SeedData
{
	public static void Initialize(HRDbContext context)
	{
		try
		{
			// ❌ DO NOT MIGRATE ON STARTUP — causes fatal crash
			// context.Database.Migrate();

			// ✔ Just ensure DB exists
			context.Database.EnsureCreated();
		}
		catch (Exception ex)
		{
			Console.WriteLine("🔥 SEED ERROR: " + ex);
			throw;
		}

		// If already seeded, stop
		if (context.Departments.Any())
			return;

		var departments = new Department[]
		{
			new Department { Name = "Human Resources", Description = "Handles employee relations, hiring and HR operations" },
			new Department { Name = "Finance", Description = "Manages company finances, payroll and budgets" },
			new Department { Name = "IT", Description = "Responsible for technology systems, software and hardware" },
			new Department { Name = "Marketing", Description = "Runs company branding, advertising and promotions" },
			new Department { Name = "Operations", Description = "Oversees daily operations and workflow management" }
		};

		context.Departments.AddRange(departments);
		context.SaveChanges();
	}
}
