using DokuFlex.Windows.Common.Services.Data;

namespace AJMigratorApp.Helpers
{
    public static class BuroFaxesHelper
    {
        public static ProcessData CreateFechaEmisionDataField(string data)
        {
            return new ProcessData { fieldName = "fecha_emision", type = "", value = data };
        }

        public static ProcessData CreateFechaLlegadaDataField(string data)
        {
            return new ProcessData { fieldName = "fecha_recepcion", type = "", value = data };
        }

        public static ProcessData CreateClienteDataField(string data)
        {
            return new ProcessData { fieldName = "cliente", type = "", value = data };
        }

        public static ProcessData CreateAsuntoDataField(string data)
        {
            return new ProcessData { fieldName = "asunto", type = "", value = data };
        }

        public static ProcessData CreateDepartamentoDataField(string data)
        {
            return new ProcessData { fieldName = "departamento", type = "", value = data };
        }

        public static ProcessData CreateFechaEntregaDataField(string data)
        {
            return new ProcessData { fieldName = "fecha_entrega", type = "", value = data };
        }

        public static ProcessData CreateFechaRespuestaDataField(string data)
        {
            return new ProcessData { fieldName = "fecha_respuesta", type = "", value = data };
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

        public static ProcessData CreateRespuestaEmitidaDataField(string data)
        {
            return new ProcessData { fieldName = "respuesta_emitida", type = "", value = data };
        }
    }
}
