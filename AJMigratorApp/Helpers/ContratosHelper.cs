using DokuFlex.Windows.Common.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJMigratorApp.Helpers
{
    public static class ContratosHelper
    {
        public static ProcessData CreateClienteDataField(string data)
        {
            return new ProcessData { fieldName = "cliente", type = "", value = data };
        }

        public static ProcessData CreateNumeroContratoDataField(string data)
        {
            return new ProcessData { fieldName = "numero_contrato", type = "", value = data };
        }

        public static ProcessData CreateTipoContratoDataField(string data)
        {
            return new ProcessData { fieldName = "tipo_contrato", type = "", value = data };
        }

        public static ProcessData CreateNIFDataField(string data)
        {
            return new ProcessData { fieldName = "nif", type = "", value = data };
        }

        public static ProcessData CreateUbicacionDataField(string data)
        {
            return new ProcessData { fieldName = "ubicacion", type = "", value = data };
        }
    }
}
