using OfficeOpenXml;
using OfficeOpenXml.Style;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ROWM.Reports
{
    public class WhartonReport : IRowmReports
    {
        #region static init
        static readonly IEnumerable<ReportDef> _Reports = new List<ReportDef>
            {
                new ReportDef { Caption = "At A Glance Report", DisplayOrder = 1, ReportCode = "glance"},
                new ReportDef { Caption = "Internal Status Report", DisplayOrder = 2, ReportCode = "internal"},
                new ReportDef { Caption = "Excternal Status Report", DisplayOrder = 3, ReportCode = "external"},
                new ReportDef { Caption = "Status Summary", DisplayOrder = 4, ReportCode = "snapshot" }
            };

        static readonly IEnumerable<ColumnMapping> _atAGlance = new List<ColumnMapping>
        {
            new ColumnMapping{ TemplateColumnIndex = "B", TemplateColumnName = "City Survey Completed", StatusCode = "Survey_City_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "C", TemplateColumnName = "USACOE Survey Completed", StatusCode = "Survey_USACOE_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "D", TemplateColumnName = "Appraisal", StatusCode = "Appraisal_Effective" },
            new ColumnMapping{ TemplateColumnIndex = "E", TemplateColumnName = "USACOE ATP Sent to City", StatusCode = "Authorization_to_Proceed_Submitted" },
            new ColumnMapping{ TemplateColumnIndex = "F", TemplateColumnName = "Just Comp Approval Memo Sent to City", StatusCode = "JC_Memo_Submitted" },
            new ColumnMapping{ TemplateColumnIndex = "G", TemplateColumnName = "City Just Comp Approved", StatusCode = "JC_Memo_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "H", TemplateColumnName = "USACOE ATP Approved", StatusCode = "Authorization_to_Proceed_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "I", TemplateColumnName = "Offer Made", StatusCode = "Offer_Made" },
            new ColumnMapping{ TemplateColumnIndex = "J", TemplateColumnName = "Counter Offer", StatusCode = "Counter_Offer_Received" },
            new ColumnMapping{ TemplateColumnIndex = "K", TemplateColumnName = "PSA Signed", StatusCode = "PSA_Signed" },
            new ColumnMapping{ TemplateColumnIndex = "L", TemplateColumnName = "Closed", StatusCode = "Inspection_Possession_Affidavit" },
            new ColumnMapping{ TemplateColumnIndex = "M", TemplateColumnName = "Condemnation", StatusCode = "Condemation_X" },
            new ColumnMapping{ TemplateColumnIndex = "N", TemplateColumnName = "Construction", StatusCode = "Construction_x" }
        };

        static readonly IEnumerable<ColumnMapping> _status = new List<ColumnMapping>
        {
            new ColumnMapping{ TemplateColumnIndex = "H", TemplateColumnName = "CONTACT Negotiator Report", StatusCode = "Survey_City_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "R", TemplateColumnName = "TITLE Commitment Effective Date", StatusCode = "title_commitment_effective" },
            new ColumnMapping{ TemplateColumnIndex = "S", TemplateColumnName = "TITLE Commitment Issue Date", StatusCode = "title_commitment_issued" },
            new ColumnMapping{ TemplateColumnIndex = "U", TemplateColumnName = "TITLE Curative Plan", StatusCode = "title_curative_plan" },
            new ColumnMapping{ TemplateColumnIndex = "X", TemplateColumnName = "Survey Requested", StatusCode = "survey_city_requested" },
            new ColumnMapping{ TemplateColumnIndex = "AB", TemplateColumnName = "Survey Date", StatusCode = "survey_city_conducted" },
            new ColumnMapping{ TemplateColumnIndex = "AC", TemplateColumnName = "Survey for CITY Approved", StatusCode = "Survey_City_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "AE", TemplateColumnName = "Survey for USACOE", StatusCode = "survey_usacoe_conducted" },
            new ColumnMapping{ TemplateColumnIndex = "AF", TemplateColumnName = "Survey for USACOE Approved", StatusCode = "Survey_USACOE_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "AL", TemplateColumnName = "Appraisal Ordered", StatusCode = "appraisal_ordered" },
            new ColumnMapping{ TemplateColumnIndex = "AT", TemplateColumnName = "Final Appraisal Effective Date", StatusCode = "appraisal_effective" },
            new ColumnMapping{ TemplateColumnIndex = "AU", TemplateColumnName = "Appraisal Review Approved Date", StatusCode = "appraisal_review_approved" },
            new ColumnMapping{ TemplateColumnIndex = "AW", TemplateColumnName = "Just Comp Approval Memo Sent to City", StatusCode = "appraisal_jc_to_city" },
            new ColumnMapping{ TemplateColumnIndex = "AX", TemplateColumnName = "USACOE ATP Sent to City", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "AY", TemplateColumnName = "City Sent ATP to USACOE for Approval", StatusCode = "appraisal_jc_usacoe" },
            new ColumnMapping{ TemplateColumnIndex = "AZ", TemplateColumnName = "Just Comp Approved Date", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "BA", TemplateColumnName = "USACOE ATP w/Offer Approved Date", StatusCode = "usacoe_atp_approved" },
            new ColumnMapping{ TemplateColumnIndex = "BC", TemplateColumnName = "Offer Pkge Documents Requested", StatusCode = "negotiation_offer_requested" },
            new ColumnMapping{ TemplateColumnIndex = "BD", TemplateColumnName = "Offer Pkge QC Approved", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "BE", TemplateColumnName = "Initial Offer Package Sent", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "BG", TemplateColumnName = "Initial Offer Package Acknowledgment", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "BH", TemplateColumnName = "Evaluation and Approval Form", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "BI", TemplateColumnName = "Property Owners Counter Offer", StatusCode = "negotiation_counter_offer" },
            new ColumnMapping{ TemplateColumnIndex = "BJ", TemplateColumnName = "NEGOTIATION Owner Representation Letter", StatusCode = "Survey_City_Approved" },
            new ColumnMapping{ TemplateColumnIndex = "BK", TemplateColumnName = "NEGOTIATION LOBR Letter", StatusCode = "negotiation_lobr_sent" },
            new ColumnMapping{ TemplateColumnIndex = "BL", TemplateColumnName = "Info About Brokerage Services Acknowledgment", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "BM", TemplateColumnName = "Purchase and Sale Agreement Executed", StatusCode = "" },
            new ColumnMapping{ TemplateColumnIndex = "BN", TemplateColumnName = "Recorded Deed", StatusCode = "" },

        };

        
        static readonly TimeZoneInfo _CST = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

        #region styling
        #endregion
        #endregion
        #region ctor
        readonly ROWM_Context _context;
        public WhartonReport(ROWM_Context c) => _context = c;
        #endregion
        public Task<ReportPayload> GenerateReport(ReportDef d)
        {
            switch (d.ReportCode)
            {
                case "glance": return DoGlance();
                case "internal": return DoInternal();
                case "external": return DoExternal();
                case "status": return DoStatusDetails(d.ReportUrl);
                case "snapshot": return DoStatus();
                default:
                    throw new KeyNotFoundException($"report not implemented");
            }
        }

        public IEnumerable<ReportDef> GetReports() => _Reports.OrderBy(rx => rx.DisplayOrder);

        #region implementation
        public async Task<ReportPayload> DoGlance()
        {
            var status = await _context.Parcel_Status.AsNoTracking().ToArrayAsync();

            var statusGruops = await _context.Activities
                .Include(a => a.ParentParcel)
                .GroupBy(a => a.ParcelStatusCode)
                .ToListAsync();


            var summary = from sx in status
                          join sg in statusGruops on sx.Code equals sg.Key into g
                          from reportg in g.DefaultIfEmpty()
                          select new { sx, reportg };


            var template = Assembly.GetExecutingAssembly().GetManifestResourceStream("ROWM.Reports.CoW_At_A_Glance.xlsx");
            using (var s = new MemoryStream())
            using (ExcelPackage p = new ExcelPackage(s, template))
            {

                p.Workbook.Worksheets.Add("raw data");
                var shit = p.Workbook.Worksheets.ToArray();

                // summary
                shit[0].Cells["B3"].Value = DateTime.Today.ToLongDateString();

                int i = 30;

                int col = 0;
                int maxRow = 0;

                // tracts
                foreach (var sx in summary.OrderBy(x => x.sx.DisplayOrder))
                {
                    // debugging
                    int row = 1;

                    col++;

                    var addr = ExcelRange.GetAddress(row, col);
                    shit[3].Cells[addr].Value = sx.sx.Code;

                    var parcels = sx.reportg?.AsQueryable().Select(ax => ax.ParentParcel).ToList() ?? new List<Parcel>();

                    foreach (var parcel in parcels.OrderBy(px => px.Tracking_Number))
                        shit[3].Cells[ExcelRange.GetAddress(++row, col)].Value = parcel.Tracking_Number;

                    //// 
                    //shit[0].Cells[$"F{i}"].Value = sx.sx.Code;
                    //shit[0].Cells[$"E{i}"].Value = row--;
                    //i++;


                    // real output
                    var acq = shit[1];
                    var map = _atAGlance.FirstOrDefault(m => m.StatusCode.Equals(sx.sx.Code, StringComparison.CurrentCultureIgnoreCase));
                    if (map != null)
                    {
                        row = 2;
                        foreach (var parcel in parcels.OrderBy(px => px.Tracking_Number))
                        {
                            var cx = acq.Cells[$"{map.TemplateColumnIndex}{row++}"];
                            cx.Value = parcel.Tracking_Number;
                            cx.Style.Font.Bold = true;
                            cx.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            cx.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                        maxRow = Math.Max(maxRow, row);
                    }
                }

                shit[1].Cells[$"A0:N{maxRow}"].Style.Border.BorderAround(ExcelBorderStyle.Double);

                p.Save();

                return new ReportPayload
                {
                    Filename = $"At-A-Glance {DateTime.Today.ToString("yyyy_MM_dd")}.xlsx",
                    Mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    Content = s.ToArray()
                };
            }
        }

        public async Task<ReportPayload> DoInternal()
        {
            var status = await _context.Parcel_Status.AsNoTracking().OrderBy(sx => sx.DisplayOrder).ToArrayAsync();

            var parcels = await _context.Parcel
                    .Include(p => p.Activities)
                    .AsNoTracking()
                    .ToListAsync();

            var template = Assembly.GetExecutingAssembly().GetManifestResourceStream("ROWM.Reports.CoW_ACQUISITION_Status_Report.xlsx");
            using (var s = new MemoryStream())
            using (ExcelPackage p = new ExcelPackage(s, template))
            {

                p.Workbook.Worksheets.Add("raw data");
                var shit = p.Workbook.Worksheets.ToArray();


                for( int row =6; row<200; row++)
                {
                    var tract = Convert.ToString( shit[0].Cells[$"A{row}"].Value);
                    tract = tract.Trim();
                    Trace.WriteLine(tract);

                    if (string.IsNullOrWhiteSpace(tract))
                        break;

                    var par = parcels.FirstOrDefault(px => px.Tracking_Number.Equals(tract, StringComparison.CurrentCultureIgnoreCase));
                    if (par != null)
                    {
                        foreach( var a in par.Activities)
                        {
                            var col = _status.FirstOrDefault(sx => sx.StatusCode.Equals(a.ParcelStatusCode, StringComparison.CurrentCultureIgnoreCase));

                            if ( col != null)
                            {
                                var c = shit[0].Cells[$"{col.TemplateColumnIndex}{row}"];
                                c.Value = a.ActivityDate.DateTime.ToOADate();
                                c.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                c.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                            }
                        }
                    }
                }


                int row2 = 1;
                int col2 = 1;
                var debug = shit[3];
                foreach( var stx in status)
                {
                    ++col2;
                    var addr = ExcelRange.GetAddress(row2, col2);
                    debug.Cells[addr].Value = stx.Code;
                }

                foreach ( var parcel in parcels.OrderBy(px => px.Tracking_Number))
                {
                    ++row2;

                    col2 = 1;

                    var addr = ExcelRange.GetAddress(row2, col2);
                    debug.Cells[addr].Value = parcel.Tracking_Number;

                    foreach ( var stx in status)
                    {
                        ++col2;

                        var evt = parcel.Activities.FirstOrDefault(ax => ax.ParcelStatusCode == stx.Code);
                        if (evt != null)
                        {
                            addr = ExcelRange.GetAddress(row2, col2);
                            var cell2 = debug.Cells[addr];
                            cell2.Value = evt.ActivityDate.DateTime.ToOADate();
                            cell2.Style.Numberformat.Format = "yyyy.mm.dd";
                            cell2.AutoFitColumns();
                        }
                    }
                }

                p.Save();

                return new ReportPayload
                {
                    Filename = $"ACQUISITION {DateTime.Today.ToString("yyyy_MM_dd")}.xlsx",
                    Mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    Content = s.ToArray()
                };
            }
        }
        public async Task<ReportPayload> DoExternal()
        {
            var payload = await DoInternal();
            return payload;
        }

        public async Task<ReportPayload> DoStatus()
        {
            var parcels = await (from p in _context.Parcel.AsNoTracking()
                                 where p.IsActive && !p.IsDeleted
                                 join c in _context.Parcel_Status on p.Parcel_Status.ParentStatusCode equals c.Code
                                 select new { p.Tracking_Number, p.Assessor_Parcel_Number, c.Description }).ToArrayAsync();

            var lines = new List<string>
            {
                $"Parcel Status Summary Report",
                $"Date Printed,{DateTime.Now}",
                $"Tracking Number,APN,Acquisition Status"
            };

            lines.AddRange(
                parcels.OrderBy(px => px.Tracking_Number, new TrackingComparer())
                .Select(px => $"{px.Tracking_Number},{px.Assessor_Parcel_Number},{px.Description}")
            );

            using (var mem = new MemoryStream())
            {
                using (var writer = new StreamWriter(mem))
                {
                    foreach (string l in lines)
                        writer.WriteLine(l);

                    writer.Close();

                    return new ReportPayload
                    {
                        Filename = $"Parcel Status - {DateTime.Now}.csv",
                        Mime = "text/csv",
                        Content = mem.GetBuffer()
                    };
                }
            }
        }
        public async Task<ReportPayload> DoStatusDetails(string milestoneCode)
        {
            var milestone = await _context.Parcel_Status.SingleOrDefaultAsync(sx => sx.Code == milestoneCode) ?? throw new ArgumentOutOfRangeException($"bad milestone requested {milestoneCode}");

            var details = from stages in _context.Parcel_Status
                          where stages.IsActive && stages.ParentStatusCode == milestoneCode
                          orderby stages.DisplayOrder
                          select stages;

            var parcels = await _context.Parcel.AsNoTracking()
                .Include(px => px.Activities)
                .Where(px => px.IsActive && !px.IsDeleted)
                .OrderBy(px => px.Tracking_Number)
                .Select(px => new { px.Tracking_Number, px.Assessor_Parcel_Number, px.ParcelId, px.Activities })
                .ToArrayAsync();

            var lines = new List<string>
            {
                $"Parcel Status Details Report,{milestone.Description}",
                $"Date Printed,{DateTime.Now}",
                $"Tracking Number,APN,{string.Join(",", details.Select(sx => sx.Description))}"
            };

            foreach (var parcel in parcels.OrderBy(px => px.Tracking_Number, new TrackingComparer()))
            {
                Trace.WriteLine($"{parcel.Tracking_Number}  {parcel.Assessor_Parcel_Number}");

                var d = new List<string>();
                d.Add(parcel.Tracking_Number);
                d.Add(parcel.Assessor_Parcel_Number);

                foreach(Parcel_Status s in details)
                {
                    if ( parcel.Activities.Any(ax => ax.ParcelStatusCode == s.Code))
                    {
                        var utc = parcel.Activities.Where(ax => ax.ParcelStatusCode == s.Code).Max(ax => ax.ActivityDate);
                        var dt = TimeZoneInfo.ConvertTimeFromUtc(utc.DateTime, _CST);
                        d.Add(dt.ToShortDateString());
                    }
                    else
                    {
                        d.Add( string.Empty );
                    }
                }

                lines.Add(string.Join(",", d));
            }

            using (var mem = new MemoryStream())
            {
                using (var writer = new StreamWriter(mem))
                {
                    foreach(string l in lines)
                        writer.WriteLine(l);

                    writer.Close();

                    return new ReportPayload
                    {
                        Filename = $"Parcel Status - {milestoneCode}.csv",
                        Mime = "text/csv",
                        Content = mem.GetBuffer()
                    };
                }
            }
        }
        #endregion

        #region Excel template helper
        internal class ColumnMapping
        {
            internal string TemplateColumnIndex { get; set; }
            internal string TemplateColumnName { get; set; }
            internal string StatusCode { get; set; }
        }
        #endregion
    }

    internal class TrackingComparer : IComparer<string>
    {
        Regex Tracking = new Regex("(\\d*)(\\.*)", RegexOptions.Singleline | RegexOptions.Compiled);

        public int Compare(string x, string y)
        {
            var xm = Tracking.Match(x);
            var ym = Tracking.Match(y);

            if (xm.Success && ym.Success)
            {
                var xn = MyValue(xm.Groups[0].Value);
                var yn = MyValue(ym.Groups[0].Value);

                var diff = xn - yn;
                if (diff == 0)
                {
                    var xa = xm.Groups[1].Value;
                    var ya = ym.Groups[1].Value;

                    return string.Compare(xa, ya);
                }
                else
                {
                    return diff;
                }
            } 
            else
            {
                if (xm.Success) return 1;
                if (ym.Success) return -1;
                return 0;
            }
        }

        static int MyValue(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0;

            if (int.TryParse(s, out var n))
                return n;
            return 0;
        }
    }
}
