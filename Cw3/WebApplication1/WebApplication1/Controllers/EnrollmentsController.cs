using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Promotion;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        /*
        [HttpPost]
        public IActionResult EnrollmentsStudent(EnrollStudentRequest request)
        {
            var st = new Student();
            st.LastName = request.LastName;
            st.FirstName = request.FirstName;
            st.BirthDate = request.BirthDate;
            st.Studies = request.Studies;
            st.Semester = 1;
            var enrollment = new Enrollment();
            enrollment.Semester = 1;
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s17470;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Studia nie istnieja.");
                    }
                    int IdStudy = (int)dr["IdStudy"];

                    enrollment.IdStudy = IdStudy;
                    dr.Close();
                    com.CommandText = "SELECT INDEXNUMBER FROM STUDENT WHERE INDEXNUMBER = @Index";
                    com.Parameters.AddWithValue("Index", request.IndexNumber);
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("W bazie istnieje juz ten number indeksu.");
                    }
                    dr.Close();
                    int IdEnrollment;
                    com.CommandText = "Select IdEnrollment FROM Enrollment WHERE Semester = 1 AND IdStudy =" + IdStudy;
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        IdEnrollment = ((int)dr["IdEnrollment"]);
                        dr.Close();
                    }
                    else if (!dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "Select IdEnrollment FROM Enrollment WHERE IdEnrollment = (Select MAX(IdEnrollment) FROM Enrollment)";
                        dr = com.ExecuteReader();
                        dr.Read();
                        IdEnrollment = ((int)dr["IdEnrollment"]) + 1;
                        dr.Close();
                        com.CommandText = "INSERT INTO ENROLLMENT (IDENROLLMENT,SEMESTER,IDSTUDY,STARTDATE) VALUES (" + IdEnrollment + ",1," + IdStudy + ", '2021-09-10')";
                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        return BadRequest("ERROR!");
                    };
                    enrollment.IdEnrollment = IdEnrollment;
                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName,LastName,BirthDate,IdEnrollment) VALUES (@index2,@firstName,@lastName,@birthDate,@idEnrollment)";
                    com.Parameters.AddWithValue("index2", request.IndexNumber);
                    com.Parameters.AddWithValue("firstName", request.FirstName);
                    com.Parameters.AddWithValue("lastName", request.LastName);
                    com.Parameters.AddWithValue("birthDate", "1996-05-20");
                    com.Parameters.AddWithValue("idEnrollment", IdEnrollment);
                    com.ExecuteNonQuery();
                    tran.Commit();
                    dr.Close();
                    com.CommandText = "Select * From Enrollment WHERE IdEnrollment = " + IdEnrollment;
                    dr = com.ExecuteReader();
                    string message = "";
                    while (dr.Read())
                    {
                        message = string.Concat(message, '\n', "Enrollment ID:", enrollment.IdEnrollment.ToString(), ", Semester: ", enrollment.Semester.ToString(), ", ID Studies: :", enrollment.IdStudy.ToString());
                    }
                    return Ok(enrollment);
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    return BadRequest(exc.ToString());
                }
            }
        }

        [HttpPost]
        public IActionResult PromotionStudents(PromoteStudents request)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s17470;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                int temp = 0;
                com.CommandText = "EXEC PROMOTESTUDENTS @STUDIES = @studies, @SEMESTER = @semester;";
                com.Parameters.AddWithValue("studies", request.Studies);
                com.Parameters.AddWithValue("semester", request.Semester);
                com.Transaction = tran;
                var dr = com.ExecuteReader();
                int idEnrollment;
                Enrollment enrollment = new Enrollment();
                if (!dr.Read())
                {
                    return BadRequest("Wrong request.");
                }
                else
                {
                    enrollment.IdEnrollment = (int)dr["IdEnrollment"];
                    enrollment.IdStudy = (int)dr["IdStudy"];
                    enrollment.Semester = (int)dr["Semester"];
                    idEnrollment = (int)dr[0];
                }
                dr.Close();
                tran.Commit();
                return Ok(enrollment);
            }
        }
        */
        private IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return Ok(_service.EnrollStudent(request));
        }
    }
}