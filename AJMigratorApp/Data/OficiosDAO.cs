using Microsoft.Office.Interop.Access.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJMigratorApp.Data
{
    class OficiosDAO
    {
        private DBEngine dbe;
        private Database db;

        public OficiosDAO()
        {
            dbe = new DBEngine();
            OpenDatabase();
        }

        public void OpenDatabase()
        {
            db = dbe.OpenDatabase(@".\App_Data\ASESORIA_JURIDICA.accdb");
        }

        public void CloseDatabase()
        {
            db?.Close();
        }

        public Recordset GetRecordSet()
        {

            Recordset rstMain = db.OpenRecordset(
                "SELECT * FROM [OFICIOS CAIXARENTING]",
                    RecordsetTypeEnum.dbOpenSnapshot);

            return rstMain;
        }
    }
}
