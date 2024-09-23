using MySql.Data.MySqlClient;
using System;

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
        public bool UserExists(long userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connection.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE telegram_id = @UserId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке пользователя: {ex.Message}");
                return false;
            }
        }

        public void AddUser(long userId, string username, string firstName)
        {
            try
            {
                using (var connection = new MySqlConnection(_connection.ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO users (telegram_id, username, first_name) VALUES (@UserId, @Username, @FirstName)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@Username", username ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FirstName", firstName ?? (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении пользователя: {ex.Message}");
            }
        }


        public void AddExercise(long userId, string exercise)
        {
            
            try
            {
                using (var connection = new MySqlConnection(_connection.ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO exercises(telegram_id, exercise_name) VALUES (@UserId, @Exercise)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@Exercise", exercise);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine($"Упражнение {exercise} добавлено для пользователя с ID {userId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void AddWeight(long userId, int weight)
        {
            try
            {
                using (var connection = new MySqlConnection(_connection.ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE exercises SET weight = @Weight WHERE telegram_id = @UserId AND exercise_name = @ExerciseName";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Weight", weight);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine($"Вес {weight} добавлен для пользователя с ID {userId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AddReps(long userId, int repetitions, DateTime date)
        {
            try
            {
                using (var connection = new MySqlConnection(_connection.ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE exercises SET repetitions = @Repetitions, date = @Date WHERE telegram_id = @UserId AND exercise_name = @ExerciseName";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Repetitions", repetitions);
                        command.Parameters.AddWithValue("@Date", date);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine($"Повторения {repetitions} и дата {date.ToShortDateString()} обновлены для пользователя с ID {userId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        // Метод для получения всех упражнений пользователя
        public void GetUserExercises(long userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connection.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM exercises WHERE UserId = @UserId";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"Упражнение: {reader["Exercise"]}, Вес: {reader["Weight"]}, " +
                                                  $"Повторения: {reader["Repetitions"]}, Дата: {reader["Date"]} , Пользователь: {userId}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных: {ex.Message}");
            }
        }
        
        
        // Метод для проверки подключения к базе данных
        public bool CheckConnect()
        {
            try
            {
                using (var connection = new MySqlConnection(_connection.ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("БД коннект есть");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
                return false;
            }
        }
    }
}