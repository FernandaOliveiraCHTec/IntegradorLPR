using IntegradorLPR.Client;
using IntegradorLPR.Models;
using IntegradorLPR.Services;
using System.Text.RegularExpressions;

namespace IntegradorLPR.Controllers
{
    public class MonitoramentoController
    {
        private readonly PlacaService _placaService;
        private readonly IntelbrasHttpClient _intelbrasHttpClient;
        private readonly string _connectionString;

        public MonitoramentoController(PlacaService placaService, IntelbrasHttpClient intelbrasHttpClient, string connectionString)
        {
            _placaService = placaService;
            _intelbrasHttpClient = intelbrasHttpClient;
            _connectionString = connectionString;
        }

        public async Task MonitorarPlacas(string urlMonitorarPlaca, string username, string password, string connectionString)

        {
            string numeroPlaca = "";
            string dataHora = "";
            string ipCamera = "";

            using (var response = await _intelbrasHttpClient.GetAsync(urlMonitorarPlaca, username, password))
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string linha = await reader.ReadLineAsync();
                            Console.WriteLine(linha);

                            if (!String.IsNullOrEmpty(linha))
                            {
                                if (linha.Contains("Events[0].TrafficCar.PlateNumber="))
                                {
                                    string padraoPlaca = @"(?<plate>Events\[0\].TrafficCar.PlateNumber=\w+.\w+)";
                                    Match placaMatch = Regex.Match(linha, padraoPlaca);

                                    numeroPlaca = placaMatch.Success ? placaMatch.Groups["plate"].Value.Split('=')[1] : string.Empty;
                                }

                                if (Regex.IsMatch(linha, @"(?<date>\d{4}-\d{2}-\d{2}.\d{2}:\d{2}:\d{2})"))
                                {
                                    string padraoData = @"(?<date>\d{4}-\d{2}-\d{2}.\d{2}:\d{2}:\d{2})";
                                    Match dateMatch = Regex.Match(linha, padraoData);

                                    dataHora = dateMatch.Success ? dateMatch.Groups["date"].Value : string.Empty;
                                }

                                if (Regex.IsMatch(linha, @"(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})"))
                                {
                                    string padraoIp = @"(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})";
                                    Match ipMatch = Regex.Match(linha, padraoIp);

                                    ipCamera = ipMatch.Success ? ipMatch.Groups["ip"].Value : string.Empty;
                                }

                                if (!String.IsNullOrEmpty(dataHora) && !String.IsNullOrEmpty(ipCamera) && !String.IsNullOrEmpty(numeroPlaca))
                                {
                                    var trafficEvent = new TrafficEvent
                                    {
                                        PlateNumber = numeroPlaca,
                                        Timestamp = DateTime.Parse(dataHora),
                                        CameraIP = ipCamera
                                    };

                                    await _placaService.InsertTrafficEvent(trafficEvent, connectionString);

                                    numeroPlaca = "";
                                    dataHora = "";
                                    ipCamera = "";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
