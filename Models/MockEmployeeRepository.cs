using System;

namespace EmployeeManagment.Models
{
	public class MockEmployeeRepository : IEmployeeRepository
	{
        private List<Employee> _employeeList;

		public MockEmployeeRepository()
		{
            _employeeList = new List<Employee>()
            {
                new Employee(){Id = 1, Name = "Mary", Department=Dept.HR, Email="Mary@gmail.com"},
                new Employee(){Id = 2, Name = "Thomas", Department=Dept.IT, Email="Thomas@gmail.com"},
                new Employee(){Id = 3, Name = "Zabuza", Department=Dept.Akatsuki, Email="Ligmaballs@gmail.com"}
            };
		}

        public Employee Add(Employee employee)
        {
            employee.Id =_employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == id);
            if(employee != null)
            {
                _employeeList.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployes()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(n => n.Id == Id);
        }

        public Employee Update(Employee employeeChanges)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == employeeChanges.Id);
            if(employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            return employee;
        }
    }
}

