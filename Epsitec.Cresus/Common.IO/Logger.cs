//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.IO;

namespace Epsitec.Common.IO
{

	/// <summary>
	/// The Logger class provides the tools used to log simple messages to files and to
	/// the console.
	/// </summary>
	public static class Logger
	{

		/// <summary>
		/// Logs message to the default log file (UserAppData\Logs\logs.txt). The format
		/// used to print message is the same as the one in Log(string, file).
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void Log(string message)
		{
			Logger.Log (message, Logger.defaultLogFile);
		}

		/// <summary>
		/// Logs message to file. Each line is printed with the date and time
		/// at the beginning.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="file">The file to use as log file.</param>
		public static void Log(string message, string file)
		{
			Logger.CreateDirectoryIfNotExist (file);

			using (StreamWriter streamWriter = File.AppendText (file))
			{
				foreach (string line in Logger.BuildLines (message))
				{
					streamWriter.WriteLine (line);
				}

				streamWriter.WriteLine ();
			}
		}

		/// <summary>
		/// Logs message to the console. Each line is printed with the date and
		/// time at the beginning.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogToConsole(string message)
		{
			foreach (string line in Logger.BuildLines (message))
			{
				System.Console.WriteLine (line);
			}

			System.Console.WriteLine ();
		}

		/// <summary>
		/// Builds the lines of the messages that is to be logged. It appends the
		/// date and time at the start of each line and removes the trailing '\n'.
		/// </summary>
		/// <param name="message">The message whose lines to be built.</param>
		/// <returns>An array containing the lines of the message.</returns>
		private static string[] BuildLines(string message)
		{
			string[] lines = message.Split ('\n');

			for (int i = 0; i < lines.Length; i++)
            {
            	lines[i] = string.Format ("[{0}]\t{1}", System.DateTime.Now, lines[i]);
			}

			return lines;
		}

		/// <summary>
		/// Creates the parent directory of file if it does not exist.
		/// </summary>
		/// <param name="file">The file whose parent directory to create.</param>
		private static void CreateDirectoryIfNotExist(string file)
		{
			DirectoryInfo directory = Directory.GetParent (file);

			if (!directory.Exists)
			{
				directory.Create ();
			}
		}

		/// <summary>
		/// The path of the default log file (UserAppData\Logs\logs.txt).
		/// </summary>
		/// <remarks>
		/// Cannot use Epsitec.Common.Support.Globals.Directories.UserAppData to build the path
		/// because Visual Studio cannot handle circular project references.
		/// </remarks>
		public static readonly string defaultLogFile = string.Format (@"{0}\Logs\logs.txt", System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.UserAppDataPath));

	}

}
