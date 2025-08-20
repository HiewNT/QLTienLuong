using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class TangLuong
{
    public int MaTangLuong { get; set; }

    public decimal? MucLuongTang { get; set; }

    public DateOnly? NgayTangLuong { get; set; }

    public string? LyDoTangLuong { get; set; }

    public virtual ICollection<HocVienTangLuong> HocVienTangLuongs { get; set; } = new List<HocVienTangLuong>();
}
