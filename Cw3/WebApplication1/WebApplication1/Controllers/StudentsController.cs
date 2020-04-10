using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;
using Cw3.DAL;
using System.Data.SqlClient;

namespace Cw3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public static List<Student> _students;
        public static List<Student> _semestry;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
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
            //... add to database
            //... generating index number
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
            }
        }
    }
}