using IntegradorLPR.Client;

namespace IntegradorLPR.Controllers
{
    public class AtualizarTimestampController
    {
        private readonly IntelbrasHttpClient _intelbrasHttpClient;

        public AtualizarTimestampController(IntelbrasHttpClient intelbrasHttpClient)
        {
            _intelbrasHttpClient = intelbrasHttpClient;
        }

        public async Task SetDateTime(string urlAtualizarDataHora, string username, string password)
        {
            DateTime dataHoraAtual = DateTime.Now;

            string formatarDataHora = dataHoraAtual.ToString("yyyy-MM-dd'%20'hh:mm:ss");

            string urlCompleta = $"{urlAtualizarDataHora}{formatarDataHora}";

            using (var response = await _intelbrasHttpClient.GetAsync(urlCompleta, username, password))
            //using (var response = await _intelbrasHttpClient.GetAsync(urlCompleta))
            {
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data e hora atualizada com sucesso!");
                }
                else
                {
                    Console.WriteLine("Erro ao atualizar a data e a hora.");
                }
            }
        }
    }
}

