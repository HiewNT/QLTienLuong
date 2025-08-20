using Microsoft.AspNetCore.Authorization;

namespace QLTienLuong.Models
{
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        public AdminOnlyAttribute()
        {
            Roles = RoleConstants.RoleNames.ADMIN;
        }
    }

    public class LopTruongOnlyAttribute : AuthorizeAttribute
    {
        public LopTruongOnlyAttribute()
        {
            Roles = RoleConstants.RoleNames.LOP_TRUONG;
        }
    }

    public class HocVienOnlyAttribute : AuthorizeAttribute
    {
        public HocVienOnlyAttribute()
        {
            Roles = RoleConstants.RoleNames.HOC_VIEN;
        }
    }

    public class NhanVienTaiChinhOnlyAttribute : AuthorizeAttribute
    {
        public NhanVienTaiChinhOnlyAttribute()
        {
            Roles = RoleConstants.RoleNames.NHAN_VIEN_TAI_CHINH;
        }
    }

    public class AdminOrNhanVienTaiChinhAttribute : AuthorizeAttribute
    {
        public AdminOrNhanVienTaiChinhAttribute()
        {
            Roles = $"{RoleConstants.RoleNames.ADMIN},{RoleConstants.RoleNames.NHAN_VIEN_TAI_CHINH}";
        }
    }

    public class AdminOrLopTruongAttribute : AuthorizeAttribute
    {
        public AdminOrLopTruongAttribute()
        {
            Roles = $"{RoleConstants.RoleNames.ADMIN},{RoleConstants.RoleNames.LOP_TRUONG}";
        }
    }

    public class AdminOrHocVienAttribute : AuthorizeAttribute
    {
        public AdminOrHocVienAttribute()
        {
            Roles = $"{RoleConstants.RoleNames.ADMIN},{RoleConstants.RoleNames.HOC_VIEN}";
        }
    }

    public class AdminOrHocVienOrLopTruongAttribute : AuthorizeAttribute
    {
        public AdminOrHocVienOrLopTruongAttribute()
        {
            Roles = $"{RoleConstants.RoleNames.ADMIN},{RoleConstants.RoleNames.HOC_VIEN},{RoleConstants.RoleNames.LOP_TRUONG}";
        }
    }
}
