using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Designer.Protocol
{
	class Program
	{
		static void Main(string[] args)
		{
			NavigatorService.DefineNavigateToStringAction (
				delegate (string id)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("NavigateToString: {0}", id));
				});

			using (Server server = new Server (true))
			{
				server.Open ();
				System.Console.ReadLine ();
			}
		}
	}
}
