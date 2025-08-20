using System;
using System.Collections.Generic;
using System.Linq; // Added for .Sum()

namespace QLTienLuong.Models
{
    public class HuongPhuCapViewModel
    {
        public int MaHuongPhuCap { get; set; }
        public string? MaHocVien { get; set; }
        public string? HoTen { get; set; }
        public string? Khoa { get; set; }
        public string? Lop { get; set; }
        public DateOnly? ThangNam { get; set; }
        public string? Ky { get; set; }

        // Các danh mục phụ cấp được áp dụng
        public List<PhuCapDetail> DanhMucPhuCaps { get; set; } = new List<PhuCapDetail>();
        
        // Tổng phụ cấp (tính từ các danh mục được áp dụng)
        public decimal? TongPhuCap { get; set; }
        
        // Các khoản trừ
        public decimal? DoanPhi { get; set; }
        public decimal? LopHoc { get; set; }
        public decimal? TrUung { get; set; }
        
        // Còn nhận
        public decimal? ConNhan { get; set; }

        // Các loại phụ cấp cụ thể
        public decimal? PhuCapCoBan { get; set; }  // Phụ cấp NCS hoặc Đại học
        public decimal? PhuCapT7CN { get; set; }   // Phụ cấp T7+CN
        public decimal? PhuCapSua { get; set; }    // Phụ cấp sữa

        // Tính toán tổng phụ cấp từ các danh mục phụ cấp được áp dụng
        public decimal? TinhTongPhuCap => DanhMucPhuCaps?.Sum(p => p.MucPhuCap ?? 0) ?? 0;

        // Tính toán còn nhận
        public decimal? TinhConNhan => TinhTongPhuCap - (DoanPhi ?? 0) - (LopHoc ?? 0) - (TrUung ?? 0);
    }

    public class PhuCapDetail
    {
        public string MaPhuCap { get; set; } = string.Empty;
        public string TenPhuCap { get; set; } = string.Empty;
        public decimal? MucPhuCap { get; set; }
        public DateOnly? NgayApDung { get; set; }
    }
}
