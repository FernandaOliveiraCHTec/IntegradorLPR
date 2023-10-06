using IntegradorLPR.Client;
using IntegradorLPR.Controllers;
using IntegradorLPR.Models;
using IntegradorLPR.Services;

namespace IntegradorLPR
{
    public class MeuServico
    {
        private IConfiguration _config;
        private IServiceProvider _serviceProvider;
        private CancellationTokenSource? _cancellationTokenSource;

        public MeuServico()
        {
            _config = LoadConfiguration();
            _serviceProvider = ConfigureServices(_config);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Iniciando...");

            // Iniciar o trabalho em segundo plano
            Task.Run(() => ExecuteControllers(_serviceProvider, _config, _cancellationTokenSource.Token));


            //foreach (var camerasSetting in _config.GetSection("AppSettings:Cameras").Get<List<CameraSettings>>())
            //{
            //    Task.Run(() => ExecuteControllers(_serviceProvider, camerasSetting, _cancellationTokenSource.Token));
            //}

            Console.WriteLine("Concluído!");
        }

        public void Stop()
        {

            _cancellationTokenSource?.Cancel();

        }

        private IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private IServiceProvider ConfigureServices(IConfiguration config)
        {
            return new ServiceCollection()
                .AddSingleton<PlacaService>()
                .AddSingleton(provider => new IntelbrasHttpClient(
                    config["Appsettings:Username"],
                    config["Appsettings:Password"]))
                .AddSingleton<AtualizarTimestampController>()
                .AddSingleton<PegarTimestampController>()
                .AddSingleton<MonitoramentoController>()
                .AddSingleton<ReiniciarCameraController>()
                .BuildServiceProvider();
        }

        //private async Task ExecuteControllers(IServiceProvider serviceProvider, IConfiguration config, CancellationToken cancellationToken)
        ////private async Task ExecuteControllers(IServiceProvider serviceProvider, CameraSettings cameraSetting, CancellationToken cancellationToken)
        //{
        //    var monitoramentoController = serviceProvider.GetRequiredService<MonitoramentoController>();

        //    var urlMonitorarPlaca = config["Appsettings:UrlMonitorarPlaca"]!;
        //    var urlconnectionString = config["Appsettings:ConnectionString"]!;
        //    var username = config["Appsettings:Username"]!;
        //    var password = config["Appsettings:Password"]!;

        //    //var monitoramentoController = serviceProvider.GetRequiredService<MonitoramentoController>();

        //    //var urlMonitorarPlaca = cameraSetting.UrlMonitorarPlaca;
        //    //var connectionString = cameraSetting.ConnectionString;
        //    //var username = cameraSetting.Username;
        //    //var password = cameraSetting.Password;

        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        // Verifique se o cancelamento foi solicitado antes de executar o trabalho
        //        try
        //        {
        //            await monitoramentoController.MonitorarPlacas(urlMonitorarPlaca, username, password, urlconnectionString);
        //            //await monitoramentoController.MonitorarPlacas(urlMonitorarPlaca, username, password, connectionString);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Ocorreu um erro no trabalho: {ex}");
        //        }

        //        // Aguarde um intervalo antes de executar o trabalho novamente
        //        await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        //    }
        //}

        private async Task ExecuteControllers(IServiceProvider serviceProvider, IConfiguration config, CancellationToken cancellationToken)
        {
            var monitoramentoController = serviceProvider.GetRequiredService<MonitoramentoController>();

            var ips = new string[] { "10.22.7.60", "10.7.2.200", "10.7.2.202", "10.7.2.203" }; // Adicione os IPs das câmeras que deseja monitorar
            var rotas = new string[]
            {
                "/cgi-bin/snapManager.cgi?action=attachFileProc&Flags[0]=Event&Events=[TrafficJunction]&heartbeat=3",
            }; // Adicione as rotas que deseja monitorar

            while (!cancellationToken.IsCancellationRequested)
            {
                var tasks = new List<Task>();

                foreach (var ip in ips)
                {
                    foreach (var rota in rotas)
                    {
                        var urlMonitorarPlaca = $"http://{ip}{rota}";
                        var urlconnectionString = config["Appsettings:ConnectionString"]!;
                        var username = config["Appsettings:Username"]!;
                        var password = config["Appsettings:Password"]!;

                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await monitoramentoController.MonitorarPlacas(urlMonitorarPlaca, username, password, urlconnectionString);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ocorreu um erro no trabalho: {ex}");
                            }
                        }));
                    }
                }

                await Task.WhenAll(tasks);

                // Aguarde um intervalo antes de executar o trabalho novamente
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }
    }
}

//using IntegradorLPR.Client;
//using IntegradorLPR.Controllers;
//using IntegradorLPR.Models;
//using IntegradorLPR.Services;
//using System.Threading.Tasks;

//namespace IntegradorLPR
//{
//    public class MeuServico
//    {
//        private IConfiguration _config;
//        private IServiceProvider _serviceProvider;
//        private CancellationTokenSource? _cancellationTokenSource;

//        public MeuServico()
//        {
//            _config = LoadConfiguration();
//            _serviceProvider = ConfigureServices(_config);
//        }

//        public void Start()
//        {
//            _cancellationTokenSource = new CancellationTokenSource();

//            Console.WriteLine("Iniciando...");

//            foreach (var camerasSetting in _config.GetSection("AppSettings:Cameras").Get<List<CameraSettings>>())
//            {
//                Task.Run(() => ExecuteControllers(_serviceProvider, camerasSetting, _cancellationTokenSource.Token));
//            }

//            Console.WriteLine("Concluído!");
//        }

//        public void Stop()
//        {

//            _cancellationTokenSource?.Cancel();

//        }

//        private IConfiguration LoadConfiguration()
//        {
//            return new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json")
//                .Build();
//        }

//        private IServiceProvider ConfigureServices(IConfiguration config)
//        {
//            return new ServiceCollection()
//                .AddSingleton<PlacaService>()
//                .AddSingleton(provider => new IntelbrasHttpClient(
//                    config["Appsettings:Username"],
//                    config["Appsettings:Password"]))
//                .AddSingleton<AtualizarTimestampController>()
//                .AddSingleton<PegarTimestampController>()
//                .AddSingleton<MonitoramentoController>()
//                .AddSingleton<ReiniciarCameraController>()
//                .BuildServiceProvider();
//        }

//        private async Task ExecuteControllers(IServiceProvider serviceProvider, CameraSettings cameraSetting, CancellationToken cancellationToken)
//        {
//            var monitoramentoController = serviceProvider.GetRequiredService<MonitoramentoController>();

//            var urlMonitorarPlaca = cameraSetting.UrlMonitorarPlaca;
//            var connectionString = cameraSetting.ConnectionString;
//            var username = cameraSetting.Username;
//            var password = cameraSetting.Password;

//            while (!cancellationToken.IsCancellationRequested)
//            {
//                var tasks = new List<Task>();

//                // Verifique se o cancelamento foi solicitado antes de executar o trabalho
//                tasks.Add(Task.Run(async () =>
//                {
//                    try
//                    {
//                        //await monitoramentoController.MonitorarPlacas(urlMonitorarPlaca, username, password, urlconnectionString);
//                        await monitoramentoController.MonitorarPlacas(urlMonitorarPlaca, username, password, connectionString);

//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"Ocorreu um erro no trabalho: {ex}");
//                    }
//                }));

//                await Task.WhenAll(tasks);

//                // Aguarde um intervalo antes de executar o trabalho novamente
//                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
//            }
//        }
//    }
//}
