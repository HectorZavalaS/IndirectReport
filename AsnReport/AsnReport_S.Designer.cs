
namespace AsnReport
{
    partial class AsnReport_S
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AsnReport_t = new System.Windows.Forms.Timer(this.components);
            this.system_events = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.system_events)).BeginInit();
            // 
            // AsnReport_t
            // 
            this.AsnReport_t.Interval = 300000;
            this.AsnReport_t.Tick += new System.EventHandler(this.AsnReport_t_Tick);
            // 
            // AsnReport_S
            // 
            this.ServiceName = "Indirect Report";
            ((System.ComponentModel.ISupportInitialize)(this.system_events)).EndInit();

        }

        #endregion

        private System.Windows.Forms.Timer AsnReport_t;
        private System.Diagnostics.EventLog system_events;
    }
}
