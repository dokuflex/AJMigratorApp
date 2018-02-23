using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Access.Dao;
using DokuFlex.Windows.Common;
using AJMigratorApp.Services;
using AJMigratorApp.Data;
using AJMigratorApp.Helpers;
using DokuFlex.Windows.Common.Services;
using DokuFlex.Windows.Common.Services.Data;
using System.Collections;
using DokuFlex.Windows.Common.Log;

namespace AJMigratorApp
{
    public class AJImportController
    {
        private string token;
        private readonly ILoginService loginService;
        private readonly IDataService dataService;
        private readonly IMainForm mainForm;
        private readonly string tempFolder = DFEnvironment.GetSpecialFolder(DFEnvironment.SpecialFolder.UploadDirectory);

        public AJImportController(ILoginService loginService, IMainForm mainForm)
        {
            this.loginService = loginService;
            dataService = DataServiceFactory.Create();
            this.mainForm = mainForm;
            ClearResources();
            InitialzeTempFolder();
        }

        private void InitialzeTempFolder()
        {
                if (!Directory.Exists(tempFolder))
                try
                {
                    Directory.CreateDirectory(tempFolder);
                }
                catch (Exception ex)
                {
                    throw  new ApplicationException("Can not create the Upload temp directory", ex);
                }
        }

        private void ClearResources()
        {
            if (Directory.Exists(tempFolder))
                try
                {
                    Directory.Delete(tempFolder,true);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Can not delete the Upload temp directory", ex);
                }
        }

        public bool Importing { get; private set; } = false;

        public async Task<bool> StartImportAsync()
        {
            if (Importing)
                throw new InvalidOperationException("An import operation is running. Please wait until the operation resume.");

            mainForm.StartProgress();

            try
            {
                //ClearResources();
                mainForm.SetProgressInfo("Iniciando sesión en dokuflex");
                token = loginService.Login();

                EnsureToken();

                mainForm.SetProgressInfo("Importando datos de Buro de faxes");
                await ImportBuroFaxesAsync();
                mainForm.SetProgressInfo("Importando datos de Oficios");
                await ImportOficiosAsync();
                mainForm.SetProgressInfo("Importando datos de Contratos");
                await ImportContratosAsync();
                mainForm.SetProgressInfo("Finalizando...");
                ClearResources();
                mainForm.SetProgressInfo("Importación finalizada!");
            }
            finally
            {
                Importing = false;
                mainForm.StopProgress();
            }

            return await Task.FromResult(false);
        }

        private void EnsureToken()
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Token no valid. the login operations fails");
        }

