using System;
using System.Linq;

namespace Unfuddle.RestClient.IntegrationTests
{
	public static class PeopleTests
	{
		public static void ExecuteAllMethods(UnfuddleRestClient api)
		{
			Console.WriteLine("");
			Console.WriteLine(new string('-', 25));
			Console.WriteLine("Executing PEOPLE methods...");

			var user = api.GetUser("desautelsj");
			Console.WriteLine("desautelsj --> Id: {0}", user);

			var people = api.GetPeople();
			Console.WriteLine("Retrieved all people. There are {0} people.", people.Count());


			Console.WriteLine(new string('-', 25));
			Console.WriteLine("");
		}
	}
}