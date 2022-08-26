using System;
namespace EmployeeManagment.Models
{
	public interface IEmployeeRepository
	{
		Employee GetEmployee(int id);

		IEnumerable<Employee> GetAllEmployes();

		Employee Add(Employee employee);

		Employee Update(Employee employee);

		Employee Delete(int id);
	}
}

