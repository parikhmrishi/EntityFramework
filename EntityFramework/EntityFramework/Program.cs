using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

namespace EntityFramework
{
    class Program
    {
        int insert_success = 0;

        static void Main(string[] args)
            {
                try
                {
                    Console.WriteLine("Select an option");
                Console.WriteLine("0. Insert into database");
                Console.WriteLine("1. Filter on location");
                    Console.WriteLine("2. Filter on Age");
                    Console.WriteLine("3. Filter on Date");

                    int option = 0;
                    option = int.Parse(Console.ReadLine());
                    select_option(option);

                    void select_option(int op)
                    {
                        switch (op)
                        {
                            case 0: insert_data();
                                break;

                            case 1:
                                String location = " ";
                                Console.WriteLine("Enter the location");
                                location = Console.ReadLine();

                                if (Regex.IsMatch(location, @"^[a-zA-Z]+$"))
                                {
                                    Filter_location(location);
                                }
                                else
                                {
                                    Console.WriteLine("Enter valid location");
                                    select_option(1);
                                }
                                break;

                            case 2:
                                string age = "";
                                Console.WriteLine("Enter the age");
                                age = Console.ReadLine();

                                if (Regex.IsMatch(age, @"^[0-9]+$"))
                                {
                                    Filter_age(int.Parse(age));
                                }
                                else
                                {
                                    Console.WriteLine("Enter valid age");
                                    select_option(2);
                                }
                                break;



                            case 3:
                                Console.WriteLine("Enter a date (mm/dd/yyyy): ");
                                DateTime userDateTime;
                                if (DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out userDateTime))
                                {
                                    // String date = userDateTime.ToShortDateString().ToString().Replace('-', '/');
                                    Filter_date(userDateTime);
                                }
                                else
                                {
                                    Console.WriteLine("You have entered an incorrect value.");
                                    select_option(3);
                                }
                                break;

                            default:
                                Console.WriteLine("Select a valid option");
                                option = int.Parse(Console.ReadLine());
                                select_option(option);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.ReadKey();

                }
            }

        static void insert_data()
        {
            try
            {
                //Console.WriteLine(id + "\t" + name + "\t" + DOB + "\t" + location + "\t\t\t" + designation + "\t\t\t" + joining);
                var data = File.ReadAllLines(@"..\..\..\records.csv").ToList();

                var records = from record in data.Skip(1)
                              let rec = record.Split(',')
                              select new
                              {
                                  ID = rec[0],
                                  Name = rec[1] + rec[2] + " " + rec[3] + " " + rec[4],
                                  DOB = rec[10],
                                  Location = rec[30],
                                  Designation = rec[34],
                                  Joining = rec[14]
                              };

                foreach (var record in records)
                {
                    DateTime.TryParseExact(record.DOB, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date);

                    using (Employee_DetailsEntities context = new Employee_DetailsEntities())
                    {
                        employee_details emp = new employee_details
                        {
                            emp_id = int.Parse(record.ID),
                            name = record.Name,
                            DOB = record.DOB,
                            age = DateTime.Now.Year - date.Year,
                            location = record.Location,
                            date_of_joining = record.Joining
                        };
                        context.employee_details.Add(emp);

                        context.SaveChanges();
                    }
                }
            }
            catch(DbUpdateException)
            {
                Console.WriteLine("Data already exists");
            }
        }

        static void Filter_location(string loc)
        {
            try
            {
                IEnumerable<employee_details> filtered_data;
                Employee_DetailsEntities context = new Employee_DetailsEntities();
                
                filtered_data = from emp in context.employee_details where emp.location.ToLower().Contains(loc.ToLower()) select emp;

                foreach (var item in filtered_data)
                {
                    Console.WriteLine(item.emp_id + "\t" + item.name + "\t" + item.DOB + "\t" + item.location + "\t\t\t" + item.date_of_joining);
                }
                Console.Read();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        static void Filter_age(int age)
        {
            try
            {
                IEnumerable<employee_details> filtered_data;
                
                Employee_DetailsEntities context = new Employee_DetailsEntities();

                filtered_data = from emp in context.employee_details where emp.age >= age select emp;

                foreach (var item in filtered_data)
                {
                    Console.WriteLine(item.emp_id + "\t" + item.name + "\t" + item.DOB + "\t" + item.location + "\t\t\t" + item.date_of_joining);
                }
                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        static void Filter_date(DateTime date)
        {
            try
            {
                IEnumerable<employee_details> filtered_data;
                DateTime doj;
                Employee_DetailsEntities context = new Employee_DetailsEntities();

                filtered_data = from emp in context.employee_details where DateTime.ParseExact(emp.date_of_joining, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None) >= date select emp;
                foreach (var item in filtered_data)
                {
                    Console.WriteLine(item.emp_id + "\t" + item.name + "\t" + item.DOB + "\t" + item.location + "\t\t\t" + item.date_of_joining);
                }
                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
}
