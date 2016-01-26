using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ColdCaller.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string StudentClass { get; set; }
        public string TeacherId { get; set; }
    }
}
