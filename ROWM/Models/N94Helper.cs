using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TxDotNeogitations;

namespace ROWM.Models
{
    public class N94Helper
    {
        IDictionary<string, PropertyInfo> _Schema;

        internal N94Helper()
        {
            var properties = typeof(N94_dto).GetProperties();
            _Schema = properties.ToDictionary<PropertyInfo, string>(px => px.Name.ToLower());
        }

        //N94_dto MakeData(Parcel p)
        //{
        //    var o = p.Ownership.FirstOrDefault()?.Owner ?? throw new IndexOutOfRangeException("missing owner");

        //    return new N94_dto
        //    {
        //        Highway = "SH 72",
        //        District = "Yoakum",
        //        Project_No = "N/A",
        //        ROW_CSJ = "0270-01-055",
        //        Owner = o.PartyName,
        //        Address = p.SitusAddress,
        //        Parcel = p.Tracking_Number,
        //        Persons = string.Join(", ", o.ContactInfo.Select(px => px.FirstName)),
        //        Telephone = "",
        //        Where_Contacted = "where",
                
        //        Negotiator = "whomever",

        //        OfferAmt = p.InitialEasementOffer_OfferAmount,
        //        ContactDt = p.InitialEasementOffer_OfferDate,

        //        Action_Taken = "test",
        //        Comments = "this is a test"
        //    };
        //}

        N94_dto MakeData(NegotiationHistory n)
        {
            var p = n.NegotiationParcels.FirstOrDefault()?.Parcel ?? throw new IndexOutOfRangeException($"cannot find parcel for negotiation");
            var o = p.Ownership.FirstOrDefault()?.Owner ?? throw new IndexOutOfRangeException("missing owner");

            return new N94_dto
            {
                Highway = "SH 72",
                District = "Yoakum",
                Project_No = "N/A",
                ROW_CSJ = "0270-01-055",
                Owner = o.PartyName,
                Address = p.SitusAddress,
                Parcel = p.TrackingNumber,
                
                Persons = n.ContactPersonName,
                Telephone = n.ContactNumber,
                Where_Contacted = n.ContactMethod,

                Negotiator = n.Negotiator?.AgentName ?? string.Empty,

                ContactDt = n.ContactDate,

                OfferAmt = n.OfferAmount.HasValue ? Convert.ToDouble(n.OfferAmount) : 0,
                CounterAmt = n.CounterOfferAmount.HasValue ? Convert.ToDouble(n.CounterOfferAmount) : 0,

                Action_Taken = n.Action,
                Comments = n.Notes
            };
        }

        internal async Task<byte[]> Generate(NegotiationHistory n)
        {
            var dto = MakeData(n);

            var mergefield = new Regex("MERGEFIELD (?<key>.+)");

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ROWM.ReportTemplates.N94.docx");


            MemoryStream working = new MemoryStream();
            await s.CopyToAsync(working);

            var dox = WordprocessingDocument.Open(working, true);


            // merge simple fields
            var fields = dox.MainDocumentPart.Document.Descendants<SimpleField>();

            foreach (var field in fields)
            {
                var key = field.Instruction.Value.Trim();

                var m = mergefield.Match(key).Groups["key"];
                if (!m.Success)
                {
                    Trace.TraceWarning(key);
                    continue;
                }

                var r = field.GetFirstChild<Run>();
                var t = r.GetFirstChild<Text>();
                r.ReplaceChild<Text>(new Text(Lookup(m.Value, dto)), t);
            }


            // complex fields   
            var cfields = dox.MainDocumentPart.Document.Descendants<FieldCode>();

            var dead = new List<FieldChar>();
            var deadier = new List<OpenXmlElement>();

            foreach (var f in cfields)
            {
                var key = f.InnerText.Trim();
                var m = mergefield.Match(key).Groups["key"];
                if (!m.Success)
                {
                    Trace.TraceWarning(key);
                    continue;
                }

                var elm = f.Parent;
                deadier.Add(elm);

                // find start
                var bx = elm.PreviousSibling();

                while (bx != null)
                {
                    var fc = bx.Descendants<FieldChar>();
                    if (fc.Any(fcx => fcx.FieldCharType == FieldCharValues.Begin))  // reached start
                    {
                        dead.AddRange(fc);
                        break;
                    }
                }

                OpenXmlElement b = elm.NextSibling();

                while (b != null)
                {
                    var fc = b.Descendants<FieldChar>();
                    if (fc.Any())
                    {
                        dead.AddRange(fc);
                        if (fc.Any(fcx => fcx.FieldCharType == FieldCharValues.End))    // done complex field
                        {
                            break;
                        }
                    }
                    else
                    {
                        var t = b.Descendants<Text>();
                        foreach (var tt in t.Where(tx => tx.Text.Contains(m.Value)))
                        {
                            b.ReplaceChild<Text>(new Text(Lookup(m.Value, dto)), tt);
                        }
                    }

                    b = b.NextSibling();
                }
            }

            // delete field markers
            foreach (var d in dead)
            {
                var dd = d.Parent;
                dd.RemoveAllChildren();
                dd.Remove();
            }

            foreach (var d in deadier)
            {
                d.RemoveAllChildren();
                d.Remove();
            }

            dox.MainDocumentPart.Document.Save();
            dox.Close();

            return working.ToArray();
        }

        string Lookup(string key, N94_dto dto)
        {
            key = key.ToLower();
            if (_Schema.TryGetValue(key, out var v))
            {
                var r = v.GetValue(dto) as string;
                return r;
            }

            return $"rrr.{key}";
            // throw new IndexOutOfRangeException(key);
        }
    }

    public class N94_dto
    { 
        public string Owner { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Persons { get; set; }
        public string Where_Contacted { get; set; }
        public string Parcel { get; set; }
        public string District { get; set; }
        public string Project_No { get; set; }
        public string ROW_CSJ { get; set; }
        public string Highway { get; set; }
        public string Date => this.ContactDt.HasValue ? this.ContactDt.Value.Date.ToLongDateString() : "";
        public string Time => this.ContactDt.HasValue ? this.ContactDt.Value.ToString("t") : "";
        public string Offer => this.OfferAmt.HasValue ? $"{this.OfferAmt:C}" : "";
        public string CounterOffer => this.CounterAmt.HasValue ? $"{this.CounterAmt:C}" : "";
        public string Comments { get; set; }
        public string Action_Taken { get; set; }

        public string Negotiator { get; set; }

        public DateTimeOffset? ContactDt { get; set; }
        public double? OfferAmt { get; set; }
        public double? CounterAmt { get; set; }
    }
}
