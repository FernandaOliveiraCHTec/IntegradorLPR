using IntegradorLPR.Client;

namespace IntegradorLPR.Controllers
{
    public class ReiniciarCameraController
    {
        private readonly IntelbrasHttpClient _intelbrasHttpClient;

        public ReiniciarCameraController(IntelbrasHttpClient intelbrasHttpClient)
        {
            _intelbrasHttpClient = intelbrasHttpClient;
        }

        public async Task ResetCamera(string urlPegarDataHora, string username, string password)
        {
            using (var response = await _intelbrasHttpClient.GetAsync(urlPegarDataHora, username, password))
            //using (var response = await _intelbrasHttpClient.GetAsync(urlPegarDataHora))
            {
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Câmera reiniciada com sucesso!");
                }
                else
                {
                    Console.WriteLine("Erro ao reiniciar a câmera.");
                }
            }
        }
    }
}
