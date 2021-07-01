using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM2.Dal.Test
{
    [TestClass]
    public class DocumentSpeedTests
    {
        static readonly string _TEST_PID = "03S36E00500";

        [TestMethod]
        public async Task Compare_Load()
        {
            var watch = new Stopwatch();

            Trace.WriteLine($"Repo {DateTime.Now}");
            using (var ctx = new ROWM_Context())
            {
                ctx.Database.Log = m => Trace.WriteLine(m);

                watch.Restart();
                var repo = new OwnerRepository(ctx);
                var p = await repo.GetParcel(_TEST_PID);
                Trace.WriteLine($"{p.ParcelId}");
                Trace.WriteLine($"took: {watch.Elapsed}");
            }

            Trace.WriteLine($"EF {DateTime.Now}");
            using (var ctx = new ROWM_Context())
            {
                ctx.Database.Log = m => Trace.WriteLine(m);

                watch.Start();
                var p = await ctx.Parcel.FirstOrDefaultAsync(px => px.Assessor_Parcel_Number.Equals(_TEST_PID));
                Trace.WriteLine($"{p.ParcelId}");
                Trace.WriteLine($"took: {watch.Elapsed}");
                watch.Restart();
                Trace.WriteLine($"{p.ContactLog.Count()}");
                Trace.WriteLine($"took: {watch.Elapsed}");
                watch.Restart();
                Trace.WriteLine($"{p.Document.Count()}");
                Trace.WriteLine($"took: {watch.Elapsed}");
            }
        }

        [TestMethod]
        public async Task Compare_Documents_Load()
        {
            var watch = new Stopwatch();

            Trace.WriteLine($"Repo {DateTime.Now}");
            using (var ctx = new ROWM_Context())
            {
                ctx.Database.Log = m => Trace.WriteLine(m);

                watch.Restart();
                var repo = new OwnerRepository(ctx);
                var docs = await repo.GetDocumentsForParcel(_TEST_PID);
                Trace.WriteLine($"{docs.Count()}");
                Trace.WriteLine($"took: {watch.Elapsed}");
            }

            Trace.WriteLine($"EF {DateTime.Now}");
            using (var ctx = new ROWM_Context())
            {
                ctx.Database.Log = m => Trace.WriteLine(m);

                watch.Start();
                var p = await ctx.Parcel.FirstOrDefaultAsync(px => px.Assessor_Parcel_Number.Equals(_TEST_PID));
                Trace.WriteLine($"{p.ParcelId}");
                Trace.WriteLine($"took: {watch.Elapsed}");
                watch.Restart();
                Trace.WriteLine($"{p.ContactLog.Count()}");
                Trace.WriteLine($"took: {watch.Elapsed}");
                watch.Restart();
                Trace.WriteLine($"{p.Document.Count()}");
                Trace.WriteLine($"took: {watch.Elapsed}");
            }
        }
    }
}
