using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class PhuCapHocVien
{
    public int MaPhuCapHocVien { get; set; }

    public string? MaHocVien { get; set; }

    public string? MaPhuCap { get; set; }

    public DateOnly? NgayApDung { get; set; }

    public virtual HocVien? MaHocVienNavigation { get; set; }

    public virtual DanhMucPhuCap? MaPhuCapNavigation { get; set; }
}
