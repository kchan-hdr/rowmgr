using geographia.ags;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backfill_Sp_folder_url
{
    internal class Backfill
    {
        internal async Task Do()
        {
            var msi = new AzureServiceTokenProvider();
            var vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(msi.KeyVaultTokenCallback));

            var appid = vaultClient.GetSecretAsync("https://atc-rowm-key.vault.azure.net/", "atc-client").GetAwaiter().GetResult();
            var apps = vaultClient.GetSecretAsync("https://atc-rowm-key.vault.azure.net/", "atc-secret").GetAwaiter().GetResult();



            var f = new AtcParcel("https://maps.hdrgateway.com/arcgis/rest/services/California/ATC_Line6943_Parcel_FS/FeatureServer") as IFeatureUpdate;

            var same = new SameParcel();


            var strategy = new SqlAzureExecutionStrategy();
            var cs = "data source=tcp:atc-rowm.database.windows.net;initial catalog=rowm_6943;persist security info=True;user id=rowm_app;password=;MultipleActiveResultSets=True;App=EntityFramework"; // Configuration.GetConnectionString("ROWM_Context");

            await strategy.Execute(async () =>
            {
                using (var ctx = new ROWM_Context(cs))
                {
                    foreach (var d in ctx.Document)
                    {
                        foreach (var myParcel in d.Parcel)
                        {
                            var sp = new SharePointCRUD(
                               d: new DocTypes(ctx), __appId: appid.Value, __appSecret: apps.Value, _url: "https://atcpmp.sharepoint.com/atcrow/line6943");

                            // var mxParcel = ctx.Parcel.Include(px => px.Ownership).FirstOrDefault(px => px.ParcelId == myParcel.ParcelId);

                            try
                            {
                                Ownership primaryOwner = myParcel.Ownership.FirstOrDefault<Ownership>(o => o.IsPrimary()); // o.Ownership_t == Ownership.OwnershipType.Primary);
                                string parcelName = String.Format("{0} {1}", myParcel.Tracking_Number, primaryOwner.Owner.PartyName);

                                //_sharePointCRUD.UploadParcelDoc(parcelName, "Other", sourceFilename, bb, null);
                                sp.UploadParcelDoc(parcelName, d.DocumentType, d.SourceFilename, d.Content, null);
                                string parcelDocUrl = sp.GetParcelFolderURL(parcelName, null);
                                await f.UpdateFeatureDocuments(myParcel.Assessor_Parcel_Number, parcelDocUrl);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError(e.Message);
                                throw;
                            }
                        }
                    }
                }
            });
        }
    }

    public class SameParcel : IEqualityComparer<Parcel>
    {
        public bool Equals(Parcel x, Parcel y) => x.Assessor_Parcel_Number.Equals(y.Assessor_Parcel_Number);
        public int GetHashCode(Parcel obj) => obj.Assessor_Parcel_Number.GetHashCode();
    }
}
