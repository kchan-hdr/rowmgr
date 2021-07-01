using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.SharePoint
{
    public class PmpHelper
    {
        protected string _SharePointSite;
        protected string _AppId;
        protected string _Tenant;
        protected X509Certificate2 _Certificate;

        ClientContext MakeContext()
        {
            var cc = new AuthenticationManager().GetAzureADAppOnlyAuthenticatedContext(
                siteUrl: _SharePointSite,
                clientId: _AppId,
                tenant: _Tenant,
                certificate: _Certificate
                );

            return cc;
        }
    }
}
