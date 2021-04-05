﻿using Microsoft.EntityFrameworkCore;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ROWM.Controllers
{
    public class B2hParcelHelper
    {
        readonly ROWM_Context _context;
        public B2hParcelHelper(ROWM_Context c) => (_context) = (c);

        public async Task<(bool,int)> UpdateAcquisition(Parcel p, string code, DateTimeOffset dt)
        {
            _ = p ?? throw new ArgumentNullException();

            if (p.ParcelStatusCode == code)
                return (false, 0);

            var o = await Find("acquisition", p.ParcelStatusCode);
            var s = await Find("acquisition", code);

            bool touched = false;

            if (o.DisplayOrder < s.DisplayOrder)
            {
                p.ParcelStatusCode = s.Code;
                _ = AddHistory(p.ParcelId, o.Code, code, dt);   
                touched = true;
            }

            return (touched, s.DomainValue ?? 0);
        }

        public async Task<(bool,int)> UpdateEntry(Parcel p, string code, DateTimeOffset dt)
        {
            _ = p ?? throw new ArgumentNullException();

            if (p.RoeStatusCode == code)
                return (false, 0);

            var o = await Find("roe", p.RoeStatusCode);
            var s = await Find("roe", code);

            bool touched = false;

            if (o.DisplayOrder < s.DisplayOrder)
            {
                p.ParcelStatusCode = s.Code;
                _ = AddHistory(p.ParcelId, o.Code, code, dt);
                touched = true;
            }

            return (touched, s.DomainValue ?? 0);
        }

        public async Task<(bool, int)> UpdateClearance(Parcel p, string code, DateTimeOffset dt)
        {
            _ = p ?? throw new ArgumentNullException();

            if (p.ClearanceCode == code)
                return (false, 0);

            var o = await Find("clearance", p.ClearanceCode);
            var s = await Find("clearance", code);

            bool touched = false;

            if (o.DisplayOrder < s.DisplayOrder)
            {
                p.ClearanceCode = s.Code;
                _ = AddHistory(p.ParcelId, o.Code, code, dt);
                touched = true;
            }

            return (touched, s.DomainValue ?? 0);
        }

        async Task<Parcel_Status> Find(string category, string code ) =>
            await _context.Parcel_Status.SingleOrDefaultAsync(sx => sx.Category == category && sx.Code == code)
                ?? throw new IndexOutOfRangeException($"code not found ({category})({code})");

        Status_Activity AddHistory(Guid pid, string oldCode, string newCode, DateTimeOffset dt) =>
            _context.Status_Activity.Add(new Status_Activity
                {
                    ParentParcelId = pid,
                    AgentId = Guid.Parse("3C75F249-F5C1-47F9-913B-A0FB8CFD57E0"),
                    ActivityDate = dt,
                    OriginalStatusCode = oldCode,
                    StatusCode = newCode
                });
    }
}