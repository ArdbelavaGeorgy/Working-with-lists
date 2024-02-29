using System;
using System.Collections.Generic;
using System.Globalization;

public class Company
{
    public string Name { get; set; }
    public List<Department> Departments { get; set; }

    public Company(string name)
    {
        Name = name;
        Departments = new List<Department>();
    }

    // Функция поиска отдела по любому атрибуту
    public Department FindDepartment(Func<Department, bool> criteria)
    {
        foreach (var department in Departments)
        {
            if (criteria(department))
            {
                return department;
            }
        }
        return null; // Возвращает null, если отдел не найден
    }

    // Функция поиска сотрудника по любому атрибуту
    public Employee FindEmployee(Func<Employee, bool> criteria)
    {
        foreach (var department in Departments)
        {
            foreach (var employee in department.Employees)
            {
                if (criteria(employee))
                {
                    return employee;
                }
            }
        }
        return null; // Возвращает null, если сотрудник не найден
    }
}

public class Department
{
    public string Name { get; set; }
    public List<Employee> Employees { get; set; }

    public Department(string name)
    {
        Name = name;
        Employees = new List<Employee>();
    }

    public int EmployeeCount
    {
        get { return Employees.Count; }
    }
}

public abstract class Employee
{
    public string FullName { get; set; }
    public string Position { get; set; }
    public decimal Salary { get; set; }

    protected Employee(string fullName, string position, decimal salary)
    {
        FullName = fullName;
        Position = position;
        Salary = salary;
    }

    public abstract decimal CalculateSalary();
}

public class StaffEmployee : Employee
{
    public decimal Bonus { get; set; }

    public StaffEmployee(string fullName, string position, decimal salary, decimal bonus)
    : base(fullName, position, salary)
    {
        Bonus = bonus;
    }

    public override decimal CalculateSalary()
    {
        return Salary + Bonus;
    }
}

public class ContractEmployee : Employee
{
    public ContractEmployee(string fullName, string position, decimal salary)
    : base(fullName, position, salary) { }

    public override decimal CalculateSalary()
    {
        return Salary;
    }
}

class Program
{
    static void Main()
    {
        var company = new Company("ООО Ромашка");
        FillCompanyWithDummyData(company);

        char repeat;
        do
        {
            Console.WriteLine("Хотите выполнить поиск в компании 'ООО Ромашка'? (y/n)");
            string searchChoice = Console.ReadLine()?.Trim().ToLower();

            if (searchChoice == "y")
            {
                Search(company);
            }
            else if (searchChoice == "n")
            {
                Console.WriteLine("До свидания!");
                break;
            }
            else
            {
                Console.WriteLine("Неправильный ввод. Попробуйте снова.");
            }

            Console.WriteLine("Хотите выполнить ещё один поиск? (y/n)");
            repeat = Console.ReadKey().KeyChar;
            Console.WriteLine();
        } while (repeat == 'y');
    }

    static void Search(Company company)
    {
        Console.WriteLine("Введите категорию для поиска (Company, Department, Employee):");
        string category = Console.ReadLine()?.Trim().ToLower() ?? "";
        string input;
        switch (category)
        {
            case "company":
                Console.WriteLine($"Компания: {company.Name}");
                // Вывод минимального количества экземпляров класса
                Console.WriteLine($"Минимальное количество отделов: {company.Departments.Count}");
                Console.WriteLine("Минимальное количество сотрудников в каждом отделе:");
                foreach (var dept in company.Departments)
                {
                    Console.WriteLine($"{dept.Name}: {dept.EmployeeCount}");
                }
                break;
            case "department":
                Console.WriteLine("Введите атрибут для поиска (Name, EmployeeCount):");
                input = Console.ReadLine()?.Trim().ToLower() ?? "";
                var department = company.FindDepartment(
                    d => d.Name.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                         d.EmployeeCount.ToString() == input
                );
                if (department != null)
                {
                    Console.WriteLine($"Отдел: {department.Name}, Количество сотрудников: {department.EmployeeCount}");
                }
                else
                {
                    Console.WriteLine("Отдел не найден.");
                }
                break;
            case "employee":
                Console.WriteLine("Введите атрибут для поиска (FullName, Position, Salary):");
                input = Console.ReadLine()?.Trim().ToLower() ?? "";
                decimal salary;
                var employee = company.FindEmployee(
                    e => e.FullName.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                         e.Position.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                         (decimal.TryParse(input, out salary) && e.Salary == salary)
                );
                if (employee != null)
                {
                    Console.WriteLine($"Сотрудник: {employee.FullName}, Должность: {employee.Position}, Зарплата: {employee.CalculateSalary():C2}");
                }
                else
                {
                    Console.WriteLine("Сотрудник не найден.");
                }
                break;
            default:
                Console.WriteLine("Неверная категория для поиска. Попробуйте снова.");
                break;
        }
    }

