using System;
using System.Collections.Generic;

namespace QLTienLuong.Models;

public partial class HuongPhuCap
{
    public int MaHuongPhuCap { get; set; }

    public string? MaHocVien { get; set; }

    public DateOnly? ThangNam { get; set; }

    public decimal? TongPhuCap { get; set; }

    public decimal? DoanPhi { get; set; }

    public decimal? LopHoc { get; set; }

    public decimal? TrUung { get; set; }

    public decimal? ConNhan { get; set; }

    public string? Ky { get; set; }

    public virtual HocVien? MaHocVienNavigation { get; set; }

    // Tính toán tổng phụ cấp từ các danh mục được nhận
    public decimal? TinhTongPhuCap => (DoanPhi ?? 0) + (LopHoc ?? 0) + (TrUung ?? 0);

    // Tính toán còn nhận
    public decimal? TinhConNhan => TinhTongPhuCap - (DoanPhi ?? 0) - (LopHoc ?? 0) - (TrUung ?? 0);
}
