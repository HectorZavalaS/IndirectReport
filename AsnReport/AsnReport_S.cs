using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace AsnReport
{
    public partial class AsnReport_S : ServiceBase
    {
        CMailSender senderM;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        Timer timer = new Timer();
        public AsnReport_S()
        {
            InitializeComponent();
            senderM = new CMailSender();
            system_events = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("Indirect Report"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "Indirect Report", "Application");
            }
            system_events.Source = "Indirect Report";
            system_events.Log = "Application";
        }

        protected override void OnStart(string[] args)
        {
            try
            {

                system_events.WriteEntry("Iniciado servicio de reporte Indirectos. ");
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = 1000; //number in milisecinds  
                timer.Enabled = true;
            }
            catch(Exception ex)
            {
                system_events.WriteEntry("Ocurrio un error al iniciar el Timer. " + ex.Message);
                //logger.Error(ex, "Ocurrio un error al iniciar el Timer.");
            }
        }

        protected override void OnStop()
        {
            AsnReport_t.Stop();
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                int day = (int)DateTime.Now.DayOfWeek;
                if (day >= 1 && day <= 6)
                {
                    if((DateTime.Now.Hour == 7 && DateTime.Now.Minute == 30 && DateTime.Now.Second == 0) || (DateTime.Now.Hour == 16 && DateTime.Now.Minute == 5 && DateTime.Now.Second == 0)) { 
                        system_events.WriteEntry("Se enviara reporte.");
                        senderM.sendMail(system_events);
                    }
                }
            }
            catch(Exception ex)
            {
                system_events.WriteEntry("Ocurrio un error al ejecutar Timer. " + ex.Message);
            }
        }
        private void AsnReport_t_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
