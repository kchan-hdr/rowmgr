using geographia.ags;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backfill_Sp_folder_url
{
    internal class Backfill
    {
        internal async Task Do()
        {
            var sp = new SharePointCRUD();
            var f = new B2hParcel() as IFeatureUpdate;

            var same = new SameParcel();

            using (var ctx = new ROWM_Context()) // DbConnection.GetConnectionString()))
            {
                var parcels = await ctx.Document.SelectMany(dx => dx.Parcel).ToArrayAsync();
                var tasks = new List<Task>();

                foreach (var p in parcels.Distinct(same))
                {
                    var parcelName = $"{p.Assessor_Parcel_Number} {p.Ownership.First(o => o.IsPrimary()).Owner.PartyName}";
                    var name = sp.GetParcelFolderName(parcelName);
                    var url = sp.GetParcelFolderURL(name);
                    Console.WriteLine($"{p.Assessor_Parcel_Number} --> {url}");

                    if (!string.IsNullOrWhiteSpace(url))
                        tasks.Add(f.UpdateFeatureDocuments(p.Assessor_Parcel_Number, url));
                }


                await Task.WhenAll(tasks);
            }
        }
    }

    public class SameParcel : IEqualityComparer<Parcel>
    {
        public bool Equals(Parcel x, Parcel y) => x.Assessor_Parcel_Number.Equals(y.Assessor_Parcel_Number);
        public int GetHashCode(Parcel obj) => obj.Assessor_Parcel_Number.GetHashCode();
    }
}
