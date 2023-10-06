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
            StudentModel context = new StudentModel();
            return context.Student.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            StudentModel context = new StudentModel();
            return context.Student.Where(p => p.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentModel context = new StudentModel();
            return context.Student.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }
        public Student FindById(int studentId)
        {
            StudentModel context = new StudentModel();
            return context.Student.FirstOrDefault(p => p.StudentID == studentId);
        }
        public void InsertUpdate(Student student)
        {
            StudentModel context = new StudentModel();
            context.Student.AddOrUpdate(student);
            context.SaveChanges();
        }
        public void UpdateAvatar(int studentId, string avatarFileName)
        {
            StudentModel context = new StudentModel();
            var student = context.Student.FirstOrDefault(p => p.StudentID == studentId);

            if (student != null)
            {
                student.Avatar = avatarFileName;
                context.SaveChanges();
            }
        }
        public void Delete(int studentId)
        {
            StudentModel context = new StudentModel();
            var studentToDelete = context.Student.FirstOrDefault(p => p.StudentID == studentId);

            if (studentToDelete != null)
            {
                context.Student.Remove(studentToDelete);
                context.SaveChanges();
            }
        }
    }
}
