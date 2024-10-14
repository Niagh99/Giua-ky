using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DE01.NewFolder1;

namespace DE01
{
    public partial class frmSinhVien : Form
    {
        public frmSinhVien()
        {
            InitializeComponent();
            LoadLop();
            LoadSinhVien();
        }

        private void LoadSinhVien()
        {
            using (var context = new StudentModel())
            {
                var sinhViens = context.Sinhviens
                    .Include(s => s.Lop)
                    .Select(s => new
                    {
                        s.MaSV,
                        s.HotenSV,
                        s.NgaySinh,
                        TenLop = s.Lop.TenLop,
                        s.MaLop
                    })
                    .ToList();

                dgvSinhVien.DataSource = sinhViens;

                if (dgvSinhVien.Columns.Contains("MaLop"))
                {
                    dgvSinhVien.Columns["MaLop"].Visible = false;
                }
            }

            BindData();
        }
        private void BindData()
        {
            // Hủy các ràng buộc cũ nếu có
            txtMaSV.DataBindings.Clear();
            txtHoTen.DataBindings.Clear();
            dtpNgaySinh.DataBindings.Clear();
            cboLop.DataBindings.Clear();

            // Thực hiện binding cho các điều khiển
            txtMaSV.DataBindings.Add("Text", dgvSinhVien.DataSource, "MaSV");
            txtHoTen.DataBindings.Add("Text", dgvSinhVien.DataSource, "HotenSV");
            dtpNgaySinh.DataBindings.Add("Value", dgvSinhVien.DataSource, "NgaySinh");

            // ComboBox Lớp sẽ binding với giá trị của MaLop
            cboLop.DataBindings.Add("SelectedValue", dgvSinhVien.DataSource, "MaLop");
        }



        private void LoadLop()
        {
            using (var context = new StudentModel())
            {
                var lops = context.Lops
                    .Select(l => new
                    {
                        l.MaLop,
                        l.TenLop
                    })
                .ToList();

                cboLop.DataSource = lops;
                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
            }
        }
        private void dgvSinhVien_SelectionChanged(object sender, EventArgs e)
        {
            BindData();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMaSV.Text) && !string.IsNullOrEmpty(txtHoTen.Text) && cboLop.SelectedIndex != -1)
            {
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thêm sinh viên này không?", "Xác nhận thêm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (var context = new StudentModel())
                    {
                        var sinhVien = new Sinhvien
                        {
                            MaSV = txtMaSV.Text,
                            HotenSV = txtHoTen.Text,
                            NgaySinh = dtpNgaySinh.Value,
                            MaLop = cboLop.SelectedValue.ToString()
                        };

                        context.Sinhviens.Add(sinhVien);
                        context.SaveChanges();
                    }

                    LoadSinhVien();
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (var context = new StudentModel())
                {
                    var sinhVien = context.Sinhviens.Find(txtMaSV.Text);
                    if (sinhVien != null)
                    {
                        context.Sinhviens.Remove(sinhVien);
                        context.SaveChanges();
                    }
                }

                LoadSinhVien();
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn sửa thông tin sinh viên này không?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (var context = new StudentModel())
                {
                    var sinhVien = context.Sinhviens.Find(txtMaSV.Text);
                    if (sinhVien != null)
                    {
                        sinhVien.HotenSV = txtHoTen.Text;
                        sinhVien.NgaySinh = dtpNgaySinh.Value;
                        sinhVien.MaLop = cboLop.SelectedValue.ToString();

                        context.SaveChanges();
                    }
                }

                LoadSinhVien();
                MessageBox.Show("Sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string keyword = txtTim.Text.Trim().ToLower();
            using (var context = new StudentModel())
            {
                var sinhViens = context.Sinhviens
                    .Include(s => s.Lop)
                    .Where(s => s.HotenSV.ToLower().Contains(keyword))
                    .Select(s => new
                    {
                        s.MaSV,
                        s.HotenSV,
                        s.NgaySinh,
                        TenLop = s.Lop.TenLop,
                        s.MaLop
                    })
                    .ToList();

                dgvSinhVien.DataSource = sinhViens;

                if (sinhViens.Count > 0)
                {
                    MessageBox.Show($"Tìm thấy {sinhViens.Count} sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();

            }
        }

    }
}
