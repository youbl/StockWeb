﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using StockWeb.Model;

namespace StockWeb.Services
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 读取上市辅导企业
        /// </summary>
        /// <param name="xslFile"></param>
        /// <returns></returns>
        public static List<ReadyEnt> ReadReadyExcel(string xslFile)
        {
            var ret = new List<ReadyEnt>();
            using (FileStream fs = new FileStream(xslFile, FileMode.Open, FileAccess.Read))
            {
                var workbook = WorkbookFactory.Create(fs); // new XSSFWorkbook();
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    throw new FileLoadException("Excel文件中没有Sheet", xslFile);
                }


                // IRow firstRow = sheet.GetRow(0);
                // 一行最后一个cell的编号 即总列数
                // int cellCount = firstRow.LastCellNum;

                // 第一行是标题列名
                for (int i = sheet.FirstRowNum + 3; i <= sheet.LastRowNum; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                        continue; //没有数据的行默认是null
                    // 序号	企业名称	注册地	主营业务	保荐机构	通过发审日期/申报IPO日期/备案日期	辅导工作进度	备注
                    var stock = new ReadyEnt();
                    ret.Add(stock);
                    stock.Sn = GetCellValue(row.GetCell(0));
                    stock.Name = GetCellValue(row.GetCell(1));
                    stock.City = GetCellValue(row.GetCell(2));
                    stock.Business = GetCellValue(row.GetCell(3));
                    stock.Org = GetCellValue(row.GetCell(4));
                    stock.Date = GetCellValue(row.GetCell(5));
                    stock.Process = GetCellValue(row.GetCell(6));
                    stock.Memo = GetCellValue(row.GetCell(7));
                    // for (int j = row.FirstCellNum; j < cellCount; ++j)
                }
            }
            return ret;
        }


        /// <summary>
        /// 读取上市企业
        /// </summary>
        /// <param name="xslFile"></param>
        /// <returns></returns>
        public static List<ListedEnt> ReadListedExcel(string xslFile)
        {
            var ret = new List<ListedEnt>();
            using (FileStream fs = new FileStream(xslFile, FileMode.Open, FileAccess.Read))
            {
                var workbook = WorkbookFactory.Create(fs); // new XSSFWorkbook();
                var sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                {
                    throw new FileLoadException("Excel文件中没有Sheet", xslFile);
                }


                // IRow firstRow = sheet.GetRow(0);
                // 一行最后一个cell的编号 即总列数
                // int cellCount = firstRow.LastCellNum;

                // 第一行是标题列名
                for (int i = sheet.FirstRowNum + 3; i <= sheet.LastRowNum; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                        continue; //没有数据的行默认是null
                    // 序号	证券代码	证券简称	办公地址	公司电话	董 秘	上市日期
                    var stock = new ListedEnt();
                    ret.Add(stock);
                    stock.Sn = GetCellValue(row.GetCell(0));
                    stock.Code = GetCellValue(row.GetCell(1));
                    stock.Name = GetCellValue(row.GetCell(2));
                    stock.Address = GetCellValue(row.GetCell(3));
                    stock.Tel = GetCellValue(row.GetCell(4));
                    stock.Secretary = GetCellValue(row.GetCell(5));
                    stock.Date = GetCellValue(row.GetCell(6));
                }
            }
            return ret;
        }


        static string GetCellValue(ICell cell)
        {
            //cell.SetCellType(CellType.String);
            //return cell.StringCellValue;

            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue.ToString();
                case CellType.Numeric: //NUMERIC:
                    // if (DateUtil.IsCellDateFormatted(cell)) 这个判断无效
                    short format = cell.CellStyle.DataFormat;
                    if (format != 0)
                    {
                        return cell.DateCellValue.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        return cell.NumericCellValue.ToString("N").Trim('0', '.');
                    }
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue.ToString();
                // case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }


        /// <summary>
        /// 获取当前单元格所在的合并单元格的位置
        /// </summary>
        /// <param name="sheet">sheet表单</param>
        /// <param name="rowIndex">行索引 0开始</param>
        /// <param name="colIndex">列索引 0开始</param>
        /// <param name="start">合并单元格左上角坐标</param>
        /// <param name="end">合并单元格右下角坐标</param>
        /// <returns>返回false表示非合并单元格</returns>
        public static bool IsMergeCell(ISheet sheet, int rowIndex, int colIndex, out Point start, out Point end)
        {
            bool result = false;
            start = new Point(0, 0);
            end = new Point(0, 0);
            if ((rowIndex < 0) || (colIndex < 0))
                return false;
            int regionsCount = sheet.NumMergedRegions;
            for (int i = 0; i < regionsCount; i++)
            {
                CellRangeAddress range = sheet.GetMergedRegion(i);
                //sheet.IsMergedRegion(range); 
                if (rowIndex >= range.FirstRow && rowIndex <= range.LastRow && colIndex >= range.FirstColumn && colIndex <= range.LastColumn)
                {
                    start = new Point(range.FirstRow, range.FirstColumn);
                    end = new Point(range.LastRow, range.LastColumn);
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取颜色值
        /// </summary>
        /// <param name="color">颜色RGB</param>
        /// <param name="workbook">Excel画布</param>
        /// <returns></returns>
        public static string GetColorIndex(HSSFWorkbook workbook, Color color)
        {
            HSSFPalette palette = workbook.GetCustomPalette();
            var v = palette.FindSimilarColor(color.R, color.G, color.B);
            if (v == null)
            {
                throw new Exception("Color is not in Palette");
            }
            else return v.GetHexString();
        }


        public static void ToExcel<T>(IEnumerable<T> arr, string filename)
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);


            //创建Excel文件名称
            using (FileStream fs = File.Create(filename))
            {
                //创建工作薄
                IWorkbook workbook = new XSSFWorkbook();

                //创建sheet
                ISheet sheet = workbook.CreateSheet("sheet0");

                //依次创建行和列
                var rowNum = 0;
                foreach (var item in arr)
                {
                    IRow row = sheet.CreateRow(rowNum);
                    var colNum = 0;
                    foreach (var prop in props)
                    {
                        ICell cell = row.CreateCell(colNum);
                        var val = GetPropVal(item, prop);
                        cell.SetCellValue(val);
                        colNum++;
                    }
                    rowNum++;
                }

                //向excel文件中写入数据并保保存
                workbook.Write(fs);
            }
        }

        static string GetPropVal(object obj, PropertyInfo info)
        {
            var ret = info.GetValue(obj);
            return Convert.ToString(ret);
        }
    }
}