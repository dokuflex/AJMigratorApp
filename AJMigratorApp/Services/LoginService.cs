using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokuFlex.WinForms.Common;

namespace AJMigratorApp.Services
{
    public interface ILoginService
    {
        string Login();
    }

    public class LoginService : ILoginService
    {
        public string Login()
        {
            return Session.GetTikect();
        }
    }
}
