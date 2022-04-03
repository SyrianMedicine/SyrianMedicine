using System.Text.Json;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;

namespace Services.Seed.GenerateData
{
    public class GenerateHospitals
    {

        private static string[] departmentsName = new string[]{
            "Urology",
            "Sexual Health",
            "Rheumatology",
            "Renal",
            "Radiotherapy",
            "Radiology",
            "Physiotherapy",
            "Otolaryngology",
            "Orthopaedics",
            "Ophthalmology",
            "Oncology",
            "Obstetrics/Gynecology",
            "Nutrition and Dietetics",
            "Neurology",
            "Nephrology",
            "Neonatal",
            "Microbiology",
            "Maternity",
            "Diagnostic Imaging",
            "Elderly Services"
        };



        private static string citiesData = File.ReadAllText(@"../Services/Services/Seed/Data/cities.json");
        private static List<City> cities = JsonSerializer.Deserialize<List<City>>(citiesData);

        private static List<User> AddUsers()
        {
            List<User> users = new();

            for (int i = 0; i < cities.Count; i++)
            {

                string firstName = Faker.Name.First();
                User user = new()
                {
                    Email = "Hospital" + firstName + i.ToString() + "@gmail.com",
                    UserName = "Hospital" + firstName + i.ToString(),
                    HomeNumber = Faker.Phone.Number(),
                    Location = Faker.Address.StreetName(),
                    FirstName = cities[i % cities.Count].Name + " hopital",
                    EmailConfirmed = true,
                    UserType = UserType.Hospital,
                    City = cities[i % cities.Count].Name,
                };

                users.Add(user);
            }
            return users;
        }

        public static (List<User>, List<Hospital>) AddHospitals()
        {
            List<User> users = AddUsers();
            List<Hospital> hospitals = new();

            int cnt = 0;
            foreach (var user in users)
            {
                Hospital hospital = new()
                {
                    AboutHospital = Faker.Lorem.Sentence(),
                    AccountState = AccountState.Approved,
                    Name = cities[cnt % cities.Count].Name + " hopital",
                    WebSite = "www." + cities[cnt % cities.Count].Name.Trim() + "hopital" + ".com",
                    UserId = user.Id,
                };
                hospitals.Add(hospital);
                cnt++;
            }

            return (users, hospitals);
        }

        public static (List<Department>, List<HospitalDepartment>) AddDepartments(List<Hospital> hospitals)
        {
            List<Department> departments = new();
            List<HospitalDepartment> hospitalsdepartments = new();

            foreach (var department in departmentsName)
            {
                Department dep = new()
                {
                    Name = department,
                };
                foreach (var hospital in hospitals)
                {
                    HospitalDepartment hp = new();
                    hp.Department = dep;
                    hp.Hospital = hospital;
                    hospitalsdepartments.Add(hp);
                }
                departments.Add(dep);
            }
            return (departments, hospitalsdepartments);
        }

        public static List<Bed> AddBeds(List<Department> departments, List<Hospital> hospitals)
        {
            List<Bed> beds = new();
            foreach (var hospital in hospitals)
            {
                foreach (var department in departments)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Bed bed = new()
                        {
                            DepartmentId = department.Id,
                            IsAvailable = i % 2 == 0,
                            HospitalId = hospital.Id
                        };
                        beds.Add(bed);
                    }
                }
            }
            return beds;
        }


    }
}