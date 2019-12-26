using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace LIN.MSA.Infrastructure
{
    public class ExcelUtil
    {
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="format">字段格式: Name|姓名;Age|年纪</param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public string ExportExcel(DataTable dt,string format, string TableName)
        {
            var list = new List<ColumnFormat>();
            foreach (var li in format.Split(';'))
            {
                var item = li.Split('|');
                ColumnFormat form = new ColumnFormat();
                form.Field = item[0];
                form.Title = item[1];
                list.Add(form);
            }
            // 移除列列表
            var removeList = new List<ColumnFormat>();
            foreach (DataColumn item in dt.Columns)
            {
                var model = list.Where(c => c.Field == item.ColumnName).FirstOrDefault();
                if (model != null)
                {
                    item.ColumnName = model.Title;
                }
                else
                {
                    ColumnFormat rem = new ColumnFormat();
                    rem.Field = item.ColumnName;
                    rem.Title = "";
                    removeList.Add(rem);
                }
            }

            foreach (var reItem in removeList)
            {
                dt.Columns.Remove(reItem.Field);
            }

            var result = "";
            result = ExportExcel(dt, TableName);

            return result;
        }

        public string ExportExcel(DataTable dt, string TableName)
        {
            var result = "";
            IWorkbook workBook = null;

            if (dt.Rows.Count > 0)
            {
                try
                {
                    workBook = new XSSFWorkbook();//创建一个新的Excel

                    int sheetcount = dt.Rows.Count / 65000;//判断记录数是65000条/选项卡的倍数

                    for (int i = 0; i <= sheetcount; i++)
                    {
                        ISheet sheet = workBook.CreateSheet();//创建sheet页
                        workBook.SetSheetName(i, TableName + (i + 1).ToString());//sheet页名称

                        ICellStyle cellStyle = workBook.CreateCellStyle();
                        IFont font = workBook.CreateFont();
                        font.FontName = "微软雅黑";
                        cellStyle.SetFont(font);
                        cellStyle.BorderBottom = BorderStyle.Thin;
                        cellStyle.BorderLeft = BorderStyle.Thin;
                        cellStyle.BorderRight = BorderStyle.Thin;
                        cellStyle.BorderTop = BorderStyle.Thin;

                        ICellStyle headerStyle = workBook.CreateCellStyle();
                        IFont hfont = workBook.CreateFont();
                        hfont.FontName = "微软雅黑";
                        headerStyle.SetFont(hfont);

                        headerStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                        headerStyle.FillPattern = FillPattern.SolidForeground;
                        headerStyle.BorderBottom = BorderStyle.Thick;
                        headerStyle.BorderLeft = BorderStyle.Thick;
                        headerStyle.BorderRight = BorderStyle.Thick;
                        headerStyle.BorderTop = BorderStyle.Thick;

                        #region 设置第一行为Header
                        int ColumnTotalCount = dt.Columns.Count;
                        sheet.CreateFreezePane(0, 1, 0, 1); //冻结行
                        IRow row = sheet.CreateRow(0);//设置第一行为Header
                        row.HeightInPoints = 20;
                        for (int j = 0; j < ColumnTotalCount; j++)
                        {
                            ICell cellj = row.CreateCell(j);
                            cellj.SetCellValue(dt.Columns[j].ToString());
                            cellj.CellStyle = headerStyle;

                            sheet.SetColumnWidth(j, 25 * 256);
                        }
                        #endregion
                        //设置数据
                        int k = 1;
                        for (int kk = (65000 * i); kk < (65000 * (i + 1)); kk++)
                        {
                            if (kk < dt.Rows.Count)
                            {
                                IRow hr = sheet.CreateRow(k);//要先定义行，不然像注释那样，那样写下面设置列自动会无效
                                for (int kkk = 0; kkk < dt.Columns.Count; kkk++)
                                {
                                    ICell hc = hr.CreateCell(kkk);
                                    hc.SetCellValue(dt.Rows[kk][kkk].ToString());
                                    hc.CellStyle = cellStyle;
                                }
                            }
                            else
                            {
                                break;
                            }
                            k++;
                        }
                        //for (int autoCn = 0; autoCn < ColumnTotalCount; autoCn++)
                        //{
                        //    sheet.AutoSizeColumn((short)autoCn); //调整每列宽度
                        //}
                        //sheet.ForceFormulaRecalculation = true;
                    }

                    string fileName = TableName + DateTime.Now.ToString("yyyyMMddHHMM") + DateTime.Now.Second.ToString() + ".xls";

                    string directory = AppDomain.CurrentDomain.BaseDirectory;

                    var path = directory + "/ExportFile";

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullFileName = directory + "/ExportFile/" + fileName;

                    FileStream file = new FileStream(fullFileName, FileMode.Create);//保存
                    workBook.Write(file);
                    file.Close();
                    file.Dispose();
                    if (File.Exists(fullFileName))
                    {
                        result = fileName;
                    }
                }
                catch (Exception ee)
                {
                    //LogUtils.ErrorLog("[后台]" + TableName + "导出Excel->异常" + ee.ToString());
                    throw ee;
                }
                finally
                {
                    if (workBook != null)
                    {
                        workBook.Close();
                        workBook = null;
                    }
                }
            }
            else
            {
                result = "没有相应的数据！";
            }
            return result;
        }

    }

    public class ColumnFormat
    {
        public string Field { get; set; }

        public string Title { get; set; }
    }
}
