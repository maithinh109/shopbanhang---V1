using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopbanhang.Models
{
    public class Muahang
    {
        public int CustomerId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public string Address { get; set; }
        public int PaymentId { get; set; }
    }
}