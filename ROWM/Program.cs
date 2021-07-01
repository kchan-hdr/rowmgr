using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ROWM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup(System.Reflection.Assembly.GetExecutingAssembly().FullName)
                .Build();
    }
}
