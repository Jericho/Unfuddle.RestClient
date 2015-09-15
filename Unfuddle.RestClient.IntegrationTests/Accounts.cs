using System;
using System.Linq;

namespace Unfuddle.RestClient.IntegrationTests
{
	public static class AccountsTests
	{
		public static void ExecuteAllMethods(UnfuddleRestClient api)
		{
			Console.WriteLine("");
			Console.WriteLine(new string('-', 25));
			Console.WriteLine("Executing ACCOUNTS methods...");

			var accounts = api.GetAccounts();
			Console.WriteLine("Retrieved all accounts. There are {0} accounts.", accounts.Count());


			Console.WriteLine(new string('-', 25));
			Console.WriteLine("");
		}
	}
}