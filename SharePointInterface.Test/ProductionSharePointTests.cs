using System;
using System.Diagnostics;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharePointInterface.Test
{
    [TestClass]
    public class ProductionSharePointTests
    {
        [TestMethod, TestCategory("SharePointCRUD")]
        public void TestConnection()
        {
            ISharePointCRUD sp = new SharePointCRUD(
                __appId: "77429b44-e9ab-403c-acfa-e90648aa4452", 
                __appSecret: "",
                _url: "https://atcpmp.sharepoint.com/atcrow/chc");
            var t = sp.GetSiteTitle();
            Assert.AreNotEqual("NA", t);
            Trace.WriteLine(t);


            var lists = sp.ListAllLists();
            Assert.AreNotEqual(0, lists.Count);

            foreach( var l in lists)
            {
                Trace.WriteLine(l.Title);
            }
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void TestConnection_ATC()
        {
            //var msi = new AzureServiceTokenProvider();
            //var vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(msi.KeyVaultTokenCallback));
            //var appid = vaultClient.GetSecretAsync("https://atc-rowm-key.vault.azure.net/", "atc-client").GetAwaiter().GetResult();
            //var apps = vaultClient.GetSecretAsync("https://atc-rowm-key.vault.azure.net/", "atc-secret").GetAwaiter().GetResult();
            //services.AddScoped<ISharePointCRUD, SharePointCRUD>(fac => new SharePointCRUD(
            //   d: fac.GetRequiredService<DocTypes>(), __appId: appid.Value, __appSecret: apps.Value, _url: "https://atcpmp.sharepoint.com/atcrow/chc"));
            var appid = "77429b44-e9ab-403c-acfa-e90648aa4452";
            var apps = "ljVTIGAd0siTJ79FmQPPs/yU0QtoEEb1J1y05SsxG+A=";


            ISharePointCRUD sp = new SharePointCRUD(__appId: appid, __appSecret: apps, _url: "https://atcpmp.sharepoint.com/atcrow/chc");
            var t = sp.GetSiteTitle();
            Assert.AreNotEqual("NA", t);
            Trace.WriteLine(t);


            var lists = sp.ListAllLists();
            Assert.AreNotEqual(0, lists.Count);

            foreach (var l in lists)
            {
                Trace.WriteLine(l.Title);
            }
        }

        [TestMethod, TestCategory("SharePointCRUD - Destructive")]
        public void InsertDoc()
        {
            string fileName = @"testdata\logs.csv";

            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            Microsoft.SharePoint.Client.Folder folder = spCRUD.GetOrCreateFolder("Test");
            bool docExists = spCRUD.DocExists(folder, "test2.txt");
            if (!docExists)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(fileName);
                docExists = spCRUD.InsertDoc(folder, "test2.txt", bytes);
            }
            Assert.IsTrue(docExists);
        }

        [TestMethod, TestCategory("SharePointCRUD - Destructive")]
        public void UploadDoc()
        {
            string fileName = @"testdata\logs.csv";

            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);
            var good = spCRUD.UploadParcelDoc("test", "test", fileName, bytes);
            Assert.IsTrue(good);
        }
    }
}
