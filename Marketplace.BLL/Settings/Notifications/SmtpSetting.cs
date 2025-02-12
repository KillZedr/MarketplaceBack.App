using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.BLL.Settings.Notifications
{
    public class SmtpSetting
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string FromEmail { get; set; }
        public string SenderName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
