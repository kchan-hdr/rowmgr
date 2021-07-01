using geographia.ags;
using ROWM.Controllers;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHillsDocumentToSharePoint
{
    internal class Mover
    {
        ROWM_Context ctx;
        ISharePointCRUD _sharePointCRUD;
        IFeatureUpdate _featureUpdate;
        ParcelStatusHelper _statusHelper;

        internal Mover()
        {
            ctx = new ROWM_Context();

            var dt = new DocTypes(ctx);
            _sharePointCRUD = new SharePointCRUD(d: dt);
            _featureUpdate = new BlackhillParcel("https://gis05.hdrgateway.com/arcgis/rest/services/California/Blackhills_Parcel_FS/FeatureServer");
        }

        internal void Copy()
        {
            foreach( var d in ctx.Document)
            {
                Trace.WriteLine($"{d.Title} {d.SourceFilename}");

                foreach( var p in d.Parcel )
                {
                    Trace.WriteLine(p.Tracking_Number);

                    Ownership primaryOwner = p.Ownership.First<Ownership>(o => o.IsPrimary()); // o.Ownership_t == Ownership.OwnershipType.Primary);
                    string parcelName = String.Format("{0} {1}", p.Tracking_Number, primaryOwner.Owner.PartyName);
                    try
                    {
                        //_sharePointCRUD.UploadParcelDoc(parcelName, "Other", sourceFilename, bb, null);
                        _sharePointCRUD.UploadParcelDoc(parcelName, d.DocumentType, d.SourceFilename, d.Content, null);
                        string parcelDocUrl = _sharePointCRUD.GetParcelFolderURL(parcelName, null);


                        var task = _featureUpdate.UpdateFeatureDocuments(p.Assessor_Parcel_Number, string.Empty, parcelDocUrl);
                        task.GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        // TODO: Return error to user?
                        Trace.WriteLine(string.Format("Error uploading document {0} type {1} to Sharepoint for {2}", d.SourceFilename, d.DocumentType, parcelName));
#if DEBUG
                        throw;
#endif
                    }

                }
            }
        }
    }
}
