using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.DesignTime;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Utils;

namespace ModernSchoolDiary.Blazor.Server
{
    public class Program : IDesignTimeApplicationFactory
    {
        static bool ContainsArgument(string[] args, string argument)
        {
            return args.Any(arg => arg.TrimStart('/').TrimStart('-').ToLower() == argument.ToLower());
        }
        public static int Main(string[] args)
        {
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                if (name.Name != null && name.Name.EndsWith(".resources"))
                {
                    var ruPath = System.IO.Path.Combine(
                        AppContext.BaseDirectory, "ru", name.Name + ".dll");
                    if (System.IO.File.Exists(ruPath))
                        return context.LoadFromAssemblyPath(ruPath);
                }
                return null;
            };
            var culture = new System.Globalization.CultureInfo("ru-RU");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
            var asm = typeof(DevExpress.Blazor.DxGrid).Assembly;
            Console.WriteLine($"=== Версия сборки: {asm.GetName().Version}");
            Console.WriteLine($"=== Путь сборки: {asm.Location}");

            // какие сателлиты реально видны рантайму
            var dir = System.IO.Path.GetDirectoryName(asm.Location);
            var ruDir = System.IO.Path.Combine(dir, "ru");
            Console.WriteLine($"=== Папка ru существует: {System.IO.Directory.Exists(ruDir)}");
            if (System.IO.Directory.Exists(ruDir))
                foreach (var f in System.IO.Directory.GetFiles(ruDir))
                    Console.WriteLine($"      {System.IO.Path.GetFileName(f)}");

            // пробуем загрузить сателлит напрямую и прочитать его версию
            try
            {
                var satPath = System.IO.Path.Combine(ruDir, "DevExpress.Blazor.v25.2.resources.dll");
                var satAsm = System.Reflection.Assembly.LoadFile(satPath);
                Console.WriteLine($"=== Сателлит версия: {satAsm.GetName().Version}, культура: {satAsm.GetName().CultureName}");
                foreach (var n in satAsm.GetManifestResourceNames())
                    Console.WriteLine($"      ресурс: {n}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Не загрузить сателлит: {ex.Message}");
            }
            if (ContainsArgument(args, "help") || ContainsArgument(args, "h"))
            {
                Console.WriteLine("Updates the database when its version does not match the application's version.");
                Console.WriteLine();
                Console.WriteLine($"    {Assembly.GetExecutingAssembly().GetName().Name}.exe --updateDatabase [--forceUpdate --silent]");
                Console.WriteLine();
                Console.WriteLine("--forceUpdate - Marks that the database must be updated whether its version matches the application's version or not.");
                Console.WriteLine("--silent - Marks that database update proceeds automatically and does not require any interaction with the user.");
                Console.WriteLine();
                Console.WriteLine($"Exit codes: 0 - {DBUpdaterStatus.UpdateCompleted}");
                Console.WriteLine($"            1 - {DBUpdaterStatus.UpdateError}");
                Console.WriteLine($"            2 - {DBUpdaterStatus.UpdateNotNeeded}");
            }
            else
            {
                DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.Latest;
                DevExpress.ExpressApp.Security.SecurityStrategy.AutoAssociationReferencePropertyMode = DevExpress.ExpressApp.Security.ReferenceWithoutAssociationPermissionsMode.AllMembers;
                IHost host = CreateHostBuilder(args).Build();
                if (ContainsArgument(args, "updateDatabase"))
                {
                    using (var serviceScope = host.Services.CreateScope())
                    {
                        return serviceScope.ServiceProvider.GetRequiredService<DevExpress.ExpressApp.Utils.IDBUpdater>().Update(ContainsArgument(args, "forceUpdate"), ContainsArgument(args, "silent"));
                    }
                }
                else
                {
                    host.Run();
                }
            }
            return 0;
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        XafApplication IDesignTimeApplicationFactory.Create()
        {
            IHostBuilder hostBuilder = CreateHostBuilder(Array.Empty<string>());
            return DesignTimeApplicationFactoryHelper.Create(hostBuilder);
        }
    }
}
