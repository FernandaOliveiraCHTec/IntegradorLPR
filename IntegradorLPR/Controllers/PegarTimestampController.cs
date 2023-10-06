using IntegradorLPR.Client;

namespace IntegradorLPR.Controllers
{
    public class PegarTimestampController
    {
        private readonly IntelbrasHttpClient _intelbrasHttpClient;

        public PegarTimestampController(IntelbrasHttpClient intelbrasHttpClient)
        {
            _intelbrasHttpClient = intelbrasHttpClient;
        }

        public async Task GetDateTime(string urlPegarDataHora, string username, string password)
        {
            using (var response = await _intelbrasHttpClient.GetAsync(urlPegarDataHora, username, password))
            //using (var response = await _intelbrasHttpClient.GetAsync(urlPegarDataHora))
            {
                if (response.IsSuccessStatusCode)
                {
                    string conteudo = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(conteudo);
                }
                else
                {
                    Console.WriteLine("Erro ao buscar a data e a hora.");
                }
            }
        }
    }
}