    static void SearchDepartment(Company company)
    {
        Console.WriteLine("Введите название отдела:");
        string deptName = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(deptName))
        {
            Console.WriteLine("Вы не ввели название отдела. Попробуйте снова.");
            return;
        }

        foreach (var department in company.Departments)
        {
            if (department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Отдел: {department.Name}, Количество сотрудников: {department.EmployeeCount}");
                return;
            }
        }
        Console.WriteLine("Отдел не найден. Убедитесь, что ввели название правильно.");
    }

    static void SearchEmployee(Company company)
    {
        Console.WriteLine("Введите ФИО сотрудника:");
        string empName = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(empName))
        {
            Console.WriteLine("Вы не ввели ФИО сотрудника. Попробуйте снова.");
            return;
        }

        bool found = false;
        foreach (var department in company.Departments)
        {
            foreach (var employee in department.Employees)
            {
                if (employee.FullName.Equals(empName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Сотрудник: {employee.FullName}, Должность: {employee.Position}, Зарплата: {employee.CalculateSalary():C2}");
                    found = true;
                }
            }
        }

        if (!found)
        {
            Console.WriteLine("Сотрудник не найден. Убедитесь, что ввели ФИО правильно.");
        }
    }

    // Функция для заполнения компании тестовыми данными
    static void FillCompanyWithDummyData(Company company)
    {
        // Создание отделов и добавление сотрудников
        var departments = new List<Department>
        {
            new Department("Отдел разработки") {
                Employees = {
                    new StaffEmployee("Иван Иванов", "Разработчик", 120000, 20000),
                    new StaffEmployee("Олег Сидоров", "Дизайнер", 95000, 15000),
                    new ContractEmployee("Елена Петрова", "Аналитик", 110000)
                }
            },
            new Department("Отдел продаж") {
                Employees = {
                    new StaffEmployee("Мария Васильева", "Менеджер по продажам", 80000, 12000),
                    new StaffEmployee("Анна Кузнецова", "Старший менеджер", 90000, 18000),
                    new ContractEmployee("Сергей Николаев", "Менеджер по работе с клиентами", 85000)
                }
            },
            new Department("Отдел маркетинга") {
                Employees = {
                    new StaffEmployee("Дарья Егорова", "Маркетолог", 105000, 17500),
                    new StaffEmployee("Ирина Алексеева", "PR-менеджер", 98000, 15000),
                    new ContractEmployee("Дмитрий Андреев", "Рекламист", 93000)
                }
            },
            new Department("Финансовый отдел") {
                Employees = {
                    new StaffEmployee("Александр Морозов", "Финансовый аналитик", 130000, 21000),
                    new StaffEmployee("Екатерина Соколова", "Бухгалтер", 91000, 14000),
                    new ContractEmployee("Михаил Михайлов", "Экономист", 88000)
                }
            },
            new Department("HR отдел") {
                Employees = {
                    new StaffEmployee("Ольга Горбунова", "HR-менеджер", 97000, 13000),
                    new StaffEmployee("Алексей Чернов", "Рекрутер", 83000, 11000),
                    new ContractEmployee("Татьяна Орехова", "HR-специалист", 87000)
                }
            },
            new Department("Отдел логистики") {
                Employees = {
                    new StaffEmployee("Егор Титов", "Логист", 99000, 15000),
                    new StaffEmployee("Денис Киселев", "Аналитик логистических систем", 92000, 16000),
                    new ContractEmployee("Вера Козлова", "Специалист по закупкам", 94000)
                }
            }
        };

        company.Departments.AddRange(departments);
    }
}
