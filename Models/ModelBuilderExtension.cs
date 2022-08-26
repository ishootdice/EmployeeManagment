using System;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagment.Models
{
	public static class ModelBuilderExtension
	{
		public static void Seed(this ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Employee>().HasData(
					new Employee
					{
						Id = 2,
						Name = "Mark",
						Department = Dept.IT,
						Email = "Mark@gmail.com",
						PhotoPath = "noimage.jpg"
					}
                );
		}
	}
}

