using System.Collections.Generic;
using Project_65131134.Models;

namespace Project_65131134.Models.ViewModels
{
    public class CartItemViewModel
    {
        public DongHo SanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => (SanPham?.GiaBan ?? 0) * SoLuong;
    }

    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TongTien { get; set; }
    }

    public class CheckoutViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TongTien { get; set; }

        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string GhiChu { get; set; }
    }
}

