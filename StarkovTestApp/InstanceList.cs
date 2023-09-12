using StarkovTestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StarkovTestApp
{
    class InstanceList<T>
    {
        public List<T> GetList(IInstanceBuilder<T> builder, List<List<string>> data)
        {
            return builder.GetInstanceList(data);
        }
    }
    interface IInstanceBuilder<T>
    {
        List<T> GetInstanceList(List<List<string>> data);
    }
    class DepartmentBuilder : IInstanceBuilder<Department>
    {
        public List<Department> GetInstanceList(List<List<string>> data)
        {
            data.RemoveAt(0);
            var result = new List<Department>();
            foreach (var row in data)
            {
                var department = new Department();
                department.Name = row[0];
                department.ParentName = row[1];
                department.ManagerName = row[2];
                department.Phone = row[3];
                result.Add(department);
            }
            return result;
        }
    }
    class EmployeeBuilder : IInstanceBuilder<Employee>
    {
        public List<Employee> GetInstanceList(List<List<string>> data)
        {
            var db = new DataContext();
            data.RemoveAt(0);
            var result = new List<Employee>();
            foreach (var row in data)
            {
                var employee = new Employee();
                employee.DepartmentName = row[0];
                employee.FullName = row[1];
                employee.Login = row[2];
                employee.Password = row[3];
                employee.JobTittleName = row[4];
                result.Add(employee);
            }
            return result;
        }
    }
}
