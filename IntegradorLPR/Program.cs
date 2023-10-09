using Topshelf;

namespace IntegradorLPR
{
    public class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            HostFactory.Run(x =>
            {
                x.Service<MeuServico>(s =>
                {
                    s.ConstructUsing(name => new MeuServico(config));
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetServiceName("IntegradorLPR");
                x.SetDisplayName("Integrador LPR");
                x.SetDescription("Integrador LPR rodando como serviço do Windows.");
                x.SetStartTimeout(TimeSpan.FromMinutes(2));
            });
        }
    }
}
