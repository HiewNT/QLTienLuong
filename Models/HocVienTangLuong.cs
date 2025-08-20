using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class HocVienTangLuong
{
    public int MaHvtl { get; set; }

    public int? MaTangLuong { get; set; }

    public string? MaHocVien { get; set; }

    public DateOnly? ThangNam { get; set; }

    public string? GhiChu { get; set; }

    public virtual HocVien? MaHocVienNavigation { get; set; }

    public virtual TangLuong? MaTangLuongNavigation { get; set; }
}
