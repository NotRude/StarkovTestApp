using StarkovTestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkovTestApp
{
    class DBWriter<T>
    {
        public void WriteData(IDBWriter<T> dbWriter,List<T> data)
        {
            dbWriter.Write(ref data);
        }
    }
    interface IDBWriter<T>
    {
        void Write(ref List<T> data);
    }
    class DepartmentsWriter : IDBWriter<Department>
    {
        public void Write(ref List<Department> data)
        {
            var db = new DataContext();
            var departmentsTable = db.Departments;
            foreach (var department in data)
            {
                var departmentPastData = departmentsTable.Where(x => x.Name == department.Name).FirstOrDefault();
                if (departmentPastData == null)
                {
                    departmentsTable.Add(department);
                }
                else
                {
                    departmentPastData.ManagerName = department.ManagerName;
                    db.Update(departmentPastData);
                }
            }
            db.SaveChanges();
        }
    }
    class EmployeesWriter : IDBWriter<Employee>
    {
        public void Write(ref List<Employee> data)
        {
            var db = new DataContext();
            var employeesTable = db.Employees;
            foreach (var employee in data)
            {
                var employeePastData= employeesTable.Where(x => x.FullName == employee.FullName).FirstOrDefault();
                if(employeePastData == null)
                {
                    employeesTable.Add(employee);
                }
                else
                {
                    employeePastData.DepartmentName = employee.DepartmentName;
                    employeePastData.JobTittleName = employee.JobTittleName;
                    employeePastData.Login = employee.Login;
                    employeePastData.Password = employee.Password;
                    db.Update(employeePastData);
                }
            }
            db.SaveChanges();
        }
    }
    class JobTitlesWriter : IDBWriter<List<string>>
    {
        public void Write(ref List<List<string>> data)
        {
            var db = new DataContext();
            var jtTable = db.JobTitles;
            data.RemoveAt(0);
            foreach(var row in data)
            {
                var jobTitle = new JobTitle();
                jobTitle.Name = row[0];
                if (jtTable.Any(j => (j.Name == jobTitle.Name)))
                    continue;
                jtTable.Add(jobTitle);
            }
            db.SaveChanges();
        }
    }  
    class DBChangeCorrector
    {
        public void Correct()
        {
            CorrectDepartment();
            CorrectEmplouee();
            //var db = new DataContext();
            //var jobTitleList = db.JobTitles.ToList();
            //var departmentList = db.Departments.ToList();
            //foreach (var employee in db.Employees)
            //{
            //    var jobTitle = jobTitleList.Where(x => x.Name == employee.JobTittleName).FirstOrDefault();
            //    if(employee.JobTittleID == 0)
            //    {                 
            //        if(jobTitle != null)
            //            employee.JobTittleID = jobTitle.ID;
            //    }
            //    else
            //    {
            //        if (jobTitle != null)
            //        {
            //            if (jobTitle.ID != employee.JobTittleID)
            //            {
            //                employee.JobTittleID = jobTitle.ID;
            //                employee.JobTittleName = jobTitle.Name;
            //            }
            //        }
            //        else
            //        {
            //            employee.JobTittleID = 0;
            //        }
            //    }
            //    var department = departmentList.Where(x => x.Name == employee.DepartmentName).FirstOrDefault();
            //    if (employee.DepartmentID == 0)
            //    {
            //        if(department != null)
            //            employee.DepartmentID = department.ID;
            //    }
            //    else
            //    {
            //        if (department != null)
            //        {
            //            if (department.ID != employee.DepartmentID)
            //            {
            //                employee.DepartmentID = department.ID;
            //                employee.DepartmentName = department.Name;
            //            }
            //        }
            //        else
            //        {
            //            employee.DepartmentID = 0;
            //        }
            //    }
            //    db.Update(employee);
            //}
            //foreach(var department in departmentList)
            //{
            //    if (department.ParentName != "")
            //    {
            //        var parent = departmentList.Where(x => x.Name == department.ParentName).FirstOrDefault();
            //        if (parent != null)
            //        {
            //            department.ParentID = parent.ID;
            //        }
            //        else
            //        {
            //            department.ParentID = -1;
            //        }
            //    }
            //    else
            //        department.ParentID = 0;
            //    var manager = db.Employees.Where(x => x.FullName == department.ManagerName).FirstOrDefault();
            //    if (department.ManagerID == 0)
            //    {
            //        if (manager != null)
            //            department.ManagerID = manager.ID;
            //    }
            //    else
            //    {
            //        if (manager != null)
            //        {
            //            if (manager.ID != department.ManagerID)
            //            {
            //                department.ManagerID = manager.ID;
            //                department.ParentName = manager.FullName;
            //            }
            //        }
            //        else
            //        {
            //            department.ManagerID = 0;
            //        }
            //    }
            //    db.Update(department);
            //}
            //db.SaveChanges();
        }
        private void CorrectDepartment()
        {
            var db = new DataContext();
            var departmentList = db.Departments.ToList();
            foreach (var department in departmentList)
            {
                if (department.ParentName != "")
                {
                    var parent = departmentList.Where(x => x.Name == department.ParentName).FirstOrDefault();
                    if (parent != null)
                    {
                        department.ParentID = parent.ID;
                    }
                    else
                    {
                        department.ParentID = -1;
                    }
                }
                else
                    department.ParentID = 0;
                var manager = db.Employees.Where(x => x.FullName == department.ManagerName).FirstOrDefault();
                if (department.ManagerID == 0)
                {
                    if (manager != null)
                        department.ManagerID = manager.ID;
                }
                else
                {
                    if (manager != null)
                    {
                        if (manager.ID != department.ManagerID)
                        {
                            department.ManagerID = manager.ID;
                            department.ParentName = manager.FullName;
                        }
                    }
                    else
                    {
                        department.ManagerID = 0;
                    }
                }
                db.Update(department);
            }
            db.SaveChanges();
        }
        private void CorrectEmplouee()
        {
            var db = new DataContext();
            var jobTitleList = db.JobTitles.ToList();
            var departmentList = db.Departments.ToList();
            foreach (var employee in db.Employees)
            {
                var jobTitle = jobTitleList.Where(x => x.Name == employee.JobTittleName).FirstOrDefault();
                if (employee.JobTittleID == 0)
                {
                    if (jobTitle != null)
                        employee.JobTittleID = jobTitle.ID;
                }
                else
                {
                    if (jobTitle != null)
                    {
                        if (jobTitle.ID != employee.JobTittleID)
                        {
                            employee.JobTittleID = jobTitle.ID;
                            employee.JobTittleName = jobTitle.Name;
                        }
                    }
                    else
                    {
                        employee.JobTittleID = 0;
                    }
                }
                var department = departmentList.Where(x => x.Name == employee.DepartmentName).FirstOrDefault();
                if (employee.DepartmentID == 0)
                {
                    if (department != null)
                        employee.DepartmentID = department.ID;
                }
                else
                {
                    if (department != null)
                    {
                        if (department.ID != employee.DepartmentID)
                        {
                            employee.DepartmentID = department.ID;
                            employee.DepartmentName = department.Name;
                        }
                    }
                    else
                    {
                        employee.DepartmentID = 0;
                    }
                }
                db.Update(employee);
            }
            db.SaveChanges();
        }
    }

}
