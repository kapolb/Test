using System;
using System.IO;
using System.Windows.Forms;
using Kontakt.DataModel;
using System.Collections.Generic;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace ExcelBL
{
    /// <summary>
    /// The main class for manipulating with excel, it means creating, filling, ect..
    /// </summary>
    public class ExcelManager
    {

        //NOTE: for testing purpose
        const string TestFile = "C:\\test.xls";
        private const string SheetName = "sheet";

        private Template _settings;

        /// <summary>
        /// Gets or sets the template directory path.
        /// </summary>
        /// <value>
        /// The template directory path.
        /// </value>
        public string TemplateDirPath { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="ExcelManager"/> class from being created by constructor.
        /// </summary>
        private ExcelManager()
        {
        }

        /// <summary>
        /// Creates the <see cref="ExcelManager"/> instance and sets the specified template directory.
        /// </summary>
        /// <param name="templateDirectoryPath">The template directory path.</param>
        /// <returns></returns>
        public static ExcelManager CreateManagerWithTemplate(string templateDirectoryPath)
        {
            if (!Directory.Exists(templateDirectoryPath))
                throw new DirectoryNotFoundException(string.Format("The tepmplate directory '{0}' was not found!", templateDirectoryPath));

            return new ExcelManager { TemplateDirPath = templateDirectoryPath };
        }

        public byte[] GenerateReportContent(string productCode, Items data)
        {
            MemoryStream str = (MemoryStream)this.GenerateReport(productCode, data);
            return str.ToArray();
        }

        public Stream GenerateReport(string productCode, Items data)
        {
            try
            {
                //initial check for argument
                if (string.IsNullOrEmpty(productCode) || productCode.Length != 3)
                    throw new ArgumentException("productCode");

                //load template settings for specified product code
                _settings = Serializator.Deserialize(Path.Combine(TemplateDirPath, string.Format("{0}.xml", productCode)));

                // Opening the Excel template...
                var fs = new FileStream(Path.Combine(TemplateDirPath, string.Format("{0}.xls", productCode)), FileMode.Open, FileAccess.Read);

                // Getting the complete workbook...
                var templateWorkbook = new HSSFWorkbook(fs, true);

                // Getting the worksheet by its name...
                var sheet = (HSSFSheet)templateWorkbook.GetSheet(SheetName);

                if (sheet == null) throw new NullReferenceException(string.Format("Excel sheet was not found! The sheet name: {0}.", SheetName));

                //fill all data to excel template
                FillSheetWithData(templateWorkbook, sheet, data);

                // Forcing formula recalculation...
                sheet.ForceFormulaRecalculation = true;

                MemoryStream ms = new MemoryStream();

                // Writing the workbook content to the FileStream...
                templateWorkbook.Write(ms);

                //NOTE: for testing purpose
                string saveAllowed = System.Configuration.ConfigurationManager.AppSettings["SaveGeneratedFile"];
                if (!string.IsNullOrEmpty(saveAllowed) &&
                    (saveAllowed.Equals("1") || saveAllowed.ToUpper().Equals("YES") || saveAllowed.ToUpper().Equals("TRUE")))
                {
                    if (File.Exists(TestFile)) File.Delete(TestFile);
                    using (var fileStream = File.Create(TestFile))
                    {
                        ms.WriteTo(fileStream);
                        //templateWorkbook.Write(fileStream);
                    }
                }

                // Sending the server processed data back to the user computer...
                return ms;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Generating failed! Unexpected exception: {0}", ex.Message),
                                "Generating failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void FillSheetWithData(HSSFWorkbook workbook, HSSFSheet sheet, Items data)
        {
            //fill date of generating
            SetValue(sheet, _settings.CreateDate, DateTime.Today);

            //fill order dates and delivery dates for each order
            FillOrdersDates(sheet, data.Orders);

            //fill data cells
            for (var i = 0; i < data.Rows.Count; i++)
            {
                var dataRow = data.Rows[i];
                var row = CopyRow(workbook, sheet, _settings.FirstRow, _settings.FirstRow + i + 1);

                //fill data from data contract first 
                SetRowStringValue(row, _settings.Values.Code, dataRow.Code);
                SetRowStringValue(row, _settings.Values.Name, dataRow.Name);
                SetRowNumericValue(row, _settings.Values.KgPerUnit, dataRow.KgPerUnit);
                SetRowNumericValue(row, _settings.Values.Usage2010, dataRow.Usages.ContainsKey("2010") ? dataRow.Usages["2010"] : 0);
                SetRowNumericValue(row, _settings.Values.Usage2011, dataRow.Usages.ContainsKey("2011") ? dataRow.Usages["2011"] : 0);
                SetRowNumericValue(row, _settings.Values.OnStock, dataRow.OnStock);

                for (var j = 0; j < _settings.Values.Ordered.Count; j++)
                {
                    if (j == _settings.Orders.Count) throw new IndexOutOfRangeException("Orders count is bigger then template can process");
                    if (j >= data.Orders.Count)
                    {
                        SetRowNumericValue(row, _settings.Values.Ordered[j], 0);
                        continue;
                    }
                    var order = dataRow.Ordered.SingleOrDefault(o => o.Id == data.Orders[j].Id);
                    SetRowNumericValue(row, _settings.Values.Ordered[j], order != null ? order.Value : 0);
                }

                //fill formulas to other specific fields
                foreach (var formula in _settings.Formulas)
                    SetRowFormula(row, formula.Cell, string.Format(formula.Value, _settings.FirstRow + i + 2));
            }

            //set total order sum at the end of the document
            SetRowFormula((HSSFRow)sheet.GetRow(_settings.FirstRow + data.Rows.Count + 3),
                _settings.OrderedSum.Cell,
                string.Format(_settings.OrderedSum.Value, _settings.FirstRow + 2, _settings.FirstRow + data.Rows.Count + 1));
        }

        private void SetRowFormula(HSSFRow row, string column, string formula)
        {
            row.GetCell(Convert.ToInt32(column)).CellFormula = formula;
        }

        /// <summary>
        /// HSSFRow Copy Command
        ///
        /// Description:  Inserts a existing row into a new row, will automatically push down
        ///               any existing rows.  Copy is done cell by cell and supports, and the
        ///               command tries to copy all properties available (style, merged cells, values, etc...)
        /// </summary>
        /// <param name="workbook">Workbook containing the worksheet that will be changed</param>
        /// <param name="worksheet">WorkSheet containing rows to be copied</param>
        /// <param name="sourceRowNum">Source Row Number</param>
        /// <param name="destinationRowNum">Destination Row Number</param>
        private HSSFRow CopyRow(HSSFWorkbook workbook, HSSFSheet worksheet, int sourceRowNum, int destinationRowNum)
        {
            // Get the source / new row
            var newRow = (HSSFRow)worksheet.GetRow(destinationRowNum);
            var sourceRow = (HSSFRow)worksheet.GetRow(sourceRowNum);

            // If the row exist in destination, push down all rows by 1 else create a new row
            if (newRow != null)
            {
                worksheet.ShiftRows(destinationRowNum, worksheet.LastRowNum, 1);
            }
            else
            {
                newRow = (HSSFRow)worksheet.CreateRow(destinationRowNum);
            }

            // Loop through source columns to add to new row
            for (var i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                var oldCell = (HSSFCell)sourceRow.GetCell(i);
                var newCell = (HSSFCell)newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null) continue;

                // Copy style from old cell and apply to new cell
                var newCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                newCell.CellStyle = newCellStyle;

                // If there is a cell comment, copy
                if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data type
                newCell.SetCellType(oldCell.CellType);

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    case CellType.BLANK:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case CellType.BOOLEAN:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case CellType.ERROR:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case CellType.FORMULA:
                        newCell.CellFormula = oldCell.CellFormula;
                        break;
                    case CellType.NUMERIC:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case CellType.STRING:
                        newCell.SetCellValue(oldCell.RichStringCellValue);
                        break;
                    case CellType.Unknown:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                }
            }

            // If there are are any merged regions in the source row, copy to new row
            for (var i = 0; i < worksheet.NumMergedRegions; i++)
            {
                var cellRangeAddress = worksheet.GetMergedRegion(i);
                if (cellRangeAddress.FirstRow != sourceRow.RowNum) continue;
                var newCellRangeAddress = new CellRangeAddress(newRow.RowNum,
                                                               (newRow.RowNum +
                                                                (cellRangeAddress.FirstRow -
                                                                 cellRangeAddress.LastRow)),
                                                               cellRangeAddress.FirstColumn,
                                                               cellRangeAddress.LastColumn);
                worksheet.AddMergedRegion(newCellRangeAddress);
            }

            return newRow;
        }

        private void FillOrdersDates(HSSFSheet sheet, IList<OrderedItems> orderItems)
        {
            for (var i = 0; i < orderItems.Count; i++)
            {
                if (i == _settings.Orders.Count) throw new IndexOutOfRangeException("Orders info count is bigger then template can process");
                if (orderItems[i].OrderDate != null) SetValue(sheet, _settings.Orders[i].OrderDate, (DateTime)orderItems[i].OrderDate);
                if (orderItems[i].DeliveryDateSK != null) SetValue(sheet, _settings.Orders[i].DeliveryDate, (DateTime)orderItems[i].DeliveryDateSK);
            }
        }

        private void SetRowNumericValue(HSSFRow row, string column, double value)
        {
            ((HSSFCell)row.GetCell(Convert.ToInt32(column))).SetCellValue(value);
        }

        private void SetRowStringValue(HSSFRow row, string column, string value)
        {
            ((HSSFCell)row.GetCell(Convert.ToInt32(column))).SetCellValue(value);
        }

        private void SetValue(HSSFSheet sheet, string coordinates, DateTime value)
        {
            GetCell(sheet, coordinates).SetCellValue(value);
        }

        private static HSSFCell GetCell(HSSFSheet sheet, string cellCoordinates)
        {
            if (!cellCoordinates.Contains(",")) throw new FormatException(string.Format("Invalid cell coordinates: {0}!", cellCoordinates));
            var coordinates = cellCoordinates.Split(',');
            return (HSSFCell)((HSSFRow)sheet.GetRow(Convert.ToInt32(coordinates[0]))).GetCell(Convert.ToInt32(coordinates[1]));
        }
    }
}
