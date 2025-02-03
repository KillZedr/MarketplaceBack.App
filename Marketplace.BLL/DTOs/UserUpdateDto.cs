using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.DTOs
{
    public class UserUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
