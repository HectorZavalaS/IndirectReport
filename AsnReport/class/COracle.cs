using Oracle.ManagedDataAccess.Client;
using ReportGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator.Class
{
    class COracle
    {
        String m_server;
        String m_SID;
        private String m_user;
        private String m_pass;
        OracleConnection m_OracleDB;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string Server { get => m_server; set => m_server = value; }
        public string SID { get => m_SID; set => m_SID = value; }

        public COracle (String serv, String Sid)
        {
            m_server = serv;
            m_SID = Sid;
            m_user = "APPS";
            m_pass = "apps";
            m_OracleDB = GetDBConnection(Server, 0, SID, m_user, m_pass);
            m_OracleDB.Open();
        }

        private OracleConnection GetDBConnection(string host, int port, String sid, String user, String password)
        {


            Console.WriteLine("Getting Connection ...");

            // 'Connection string' to connect directly to Oracle.
            string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
                 + Server + ")(PORT = " + "1521" + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
                 + SID + ")));Password=" + m_pass + ";User ID=" + m_user + ";Enlist=false;Pooling=true";

            OracleConnection conn = new OracleConnection();
            try
            {
                conn.ConnectionString = connString;
            }
            catch(Exception ex)
            {
                conn = null;
                logger.Error(ex, "Error al conectarse a base de datos de Oracle");
            }

            return conn;
        }
        public  bool QuerySerial(String serial, ref int resultTest)
        {
            bool result = false;
            string sql = "SELECT * FROM insp_result_summary_info where board_barcode in ('" + serial.ToUpper() + "')"; ;

            try
            {
                // Create command.
                OracleCommand cmd = new OracleCommand();

                // Set connection for command.
                cmd.Connection = m_OracleDB;
                cmd.CommandText = sql;


                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        result = true;

                        while (reader.Read())
                        {
                            int IRCODEIndex = reader.GetOrdinal("INSP_RESULT_CODE");
                            int VLRCODEIndex = reader.GetOrdinal("VC_LAST_RESULT_CODE");

                            long? INSP_RESULT_CODE = null;
                            long? VC_LAST_RESULT_CODE = null;

                            if (!reader.IsDBNull(IRCODEIndex))
                                INSP_RESULT_CODE = Convert.ToInt64(reader.GetValue(IRCODEIndex));
                            if (!reader.IsDBNull(VLRCODEIndex))
                                VC_LAST_RESULT_CODE = Convert.ToInt64(reader.GetValue(VLRCODEIndex));

                            if (INSP_RESULT_CODE == 0 && VC_LAST_RESULT_CODE == null)
                                resultTest = 1;   //// OK
                            if (INSP_RESULT_CODE != 0 && VC_LAST_RESULT_CODE != 0)
                                resultTest = 2;   //// NG
                            if (INSP_RESULT_CODE != 0 && VC_LAST_RESULT_CODE == 0)
                                resultTest = 3;   //// FALSE CALL (OK)
                            if (INSP_RESULT_CODE != 0 && VC_LAST_RESULT_CODE == null)
                                resultTest = 4;   //// NO JUZGADA

                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, "[QuerySerial] Error en serial: " + serial);
                result = false;
            }
            return result;

        }

        public bool QuerySerials(List<String> serials, ref int resultTest)
        {
            bool result = false;
            String qSerials = "";

            foreach(String serial in serials)
            {
                qSerials += "'" + serial.ToUpper() + "',";
            }

            string sql = "SELECT * FROM insp_result_summary_info where board_barcode in (" + qSerials.Substring(0,qSerials.Length-1) + ")"; ;

            try
            {
                // Create command.
                OracleCommand cmd = new OracleCommand();

                // Set connection for command.
                cmd.Connection = m_OracleDB;
                cmd.CommandText = sql;


                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        result = true;
                        
                        while (reader.Read())
                        {
                            int IRCODEIndex = reader.GetOrdinal("INSP_RESULT_CODE");
                            int VLRCODEIndex = reader.GetOrdinal("VC_LAST_RESULT_CODE");

                            long? INSP_RESULT_CODE = null;
                            long? VC_LAST_RESULT_CODE = null;

                            if (!reader.IsDBNull(IRCODEIndex))
                                INSP_RESULT_CODE = Convert.ToInt64(reader.GetValue(IRCODEIndex));
                            if (!reader.IsDBNull(VLRCODEIndex))
                                VC_LAST_RESULT_CODE = Convert.ToInt64(reader.GetValue(VLRCODEIndex));

                            if (INSP_RESULT_CODE == 0 && VC_LAST_RESULT_CODE == null)
                                resultTest = 1;   //// OK
                            if (INSP_RESULT_CODE != 0 && VC_LAST_RESULT_CODE != 0)
                                resultTest = 2;   //// NG
                            if (INSP_RESULT_CODE != 0 && VC_LAST_RESULT_CODE == 0)
                                resultTest = 3;   //// FALSE CALL (OK)
                            if (INSP_RESULT_CODE != 0 && VC_LAST_RESULT_CODE == null)
                                resultTest = 4;   //// NO JUZGADA
                            //break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[QuerySerial] Error ");
                result = false;
            }
            return result;

        }
        public bool getSimosOnHand()
        {
            String fileName = "InvOH_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            bool result = false;
            String pathReport = "";
            excel m_excel = new excel();

            String query = "SELECT A.ITEM_NAME, A.SUBINVENTORY_CODE, A.LOT_NUMBER, A.TOTAL_QOH, A.BATCH_NO, A.LOCATOR, " +
					"A.ITEM_DESCRIPTION, TO_CHAR(B.CREATED_DT, 'DD-Mon-YYYY') RECEIPT_DATE, A.RECEIPT_NUM, " + 
					"A.EXPIRY_DATE, A.SRC, A.ITEM_TYPE, A.UOM, A.ITEM_CATEGORY, A.PROJECT_CODE, A.SUPPLIER_LOT_NUMBER, " +
					"A.MKR_PRT_CD, TO_CHAR(B.UPLOAD_SUCCESS_DATE, 'DD-Mon-YYYY HH24:MI:SS') UPLOAD_ORACLE, " +
					"	( CASE A.IQC_STATUS " +
					"	WHEN 1 THEN 'Pending For Storage' " +
					"	WHEN 2 THEN 'Rejected' " +
					"	WHEN 3 THEN 'Not Completed' " +
					"	WHEN 4 THEN 'Accept' " +
					"	END ) AS IQC_STATUS_DECODEING_CODE, B.STORED " +
				"FROM SIIXSEM.V_SIMOS_STOCK_ENQUIRY A, SIIXSEM.INCOMING_LOT_DETAILS B " +
				"WHERE  A.ORGANIZATION_ID = 81 AND A.LOT_NUMBER = B.LOT_NUMBER AND A.SRC <> 'IN-TRANSIT'";

            try
            {
                DataTable data = new DataTable();
                // Create command.
                OracleCommand cmd = new OracleCommand();

                // Set connection for command.
                cmd.Connection = m_OracleDB;
                cmd.CommandText = query;


                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        result = true;

                        data.Load(reader);
                        //m_excel.write_fileOLE(data, fileName, @"G:\EVERYONE\Simos_Inventory_OnHand\2020_06_Stocktake", ref pathReport);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getSimosOnHand] Error ");
                result = false;
            }
            return result;

        }
        public bool preStocktake()
        {
            String fileName = "Pre_Stock_take_data_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
            bool result = false;
            String pathReport = "";
            excel m_excel = new excel();

            //SELECT 
            //       A.ITEM_NAME,
            //       A.ITEM_DESC,
            //       A.LOT_NUMBER,
            //       A.SUBINVENTORY_CODE SUBINVENTORY,
            //       A.LOCATOR,
            //       A.SCANNED_QTY,
            //       A.BATCH_NO,
            //       A.SCANNED_BY,
            //       A.SCANNED_DT,
            //       B.SUBINVENTORY_CODE ACT_SUBINVENTORY,
            //      B.LOCATOR ACT_LOCATOR,
            //      B.TOTAL_QOH ACT_QTY
            //FROM 
            //       SIIXSEM.PRE_STOCK_TAKE_DATA A,
            //       SIIXSEM.V_SIMOS_STOCK_ENQUIRY B
            //WHERE
            //       A.LOT_NUMBER = B.LOT_NUMBER


            String query = "SELECT A.ITEM_NAME, A.ITEM_DESC, A.LOT_NUMBER, A.SUBINVENTORY_CODE SUBINVENTORY, A.LOCATOR, A.SCANNED_QTY, A.BATCH_NO, A.SCANNED_BY, A.SCANNED_DT, " +
                                    "B.SUBINVENTORY_CODE ACT_SUBINVENTORY, B.LOCATOR ACT_LOCATOR, B.TOTAL_QOH ACT_QTY " +
                            "FROM   SIIXSEM.PRE_STOCK_TAKE_DATA A, SIIXSEM.V_SIMOS_STOCK_ENQUIRY B " +
                            "WHERE A.LOT_NUMBER = B.LOT_NUMBER";
            //String query = "SELECT ITEM_NAME, ITEM_DESC, LOT_NUMBER, SCANNED_QTY, SUBINVENTORY_CODE, LOCATOR, BATCH_NO, SCANNED_BY, TO_CHAR(SCANNED_DT, 'DD-Mon-YYYY HH24:MI:SS') SCANNED_TIME FROM SIIXSEM.PRE_STOCK_TAKE_DATA";

            try
            {
                DataTable data = new DataTable();
                // Create command.
                OracleCommand cmd = new OracleCommand();

                // Set connection for command.
                cmd.Connection = m_OracleDB;
                cmd.CommandText = query;


                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        result = true;

                        data.Load(reader);
                        ///m_excel.write_fileOLE(data, fileName, @"G:\EVERYONE\Simos_Inventory_OnHand\2021_01_Stocktake", ref pathReport);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[getSimosOnHand] Error ");
                result = false;
            }
            return result;
        }
        public bool get_Ind_report(ref String pathReport, System.Diagnostics.EventLog system_events)
        {

            String fileName = "INDIRECT_DTL" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
            bool result = false;
            pathReport = "";
            excel m_excel = new excel();

          
            String query = "SELECT SIIXSEM.INCOMING_IND_DTL.RECEIPT_NUM, SIIXSEM.INCOMING_IND_DTL.VENDOR_NAME, SIIXSEM.INCOMING_IND_DTL.VENDOR_SITE_CODE, SIIXSEM.INCOMING_IND_DTL.PO_NUM, SIIXSEM.INCOMING_IND_DTL.PO_LINE_NUM, SIIXSEM.INCOMING_IND_DTL.ITEM_NAME, " +
                           "SIIXSEM.INCOMING_IND_DTL.ITEM_DESCRIPTION, SIIXSEM.INCOMING_IND_DTL.MKR_PRT_CD, " +
                           "CASE ASN_SHIPPED_QTY WHEN 0 THEN SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY ELSE SIIXSEM.INCOMING_IND_DTL.ASN_SHIPPED_QTY END AS ASN_SHIPPED_QTY, " +
                           "SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY, SIIXSEM.INCOMING_IND_DTL.CREATED_BY, SIIXSEM.INCOMING_IND_HDR.CREATED_DT, REPLACE(TO_CHAR(EXTRACT(month FROM SIIXSEM.INCOMING_IND_HDR.CREATED_DT), '00') || '+' || TO_CHAR(EXTRACT(day FROM SIIXSEM.INCOMING_IND_HDR.CREATED_DT), '00'), ' ', '') AS \"MONTH+DAY\", " +
                           "(CASE ASN_SHIPPED_QTY WHEN 0 THEN SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY ELSE SIIXSEM.INCOMING_IND_DTL.ASN_SHIPPED_QTY END) - SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY AS BALANCE, " +
                           "CASE ASN_SHIPPED_QTY WHEN 0 THEN NVL(ROUND(SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY/ SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY,6),0) ELSE NVL(ROUND(ALLOTED_LOT_QTY/ ASN_SHIPPED_QTY,6),0) END AS \"% RECEIVED\", APPROVE_FLAG, SIIXSEM.INCOMING_IND_HDR.STATUS, SIIXSEM.INCOMING_IND_HDR.CARGO_ARR_NO , " +
                           "NVL((SELECT iu.urgent_flag " +
                           "  FROM SIIXSEM.ITEM_URGENT iu " +
                           "  WHERE  SIIXSEM.INCOMING_IND_DTL.po_header_id = iu.po_header_id " +
                           "  AND  SIIXSEM.INCOMING_IND_DTL.po_line_id = iu.po_line_id),'N') ITEM_URGENT, " +
                           "SIIXSEM.INCOMING_IND_INTRANSIT_AUDIT.CREATED_DT AS ARRIVED_DATE, SIIXSEM.INCOMING_IND_DTL.ETA_DATE AS ETA_DATE " +
                    "FROM SIIXSEM.INCOMING_IND_DTL " +
                    "INNER JOIN SIIXSEM.INCOMING_IND_HDR ON SIIXSEM.INCOMING_IND_HDR.INCOMING_IND_HDR_ID = SIIXSEM.INCOMING_IND_DTL.INCOMING_IND_HDR_ID " +
                    "LEFT JOIN SIIXSEM.INCOMING_IND_INTRANSIT_AUDIT ON SIIXSEM.INCOMING_IND_INTRANSIT_AUDIT.INCOMING_IND_HDR_ID = SIIXSEM.INCOMING_IND_DTL.INCOMING_IND_HDR_ID " +
                    "WHERE SIIXSEM.INCOMING_IND_DTL.INCOMING_IND_HDR_ID >= 0 " +
                    "AND((SIIXSEM.INCOMING_IND_DTL.ASN_SHIPPED_QTY > 0 " +
                    "AND SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY > 0) " +
                    "OR(SIIXSEM.INCOMING_IND_DTL.ASN_SHIPPED_QTY > 0 " +
                    "AND SIIXSEM.INCOMING_IND_DTL.ALLOTED_LOT_QTY >= 0)) " +
                    "ORDER BY REPLACE(TO_CHAR(EXTRACT(month FROM SIIXSEM.INCOMING_IND_HDR.CREATED_DT), '00') || '+' || TO_CHAR(EXTRACT(day FROM SIIXSEM.INCOMING_IND_HDR.CREATED_DT), '00'), ' ', ''), SIIXSEM.INCOMING_IND_DTL.RECEIPT_NUM, SIIXSEM.INCOMING_IND_DTL.ITEM_NAME, SIIXSEM.INCOMING_IND_DTL.PO_NUM, SIIXSEM.INCOMING_IND_DTL.PO_LINE_NUM"; 
            try
            {
                DataTable data = new DataTable();
                // Create command.
                OracleCommand cmd = new OracleCommand();

                // Set connection for command.
                cmd.Connection = m_OracleDB;
                cmd.CommandText = query;

                system_events.WriteEntry("Obteniendo registros de base de datos de Oracle. \n" + query);
                //logger.Info()
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        result = true;
                        data.Load(reader);
                        system_events.WriteEntry("Se obtuvieron " + data.Rows.Count.ToString() + " registros."); 
                        m_excel.write_fileOLE(data, fileName, "C:\\Reports", ref pathReport, system_events);
                    }
                }
            }
            catch (Exception ex)
            {
                system_events.WriteEntry("Ocurrio un error al realizar la consulta en Oracle. " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool get_error_report(ref String pathReport, System.Diagnostics.EventLog system_events)
        {

            String fileName = "Receiving_error_messages_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";
            bool result = false;
            pathReport = "";
            excel m_excel = new excel();

            string queryPre = "ALTER SESSION SET NLS_DATE_FORMAT = 'MM/DD/YY'";
            String query = "SELECT PROJECT_CODE, RECEIPT_NUM, PO_NUM, PO_LINE_NUM, VENDOR_NAME, VENDOR_SITE_CODE, ITEM_NAME, LOT_NUMBER, LOT_QTY, CREATED_DT, IQC_CHECKED_DT, ERROR_MESSAGE " +
                             "FROM SIIXSEM.INCOMING_LOT_DETAILS " +
                             "WHERE ERROR_MESSAGE NOT LIKE '%error%' AND CREATED_DT > TO_DATE('24/04/21', 'DD/MM/YY')";
            try
            {
                DataTable data = new DataTable();
                // Create command.
                OracleCommand cmd = new OracleCommand();

                // Set connection for command.
                cmd.Connection = m_OracleDB;
                cmd.CommandText = query;

                system_events.WriteEntry("Obteniendo registros de base de datos de Oracle. \n" + query);
                //logger.Info()
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        result = true;
                        data.Load(reader);
                        system_events.WriteEntry("Se obtuvieron " + data.Rows.Count.ToString() + " registros.");
                        m_excel.write_report_error(data, fileName, "C:\\Reports", ref pathReport, system_events);

                    }
                }
            }
            catch (Exception ex)
            {
                system_events.WriteEntry("Ocurrio un error al realizar la consulta en Oracle. " + ex.Message);
                result = false;
            }
            return result;
        }
        public void Close()
        {
            m_OracleDB.Dispose();
            m_OracleDB.Close();
            OracleConnection.ClearPool(m_OracleDB);
        }
    }
}
