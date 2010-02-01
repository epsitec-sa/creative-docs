//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Epsitec.App.BanquePiguet
{

	static class Tools
	{

		public static Stream GetResourceStream(string fileName)
		{
			string path = String.Format("{0}.{1}", Tools.resourcesLocation, fileName);

			if (!assembly.GetManifestResourceNames ().Contains (path))
			{
				string message = String.Format ("The requested resource does not exist: {0}", fileName);
				throw new ArgumentException (message);
			}

			return Tools.assembly.GetManifestResourceStream (path);
		}

		public static void Error(string message)
		{
			Tools.Error (new Exception (message));
		}

		public static void Error(Exception exception)
		{
			string error = "========= New entry =========";

			for (Exception e = exception; e != null; e = e.InnerException)
			{
				error = String.Format ("{0}\n\n{1}\n{2}\n{3}\n{4}", 
					error,
					String.Format ("Type: {0}", e.GetType()),
					String.Format ("Message: {0}", e.Message),
					String.Format ("Source: {0}", e.Source),
					String.Format ("Stack trace:\n{0}", e.StackTrace)
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
				Array.ForEach (
					message.Split ('\n'),
					line => streamWriter.WriteLine (String.Format ("[{0}]\t{1}", DateTime.Now, line))
				);

				streamWriter.WriteLine ();
			}
		}

		private static Assembly assembly = Assembly.GetExecutingAssembly ();

		private static string resourcesLocation = "Epsitec.App.BanquePiguet.Resources";

		private static string logFile = String.Format (@"{0}\Logs\logs.txt", Globals.Directories.UserAppData);

		private static string errorFile = String.Format (@"{0}\Logs\errors.txt", Globals.Directories.UserAppData);

	}

}
