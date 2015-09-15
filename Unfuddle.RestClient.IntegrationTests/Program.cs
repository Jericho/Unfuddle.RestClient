using System;
using System.Configuration;

namespace Unfuddle.RestClient.IntegrationTests
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("{0} Executing all Unfuddle API methods ... {0}", new string('=', 10));

			try
			{
				ExecuteAllMethods();
			}
			catch (Exception e)
			{
				Console.WriteLine("");
				Console.WriteLine("");
				Console.WriteLine("An error has occured: {0}", e.Message);
			}
			finally
			{
				// Clear the keyboard buffer
				while (Console.KeyAvailable) { Console.ReadKey(); }

				Console.WriteLine("");
				Console.WriteLine("Press any key...");
				Console.ReadKey();
			}
		}

		static void ExecuteAllMethods()
		{
			var userName = ConfigurationManager.AppSettings["UserName"];
			var password = ConfigurationManager.AppSettings["Password"];
			var subDomain = ConfigurationManager.AppSettings["SubDomain"];

			var api = new UnfuddleRestClient(userName, password, subDomain, true);

			PeopleTests.ExecuteAllMethods(api);
			AccountsTests.ExecuteAllMethods(api);
		}
	}
}
