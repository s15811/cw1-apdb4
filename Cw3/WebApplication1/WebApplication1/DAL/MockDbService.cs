using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Controllers;
using Cw3.Models;

namespace Cw3.DAL
{
    public class MockDbService : IDbService
    {
        
        
        static MockDbService()
        {
            
        }
        
        public List<Student> GetStudents()
        {
            return StudentsController._students;
        }
    }
}
