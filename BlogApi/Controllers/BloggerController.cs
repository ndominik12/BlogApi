using BlogApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloggerController : ControllerBase
    {
        public string ConnetionString { get; set; } = "server=localhost;database=blog;uid=root;password=";
        [HttpGet]
        public object GetAllBlogger()
        {
            List<Models.Blogger> bloggers = new List<Models.Blogger>();
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);

            connector.Open();

            string sql = "SELECT * FROM `blogger`";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var blogger = new Models.Blogger();
                blogger.Id = datareader.GetInt32("id");
                blogger.Name = datareader.GetString("name");
                blogger.Email = datareader.GetString("email");
                blogger.Age = datareader.GetInt32("age");
                blogger.Password = datareader.GetString("password");
                blogger.RegistrationTime = datareader.GetDateTime("registration_time");
                bloggers.Add(blogger);
            }
            
            connector.Close();

            return new { message = "Sikeres lekérdezés"};
        }
        [HttpGet]
        public object GetBloggerById(int id)
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);

            connector.Open();

            string sql = "SELECT * FROM `blogger` WHERE id = @id";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();
            datareader.Read();

            connector.Close();
            return new { message = "Sikeres találat" };
        }
    }
}

