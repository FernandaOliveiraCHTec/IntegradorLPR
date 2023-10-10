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

        public MeuServico(IConfiguration config)
        {
            _config = config;
            _serviceProvider = ConfigureServices(config);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Iniciando...");

            // Iniciar o trabalho em segundo plano
            Task.Run(() => ExecuteControllers(_serviceProvider, _config, _cancellationTokenSource.Token));

            Console.WriteLine("Concluído!");
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        public IServiceProvider ConfigureServices(IConfiguration config)
        {
            var cameraSettings = config.GetSection("AppSettings:Cameras").Get<List<CameraSettings>>() ?? new List<CameraSettings>();

            var services = new ServiceCollection();
            services.AddSingleton<PlacaService>();
            services.AddSingleton<AtualizarTimestampController>();
            services.AddSingleton<PegarTimestampController>();
            services.AddSingleton<ReiniciarCameraController>();

            foreach (var cameraSetting in cameraSettings)
            {
                var intelbrasHttpClient = new IntelbrasHttpClient(cameraSetting.Username, cameraSetting.Password);

                services.AddSingleton<MonitoramentoController>(provider => new MonitoramentoController(
                    provider.GetRequiredService<PlacaService>(),
                    intelbrasHttpClient,
                    cameraSetting.ConnectionString));
            }

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        private async Task ExecuteControllers(IServiceProvider serviceProvider, IConfiguration config, CancellationToken cancellationToken)
        {
            var monitoramentoController = serviceProvider.GetRequiredService<MonitoramentoController>();

            var cameraSettingsList = config.GetSection("AppSettings:Cameras").Get<List<CameraSettings>>() ?? new List<CameraSettings>();

            var rotas = new string[]
            {
                "/cgi-bin/snapManager.cgi?action=attachFileProc&Flags[0]=Event&Events=[TrafficJunction]&heartbeat=3",
            }; // Adicione as rotas que deseja monitorar

            while (!cancellationToken.IsCancellationRequested)
            {
                var tasks = new List<Task>();

                foreach (var cameraSettings in cameraSettingsList)
                {
                    var ip = cameraSettings.Ip;
                    var username = cameraSettings.Username!;
                    var password = cameraSettings.Password!;
                    var connectionString = cameraSettings.ConnectionString!;

                    foreach (var rota in rotas)
                    {
                        var urlMonitorarPlaca = $"http://{ip}{rota}";

                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await monitoramentoController.MonitorarPlacas(urlMonitorarPlaca, username, password, connectionString);
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine($"Ocorreu um erro no trabalho: {ex}");

                                var exceptionModel = new ExceptionModel
                                {
                                    IpCamera = ip,
                                    Message = ex.Message
                                };

                                // Crie uma instância da classe PlacaService
                                var placaService = serviceProvider.GetRequiredService<PlacaService>();

                                // Método para salvar a exceção no banco de dados
                                await placaService.InsertException(exceptionModel, connectionString);

                                Console.WriteLine($"Erro ao monitorar câmera {ip}: {ex.Message}");
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
