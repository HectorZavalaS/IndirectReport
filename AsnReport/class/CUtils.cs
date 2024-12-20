using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ReportGenerator.Class
{
    public class CUtils
    {
        private excel m_excel;

        public CUtils()
        {

        }
        public bool getHTMLFromFile(String ruta, ref String html, ref string info)
        {
            DataTable tmp, dthoja;
            int periodo = 0, diasAcum = 0;
            String fecha_i_periodo, fecha_f_periodo, fecha_i_incid, fecha_f_incid, fecha_lim;
            html = "";
            //String html

            bool result = false;

            m_excel = new excel(ruta);
            m_excel.loadBookExcel();
            tmp = m_excel.Book;
            foreach (DataRow hoja in tmp.Rows)
            {
                dthoja = m_excel.ReadSheet(Convert.ToString(hoja["TABLE_NAME"]));
                if (dthoja.Rows.Count > 0)
                {
                    int i = 0;
                    html = "<table class='layout'>";
                    foreach (DataRow fila in dthoja.Rows)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(fila[0])) && !String.IsNullOrEmpty(Convert.ToString(fila[1])) && !String.IsNullOrEmpty(Convert.ToString(fila[2])))
                            {
                                periodo = Convert.ToInt32(fila[0]);
                                fecha_i_periodo = Convert.ToString(fila[1]);
                                fecha_f_periodo = Convert.ToString(fila[2]);
                                diasAcum = Convert.ToInt32(fila[5]);
                                fecha_i_incid = Convert.ToString(fila[6]);
                                fecha_f_incid = Convert.ToString(fila[7]);
                                fecha_lim = Convert.ToString(fila[8]);

                            }
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            info = ex.Message + " - " + ex.InnerException;
                        }
                        i++;
                    }
                    if (i == dthoja.Rows.Count) result = true;
                }
            }
            return result;
        }
        public bool createDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return false;
                }

                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                //MessageBox.Show("Ocurrio un error al crear el directorio:" + e.Message);
                return false;
            }
            finally { }
            return true;
        }
        public void translateM(ref String month)
        {
            month = month.Replace("January", "Enero");
            month = month.Replace("February", "Febrero");
            month = month.Replace("March", "Marzo");
            month = month.Replace("April", "Abril");
            month = month.Replace("May", "Mayo");
            month = month.Replace("June", "Junio");
            month = month.Replace("July", "Julio");
            month = month.Replace("August", "Agosto");
            month = month.Replace("September", "Septiembre");
            month = month.Replace("October", "Octubre");
            month = month.Replace("November", "Noviembre");
            month = month.Replace("December", "Diciembre");
            month = month.Replace("01", "Enero");
            month = month.Replace("02", "Febrero");
            month = month.Replace("03", "Marzo");
            month = month.Replace("04", "Abril");
            month = month.Replace("05", "Mayo");
            month = month.Replace("06", "Junio");
            month = month.Replace("07", "Julio");
            month = month.Replace("08", "Agosto");
            month = month.Replace("09", "Septiembre");
            month = month.Replace("10", "Octubre");
            month = month.Replace("11", "Noviembre");
            month = month.Replace("12", "Diciembre");
            month = month.Replace("1", "Enero");
            month = month.Replace("2", "Febrero");
            month = month.Replace("3", "Marzo");
            month = month.Replace("4", "Abril");
            month = month.Replace("5", "Mayo");
            month = month.Replace("6", "Junio");
            month = month.Replace("7", "Julio");
            month = month.Replace("8", "Agosto");
            month = month.Replace("9", "Septiembre");
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //String fieldName = prop.Name.Replace("_", " ");
                //int r = 0;
                //try
                //{
                //    if(int.TryParse(fieldName.ElementAt(fieldName.Length-1).ToString(), out r))
                //        fieldName = fieldName.Substring(0, fieldName.Length - 1);
                //}
                //catch (Exception ex) { }
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

    }
}