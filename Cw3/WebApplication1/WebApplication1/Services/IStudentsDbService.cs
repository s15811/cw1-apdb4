using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentsDbService
    {
        Enrollment EnrollStudent(EnrollStudentRequest request);
        Enrollment PromoteStudents(int semester, string studies);
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string index);
        bool CheckIndex(string indexNumber);
        LoginResponse LoginStudentResponse(LoginRequest request);
        SaveRefreshTokenResponse SaveRefreshToken(SaveRefreshTokenRequest request);
        RefreshTokenResponse RefreshToken(RefreshTokenRequest request);
    }
}
