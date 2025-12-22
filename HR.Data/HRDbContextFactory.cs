using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace HR.Data
{
	public class HRDbContextFactory : IDesignTimeDbContextFactory<HRDbContext>
	{
		public HRDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<HRDbContext>();

			// MySQL connection string
			optionsBuilder.UseMySql(
				"Server=localhost;Port=3306;Database=hrdb;User Id=root;Password=Mwenda2sana.;",
				new MySqlServerVersion(new Version(8, 0, 43))// MySQL version
			);

			return new HRDbContext(optionsBuilder.Options);
		}
	}
}
