using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<ProductType> ProductTypes { get; set; }
        public IEnumerable<SpecialTag> SpecialTags { get; set; }
    }
}
