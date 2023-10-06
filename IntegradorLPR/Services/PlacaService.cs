using IntegradorLPR.Models;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace IntegradorLPR.Services
{
    public class PlacaService
    {
        public async Task InsertTrafficEvent(TrafficEvent trafficEvent, string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                using (OracleCommand command = new OracleCommand("SP_INTEGRADOR_LPR", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new OracleParameter("P_IP_CAMERA", OracleDbType.Varchar2)).Value = trafficEvent.CameraIP;
                        command.Parameters.Add(new OracleParameter("P_NUM_PLACA", OracleDbType.Varchar2)).Value = trafficEvent.PlateNumber;

                        await command.ExecuteNonQueryAsync();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Erro ao inserir os dados no banco de dados." + ex.ToString());
                    }
                }
            }
        }
    }
}
