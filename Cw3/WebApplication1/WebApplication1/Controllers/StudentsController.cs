using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;
using Cw3.DAL;
using System.Data.SqlClient;
using Cw3.DTOs.Requests;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Cw3.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;
        public static List<Student> _students;
        public static List<Student> _semestry;

        public IConfiguration Configuration { get; set; }

        public StudentsController(IStudentsDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetStudents(string index)
        {
            string indexNumber = index;

            using (var client = new SqlConnection("Data Source = db-mssql; Initial Catalog = s15811; Integrated Security = True"))
            using (var con = new SqlCommand())
            {
                con.Connection = client;
                con.CommandText = "Select * FROM Student WHERE IndexNumber = @indexNumber";
                con.Parameters.AddWithValue("indexNumber", indexNumber);

                client.Open();
                var dr = con.ExecuteReader();
                int id = 1;
                _students = new List<Student>();

                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.BirthDate = (DateTime)dr["BirthDate"];
                    st.IdEnrollment = (int)dr["IdEnrollment"];
                    st.IdStudent = id;

                    _students.Add(st);
                    id++;
                }
                return Ok(_dbService.GetStudents());
            }


        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            int idStudenta = id;
            listaStudentow();

            string index = null;
            for (int i = 0; i < _students.Count; i++)
            {
                if (_students[i].IdStudent.Equals(idStudenta))
                {
                    index = _students[i].IndexNumber;
                    break;
                }
            }

            if (index != null)
            {
                using (var client = new SqlConnection("Data Source = db-mssql; Initial Catalog = s15811; Integrated Security = True"))
                using (var con = new SqlCommand())
                {
                    con.Connection = client;
                    con.CommandText = "Select * FROM Student, Enrollment WHERE Student.IdEnrollment=Enrollment.IdEnrollment AND Student.IndexNumber=@index";
                    con.Parameters.AddWithValue("index", index);

                    client.Open();
                    var dr = con.ExecuteReader();

                    List<String> _semestry = new List<String>();

                    while (dr.Read())
                    {
                        var st = "IdEnrollment ";
                        st += dr["IdEnrollment"].ToString();
                        st += " Semester ";
                        st += dr["Semester"].ToString();

                        _semestry.Add(st);
                    }
                    client.Close();
                    return Ok(_semestry);
                }
            }
            else
            {
                return Ok("Nie znaleziono studenta");
            }
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut]
        public IActionResult UpdateStudent(Student student)
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete]
        public IActionResult DeleteStudent(Student student)
        {
            return Ok("Usuwanie ukończone");
        }



        public void listaStudentow()
        {
            using (var client = new SqlConnection("Data Source = db-mssql; Initial Catalog = s15811; Integrated Security = True"))
            using (var con = new SqlCommand())
            {
                con.Connection = client;
                con.CommandText = "Select * FROM Student";

                client.Open();
                var dr = con.ExecuteReader();
                int id = 1;
                _students = new List<Student>();

                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.BirthDate = (DateTime)dr["BirthDate"];
                    st.IdEnrollment = (int)dr["IdEnrollment"];
                    st.IdStudent = id;

                    _students.Add(st);
                    id++;
                }
                client.Close();
            }
        }

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            var response = _dbService.LoginStudentResponse(request);
            if (Validate(request.Haslo, response.Salt, response.Password))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, request.Index),
                    new Claim(ClaimTypes.Name, request.Index),
                    new Claim(ClaimTypes.Role, "student")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );


                var tokenData = (new
                {
                    accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    refreshToken = Guid.NewGuid()
                });

                var refreshToken = new SaveRefreshTokenRequest();
                refreshToken.indexNumber = request.Index;
                refreshToken.refreshToken = tokenData.refreshToken.ToString();

                var saveRefreshTokenResponse = _dbService.SaveRefreshToken(refreshToken);

                return Ok("Poprawnie zalogowano");
            }
            else
            {
                return Ok("Błąd logowania");
            }
        }

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(RefreshTokenRequest refToken)
        {
            var response = _dbService.RefreshToken(refToken);
            if (null == response.IndexNumber)
            {
                return Ok(response.Message);
            }

            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, response.IndexNumber),
                    new Claim(ClaimTypes.Name, response.IndexNumber),
                    new Claim(ClaimTypes.Role, "student")
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );


            var tokenData = (new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });

            var newToken = new SaveRefreshTokenRequest();
            newToken.indexNumber = response.IndexNumber;
            newToken.refreshToken = tokenData.refreshToken.ToString();

            var saveRefreshTokenResponse = _dbService.SaveRefreshToken(newToken);


            return Ok(response.Message + "\n" + "Nowy Refresh Token: " + newToken.refreshToken.ToString());
        }

        public static bool Validate(string value, string salt, string hash)
        {
            return Create(value, salt) == hash;
        }

        public static string Create(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }
    }
}