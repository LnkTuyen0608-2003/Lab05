using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab05.BUS;
using Lab05.DAL.Entities;

namespace Lab05.GUI
{
    public partial class frmRegister : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        public frmRegister()
        {
            InitializeComponent();
        }

        private void frmRegister_Load(object sender, EventArgs e)
        {
            try
            {
                var listFacultys = facultyService.GetAll();
                FillFalcultyCombobox(listFacultys);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);
                var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                BindGrid(listStudents);
            }
        }
        private void FillMajorCombobox(List<Major> listMajors)
        {
            this.cmbMajor.DataSource = listMajors;
            this.cmbMajor.DisplayMember = "Name"; 
            this.cmbMajor.ValueMember = "MajorID"; 
        }

        private void BindGrid(List<Student> listStuent)
        {
            dgvStudent.Rows.Clear();
            foreach(var item in listStuent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[1].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[2].Value = item.FullName;
                if(item.Faculty != null)
                {
                    dgvStudent.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                }
                dgvStudent.Rows[index].Cells[4].Value = item.AverageScore + " ";
                if(item.MajorID != null)
                {
                    dgvStudent.Rows[index].Cells[5].Value = item.MajorID  + " ";
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (Model1 context = new Model1())
            {
                foreach (DataGridViewRow row in dgvStudent.Rows)
                {
                    DataGridViewCheckBoxCell checkBox = row.Cells["Chon"] as DataGridViewCheckBoxCell;
                    if (checkBox != null && checkBox.Value != null && (bool)checkBox.Value)
                    {
                        int studentID = (int)row.Cells[1].Value;
                        int majorID = (int)cmbMajor.SelectedValue;
                        var student = context.Students.FirstOrDefault(s => s.StudentID == studentID.ToString());

                        if (student != null)
                        {
                            student.MajorID = majorID;
                        }
                    }
                }
                context.SaveChanges();
                Form1 form1 = Application.OpenForms["Form1"] as Form1;
                if (form1 != null)
                {
                    form1.UpdateDataGridView();
                }
                MessageBox.Show("Thông tin chuyên ngành đã được cập nhật cho sinh viên.");
            }
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
