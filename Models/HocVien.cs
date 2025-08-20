using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class HocVien
{
    public string MaHocVien { get; set; } = null!;

    public string? HoTen { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? Khoa { get; set; }

    public string? Lop { get; set; }

    public string? Nganh { get; set; }

    public string? DonVi { get; set; }

    public string? MaQuocTich { get; set; }

    public int? NamTotNghiep { get; set; }

    public virtual ICollection<HocVienTangLuong> HocVienTangLuongs { get; set; } = new List<HocVienTangLuong>();

    public virtual ICollection<HuongPhuCap> HuongPhuCaps { get; set; } = new List<HuongPhuCap>();

    public virtual ICollection<KhenThuongKyLuat> KhenThuongKyLuats { get; set; } = new List<KhenThuongKyLuat>();

    public virtual QuocTich? MaQuocTichNavigation { get; set; }

    public virtual ICollection<PhuCapHocVien> PhuCapHocViens { get; set; } = new List<PhuCapHocVien>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
