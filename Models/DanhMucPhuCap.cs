using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class DanhMucPhuCap
{
    public string MaPhuCap { get; set; } = null!;

    public string? TenPhuCap { get; set; }

    public string? MoTa { get; set; }

    public decimal? MucPhuCapCoBan { get; set; }

    public virtual ICollection<PhuCapHocVien> PhuCapHocViens { get; set; } = new List<PhuCapHocVien>();
}
