using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class KhenThuongKyLuat
{
    public string MaQuyetDinh { get; set; } = null!;

    public string? MaHocVien { get; set; }

    public string? LoaiHinh { get; set; }

    public DateOnly? NgayQuyetDinh { get; set; }

    public string? LyDo { get; set; }

    public decimal? SoTien { get; set; }

    public virtual HocVien? MaHocVienNavigation { get; set; }
}
