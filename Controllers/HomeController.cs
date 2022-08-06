using Itask6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;
using System.Text;

namespace Itask6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Produces("application/json")]
        [HttpPost("initial")]
        [Route("api/generator/initial")]

        public async Task<IActionResult> Initial([FromBody] FakerOptions faker)
        {
            try
            {
                if(faker != null)
                {
                    return Ok(GenerateData(new FakerOptions(faker.length, faker.selectedCountry, faker.error, faker.seed)).ToList());
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [Produces("application/json")]
        [HttpPost("additional")]
        [Route("api/generator/additional")]
        public async Task<IActionResult> Additional([FromBody] FakerOptions faker)
        {
            try
            {
                return Ok(GenerateData(new FakerOptions(faker.length, faker.selectedCountry, faker.error, faker.seed)).ToList());  
            }
            catch
            {
                return BadRequest();
            }
        }

        public static List<User> GenerateData(FakerOptions options)
        {
            Faker<User> faker = FakerRules(options);

            var fakerNums = new Faker();
            Randomizer.Seed = new Random(options.seed + options.length);

            List<User> users = new List<User>();

            if (options != null)
            {
                if (options.length == 0)
                {
                    faker.UseSeed(options.seed + options.length);
                    for (int i = 0; i < 20; i++)
                    {
                        User user = new User();
                        user = RandomFormatUser(faker, fakerNums);
                        users.Add(ApplyErrors(user, options));
                    }
                }
                else if (options.length > 0)
                {
                    faker.UseSeed(options.seed + options.length);
                    for (int i = 0; i < 10; i++)
                    {
                        User user = new User();
                        user = RandomFormatUser(faker, fakerNums);
                        users.Add(ApplyErrors(user, options));
                    }
                }
            }
            return users;
        }

        private static Faker<User> FakerRules(FakerOptions options)
        {
            return new Faker<User>(options.selectedCountry).RuleSet("First", (set) =>
            {
                set.StrictMode(true);
                set.RuleFor(u => u.Id, f => f.Random.Number(10000));
                set.RuleFor(u => u.Name, f => f.Name.FirstName() + " " + f.Name.FirstName().OrNull(f, .2f) + " " + f.Name.LastName());
                set.RuleFor(u => u.Address, f => f.Address.ZipCode().OrNull(f, .1f) + " " + f.Address.City() + ", " + f.Address.StreetAddress() + ",  " + ReturnCountry(f.Locale));
                set.RuleFor(u => u.Phone, f => f.Phone.PhoneNumber(ReturnPhoneNumberFormat(f.Locale)));
            }).RuleSet("Second", (set) =>
            {
                set.StrictMode(true);
                set.RuleFor(u => u.Id, f => f.Random.Number(10000));
                set.RuleFor(u => u.Name, f => f.Name.FirstName() + " " + f.Name.FirstName().OrNull(f, .2f) + " " + f.Name.LastName());
                set.RuleFor(u => u.Address, f => f.Address.ZipCode().OrNull(f, .1f) + " " + f.Address.City() + ", " + f.Address.StreetName() + " " + f.Address.SecondaryAddress() + ", " + ReturnCountry(f.Locale));
                set.RuleFor(u => u.Phone, f => f.Phone.PhoneNumber(ReturnPhoneNumberFormat(f.Locale)));
            })
                            .StrictMode(true)
                            .RuleFor(u => u.Id, f => f.Random.Number(10000))
                            .RuleFor(u => u.Name, f => f.Name.FirstName() + " " + f.Name.FirstName().OrNull(f, .2f) + " " + f.Name.LastName())
                            .RuleFor(u => u.Address, f => ReturnCountry(f.Locale) + ", " + f.Address.ZipCode() + ", " + f.Address.City() + ", " + f.Address.StreetName() + ", " + f.Address.BuildingNumber())
                            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("###-###-###"));
        }

        private static User RandomFormatUser(Faker<User> faker, Faker fakerNums)
        {
            User user;
            switch (fakerNums.Random.Number(2))
            {
                case 0:
                    user = faker.Generate("default");
                    break;
                case 1:
                    user = faker.Generate("First");
                    break;
                case 2:
                    user = faker.Generate("Second");
                    break;
                default:
                    user = faker.Generate("default");
                    break;
            }

            return user;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private static User ApplyErrors(User user, FakerOptions options)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            string polishLetters = "ąćęłńóśźż";
            string frenchLetters = "éûéîç";
            bool isPolish = false;
            bool isFrench = false;

            var fakerNumber = new Faker();

            Randomizer.Seed = new Random(options.seed + options.length);

            if (options.selectedCountry == "fr")
            {
                isFrench = true;
            }
            else if (options.selectedCountry == "pl")
            {
                isPolish = true;
            }

            if (options.error > 0)
            {
                for (int i = 0; i < options.error; i++)
                {
                    int randomCategory = fakerNumber.Random.Number(3);
                    int randomError = fakerNumber.Random.Number(3);

                    switch (randomCategory)
                    {
                        case 0:
                            switch (randomError)
                            {
                                case 0:
                                    user.Name = DeleteError(user.Name, fakerNumber);
                                    break;
                                case 1:
                                    user.Name = AddRandomError(user.Name, isPolish, isFrench, false, alphabet, polishLetters, frenchLetters, fakerNumber);
                                    break;
                                case 3:
                                    user.Name = SwapError(user.Name, fakerNumber);
                                    break;
                            }
                            break;
                        case 1:
                            switch (randomError)
                            {
                                case 0:
                                    user.Phone = DeleteError(user.Phone, fakerNumber);
                                    break;
                                case 1:
                                    user.Phone = AddRandomError(user.Phone, isPolish, isFrench, true, alphabet, polishLetters, frenchLetters, fakerNumber);
                                    break;
                                case 3:
                                    user.Phone = SwapError(user.Phone, fakerNumber);
                                    break;
                            }
                            break;
                        case 2:
                            switch (randomError)
                            {
                                case 0:
                                    user.Address = DeleteError(user.Address, fakerNumber);
                                    break;
                                case 1:
                                    user.Address = AddRandomError(user.Address, isPolish, isFrench, false, alphabet, polishLetters, frenchLetters, fakerNumber);
                                    break;
                                case 3:
                                    user.Address = SwapError(user.Address, fakerNumber);
                                    break;
                            }
                            break;

                    }
                }
                return user;
            }
            else
            {
                return user;
            }
        }


        private static string ReturnCountry(string region)
        {
            string countryName = "";
            switch (region)
            {
                case "pl":
                    countryName = "Polska";
                    break;
                case "fr":
                    countryName = "France";
                    break;
                case "en_GB":
                    countryName = "United Kingdom";
                    break;
            }

            return countryName;
        }


        private static string ReturnPhoneNumberFormat(string region)
        {
            string format = "";
            switch (region)
            {
                case "pl":
                    format = "+48 ### ### ###";
                    break;
                case "fr":
                    format = "+33 ### ### ###";
                    break;
                case "en_GB":
                    format = "+44 ### ### ###";
                    break;
            }

            return format;
        }

    
        private static string DeleteError(string s, Faker f)
        {
            if (s.Length > 15)
            {
                int idx = f.Random.Number(0, s.Count() - 1);
                return s.Remove(idx, 1);
            }
            else
            {
                return s;
            }
        }


        private static string AddRandomError(string s, bool isPolish, bool isFrench, bool isNumber, string alphabet, string polishLetters, string frenchLetters, Faker f)
        {
            if(s.Length > 10 && s.Length < 20)
            {
                int idx = f.Random.Number(0, s.Length);
                if (isPolish && isNumber)
                {
                    alphabet = "012345679";
                }
                else if (isPolish && !isNumber)
                {
                    alphabet += polishLetters;
                }
                else if (isFrench && isNumber)
                {
                    alphabet = "012345679";
                }
                else if (isFrench)
                {
                    alphabet += frenchLetters;
                }
                else if (isNumber)
                {
                    alphabet = "012345679";
                }
                return AddRandomChar(s, idx, alphabet, f);
            }
            else
            {
                return s;
            }
         
        }


        private static string AddRandomChar(string s, int idx, string alphabet, Faker f)
        {
            if(s.Length < 20 )
            {
                int charToInsert = f.Random.Number(0, alphabet.Length - 1);
                return s.Insert(idx, alphabet.ToCharArray()[charToInsert].ToString());
            }
            else
            {
                return s;
            }
        }


        private static string SwapError(string s, Faker f)
        {
            int idx1 = f.Random.Number(0, s.Length-1);
            int idx2 = f.Random.Number(0, s.Length-1);
            char[] chars = s.ToCharArray();

            char tmp = chars[idx1];
            chars[idx1] = chars[idx2];
            chars[idx2] = tmp;

            StringBuilder sb = new StringBuilder();

            foreach (char ch in chars)
            {
                sb.Append(ch);
            }

            return sb.ToString();
        }

    }
}