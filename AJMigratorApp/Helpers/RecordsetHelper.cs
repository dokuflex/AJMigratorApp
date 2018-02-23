using Microsoft.Office.Interop.Access.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJMigratorApp.Helpers
{
    public static class RecordsetHelper
    {
        public static Recordset2 GetRecordset2(string fieldName, Recordset recordset)
        {
            Recordset2 rstAttach = recordset.Fields[fieldName].Value;
            return rstAttach;
        }
    }
}
