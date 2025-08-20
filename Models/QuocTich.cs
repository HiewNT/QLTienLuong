using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class QuocTich
{
    public string MaQuocTich { get; set; } = null!;

    public string? TenQuocTich { get; set; }

    public virtual ICollection<HocVien> HocViens { get; set; } = new List<HocVien>();
}
