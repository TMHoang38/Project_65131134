using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_65131134.Models.ViewModels
{
    public class DongHoPaginationViewModel
    {
        public IEnumerable<DongHo> DanhSachDongHo { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}