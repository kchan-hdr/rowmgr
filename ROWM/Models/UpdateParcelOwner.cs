using System;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class UpdateParcelOwner
    {
        public string ModifiedBy { get; set; } = "UP";

        readonly ROWM_Context _context;

        string partyName;
        string ownerType;
        Parcel myParcel;

        public UpdateParcelOwner(ROWM_Context context, Parcel parcel, string party, string ownerType)
        {
            this._context = context;

            this.partyName = party;
            this.ownerType = ownerType;
            this.myParcel = parcel;
        }

        public async Task<Parcel> Apply()
        {
            var dt = DateTimeOffset.Now;

            var o = new Owner
            {
                ContactInfo = new ContactInfo[]
                {
                    new ContactInfo
                    {
                        FirstName = partyName,
                        LastName = partyName,
                        Created = dt,
                        LastModified = dt,
                        ModifiedBy = this.ModifiedBy
                    }
                },

                PartyName = this.partyName,
                OwnerType = this.ownerType,

                Created = dt,
                LastModified = dt,
                ModifiedBy = this.ModifiedBy
            };

            var oship = new Ownership
            {
                Parcel = this.myParcel,
                Owner = o,
                Ownership_t = (int)Ownership.OwnershipType.Primary,
                Created = dt,
                LastModified = dt,
                ModifiedBy = this.ModifiedBy
            };

            this.myParcel.Ownership.Add(oship);
            o.Ownership = new Ownership[] { oship };

            this._context.Owner.Add(o);
            await this._context.SaveChangesAsync();

            return this.myParcel;
        }
    }
}
