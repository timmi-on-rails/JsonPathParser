using System;

namespace JsonParser
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			while (true) {
				string input = Console.ReadLine ().TrimEnd ('\n');

				try {
					JsonPath path = JsonPath.Parse (input);

					foreach (var token in path) {
						Console.WriteLine (token);
					}
				} catch (ArgumentException e) {
					Console.WriteLine (e.Message);
				}
			}
		}
	}
}
