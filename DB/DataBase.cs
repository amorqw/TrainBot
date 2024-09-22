using System.Threading.Channels;
using MySql.Data.MySqlClient;

namespace TrainBot.DB
{
    
    public class DataBase
    { 
        private MySqlConnection _connection;
        

        public DataBase()
        {
            var config = Config.LoadConfig();
            _connection = new MySqlConnection(config.ConnectionStringss);
        }

        public bool CheckConnect()
        {
            Console.WriteLine(_connection.ConnectionString);
            try
            {
                _connection.Open();
                Console.WriteLine("Connection заебися заработало!");
                _connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                
                Console.WriteLine(ex.Message);
                return false;
            }finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }
        } 
        
        

    }
}