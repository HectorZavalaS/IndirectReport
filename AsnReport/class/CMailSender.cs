using ReportGenerator;
using ReportGenerator.Class;
using smtLocations.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsnReport{
    class CMailSender
    {
        private COracle m_oracle;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void sendMail(System.Diagnostics.EventLog system_events)
        {
            try
            {
                m_oracle = new COracle("192.168.0.23", "SEMPROD");
                excel m_excel = new excel();
                String pathReport = "";
                String pathReporterror = "";
                CUtils utils = new CUtils();
                String error = "";
                //system_events.WriteEntry("Obteniendo registros de base de datos de Oracle.");
                if(m_oracle.get_Ind_report(ref pathReport, system_events)) { 

                    List<string> lstArchivos = new List<string>();
                    lstArchivos.Add(pathReport);

                    //if(m_oracle.get_error_report(ref pathReporterror, system_events))
                    //    lstArchivos.Add(pathReporterror);
                    //claudia.quintos.condor@gmail.com
                    String mails = "sem.ap@siix-global.com;sem.ap@siix-global.com;indirectos@siix-global.com;antonio.hernandez@siix-global.com";
                    //String mails = "asn-sem@siix-sem.com.mx";
                    //String mails = "asn-sem@siix-sem.com.mx;warehouse.receiving@SIIX-SEM.com.mx;ruben.regis@SIIX-SEM.com.mx;kenny.manzanilla@SIIX-SEM.com.mx;christian.gonzalez@siix-sem.com.mx;luisfernando.torres@SIIX-SEM.com.mx;cristobal.munoz@siix-sem.com.mx;antonio.hernandez@siix-sem.com.mx;javier.gallardo@siix-sem.com.mx;pre-receiving@siix.mx;dulce.loredo@siix-sem.com.mx;raymundo.salas@siix-sem.com.mx;indirectos@siix-sem.com.mx;leslie.castaneda@SIIX-SEM.com.mx;practicantefinanzas@siix.mx;carolina.perez@SIIX-SEM.com.mx";
                    //String mails = "antonio.hernandez@siix-sem.com.mx";

                    //creamos nuestro objeto de la clase que hicimos

                    //CMail oMail = new CMail("ASN_Report@siix.mx", mails,
                    //                     "ASNs Report", "ASNs Report", lstArchivos);

                    CMail oMail = new CMail("siixsem.reports@siix-global.com", mails,
                                         "Indirect Report", "Indirect Report", lstArchivos);

                    oMail.Message = "Se anexa reporte de Indirectos / Attached you will find Indirect report.<br><br> Saludos / Regards.";

                    //y enviamos
                    if (oMail.enviaMail(ref error))
                    {
                        system_events.WriteEntry("Se envio por E-mail Indirect Report.");

                    }
                    else
                    {
                        system_events.WriteEntry("No se envio el mail: " + oMail.error + "  \n" + error);
                       //logger.Error("No se envio el mail: " + oMail.error);

                    }
                }
            }
            catch(Exception ex)
            {
                system_events.WriteEntry("Ocurrio un error al Construir Reporte. " + ex.Message);
            }
        }
    }
}
