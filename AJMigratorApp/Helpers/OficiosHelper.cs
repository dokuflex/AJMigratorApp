using DokuFlex.Windows.Common.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJMigratorApp.Helpers
{
    public static class OficiosHelper
    {
        public static ProcessData CreateAsuntoDataField(string data)
        {
            return new ProcessData { fieldName = "asunto", type = "", value = data };
        }

        public static ProcessData CreateDenominacionSocialDataField(string data)
        {
            return new ProcessData { fieldName = "denominacion_social_arrendatario", type = "", value = data };
        }

        public static ProcessData CreateMatriculaVehiculoDataField(string data)
        {
            return new ProcessData { fieldName = "matricula_vehiculo", type = "", value = data };
        }

        public static ProcessData CreateNumeroContratoDataField(string data)
        {
            return new ProcessData { fieldName = "numero_contrato", type = "", value = data };
        }

        public static ProcessData CreateEntidadSolicitanteDataField(string data)
        {
            return new ProcessData { fieldName = "entidad_solicitante_oficios", type = "", value = data };
        }

        public static ProcessData CreateLocalidadDataField(string data)
        {
            return new ProcessData { fieldName = "localidad_provincia", type = "", value = data };
        }

        public static ProcessData CreateDireccionSolicitanteDataField(string data)
        {
            return new ProcessData { fieldName = "direccion", type = "", value = data };
        }

        public static ProcessData CreateCodigoPostalDataField(string data)
        {
            return new ProcessData { fieldName = "codigo_postal", type = "", value = data };
        }

        public static ProcessData CreateTelefonoDataField(string data)
        {
            return new ProcessData { fieldName = "telefono", type = "", value = data };
        }

        public static ProcessData CreateFaxDataField(string data)
        {
            return new ProcessData { fieldName = "fax", type = "", value = data };
        }

        public static ProcessData CreateCorreoElectronicoDataField(string data)
        {
            return new ProcessData { fieldName = "email", type = "", value = data };
        }

        public static ProcessData CreatePersonaContactoDataField(string data)
        {
            return new ProcessData { fieldName = "contacto", type = "", value = data };
        }

        public static ProcessData CreateFechaYMedioDataField(string data)
        {
            return new ProcessData { fieldName = "fecha_medio_solicitud", type = "", value = data };
        }

        public static ProcessData CreateFechaRespuestaCxrDataField(string data)
        {
            return new ProcessData { fieldName = "fecha_respuesta_cxr_medio", type = "", value = data };
        }

        public static ProcessData CreateComentario1DataField(string data)
        {
            return new ProcessData { fieldName = "comentarios1", type = "", value = data };
        }

        public static ProcessData CreateComentario2DataField(string data)
        {
            return new ProcessData { fieldName = "comentarios2", type = "", value = data };
        }

        public static ProcessData CreateComentario3DataField(string data)
        {
            return new ProcessData { fieldName = "comentarios3", type = "", value = data };
        }

        public static ProcessData CreateLugarExpedienteDataField(string data)
        {
            return new ProcessData { fieldName = "lugar_expediente_fisico", type = "", value = data };
        }
    }
}
