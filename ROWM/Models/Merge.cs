using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ROWM.Models
{
    /// <summary>
    /// pretends to be mail merge
    /// </summary>
    internal static class Merge
    {
        static readonly Regex mergefield = new Regex("MERGEFIELD (?<key>.+)");

        internal static WordprocessingDocument DoMerge(WordprocessingDocument dox, Dictionary<string, string> dto)
        {
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
                r.ReplaceChild<Text>(new Text(dto[m.Value]), t);
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
                            b.ReplaceChild<Text>(new Text(dto[m.Value]), tt);
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

            return dox;
        }


        internal static WordprocessingDocument AppendLogs(WordprocessingDocument dox, IEnumerable<ContactLog> logs)
        {
            var tab = dox.MainDocumentPart.Document.Descendants<Table>();
            if (!tab.Any())
                return dox;

            var table = tab.First();

            var rows = table.Descendants<TableRow>();
            var rt = rows.Skip(7).First();
            var cells = rt.Descendants<TableCell>();
            var dateProperties = cells.First().TableCellProperties;
            var logProperties = cells.Last().TableCellProperties;

            dateProperties.TableCellBorders.AppendChild(new RightBorder { Val= BorderValues.Single, Size=4, Color="Auto" });
            logProperties.TableCellBorders.AppendChild(new LeftBorder());

            foreach (ContactLog log in logs)
            {
                var r = new TableRow();

                var d = new Paragraph(new Run(new Text(log.DateAdded.LocalDateTime.ToString("MM/dd/yy"))));
                d.AppendChild(new ParagraphProperties( new Justification { Val= JustificationValues.Center }));
                var dateCell = new TableCell(d);
                dateCell.AppendChild(dateProperties.CloneNode(true));

                var nr = new Run();
                if (!string.IsNullOrWhiteSpace(log.Title))
                    nr.Append(new Text($"Title: {log.Title}"), new Break());

                if (!string.IsNullOrWhiteSpace(log.Agent?.AgentName ?? ""))
                    nr.Append(new Text($"Agent: {log.Agent?.AgentName}"), new Break());

                if (!string.IsNullOrWhiteSpace(log.ContactChannel))
                    nr.Append(new Text($"Contact Method: {log.ContactChannel}"), new Break());

                if (nr.HasChildren)
                    nr.AppendChild(new Break());

                nr.AppendChild(new Text(log.Notes));
                var n = new Paragraph(nr);
                var logCell = new TableCell(n); //  new Paragraph(new Run(new Text(log.Notes))));
                logCell.AppendChild(logProperties.CloneNode(true));

                r.Append(dateCell, logCell);

                table.AppendChild(r);
            }
            return dox;
        }
    }
}
