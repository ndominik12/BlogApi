using BlogApi.DTOs;
using BlogApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mysqlx.Crud;
using System.Xml.Linq;

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

            return new { message = "Sikeres lekérdezés", result = bloggers };
        }

        [HttpGet("names")]
        public object GetBloggerNames()
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);
            connector.Open();

            string sql = "SELECT `name`, `email` FROM `blogger` ORDER BY `name` ASC";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            var result = new List<DTOs.BloggerNameEmailDto>();

            while (datareader.Read())
            {
                var item = new DTOs.BloggerNameEmailDto();
                item.Name = datareader.GetString("name");
                item.Email = datareader.IsDBNull(datareader.GetOrdinal("email")) ? null : datareader.GetString("email");
                result.Add(item);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés", result = result };
        }

        // GET: api/blogger/find?name=nev&email=email
        [HttpGet("find")]
        public object LekerdezNevEsEmail([FromQuery] string name, [FromQuery] string email)
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);
            connector.Open();

            string sql = "SELECT `name`, `email` FROM `blogger` WHERE `name` = @name AND `email` = @email LIMIT 1";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email", email);

            var datareader = cmd.ExecuteReader();

            if (datareader.Read())
            {
                var item = new DTOs.BloggerNameEmailDto();
                item.Name = datareader.GetString("name");
                item.Email = datareader.IsDBNull(datareader.GetOrdinal("email")) ? null : datareader.GetString("email");
                connector.Close();
                return new { message = "Sikeres találat", result = item };
            }

            connector.Close();
            return new { message = "Nincs találat", result = (object?)null };
        }

        // GET: api/blogger/{id}/posts
        // Visszaadja egy blogger nevét és az összes hozzátartozó post title és content értékét
        [HttpGet("{id}/posts")]
        public object GetBloggerWithPosts(int id)
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);
            connector.Open();

            // Először megszerezzük a blogger nevét
            string sqlB = "SELECT `name` FROM `blogger` WHERE `id` = @id LIMIT 1";
            var cmdB = new MySqlConnector.MySqlCommand(sqlB, connector);
            cmdB.Parameters.AddWithValue("@id", id);
            var readerB = cmdB.ExecuteReader();

            if (!readerB.Read())
            {
                connector.Close();
                return new { message = "Nincs ilyen blogger", result = (object?)null };
            }

            var bloggerName = readerB.GetString("name");
            readerB.Close();

            // Majd lekérdezzük a hozzátartozó posztokat (title, content)
            string sqlP = "SELECT `title`, `content` FROM `post` WHERE `blogger_id` = @id ORDER BY `id` DESC";
            var cmdP = new MySqlConnector.MySqlCommand(sqlP, connector);
            cmdP.Parameters.AddWithValue("@id", id);
            var readerP = cmdP.ExecuteReader();

            var posts = new List<object>();
            while (readerP.Read())
            {
                var title = readerP.IsDBNull(readerP.GetOrdinal("title")) ? null : readerP.GetString("title");
                var content = readerP.IsDBNull(readerP.GetOrdinal("content")) ? null : readerP.GetString("content");
                posts.Add(new { title, content });
            }

            readerP.Close();
            connector.Close();

            return new { message = "Sikeres lekérdezés", result = new { name = bloggerName, posts = posts } };
        }
        [HttpGet("count")]
        public object GetBloggerCount()
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);

            connector.Open();

            string sql = "SELECT COUNT(*) AS cnt FROM `blogger`";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            int count = 0;
            if (datareader.Read())
            {
                count = datareader.GetInt32(0);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés", count = count };
        }

        // GET: api/blogger/posts/count
        // Visszaadja az összes post (bejegyzés) számát az adatbázisban
        [HttpGet("posts/count")]
        public object OsszesPostSzam()
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);

            connector.Open();

            string sql = "SELECT COUNT(*) AS cnt FROM `post`";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            int count = 0;
            if (datareader.Read())
            {
                count = datareader.GetInt32(0);
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés", count = count };
        }

        [HttpGet("{id}")]
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
        [HttpPost]
        public object AddNewBlogger(AddNewBloggerDto addNewBloggerDto)
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);
            connector.Open();

            string sql = "INSERT INTO `blogger` (`name`, `email`, `age`, `password`, `registration_time`) " +
                         "VALUES (@name, @email, @age, @password, @registrationTime)";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@name", addNewBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", addNewBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", addNewBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", addNewBloggerDto.Password);
            cmd.Parameters.AddWithValue("@registrationTime", DateTime.UtcNow);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "sikeres hozzáadás.", result = addNewBloggerDto };

        }

        [HttpPost("login")]
        public object LoginBlogger(LoginBloggerDto loginBloggerDto)
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);

            connector.Open();

            string sql = @"SELECT `id` FROM `blogger` 
                           WHERE `email` = @email AND `password` = @password;";

            var cmd = new MySqlConnector.MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@email", loginBloggerDto.Email);
            cmd.Parameters.AddWithValue("@password", loginBloggerDto.Password);

            var datareader = cmd.ExecuteReader();

            if (datareader.Read() == true)
            {
                var id = datareader.GetInt32("id");
                connector.Close();
                return new { message = "Sikeres belépés.", result = datareader.GetInt32("id") };
            }
            else
            {
                connector.Close();
                return new { message = "Sikertelen belépés.", result = loginBloggerDto };
            }
        }
        [HttpDelete("delete")]
        public object DeleteBlogger(DeleteBloggerDto deleteBloggerDto)
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);
            connector.Open();
            string sql = @"DELETE FROM `blogger` 
                           WHERE `email` = @email AND `password` = @password;";
            var cmd = new MySqlConnector.MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@email", deleteBloggerDto.Email);
            cmd.Parameters.AddWithValue("@password", deleteBloggerDto.Password);
            int rowsAffected = cmd.ExecuteNonQuery();
            connector.Close();
            if (rowsAffected > 0)
            {
                return new { message = "Sikeres törlés.", result = deleteBloggerDto };
            }
            else
            {
                return new { message = "Sikertelen törlés. Nincs ilyen blogger.", result = deleteBloggerDto };
            }
        }
        [HttpPut("put")]
        public object PutBloggerDto(PutBloggerDto putBloggerDto)
        {
            var connector = new MySqlConnector.MySqlConnection(ConnetionString);
            connector.Open();
            string sql = @"UPDATE `blogger` 
                           SET `name` = @name, `age` = @age, `password` = @password
                           WHERE `email` = @email;";
            var cmd = new MySqlConnector.MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@name", putBloggerDto.Name);
            cmd.Parameters.AddWithValue("@age", putBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", putBloggerDto.Password);
            cmd.Parameters.AddWithValue("@email", putBloggerDto.Email);
            int rowsAffected = cmd.ExecuteNonQuery();
            connector.Close();
            if (rowsAffected > 0)
            {
                return new { message = "Sikeres frissítés.", result = putBloggerDto };
            }
            else
            {
                return new { message = "Sikertelen frissítés. Nincs ilyen blogger.", result = putBloggerDto  };
            }
        }
    }
}