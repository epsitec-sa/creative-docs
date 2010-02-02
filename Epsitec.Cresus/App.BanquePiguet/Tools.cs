//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using System.IO;
using System.Linq;
using System.Reflection;

namespace Epsitec.App.BanquePiguet
{

	static class Tools
	{

		public static Stream GetResourceStream(string fileName)
		{
			string path = string.Format("{0}.{1}", Tools.resourcesLocation, fileName);

			if (!assembly.GetManifestResourceNames ().Contains (path))
			{
				string message = string.Format ("The requested resource does not exist: {0}", fileName);
				throw new System.ArgumentException (message);
			}

			return Tools.assembly.GetManifestResourceStream (path);
		}

		public static void Error(string message)
		{
			Tools.Error (new System.Exception (message));
		}

		public static void Error(System.Exception exception)
		{
			string error = "========= New entry =========";

			for (System.Exception e = exception; e != null; e = e.InnerException)
			{
				error = string.Format ("{0}\n\nType: {1}\nMessage: {2}\nSource: {3}\nStack trace: {4}", 
					error,
					e.GetType(),
					e.Message,
					e.Source,
					e.StackTrace
				);
				
			}

			Tools.LogError (error);

			throw exception;
		}

		public static void LogMessage(string message)
		{
			Tools.Log (message, Tools.logFile);
		}

		public static void LogError(string message)
		{
			Tools.Log (message, Tools.errorFile);
		}

		private static void Log(string message, string file)
		{
			DirectoryInfo logDirectory = Directory.GetParent(file);

			if (!logDirectory.Exists)
			{
				logDirectory.Create ();
			}

			using (StreamWriter streamWriter = File.AppendText (file))
			{
				System.Array.ForEach (
					message.Split ('\n'),
					line => streamWriter.WriteLine (string.Format ("[{0}]\t{1}", System.DateTime.Now, line))
				);

				streamWriter.WriteLine ();
			}
		}

		private static Assembly assembly = Assembly.GetExecutingAssembly ();

		private static string resourcesLocation = "Epsitec.App.BanquePiguet.Resources";

		private static string logFile = string.Format (@"{0}\Logs\logs.txt", Globals.Directories.UserAppData);

		private static string errorFile = string.Format (@"{0}\Logs\errors.txt", Globals.Directories.UserAppData);

	}

}
