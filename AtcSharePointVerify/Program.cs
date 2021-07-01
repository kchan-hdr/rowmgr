using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtcSharePointVerify
{
    class Program
    {
        string app = "9a8911eb-6c40-4d6b-a832-91ea373b6134";
        string docHelper = "4Woy~gQzOP837rP4zxMr_-Z~1Mu86Q2u_4";

        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener(true));

            var cb = new SqlConnectionStringBuilder
            {
                DataSource = "tcp:atc-rowm.database.windows.net",
                InitialCatalog = "rowm_ally",
                UserID = "rowm_app",
                Password = "SbhrDX6Cq5VPcR9z",
                MultipleActiveResultSets = true,
                ApplicationName = "ROWM-Verifier",
                ApplicationIntent = ApplicationIntent.ReadWrite,              
            };

            var np = 0;

            using (var ctx0 = new ROWM_Context(cb.ConnectionString))
            {
                // check every parcel
                np = ctx0.Parcel.Count();
            }


            for (int i = 0; i <= np; i += 10)
            {
                using (var ctx = new ROWM_Context(cb.ConnectionString))
                {
                    ctx.Database.Log += (s) => Console.WriteLine(s);
                    ctx.Database.CommandTimeout = 600;
                    var doct = new DocTypes(ctx);
                    var v = new DocumentVerify(doct);

                    foreach (Parcel p in ctx.Parcel
                                            .OrderBy(px => px.Tracking_Number)
                                            .Skip(i)
                                            .Take(10))
                    {
                        ctx.Entry(p).Collection(px => px.Document).Query().Where(dx => !dx.IsDeleted).Load();

                        v.DoVerify(p);
                    }

                    var touched = ctx.SaveChanges();
                    Console.WriteLine($"Written {touched}");
                }
            }


            Console.WriteLine("That's all folks.");
            Console.ReadKey();
        }
    }
}
