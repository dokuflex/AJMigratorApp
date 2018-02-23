using AJMigratorApp.AsesoriaJuridicaDataSetTableAdapters;
using Microsoft.Office.Interop.Access.Dao;
using static AJMigratorApp.AsesoriaJuridicaDataSet;

namespace AJMigratorApp.Data
{
    public class BuroFaxesDAO
    {
        private DBEngine dbe;
        private Database db;

        public BuroFaxesDAO()
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
                "SELECT * FROM [BUROFAXES CAIXARENTING]",
                    RecordsetTypeEnum.dbOpenSnapshot);

            return rstMain;
        }
    }
}
