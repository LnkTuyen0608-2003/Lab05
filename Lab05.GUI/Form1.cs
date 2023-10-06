using Lab05.BUS;
using Lab05.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Transactions;

namespace Lab05.GUI
{
    public partial class Form1 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFacultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFacultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }
        private void BindGrid(List<Student> listStudents)
        {
            dgvStudent.Rows.Clear();
            foreach (Student item in listStudents)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                if(item.FacultyID != null)
                {
                    dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                }
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore + " ";
                if(item.MajorID != null)
                {
                    dgvStudent.Rows[index].Cells[4].Value = item.MajorID + " ";
                }
                ShowAvatar(item.Avatar);
            }
        }
        private void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                picAvatar.Image = null;
            }
            else
            { 
                string imageDirectory = Path.Combine(Application.StartupPath, "Images");
                string imagePath = Path.Combine(imageDirectory, ImageName);
                if (File.Exists(imagePath))
                {
                    picAvatar.Image = Image.FromFile(imagePath);
                    picAvatar.Refresh();
                }    
            }
        }
        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkUnregisterMajor.Checked)
            {
                listStudents = studentService.GetAllHasNoMajor();
                tbStudentID.Text = "";
                tbFullName.Text = "";
                tbAverageScore.Text = "";
                cmbFaculty.SelectedIndex = 0;
                picAvatar.Image = null;
            }
            else
            {
                listStudents = studentService.GetAll();
            }
            BindGrid(listStudents);
        }

        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int studentID;
                if (int.TryParse(tbStudentID.Text, out studentID))
                {
                    string fullName = tbFullName.Text;
                    double averageScore;
                    if (!double.TryParse(tbAverageScore.Text, out averageScore))
                    {
                        MessageBox.Show("Điểm trung bình phải là một số hợp lệ.");
                        return;
                    }

                    int facultyID;
                    if (!int.TryParse(cmbFaculty.SelectedValue.ToString(), out facultyID))
                    {
                        MessageBox.Show("Vui lòng chọn một khoa hợp lệ.");
                        return;
                    }

                    // Tạo một đối tượng Student mới
                    Student student = new Student
                    {
                        StudentID = studentID.ToString(),
                        FullName = fullName,
                        AverageScore = averageScore,
                        FacultyID = facultyID,
                    };

                    Student existingStudent = studentService.FindById(studentID);
                    if (existingStudent == null)
                    {
                        // Thêm một sinh viên mới
                        studentService.InsertUpdate(student);
                    }
                    else
                    {
                        // Cập nhật một sinh viên đã tồn tại
                        existingStudent.FullName = student.FullName;
                        existingStudent.AverageScore = student.AverageScore;
                        existingStudent.FacultyID = student.FacultyID;

                        studentService.InsertUpdate(existingStudent);
                    }

                    // Kiểm tra xem PictureBox có hình ảnh không
                    if (picAvatar.Image != null)
                    {
                        string avatarFileName = $"{studentID}.jpg";
                        string imageDirectory = Path.Combine(Application.StartupPath, "Images");
                        string imagePath = Path.Combine(imageDirectory, avatarFileName);
                        picAvatar.Image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                        // Cập nhật cột Avatar trong bảng Student
                        studentService.UpdateAvatar(studentID, avatarFileName);
                    }

                    var listStudents = studentService.GetAll();
                    BindGrid(listStudents);

                    tbStudentID.Text = "";
                    tbFullName.Text = "";
                    tbAverageScore.Text = "";
                    cmbFaculty.SelectedIndex = 0;
                    picAvatar.Image = null;

                    MessageBox.Show("Thông tin sinh viên đã được lưu thành công.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Hình ảnh (*.jpg, *.png)|*.jpg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Lấy đường dẫn đến hình ảnh đã chọn
                string imagePath = openFileDialog.FileName;

                // Hiển thị hình ảnh trên PictureBox 
                picAvatar.Image = Image.FromFile(imagePath);
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvStudent.Rows[e.RowIndex];
                string studentID = selectedRow.Cells[0].Value.ToString();
                string fullName = selectedRow.Cells[1].Value.ToString();
                string facultyName = selectedRow.Cells[2].Value.ToString();
                string averageScore = selectedRow.Cells[3].Value.ToString();

                tbStudentID.Text = studentID;
                tbFullName.Text = fullName;
                cmbFaculty.Text = facultyName;
                tbAverageScore.Text = averageScore;

                // Kiểm tra hình ảnh
                string avatarFileName = studentID + ".jpg";
                string imageDirectory = Path.Combine(Application.StartupPath, "Images");
                string imagePath = Path.Combine(imageDirectory, avatarFileName);

                if (File.Exists(imagePath))
                {
                    ShowAvatar(avatarFileName);
                }
                else
                {
                    picAvatar.Image = null;
                }
            }
        }

        private void đKChuyênNgànhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRegister f = new frmRegister();
            f.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int studentIDToDelete;
                if (int.TryParse(tbStudentID.Text, out studentIDToDelete))
                {
                    using (var scope = new TransactionScope())
                    {
                        if (picAvatar.Image != null)
                        {
                            picAvatar.Image.Dispose();
                            picAvatar.Image = null;
                        }

                        studentService.Delete(studentIDToDelete);
                        string avatarFileName = $"{studentIDToDelete}.jpg";
                        string imageDirectory = Path.Combine(Application.StartupPath, "Images");
                        string imagePath = Path.Combine(imageDirectory, System.IO.Path.GetFileName(avatarFileName));
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        File.Delete(imagePath);
                        var listStudents = studentService.GetAll();
                        BindGrid(listStudents);

                        tbStudentID.Text = "";
                        tbFullName.Text = "";
                        tbAverageScore.Text = "";
                        cmbFaculty.SelectedIndex = 0;

                        scope.Complete();
                        MessageBox.Show("Sinh viên đã được xóa thành công.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập một ID sinh viên hợp lệ.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }
        public void UpdateDataGridView()
        {
            var listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }
    }
}
