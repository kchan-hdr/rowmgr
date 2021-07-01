using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ROWM.Dal;
using ROWM.Models;
using TxDotNeogitations;

namespace ROWM.Controllers
{
    [ApiController]
    public class SH72Controller : ControllerBase
    {
        readonly OwnerRepository _repo;
        readonly Sh72_Context _ctx;

        public SH72Controller(OwnerRepository r, Sh72_Context c) => (_repo, _ctx) = (r, c);

        [HttpGet("parcels/{pid}/n94")]
        public async Task<IActionResult> N94Report(string pid)
        {
            var h = new TxDotNeogitations.Sh72(_ctx);
            var list = await h.GetNegotiations(pid, true);
            if (!list.Any())
                return BadRequest();

            var nId = list.OrderByDescending(hx => hx.ContactDateTime).First().NegotiationId;
            return await GetN94Report(pid, nId);
        }
        [Route("parcels/{pId}/negotiation/{nId}/n94"), HttpGet]
        public async Task<IActionResult> GetN94Report(string pId, Guid nId)
        {
            // var p = await _repo.GetParcel(pId);
            var sh = new TxDotNeogitations.Sh72(_ctx);
            var n = await sh.Load(pId, nId);
            if (n == null)
                return BadRequest();

            var h = new N94Helper();
            var working = await h.Generate(n);
            return File(working.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{pId} N-94.docx");
        }

        [Route("parcels/{pid}/negotiation"), HttpPost]
        public async Task<IActionResult> AddNegotiationHistory(string pid, Sh72_Req req)
        {
            var p = await _repo.GetParcel(pid);

            var dto = req.Write();

            var aid = Guid.Parse(req.NegotiationAgentId);
            var cid = Guid.Parse(req.ContactId);

            var a = (await _repo.GetAgents()).FirstOrDefault(ax => ax.AgentId == aid);
            var c = await _ctx.ContactInfo.FindAsync(cid);

            dto.ContactId = cid;
            dto.ContactPerson = string.Join(" ", new[] { c.FirstName, c.LastName });

            dto.Negotiator = a.AgentName;

            var h = new TxDotNeogitations.Sh72(_ctx);
            var n = await h.Store(p.ParcelId, Guid.Empty, c.ContactId, dto, User.Identity.Name);

            return CreatedAtAction(nameof(RowmController.GetParcel), "ROWM", new { pid = pid });
        }

        [Route("parcels/{pid}/negotiation/{nId}"), HttpPut]
        public async Task<IActionResult> UpdateNegotiationHistory(string pid, Guid nId, Sh72Dto dto)
        {
            var p = await _repo.GetParcel(pid);

            var h = new TxDotNeogitations.Sh72(_ctx);
            var n = await h.Update(p.ParcelId, Guid.Empty, Guid.Empty, dto, User.Identity.Name);

            return CreatedAtAction(nameof(RowmController.GetParcel), "ROWM", new { pid = pid });
        }

        [Route("parcels/{pid}/negotiation"), HttpGet]
        public async Task<IActionResult> GetNegotiationHistory(string pid)
        {
            var h = new TxDotNeogitations.Sh72(_ctx);
            var list = await h.GetNegotiations(pid, true);

            foreach (var hx in list)
            {
                hx.Links.N94Docx = Url.Action(nameof(GetN94Report), new { pId = pid, nId = hx.NegotiationId });
                hx.Links.Document = hx.DocumentId == Guid.Empty ? null : Url.Action(nameof(DocumentController.GetDocument), "Document", new { docId = hx.DocumentId });
            }

            return Ok(list);
        }

        #region dto
        public class Sh72_Req
        {
            public long ContactUnix { get; set; }
            public DateTimeOffset ContactDateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.ContactUnix);
            public double OfferAmount { get; set; }
            public double CounterAmount { get; set; }

            public string ContactId { get; set; }
            public string NegotiationAgentId { get; set; }

            //public string ContactPerson { get; set; }
            public string ContactWhere { get; set; }
            public string ActionTaken { get; set; }
            public string Notes { get; set; }
            //public string Negotiator { get; set; }
            
            public IEnumerable<string> ParcelIds { get; set; } = new List<string>();

            internal Sh72Dto Write()
            {
                var dto = new Sh72Dto();

                dto.ActionTaken = this.ActionTaken;
                dto.ContactDateTime = this.ContactDateTime;
                //dto.ContactPerson = this.ContactPerson;
                dto.ContactWhere = this.ContactWhere;
                dto.CounterAmount = this.CounterAmount;
                //dto.Negotiator = this.Negotiator;
                dto.Notes = this.Notes;
                dto.OfferAmount = this.OfferAmount;
                dto.ParcelIds = this.ParcelIds;

                return dto;
            }
        }
        #endregion
    }
}