        private async Task<bool> ImportBuroFaxesAsync()
        {
            var dao = new BuroFaxesDAO();

            dao.OpenDatabase();

            try
            {
                var rstMain = dao.GetRecordSet();
                var processDataList = new List<ProcessData>();
                var uploadFileList = new List<string>();

                while (!rstMain.EOF)
                {
                    processDataList.Add(BuroFaxesHelper.CreateFechaEmisionDataField(rstMain.Fields["FECHA DE EMISIÓN"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateFechaLlegadaDataField(rstMain.Fields["FECHA DE LLEGADA A CAIXARENTING"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateFechaEntregaDataField(rstMain.Fields["FECHA EN LA QUE SE ENTREGA AL DEPARTAMENTO CORRESPONDIENTE"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateFechaRespuestaDataField(rstMain.Fields["FECHA DE RESPUESTA POR PARTE DEL DEPARTAMENTO CORRESPONDIENTE"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateClienteDataField(rstMain.Fields["CLIENTE"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateAsuntoDataField(rstMain.Fields["ASUNTO"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateDepartamentoDataField(rstMain.Fields["DEPARTAMENTO AL QUE PERTENECE"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateRespuestaEmitidaDataField(rstMain.Fields["RESPUESTA EMITIDA (ASUNTO-TEMA)"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateComentario1DataField(rstMain.Fields["COMENTARIO (I)"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateComentario2DataField(rstMain.Fields["COMENTARIO (II)"].Value.ToString()));
                    processDataList.Add(BuroFaxesHelper.CreateComentario3DataField(rstMain.Fields["COMENTARIO (III)"].Value.ToString()));

                    var dataId = string.Empty;

                    try
                    {
                        dataId = await AddBuroFaxesProcessData(token, processDataList.ToArray());
                    }
                    catch (RestResponseException ex)
                    {
                        LogFactory.CreateLog().LogError(ex);
                        continue;
                    }

                    // Export files from first column
                    var rstAttach = RecordsetHelper.GetRecordset2("DOCUMENTO ADJUNTO (I)", rstMain);

                    try
                    {
                        uploadFileList.AddRange(ExportFilesFromRecordset(rstAttach));
                    }
                    finally
                    {
                        rstAttach.Close();
                    }

                    // Export files from second column
                    rstAttach = RecordsetHelper.GetRecordset2("DOCUMENTO ADJUNTO (II)", rstMain);

                    try
                    {
                        uploadFileList.AddRange(ExportFilesFromRecordset(rstAttach));
                    }
                    finally
                    {
                        rstAttach.Close();
                    }

                    foreach (var fileName in uploadFileList)
                    {
                        try
                        {
                            await AddBuroFaxesProcessFiles(token, dataId, new FileInfo(fileName));
                        }
                        catch (RestResponseException ex)
                        {
                            LogFactory.CreateLog().LogError(ex);
                            continue;
                        }
                    }

                    processDataList.Clear();
                    uploadFileList.Clear();
                    rstMain.MoveNext();
                }
            }
            finally
            {
                dao.CloseDatabase();
                dao = null;
            }

            return true;
        }

        private List<string> ExportFilesFromRecordset(Recordset2 rstAttach)
        {
            var result = new List<string>();

            while (!rstAttach.EOF)
            {
                var fileName = $"{tempFolder}\\{rstAttach.Fields["FileName"].Value}";
                Field2 fld = (Field2)rstAttach.Fields["FileData"];
                fld.SaveToFile(fileName);
                result.Add(fileName);
                rstAttach.MoveNext();
            }

            return result;
        }

        private async Task<bool> ImportOficiosAsync()
        {
            var dao = new OficiosDAO();

            dao.OpenDatabase();

            try
            {
                var rstMain = dao.GetRecordSet();
                var processDataList = new List<ProcessData>();
                var uploadFileList = new List<string>();

                while (!rstMain.EOF)
                {
                    processDataList.Add(OficiosHelper.CreateAsuntoDataField(rstMain.Fields["ASUNTO"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateDenominacionSocialDataField(rstMain.Fields["DENOMINACIÓN SOCIAL/NOMBRE, APELLIDOS ARRENDATARIO"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateMatriculaVehiculoDataField(rstMain.Fields["MATRÍCULA VEHÍCULO"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateNumeroContratoDataField(rstMain.Fields["NÚMERO DE CONTRATO"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateEntidadSolicitanteDataField(rstMain.Fields["ENTIDAD SOLICITANTE DE LOS DATOS/OFICIOS"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateLocalidadDataField(rstMain.Fields["LOCALIDAD/PROVINCIA SOLICITANTE"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateDireccionSolicitanteDataField(rstMain.Fields["DIRECCION SOLICITANTE"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateCodigoPostalDataField(rstMain.Fields["CÓDIGO POSTAL SOLICITANTE"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateTelefonoDataField(rstMain.Fields["TELÉFONO SOLICITANTE"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateFaxDataField(rstMain.Fields["FAX SOLICITANTE"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateCorreoElectronicoDataField(rstMain.Fields["CORREO ELECTRÓNICO SOLICITANTE"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreatePersonaContactoDataField(rstMain.Fields["PERSONA DE CONTACTO SOLICITANTE"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateFechaYMedioDataField(rstMain.Fields["FECHA Y MEDIO A TRAVÉS DEL CUAL HA LLEGADO LA SOLICITUD"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateFechaRespuestaCxrDataField(rstMain.Fields["FECHA DE RESPUESTA DE CXR Y A TRAVÉS DE QUE MEDIO"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateComentario1DataField(rstMain.Fields["COMENTARIOS (I)"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateComentario2DataField(rstMain.Fields["COMENTARIOS (II)"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateComentario3DataField(rstMain.Fields["COMENTARIOS (III)"].Value.ToString()));
                    processDataList.Add(OficiosHelper.CreateLugarExpedienteDataField(rstMain.Fields["LUGAR EN EL QUE SE ENCUENTRA EL EXPEDIENTE FISICO"].Value.ToString()));

                    var dataId = string.Empty;

                    try
                    {
                        dataId = await AddOficiosProcessData(token, processDataList.ToArray());
                    }
                    catch (RestResponseException ex)
                    {
                        LogFactory.CreateLog().LogError(ex);
                        continue;
                    }

                    // Export files from first column
                    var rstAttach = RecordsetHelper.GetRecordset2("DATOS ADJUNTOS (I)", rstMain);

                    try
                    {
                        uploadFileList.AddRange(ExportFilesFromRecordset(rstAttach));
                    }
                    finally
                    {
                        rstAttach.Close();
                    }

                    // Export files from second column
                    rstAttach = RecordsetHelper.GetRecordset2("DATOS ADJUNTOS (II)", rstMain);

                    try
                    {
                        uploadFileList.AddRange(ExportFilesFromRecordset(rstAttach));
                    }
                    finally
                    {
                        rstAttach.Close();
                    }

                    // Export files from third column
                    rstAttach = RecordsetHelper.GetRecordset2("DATOS ADJUNTOS (III)", rstMain);

                    try
                    {
                        uploadFileList.AddRange(ExportFilesFromRecordset(rstAttach));
                    }
                    finally
                    {
                        rstAttach.Close();
                    }

                    foreach (var fileName in uploadFileList)
                    {
                        try
                        {
                            await AddOficiosProcessFiles(token, dataId, new FileInfo(fileName));
                        }
                        catch (RestResponseException ex)
                        {
                            LogFactory.CreateLog().LogError(ex);
                            continue;
                        }
                    }

                    processDataList.Clear();
                    uploadFileList.Clear();
                    rstMain.MoveNext();
                }
            }
            finally
            {
                dao.CloseDatabase();
                dao = null;
            }

            return true;
        }

        private async Task<bool> ImportContratosAsync()
        {
            var dao = new ContratosDAO();

            dao.OpenDatabase();

            try
            {
                var rstMain = dao.GetRecordSet();
                var processDataList = new List<ProcessData>();
                var uploadFileList = new List<string>();

                while (!rstMain.EOF)
                {
                    processDataList.Add(ContratosHelper.CreateClienteDataField(rstMain.Fields["Cliente"].Value.ToString()));
                    processDataList.Add(ContratosHelper.CreateTipoContratoDataField(rstMain.Fields["Tipo de contrato"].Value.ToString()));
                    processDataList.Add(ContratosHelper.CreateNIFDataField(rstMain.Fields["NIF"].Value.ToString()));
                    processDataList.Add(ContratosHelper.CreateNumeroContratoDataField(rstMain.Fields["Número crto"].Value.ToString()));
                    processDataList.Add(ContratosHelper.CreateUbicacionDataField(rstMain.Fields["Ubicación"].Value.ToString()));

                    var dataId = string.Empty;

                    try
                    {
                        dataId = await AddContratosProcessData(token, processDataList.ToArray());
                    }
                    catch (RestResponseException ex)
                    {
                        LogFactory.CreateLog().LogError(ex);
                        continue;
                    }

                    // Export files from first column
                    var rstAttach = RecordsetHelper.GetRecordset2("Datos adjuntos", rstMain);

                    try
                    {
                        uploadFileList.AddRange(ExportFilesFromRecordset(rstAttach));
                    }
                    finally
                    {
                        rstAttach.Close();
                    }

                    foreach (var fileName in uploadFileList)
                    {
                        try
                        {
                            await AddContratosProcessFiles(token, dataId, new FileInfo(fileName));
                        }
                        catch (RestResponseException ex)
                        {
                            LogFactory.CreateLog().LogError(ex);
                            continue;
                        }
                    }

                    processDataList.Clear();
                    uploadFileList.Clear();
                    rstMain.MoveNext();
                }
            }
            finally
            {
                dao.CloseDatabase();
                dao = null;
            }

            return true;
        }

        private async Task<string> AddBuroFaxesProcessData(string token, params ProcessData[] dataArr)
        {
            var result = await dataService.ProcessUpdateDataAsync(token, "4b5e1bb0-96ae-4d72-a41a-60da03691268", "5cd6c372-c4c6-4d23-9323-65900ab60429", string.Empty, dataArr);

            return result;
        }

        private async Task<bool> AddBuroFaxesProcessFiles(string token, string dataId, FileInfo file)
        {
            var result = await dataService.ProcessUploadAsync(token, "4b5e1bb0-96ae-4d72-a41a-60da03691268", dataId, "scan", file);
            return result != null;
        }

        private async Task<string> AddContratosProcessData(string token, params ProcessData[] dataArr)
        {
            var result = await dataService.ProcessUpdateDataAsync(token, "876cbcb1-3d13-477a-bf7b-159e23b8c4ef", "5cd6c372-c4c6-4d23-9323-65900ab60429", string.Empty, dataArr);
            return result;
        }

        private async Task<bool> AddContratosProcessFiles(string token, string dataId, FileInfo file)
        {
            var result = await dataService.ProcessUploadAsync(token, "876cbcb1-3d13-477a-bf7b-159e23b8c4ef", dataId, "scan", file);
            return result != null;
        }

        private async Task<string> AddOficiosProcessData(string token, params ProcessData[] dataArr)
        {
            var result = await dataService.ProcessUpdateDataAsync(token, "ec98e835-9b41-4b85-a437-c62f3bf2c8e3", "5cd6c372-c4c6-4d23-9323-65900ab60429", string.Empty, dataArr);
            return result;
        }

        private async Task<bool> AddOficiosProcessFiles(string token, string dataId, FileInfo file)
        {
            var result = await dataService.ProcessUploadAsync(token, "ec98e835-9b41-4b85-a437-c62f3bf2c8e3", dataId, "scan", file);
            return result != null;
        }
    }
}
