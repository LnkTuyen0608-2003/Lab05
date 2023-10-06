using Lab05.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab05.BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            Model1 context = new Model1();
            return context.Students.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            Model1 context = new Model1();
            return context.Students.Where(p => p.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            Model1 context = new Model1();
            return context.Students.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }
        public Student FindById(int studentId)
        {
            Model1 context = new Model1();
            return context.Students.FirstOrDefault(p => p.StudentID == studentId.ToString());
        }
        public void InsertUpdate(Student student)
        {
            Model1 context = new Model1();
            context.Students.AddOrUpdate(student);
            context.SaveChanges();
        }
        public void UpdateAvatar(int studentId, string avatarFileName)
        {
            Model1 context = new Model1();
            var student = context.Students.FirstOrDefault(p => p.StudentID == studentId.ToString());

            if (student != null)
            {
                student.Avatar = avatarFileName;
                context.SaveChanges();
            }
        }
        public void Delete(int studentId)
        {
            Model1 context = new Model1();
            var studentToDelete = context.Students.FirstOrDefault(p => p.StudentID == studentId.ToString()  );

            if (studentToDelete != null)
            {
                context.Students.Remove(studentToDelete);
                context.SaveChanges();
            }
        }
    }
}
