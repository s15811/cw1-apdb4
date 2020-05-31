using Cw3.DTOs.Requests;
using Cw3.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        public Enrollment EnrollStudent(EnrollStudentRequest request)
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
                    com.Transaction = tran;
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback(); 
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
                    }

                    dr.Close();

                    int IdEnrollment = 0;

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
                        tran.Rollback();
                    };

                    enrollment.IdEnrollment = IdEnrollment;

                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName,LastName,BirthDate,IdEnrollment) VALUES (@index2,@firstName,@lastName,@birthDate,@idEnrollment)";
                    com.Parameters.AddWithValue("index2", request.IndexNumber);
                    com.Parameters.AddWithValue("firstName", request.FirstName);
                    com.Parameters.AddWithValue("lastName", request.LastName);
                    com.Parameters.AddWithValue("birthDate", "1993-09-11");
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
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                }

            }

            return enrollment;

            }

        public Enrollment PromoteStudents(int semester, string studies)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s17470;Integrated Security=True"))
            using (var com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                int temp = 0;

                com.CommandText = "EXEC PROMOTESTUDENTS @STUDIES = @studies, @SEMESTER = @semester;";

                com.Parameters.AddWithValue("studies", studies);
                com.Parameters.AddWithValue("semester", semester);

                com.Transaction = tran;
                var dr = com.ExecuteReader();
                int idEnrollment;

                Enrollment enrollment = new Enrollment();

                if (!dr.Read())
                {
                    tran.Rollback();
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

                return enrollment;

            }
        }
        public Student GetStudent(string index)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s17470;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT * FROM STUDENT WHERE INDEXNUMBER = @index";
                com.Parameters.AddWithValue("index", index);
                var dr = com.ExecuteReader();

                if (!dr.Read())
                {
                    dr.Close();
                    return null;
                }

                return new Student { FirstName = dr["FirstName"].ToString(), LastName = dr["LastName"].ToString(), IndexNumber = dr["IndexNumber"].ToString() };

            }
        }

        public IEnumerable<Student> GetStudents()
        {
            return null;
        }
    }
}
