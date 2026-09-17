using System;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Summary description for Blogger
/// </summary>
/// 
namespace BlogApi.Models
{
    public class Blogger
{
	public Blogger()
	{
			public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
            public string Password { get; set; }
            public DateTime RegistrationTime { get; set } 

        //
        // TODO: Add constructor logic here
        //
    }
}
