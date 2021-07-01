using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Storage;
using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxDotNeogitations;

namespace ROWM.Controllers
{
    public class Sh72Helper
    {
        internal static Sh72Dto TryMake(FormValueProvider prod)
        {
            var date = prod.GetValue("negotiation-contact-dt");     // this is a json date
            var channel = prod.GetValue("negotiation-where");
            var contact = prod.GetValue("negotiation-contact");
            var phone = prod.GetValue("negotiation-contact-phone");
            var offer = prod.GetValue("negotiation-offer-amount");
            var counter = prod.GetValue("negotiation-counteroffer-amount");
            var negotiator = prod.GetValue("negotiation-negotiator");
            var action = prod.GetValue("negotiation-action-taken");
            var notes = prod.GetValue("negotiation-notes");

            var parcels = prod.GetValue("ParcelIds");   // array

            if (!parcels.Any())
                throw new InvalidOperationException();


            // ^^^

            var r = new Sh72Dto();

            r.ParcelIds = parcels.Select(px => px);

            if (date.Any())
            {
                if (long.TryParse(date.FirstValue, out var ms))
                    r.ContactDateTime = DateTimeOffset.FromUnixTimeMilliseconds(ms);
            }

            if (channel.Any())
                r.ContactWhere = channel.FirstValue;

            if (contact.Any())
                r.ContactId = Guid.Parse(contact.FirstValue);

            if (offer.Any())
            {
                if (double.TryParse(offer.FirstValue, out var offerAmt))
                    r.OfferAmount = offerAmt;
            }

            if (counter.Any())
            {
                if (double.TryParse(counter.FirstValue, out var counterAmt))
                    r.CounterAmount = counterAmt;
            }

            if (negotiator.Any())
                r.Negotiator = negotiator.FirstValue;

            if (action.Any())
                r.ActionTaken = action.FirstValue;

            if (notes.Any())
                r.Notes = notes.FirstValue;

            return r;
        }
    }
}
