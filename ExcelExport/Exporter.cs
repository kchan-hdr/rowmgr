using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExport
{   using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Drawing;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;
    using System.IO;
    using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;
    using A = DocumentFormat.OpenXml.Drawing;

    /// <summary>
    /// output formatted excel for b2h
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Exporter<T>
    {
        protected IEnumerable<T> items;
        protected string reportname = "Unknown";
        protected WorkbookPart bookPart;
        protected Sheets sheets;
        protected string logoPath;

        public Exporter(IEnumerable<T> data) {
            items = data;
        }

        public Exporter(IEnumerable<T> data, string logo)
        {
            items = data;
            logoPath = logo;
        }

        virtual public byte[] Export()
        {
            using (var memory = new MemoryStream())
            {
                var doc = MakeDoc(memory);
                bookPart = doc.AddWorkbookPart();
                bookPart.Workbook = new Workbook();
                sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                MakeStyles();

                WriteCoverPage(1, reportname);
                Write(2);

                doc.Close();

                return memory.ToArray();
            }
        }
        virtual protected void Write(uint pageId) { }

        #region implementation
        void WriteCoverPage(uint id, string name)
        {
            var p = bookPart.AddNewPart<WorksheetPart>($"uId{id}");
            var d = new SheetData();
            p.Worksheet = new Worksheet(d);

            uint rowId = 1;

            var r = InsertRow(rowId, d);
            WriteText(r, "A", name, 1);
            WriteText(r, "B", DateTime.Now.ToLongDateString());
            WriteText(r, "C", DateTime.Now.ToLongTimeString());

            rowId += 2;

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                this.insertLogo(p, logoPath, 1, rowId);
            }
        
            sheets.Append(new Sheet { Id = bookPart.GetIdOfPart(p), SheetId = id, Name = name });
            bookPart.Workbook.Save();
        }

        protected uint WriteLogo(uint rowId, WorksheetPart p, SheetData d, string name)
        {
            var r = InsertRow(rowId++, d);
            WriteText(r, "A", name, 1);
            WriteText(r, "B", DateTime.Now.ToLongDateString());
            WriteText(r, "C", DateTime.Now.ToLongTimeString());

            r = InsertRow(rowId++, d);

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                this.insertLogo(p, logoPath, 1, rowId);
            }

            return rowId += 5;
        }

        /// <summary>
        /// https://code.msdn.microsoft.com/office/How-to-insert-image-into-93964561
        /// </summary>
        /// <param name="p"></param>
        /// <param name="logoPath"></param>
        void insertLogo(WorksheetPart p, string logoPath, uint colNumber =1, uint rowNumber = 1)
        {
            var drawingsPart = p.AddNewPart<DrawingsPart>();
            if ( !p.Worksheet.ChildElements.OfType<Drawing>().Any())
            {
                p.Worksheet.Append(new Drawing { Id = p.GetIdOfPart(drawingsPart) });
            }

            if ( drawingsPart.WorksheetDrawing==null)
            {
                drawingsPart.WorksheetDrawing = new DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing();
            }

            var worksheetDrawing = drawingsPart.WorksheetDrawing;
            var imagePart = drawingsPart.AddImagePart(ImagePartType.Png);   // PNG
            using (var s = new FileStream(logoPath, FileMode.Open))
            {
                imagePart.FeedData(s);
            }

            // bogus
            var bm = new System.Drawing.Bitmap(logoPath);
            var extents = new DocumentFormat.OpenXml.Drawing.Extents();
            var extentsCx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
            var extentsCy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
            bm.Dispose();

            var colOffset = 0;
            var rowOffset = 0;

            var nvps = worksheetDrawing.Descendants<Xdr.NonVisualDrawingProperties>();
            var nvpId = nvps.Count() > 0 ?
                (UInt32Value)worksheetDrawing.Descendants<Xdr.NonVisualDrawingProperties>().Max(prop => prop.Id.Value) + 1 :
                1U;

            var oneCellAnchor = new Xdr.OneCellAnchor(
                new Xdr.FromMarker
                {
                    ColumnId = new Xdr.ColumnId((colNumber - 1).ToString()),
                    RowId = new Xdr.RowId((rowNumber - 1).ToString()),
                    ColumnOffset = new Xdr.ColumnOffset(colOffset.ToString()),
                    RowOffset = new Xdr.RowOffset(rowOffset.ToString())
                },
                new Xdr.Extent { Cx = extentsCx, Cy = extentsCy },
                new Xdr.Picture(
                    new Xdr.NonVisualPictureProperties(
                        new Xdr.NonVisualDrawingProperties { Id = nvpId, Name = "Picture " + nvpId, Description = logoPath },
                        new Xdr.NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true })
                    ),
                    new Xdr.BlipFill(
                        new A.Blip { Embed = drawingsPart.GetIdOfPart(imagePart), CompressionState = A.BlipCompressionValues.Print },
                        new A.Stretch(new A.FillRectangle())
                    ),
                    new Xdr.ShapeProperties(
                        new A.Transform2D(
                            new A.Offset { X = 0, Y = 0 },
                            new A.Extents { Cx = extentsCx, Cy = extentsCy }
                        ),
                        new A.PresetGeometry { Preset = A.ShapeTypeValues.Rectangle }
                    )
                ),
                new Xdr.ClientData()
            );

            worksheetDrawing.Append(oneCellAnchor);
        }
        #endregion

        #region helpers
        static protected string GetColumnCode(int c = 0) => ((char)('A' + c)).ToString();   // only up to 24 columns

        static SpreadsheetDocument MakeDoc(Stream s) => SpreadsheetDocument.Create(s, SpreadsheetDocumentType.Workbook);

        static SpreadsheetDocument MakeDoc(string path) => SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);

        static protected Cell WriteDate(Row row, string c, DateTime date)
        {
            var cell = InsertCell(row, c);
            cell.CellValue = new CellValue(date.ToOADate().ToString());
            cell.DataType = CellValues.Number;
            cell.StyleIndex = 2;
            return cell;
        }

        static protected Cell WriteDate(Row row, string c, DateTimeOffset? date)
        {
            CellValue myValue;
            if (date == null)
            {
                myValue = new CellValue("");
            }
            else
            {
                myValue = new CellValue(date.Value.LocalDateTime.ToOADate().ToString());
            }

            var cell = InsertCell(row, c);
            cell.CellValue = myValue;
            cell.DataType = CellValues.Number;
            cell.StyleIndex = 2;
            return cell;
        }

        static protected Cell WriteNumber(Row row, string c, string text) => WriteCell(row, c, text, CellValues.Number);
        static protected Cell WriteTrueFalse(Row row, string c, string text) => WriteCell(row, c, text, CellValues.Boolean);
        static protected Cell WriteText(Row row, string c, string text, uint? styleIndex = null)
        {
            var cell = WriteCell(row, c, text, CellValues.String);
            if (styleIndex.HasValue)
                cell.StyleIndex = styleIndex.Value;
            return cell;
        }

        static protected Cell WriteCell(Row row, string c, string text, CellValues cell_t)
        {
            var cell = InsertCell(row, c);
            cell.DataType = cell_t;
            cell.CellValue = new CellValue(text);
            return cell;
        }
        static protected Cell InsertCell(Row row, string c)
        {
            var myRange = $"{c}{row.RowIndex}";
            Cell cell;
            if (row.Elements<Cell>().Count(cx => cx.CellReference.Value == myRange) > 0)
            {
                cell = row.Elements<Cell>().First(cx => cx.CellReference.Value == myRange);
            }
            else
            {
                Cell refc = null;
                foreach (var cx in row.Elements<Cell>())
                {
                    if (cx.CellReference.Value.Length == myRange.Length)
                    {
                        if (string.Compare(cx.CellReference.Value, myRange, true) > 0)
                        {
                            refc = cx;
                            break;
                        }
                    }
                }

                cell = new Cell { CellReference = myRange };
                row.InsertBefore(cell, refc);
            }
            return cell;
        }

        static protected Row InsertRow(uint r, SheetData d)
        {
            Row row;
            if ( d.Elements<Row>().Where( rx=> rx.RowIndex==r).Count() <= 0)
            {
                row = new Row() { RowIndex = r };
                d.Append(row);
            }
            else
            {
                row = d.Elements<Row>().First(rx => rx.RowIndex == r);
            }
            return row;
        }
        #endregion
        #region format helper
        void MakeStyles()
        {
            var myStyles = bookPart.AddNewPart<WorkbookStylesPart>();
            var styles = new Stylesheet();
            var font0 = new Font();
            var font1 = new Font();
            font1.Append(new Bold());
            var fonts = new DocumentFormat.OpenXml.Spreadsheet.Fonts();
            fonts.Append(font0);
            fonts.Append(font1);

            var fill0 = new DocumentFormat.OpenXml.Spreadsheet.Fill();
            var fills = new Fills();
            fills.Append(fill0);

            var border0 = new Border();
            var borders = new Borders();
            borders.Append(border0);

            var cellformat0 = new CellFormat
            {
                FormatId = 0,
                FillId = 0,
                BorderId = 0
            };
            var cellformat1 = new CellFormat
            {
                FontId = 1
            };
            var cellformatDate = new CellFormat
            {
                NumberFormatId = 14,
                ApplyNumberFormat = true
            };
            var cellformats = new CellFormats();
            cellformats.Append(cellformat0);
            cellformats.Append(cellformat1);
            cellformats.Append(cellformatDate);

            styles.Append(fonts);
            styles.Append(fills);
            styles.Append(borders);
            styles.Append(cellformats);

            myStyles.Stylesheet = styles;
            myStyles.Stylesheet.Save();
        }
        #endregion
    }
}
