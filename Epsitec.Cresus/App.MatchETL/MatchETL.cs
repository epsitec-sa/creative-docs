using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Epsitec.Data.Platform;

namespace App.MatchETL
{
	class MatchETL
	{
		abstract class CommonSubOptions
		{
			[Option ('f', "file", Required = true, HelpText = "Output filename")]
			public string Filename
			{
				get;
				set;
			}
		}

		class ZipSubOptions : CommonSubOptions
		{
			
		}

		class MatchETLOptions
		{
			[VerbOption("zip", HelpText = "Extract ZipCode from Match")]
			public ZipSubOptions Zip
			{
				get;
				set;
			}

			[HelpOption]
			public string GetUsage()
			{
				var usage = new StringBuilder ();
				usage.AppendLine ("Usage: match zip -f s:\\nupost.txt");
				return usage.ToString ();
			}
		}

		static void Main(string[] args)
		{
			string invokedCmd = "";
			object invokedCmdInstance = null;

			var options = new MatchETLOptions ();
			if (args.Length > 0)
			{
				var parser       = CommandLine.Parser.Default;
				var hasArguments = parser.ParseArguments (args, options,
												(verb, subOptions) =>
												{
													invokedCmd = verb;
													invokedCmdInstance = subOptions;
												});
				if (!hasArguments)
				{
					Console.WriteLine (options.GetUsage ());
					Environment.Exit (CommandLine.Parser.DefaultExitCodeFail);
				}

				switch (invokedCmd)
				{
					case "zip":
					var zipOptions = (ZipSubOptions) invokedCmdInstance;
					SwissPost.GenerateCresusNupoFile (zipOptions.Filename);
					Environment.Exit (0);
					break;
					default:
					Console.WriteLine (options.GetUsage ());
					Environment.Exit (CommandLine.Parser.DefaultExitCodeFail);
					break;
				}
			}
			else
			{
				Console.WriteLine (options.GetUsage ());
				Environment.Exit (CommandLine.Parser.DefaultExitCodeFail);
			}
		}
	}
}
