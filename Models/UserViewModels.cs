using System.ComponentModel.DataAnnotations;

namespace QLTienLuong.Models
{
    public class UserCreateViewModel
    {
        public string? MaHocVien { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = null!;
        
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;
        
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [Display(Name = "Vai trò")]
        public int MaRole { get; set; }
        
        [Display(Name = "Kích hoạt")]
        public bool Active { get; set; } = true;
    }

    public class UserEditViewModel
    {
        public int MaUser { get; set; }
        
        public string? MaHocVien { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = null!;
        
        [Display(Name = "Mật khẩu mới")]
        public string? Password { get; set; }
        
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [Display(Name = "Vai trò")]
        public int MaRole { get; set; }
        
        [Display(Name = "Kích hoạt")]
        public bool Active { get; set; } = true;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }

    public class EditProfileViewModel
    {
        public int MaUser { get; set; }
        
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = null!;
        
        public string? MaHocVien { get; set; }
        
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string? HoTen { get; set; }
        
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateOnly? NgaySinh { get; set; }
        
        [Display(Name = "Khóa")]
        public string? Khoa { get; set; }
        
        [Display(Name = "Lớp")]
        public string? Lop { get; set; }
        
        [Display(Name = "Ngành")]
        public string? Nganh { get; set; }
        
        [Display(Name = "Đơn vị")]
        public string? DonVi { get; set; }
        
        [Display(Name = "Quốc tịch")]
        public string? MaQuocTich { get; set; }
        
        [Display(Name = "Năm tốt nghiệp")]
        [Range(1900, 2100, ErrorMessage = "Năm tốt nghiệp phải từ 1900 đến 2100")]
        public int? NamTotNghiep { get; set; }
    }

    public class ProfileViewModel
    {
        public User User { get; set; } = null!;
        public List<PhuCapHocVien> PhuCapHocViens { get; set; } = new List<PhuCapHocVien>();
        public List<KhenThuongKyLuat> KhenThuongKyLuats { get; set; } = new List<KhenThuongKyLuat>();
        public List<HocVienTangLuong> HocVienTangLuongs { get; set; } = new List<HocVienTangLuong>();
        public List<HuongPhuCap> HuongPhuCaps { get; set; } = new List<HuongPhuCap>();
    }
}
