using System;

namespace JsonParser
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			string input = Console.ReadLine ().TrimEnd ('\n');
			JsonPath path = JsonPath.Parse (input);

			foreach (var token in path) {
				Console.WriteLine (token);
			}
		}
	}
}
