//using GleamTech.DocumentUltimate;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using ReportGenerator.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;

namespace ReportGenerator
{
    public class excel
    {
        private String m_path;
        private System.Data.DataTable m_book;
        private CUtils m_utils;
        //DocumentConverterResult m_resultConvert;

        public string Path
        {
            get{ return m_path; }
            set{ m_path = value; }
        }
        public System.Data.DataTable Book
        {
            get{ return m_book; }
            set{ m_book = value; }
        }

        public excel()
        {
            m_path = "";
            m_book = new System.Data.DataTable();
            m_utils = new CUtils();
        }
        public excel(String path)
        {
            m_path = path;
            m_book = new System.Data.DataTable();
            m_utils = new CUtils();
        }
        public bool write_fileOLE(System.Data.DataTable dt, String fileName, string path, ref String strFileName, System.Diagnostics.EventLog system_events)
        {
            bool result = false;
            try
            {
                string finalFileNameWithPath = string.Empty;
                string finalFileNameWithPathXML = string.Empty;

                int mes = DateTime.Now.Month;
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

                //fileName = path + fileName;
                string strMonthName = mes.ToString();//mfi.GetMonthName(mes).ToString();
                m_utils.translateM(ref strMonthName);

                finalFileNameWithPath = path + "\\" + fileName;
                //Delete existing file with same file name.
                system_events.WriteEntry("El archivo se guardara como: " + finalFileNameWithPath);
                if (File.Exists(finalFileNameWithPath))
                    File.Delete(finalFileNameWithPath);

                var newFile = new FileInfo(finalFileNameWithPath);

                //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
                using (var package = new ExcelPackage(newFile))
                {
                    //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Indirect Report");
                    ExcelAddress _formatRangeAddress = new ExcelAddress("O2:O"+dt.Rows.Count + 1);
                    ExcelAddress _formatRangeAddress1 = new ExcelAddress("L2:L" + dt.Rows.Count + 1);
                    ExcelAddress _formatRangeAddress2 = new ExcelAddress("T2:T" + dt.Rows.Count + 1);


                    //Step 3 : Start loading datatable form A1 cell of worksheet.
                    worksheet.PrinterSettings.HeaderMargin = 0.5M;
                    worksheet.PrinterSettings.LeftMargin = 0.35M;
                    worksheet.PrinterSettings.BottomMargin = 0.5M;
                    worksheet.PrinterSettings.RightMargin = 0.3M;

                    worksheet.PrinterSettings.PaperSize = ePaperSize.Letter;
                    worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                    //worksheet.Cells["A1:T1"].Style.Font.Bold = true;
                    //worksheet.Cells["A1:T1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells["A1:T1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);


                    //worksheet.ConditionalFormatting.
                    worksheet.Cells["L2:L" + (dt.Rows.Count + 1).ToString()].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells["T2:T" + (dt.Rows.Count + 1).ToString()].Style.Numberformat.Format = "dd/mm/yyyy h:mm";
                    worksheet.Cells["O2:O"+(dt.Rows.Count + 1).ToString()].Style.Numberformat.Format = "0.00%";
                    worksheet.ConditionalFormatting.AddThreeIconSet(_formatRangeAddress, eExcelconditionalFormatting3IconsSetType.TrafficLights1);
                    //worksheet.Cells["J1:L1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                    worksheet.Cells.AutoFitColumns();
                    for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                    {
                        int pos = row % 2;
                        ExcelRow rowRange = worksheet.Row(row);
                        ExcelFill RowFill = rowRange.Style.Fill;
                        RowFill.PatternType = ExcelFillStyle.Solid;
                        switch (pos)
                        {
                            case 0:
                                RowFill.BackgroundColor.SetColor(Color.FromArgb(185, 215, 238));


                                break;
                            case 1:
                                RowFill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));
                                break;

                        }

                        worksheet.Row(row).Height = 24;
                    }

                    worksheet.Row(1).Height = 37.5;
                    worksheet.Cells["A1:U1"].Style.Font.Bold = true;
                    worksheet.Cells["A1:U1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A1:U1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 78, 120));
                    worksheet.Cells["A1:U1"].Style.Font.Color.SetColor(Color.White);
                    worksheet.Cells["A1:U1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A1:U1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A1:U1"].AutoFilter = true;

                    worksheet.Cells["V1:X" + (dt.Rows.Count + 1).ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["V1:X" + (dt.Rows.Count + 1).ToString()].Style.Fill.BackgroundColor.SetColor(Color.White);

                    //worksheet.Select("A2:O" + dt.Rows.Count + 1);

                    //package.Workbook.CreateVBAProject();

                    //StringBuilder vbaCode = new StringBuilder();

                    //vbaCode.AppendLine("Sub Color_by_BOMs()");
                    //vbaCode.AppendLine("Application.ScreenUpdating = False");
                    //vbaCode.AppendLine("Nuevo = False");
                    //vbaCode.AppendLine("Total = Selection.Rows.Count");
                    //vbaCode.AppendLine("ActiveCell.Offset(1, 0).Select");
                    //vbaCode.AppendLine("For A = 1 To Total - 1");
                    //vbaCode.AppendLine("Actual = ActiveCell.Value");
                    //vbaCode.AppendLine("Arriba = ActiveCell.Offset(-1, 0).Value");
                    //vbaCode.AppendLine("If Actual = Arriba Then");
                    //vbaCode.AppendLine("If Nuevo Then");
                    //vbaCode.AppendLine("Selection.EntireRow.Select");
                    //vbaCode.AppendLine("With Selection.Interior");
                    //vbaCode.AppendLine(".ColorIndex = 0");
                    //vbaCode.AppendLine("'.Pattern = xlGray25");
                    //vbaCode.AppendLine("'.Pattern = xlCrissCross");
                    //vbaCode.AppendLine(".PatternColorIndex = 37");
                    //vbaCode.AppendLine(".ThemeColor = xlThemeColorAccent5");
                    //vbaCode.AppendLine(".TintAndShade = 0.599993896298105");
                    //vbaCode.AppendLine(".PatternTintAndShade = 0");
                    //vbaCode.AppendLine("End With");
                    //vbaCode.AppendLine("End If");
                    //vbaCode.AppendLine("Else");
                    //vbaCode.AppendLine("If Nuevo Then");
                    //vbaCode.AppendLine("Nuevo = False");
                    //vbaCode.AppendLine("Else");
                    //vbaCode.AppendLine("Selection.EntireRow.Select");
                    //vbaCode.AppendLine("With Selection.Interior");
                    //vbaCode.AppendLine(".ColorIndex = 0");
                    //vbaCode.AppendLine("'.Pattern = xlGray25");
                    //vbaCode.AppendLine("'.Pattern = xlCrissCross");
                    //vbaCode.AppendLine(".PatternColorIndex = 37");
                    //vbaCode.AppendLine(".ThemeColor = xlThemeColorAccent5");
                    //vbaCode.AppendLine(".TintAndShade = 0.599993896298105");
                    //vbaCode.AppendLine(".PatternTintAndShade = 0");
                    //vbaCode.AppendLine("End With");
                    //vbaCode.AppendLine("Nuevo = True");
                    //vbaCode.AppendLine("End If");
                    //vbaCode.AppendLine("End If");
                    //vbaCode.AppendLine("ActiveCell.Offset(1, 0).Select");
                    //vbaCode.AppendLine("Next");
                    //vbaCode.AppendLine("Application.ScreenUpdating = True");
                    //vbaCode.AppendLine("End Sub");

                    //package.Workbook.CodeModule.Code = vbaCode.ToString();
                    //package.Workbook.CodeModule.

                    //Step 4 : (Optional) Set the file properties like title, author and subject
                    package.Workbook.Properties.Title = @"ASNs Report";
                    package.Workbook.Properties.Author = @"SIIX Corp., II. José Antonio Hernández García.";
                    package.Workbook.Properties.Subject = @"Departamento de Informática.";
                    //package

                    //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                    package.Save();

                    try
                    {

                        Application excelApplication = new Application();

                        // ...

                        // open book in any format
                        Workbook workbook = excelApplication.Workbooks.Open(finalFileNameWithPath, XlUpdateLinks.xlUpdateLinksNever, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                        // save in XlFileFormat.xlExcel12 format which is XLSB
                        workbook.SaveAs(finalFileNameWithPath.Replace("xlsx", "xlsb"), XlFileFormat.xlExcel12, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                        // close workbook
                        workbook.Close(false, Type.Missing, Type.Missing);

                        // ...

                        // shutdown excel
                        excelApplication.Quit();
                    }
                    catch(Exception ex)
                    {
                        system_events.WriteEntry("Ocurrio el siguiente error: " + ex.Message);
                    }

                    //Aspose.Cells.Workbook wbAspose = new Aspose.Cells.Workbook(finalFileNameWithPath);
                    //wbAspose.Save(finalFileNameWithPath.Replace("xlsx", "xlsb"));

                    //m_resultConvert = DocumentConverter.Convert(finalFileNameWithPath, finalFileNameWithPath.Replace("xlsx", "xlsb"), DocumentFormat.Xlsb);

                    //if (File.Exists(finalFileNameWithPath))
                    //{
                    //    File.Delete(finalFileNameWithPath);
                    //}

                    ////~~> Define your Excel Objects
                    //Excel.Application xlApp = new Excel.Application();

                    //Excel.Workbook xlWorkBook;

                    ////~~> Start Excel and open the workbook.
                    //xlWorkBook = xlApp.Workbooks.Open(finalFileNameWithPath);

                    ////~~> Run the macros by supplying the necessary arguments
                    //xlApp.Run("Color_by_BOMs");

                    ////~~> Clean-up: Close the workbook
                    //xlWorkBook.Close(false);

                    ////~~> Quit the Excel Application
                    //xlApp.Quit();

                    ////~~> Clean Up
                    //releaseObject(xlApp);
                    //releaseObject(xlWorkBook);

                    //MessageBox.Show(string.Format("Archivo '{0}' generado éxitosamente.", fileName)
                    //    , "Archivo generado con éxito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    strFileName =  path = finalFileNameWithPath.Replace("xlsx", "xlsb");
                    //strFileName = path = finalFileNameWithPath;
                }
            }
            catch (Exception ex)
            {
                system_events.WriteEntry("Ocurrio un error al crear archivo excel. " + ex.Message + "\n\n" + ex.InnerException + "\n\n" + ex.StackTrace);
            }
            return result;
        }
        //~~> Release the objects
        public bool write_report_error(System.Data.DataTable dt, String fileName, string path, ref String strFileName, System.Diagnostics.EventLog system_events)
        {
            bool result = false;
            try
            {
                string finalFileNameWithPath = string.Empty;
                string finalFileNameWithPathXML = string.Empty;

                int mes = DateTime.Now.Month;
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

                //fileName = path + fileName;
                string strMonthName = mes.ToString();//mfi.GetMonthName(mes).ToString();
                m_utils.translateM(ref strMonthName);

                finalFileNameWithPath = path + "\\" + fileName;
                //Delete existing file with same file name.
                system_events.WriteEntry("El archivo se guardara como: " + finalFileNameWithPath);
                if (File.Exists(finalFileNameWithPath))
                    File.Delete(finalFileNameWithPath);

                var newFile = new FileInfo(finalFileNameWithPath);

                //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
                using (var package = new ExcelPackage(newFile))
                {
                    //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ASN's Report");


                    //Step 3 : Start loading datatable form A1 cell of worksheet.
                    worksheet.PrinterSettings.HeaderMargin = 0.5M;
                    worksheet.PrinterSettings.LeftMargin = 0.35M;
                    worksheet.PrinterSettings.BottomMargin = 0.5M;
                    worksheet.PrinterSettings.RightMargin = 0.3M;

                    worksheet.PrinterSettings.PaperSize = ePaperSize.Letter;
                    worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                    worksheet.Cells["A1:L1"].Style.Font.Bold = true;
                    //worksheet.Cells["A1:L1"].Style.Font.
                    worksheet.Cells["A1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A1:L1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    //worksheet.ConditionalFormatting.
                    worksheet.Cells["J2:J" + (dt.Rows.Count + 1).ToString()].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells["K2:K" + (dt.Rows.Count + 1).ToString()].Style.Numberformat.Format = "dd/mm/yyyy HH:mm:ss";
                    //worksheet.Cells["J1:L1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                    worksheet.Cells.AutoFitColumns();
                    worksheet.Cells["A1:L1"].AutoFilter = true;

                    //worksheet.Select("A2:O" + dt.Rows.Count + 1);

                    //package.Workbook.CreateVBAProject();

                    //StringBuilder vbaCode = new StringBuilder();

                    //vbaCode.AppendLine("Sub Color_by_BOMs()");
                    //vbaCode.AppendLine("Application.ScreenUpdating = False");
                    //vbaCode.AppendLine("Nuevo = False");
                    //vbaCode.AppendLine("Total = Selection.Rows.Count");
                    //vbaCode.AppendLine("ActiveCell.Offset(1, 0).Select");
                    //vbaCode.AppendLine("For A = 1 To Total - 1");
                    //vbaCode.AppendLine("Actual = ActiveCell.Value");
                    //vbaCode.AppendLine("Arriba = ActiveCell.Offset(-1, 0).Value");
                    //vbaCode.AppendLine("If Actual = Arriba Then");
                    //vbaCode.AppendLine("If Nuevo Then");
                    //vbaCode.AppendLine("Selection.EntireRow.Select");
                    //vbaCode.AppendLine("With Selection.Interior");
                    //vbaCode.AppendLine(".ColorIndex = 0");
                    //vbaCode.AppendLine("'.Pattern = xlGray25");
                    //vbaCode.AppendLine("'.Pattern = xlCrissCross");
                    //vbaCode.AppendLine(".PatternColorIndex = 37");
                    //vbaCode.AppendLine(".ThemeColor = xlThemeColorAccent5");
                    //vbaCode.AppendLine(".TintAndShade = 0.599993896298105");
                    //vbaCode.AppendLine(".PatternTintAndShade = 0");
                    //vbaCode.AppendLine("End With");
                    //vbaCode.AppendLine("End If");
                    //vbaCode.AppendLine("Else");
                    //vbaCode.AppendLine("If Nuevo Then");
                    //vbaCode.AppendLine("Nuevo = False");
                    //vbaCode.AppendLine("Else");
                    //vbaCode.AppendLine("Selection.EntireRow.Select");
                    //vbaCode.AppendLine("With Selection.Interior");
                    //vbaCode.AppendLine(".ColorIndex = 0");
                    //vbaCode.AppendLine("'.Pattern = xlGray25");
                    //vbaCode.AppendLine("'.Pattern = xlCrissCross");
                    //vbaCode.AppendLine(".PatternColorIndex = 37");
                    //vbaCode.AppendLine(".ThemeColor = xlThemeColorAccent5");
                    //vbaCode.AppendLine(".TintAndShade = 0.599993896298105");
                    //vbaCode.AppendLine(".PatternTintAndShade = 0");
                    //vbaCode.AppendLine("End With");
                    //vbaCode.AppendLine("Nuevo = True");
                    //vbaCode.AppendLine("End If");
                    //vbaCode.AppendLine("End If");
                    //vbaCode.AppendLine("ActiveCell.Offset(1, 0).Select");
                    //vbaCode.AppendLine("Next");
                    //vbaCode.AppendLine("Application.ScreenUpdating = True");
                    //vbaCode.AppendLine("End Sub");

                    //package.Workbook.CodeModule.Code = vbaCode.ToString();
                    //package.Workbook.CodeModule.

                    //Step 4 : (Optional) Set the file properties like title, author and subject
                    package.Workbook.Properties.Title = @"ASNs Report";
                    package.Workbook.Properties.Author = @"SIIX Corp., II. José Antonio Hernández García.";
                    package.Workbook.Properties.Subject = @"Departamento de Informática.";
                    //package

                    //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                    package.Save();

                    //m_resultConvert = DocumentConverter.Convert(finalFileNameWithPath, finalFileNameWithPath.Replace("xlsx", "xlsb"), DocumentFormat.Xlsb);

                    //if (File.Exists(finalFileNameWithPath))
                    //{
                    //    File.Delete(finalFileNameWithPath);
                    //}

                    ////~~> Define your Excel Objects
                    //Excel.Application xlApp = new Excel.Application();

                    //Excel.Workbook xlWorkBook;

                    ////~~> Start Excel and open the workbook.
                    //xlWorkBook = xlApp.Workbooks.Open(finalFileNameWithPath);

                    ////~~> Run the macros by supplying the necessary arguments
                    //xlApp.Run("Color_by_BOMs");

                    ////~~> Clean-up: Close the workbook
                    //xlWorkBook.Close(false);

                    ////~~> Quit the Excel Application
                    //xlApp.Quit();

                    ////~~> Clean Up
                    //releaseObject(xlApp);
                    //releaseObject(xlWorkBook);

                    //MessageBox.Show(string.Format("Archivo '{0}' generado éxitosamente.", fileName)
                    //    , "Archivo generado con éxito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //strFileName = path = finalFileNameWithPath.Replace("xlsx", "xlsb");
                    strFileName = path = finalFileNameWithPath;
                }
            }
            catch (Exception ex)
            {
                system_events.WriteEntry("Ocurrio un error al crear archivo excel. " + ex.Message + "\n\n" + ex.InnerException + "\n\n" + ex.StackTrace);
            }
            return result;
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        public void exportToCSV(System.Data.DataTable sourceTable, TextWriter writer, bool includeHeaders)
        {
            if (includeHeaders)
            {
                IEnumerable<String> headerValues = sourceTable.Columns
                    .OfType<DataColumn>()
                    .Select(column => QuoteValue(column.ColumnName));

                writer.WriteLine(String.Join(",", headerValues));
            }

            IEnumerable<String> items = null;

            foreach (DataRow row in sourceTable.Rows)
            {
                //items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                items = row.ItemArray.Select(o => o.ToString());
                writer.WriteLine(String.Join(",", items));
            }

            writer.Flush();
        }

        private string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

        #region excel
        public bool checkSheetOLE(System.Data.DataTable info, ref string TextFail)
        {

            bool result = true;

            if (info.Columns.Count != 28)////conversa
            //if (info.Columns.Count != 12)///sto
            {
                TextFail = "El número de columnas en la hoja de datos no es el correcto.";
                return false;
            }

            result = true;

            return result;
        }
        public System.Data.DataTable ReadSheet(string sheetName)
        {
            using (OleDbConnection conn = new OleDbConnection())
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                string fileExtension = System.IO.Path.GetExtension(Path);
                if (fileExtension == ".xls")
                    conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path + ";" + "Extended Properties='Excel 8.0;HDR=YES;'";
                if (fileExtension == ".xlsx")
                    conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                using (OleDbCommand comm = new OleDbCommand())
                {
                    comm.CommandText = "Select * from [" + sheetName + "]";

                    comm.Connection = conn;

                    using (OleDbDataAdapter da = new OleDbDataAdapter())
                    {
                        da.SelectCommand = comm;
                        da.Fill(dt);
                        return dt;
                    }

                }
            }
        }
        public void loadBookExcel(/*ref DataTable listSheet*/)
        {
            OleDbConnectionStringBuilder sbConnection = new OleDbConnectionStringBuilder();
            String strExtendedProperties = String.Empty;
            sbConnection.DataSource = Path;
            //listSheet = new DataTable();
            m_book.Columns.Add("TABLE_NAME");
            if (System.IO.Path.GetExtension(Path).Equals(".xls"))//for 97-03 Excel file
            {
                sbConnection.Provider = "Microsoft.Jet.OLEDB.4.0";
                strExtendedProperties = "Excel 8.0;HDR=Yes;IMEX=1";//HDR=ColumnHeader,IMEX=InterMixed
            }
            else if (System.IO.Path.GetExtension(Path).Equals(".xlsx"))  //for 2007 Excel file
            {
                sbConnection.Provider = "Microsoft.ACE.OLEDB.12.0";
                strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";
            }
            sbConnection.Add("Extended Properties", strExtendedProperties);
            using (OleDbConnection conn = new OleDbConnection(sbConnection.ToString()))
            {
                conn.Open();
                System.Data.DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                m_book.Clear();
                foreach (DataRow drSheet in dtSheet.Rows)
                {
                    if (drSheet["TABLE_NAME"].ToString().Contains("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                    {
                        m_book.Rows.Add(drSheet["TABLE_NAME"].ToString());
                    }
                }
            }
            //return listSheet;
        }
        #endregion

    }
}