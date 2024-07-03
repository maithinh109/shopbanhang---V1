using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shopbanhang.Models
{
    
        public class cartitem
        {
            public int id { get; set; }
            public int price { get; set; }
            public string img { set; get; }

            public string name { get; set; }
            public int soluong { get; set; }
            public int thanhtien
            {
                get
                {
                    return soluong * price;
                }
            }


        }
    
}