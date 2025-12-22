using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project_65131134.Models.ViewModels
{
    public class DangKyViewModel
    {
        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập!")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự!")]
        public string TenDangNhap { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự!")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [DisplayName("Xác nhận mật khẩu")]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu!")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        [DataType(DataType.Password)]
        public string XacNhanMatKhau { get; set; }

        [DisplayName("Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên!")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự!")]
        public string HoTen { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Vui lòng nhập email!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự!")]
        public string Email { get; set; }

        [DisplayName("Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        [StringLength(10, ErrorMessage = "Số điện thoại không được vượt quá 10 ký tự!")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ được chứa số!")]
        public string SoDienThoai { get; set; }

        [DisplayName("Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự!")]
        public string DiaChi { get; set; }
    }
}

