using StarkovTestApp;
using StarkovTestApp.Models;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;

FileInfo file = new FileInfo(@"./dbsettings.txt");
if (!file.Exists)
{
    using (StreamWriter sw = file.CreateText())
    {
        sw.WriteLine("Host=localhost;\nPort=5432;\nDatabase=*****;\nUsername=postgres;\nPassword=****");
    }
    Console.WriteLine("Введите конфигурационные данные базы данных в dbsettings.txt");
    return;
}
var app = new ConsoleApp();
app.Start();
public static class StringExtensions
{
    public static string Repeat(this string value, int count) => string.Concat(Enumerable.Repeat(value, count));
}
class ConsoleApp
{

    public void Start()
    {
        var db = new DataContext();
        if (!db.Database.CanConnect())
        {
            Console.ReadLine();
            return;
        }
        Console.WriteLine();
        Console.WriteLine("Выберите тип импорта или запросите информацию о состоянии базы данных:");
        Console.WriteLine("impdep - импорт подразделений");
        Console.WriteLine("impem - импорт сотрудников");
        Console.WriteLine("impjt - импорт должностей");
        Console.WriteLine("show [id подразделения] - вывод состояния базы данных [вывод информации о подразделении]");
        var input = Console.ReadLine();
        Console.WriteLine();
        var splitedInput = input.Split(" ");
        if (splitedInput.Length > 2)
        {
            Console.WriteLine("Неизвестная команда");
            Start();
            return;

        }
        switch (splitedInput[0])
        {
            case "impdep":
                ImpDep();
                break;
            case "impem":
                ImpEm();
                break;
            case "impjt":
                ImpJT();
                break;
            case "show":
                Show(splitedInput);
                break;
            default:
                {
                    Console.WriteLine("Неизвестная команда");
                    Start();
                    return;

                }               
        }
        Start();
    }
    public void ImpDep()
    {
        Console.WriteLine("Введите путь к файлу:");
        var path = Console.ReadLine();
        string text;
        var parser = new Parser();
        if (!File.Exists(path))
        {
            Console.WriteLine("По данному пути файл не найден");
            return;
        }
        if(Path.GetExtension(path) != ".tsv")
        {
            Console.WriteLine("Не верный формат файла");
            return;
        }
        using (StreamReader reader = new StreamReader(path))
        {
            text = reader.ReadToEnd();
        }
        var departmentsData = parser.GetTableData(new DepartmentParser(), text);
        if (departmentsData == null)
        {
            Console.WriteLine("Не верный формат файла");
            return;
        }
        var departmentsList = new InstanceList<Department>().GetList(new DepartmentBuilder(), departmentsData);
        new DBWriter<Department>().WriteData(new DepartmentsWriter(), departmentsList);
        new DBChangeCorrector().Correct();
        Show(new string[] { "show" });
    }
    public void ImpEm()
    {
        Console.WriteLine("Введите путь к файлу:");
        var path = Console.ReadLine();
        string text;
        var parser = new Parser();
        if (!File.Exists(path))
        {
            Console.WriteLine("По данному пути файл не найден");
            return;
        }
        if (Path.GetExtension(path) != ".tsv")
        {
            Console.WriteLine("Не верный формат файла");
            return;
        }
        using (StreamReader reader = new StreamReader(path))
        {
            text = reader.ReadToEnd();
        }
        var employeeData = parser.GetTableData(new EmployeeParser(), text);
        if (employeeData == null)
        {
            Console.WriteLine("Не верный формат файла");
            return;
        }
        var employeeList = new InstanceList<Employee>().GetList(new EmployeeBuilder(), employeeData);
        new DBWriter<Employee>().WriteData(new EmployeesWriter(), employeeList);
        new DBChangeCorrector().Correct();
        Show(new string[] { "show" });
    }
    public void ImpJT()
    {
        Console.WriteLine("Введите путь к файлу:");
        var path = Console.ReadLine();
        string text;
        var parser = new Parser();
        if (!File.Exists(path))
        {
            Console.WriteLine("По данному пути файл не найден");
            return;
        }
        if (Path.GetExtension(path) != ".tsv")
        {
            Console.WriteLine("Не верный формат файла");
            return;
        }
        using (StreamReader reader = new StreamReader(path))
        {
            text = reader.ReadToEnd();
        }
        var jobTitleData = parser.GetTableData(new JobTitleParser(), text);
        if (jobTitleData == null)
        {
            Console.WriteLine("Не верный формат файла");
            return;
        }
        new DBWriter<List<string>>().WriteData(new JobTitlesWriter(), jobTitleData);
        new DBChangeCorrector().Correct();
        Show(new string[] { "show" });
    }
    public void Show(string[] text)
    {
        var db = new DataContext();
        if(text.Length == 2)
        {
            int departmentID;
            if (!Int32.TryParse(text[1], out departmentID))
            {
                Console.WriteLine("ID должен быть числом!");
                Start();
                return;
            }
            departmentID = Int32.Parse(text[1]);
            ShowSingleDepartment(departmentID);
        }
        else
        {
            var departments = db.Departments;
            var parentDepartments = new List<Department>();
            var orphanDepartments = new List<Department>();
            foreach (var department in departments)
            {
                if(department.ParentID > 0)
                {
                    var parent = departments.Find(department.ParentID);
                    parent.SubDepartments.Add(department);
                }
                if (department.ParentID == 0)
                    parentDepartments.Add(department);
                if (department.ParentID == -1)
                {
                    orphanDepartments.Add(department);
                }
            }
            parentDepartments.Sort((x,y) => x.Name.CompareTo(y.Name));
            foreach(var depatment in parentDepartments)
            {
                ShowDepartmentBranch(depatment, 1);
            }
            if (orphanDepartments.Any())
            {
                Console.WriteLine("Подразделения с потеряными родителями (в базе данных отсутвует информация о родительком подразделении)");
                foreach (var department in orphanDepartments)
                    ShowDepartmentBranch(department, 1);
            }
            var jobless = db.Employees.Where(x => x.DepartmentID == 0);
            if(jobless.Any())
                foreach(var employee in jobless)
                    Console.WriteLine(" " + "- " + employee.FullName + " ID=" + employee.ID);
            Console.WriteLine("Список должностей:");
            foreach (var jobTitle in db.JobTitles)
                Console.WriteLine(jobTitle.Name + " ID=" + jobTitle.ID);
        }
    }
    public void ShowDepartmentBranch(Department department, int prefixCount)
    {
        var db = new DataContext();
        Console.WriteLine("=".Repeat(prefixCount) + " " + department.Name + " ID=" + department.ID);
        var manager = db.Employees.Find(department.ManagerID);
        if(manager != null)
            Console.WriteLine(" ".Repeat(prefixCount) + "* " + manager.FullName + " ID=" + manager.ID);
        var employeesList = db.Employees.Where(x => x.DepartmentID == department.ID).ToList();
        foreach (var employee in employeesList)
        {
            if (employee.ID == department.ManagerID)
                continue;
            Console.WriteLine(" ".Repeat(prefixCount) + "- " + employee.FullName + " ID=" + employee.ID);
        }
        department.SubDepartments.Sort((x, y) => x.Name.CompareTo(y.Name));
        foreach (var subDepartment in department.SubDepartments)
            ShowDepartmentBranch(subDepartment, prefixCount + 1);
    }
    public void ShowSingleDepartment(int departmentID)
    {
        var db = new DataContext();
        
        var department = db.Departments.Find(departmentID);
        if (department == null)
        {
            Console.WriteLine("Подразделение с данным ID не найдено");
            Start();
            return;
        }
        var departmentStack = new Stack<Department>();
        departmentStack.Push(department);
        while (department.ParentID != 0)
        {
            department = db.Departments.Find(department.ParentID);
            if (department == null)
            {
                Console.WriteLine("Недостаточно информации в базе данных для вывода родительских подразделений");
                break;
            }
            departmentStack.Push(department);
        }
        var prefixCount = 1;
        while (departmentStack.Count > 1)
        {
            department = departmentStack.Pop();
            Console.WriteLine("=".Repeat(prefixCount) + " " + department.Name + " ID=" + department.ID);
            prefixCount++;
        }
        department = departmentStack.Pop();
        Console.WriteLine("=".Repeat(prefixCount) + " " + department.Name + " ID=" + department.ID);
        var manager = db.Employees.Find(department.ManagerID);
        if (manager != null)
            Console.WriteLine(" ".Repeat(prefixCount) + "* " + manager.FullName + " ID=" + manager.ID);
        var employeesList = db.Employees.Where(x => x.DepartmentID == department.ID).ToList();
        foreach (var emplouee in employeesList)
        {
            if (emplouee.ID == department.ManagerID)
                continue;
            Console.WriteLine(" ".Repeat(prefixCount) + "- " + emplouee.FullName + " ID=" + emplouee.ID);
        }       
    }

}



