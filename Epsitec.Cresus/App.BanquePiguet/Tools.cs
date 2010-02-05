//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The Tools class contains some static methods that are usefull somewhere else.
	/// </summary>
	static class Tools
	{

		/// <summary>
		/// Gets a Stream for the resource given by fileName.
		/// </summary>
		/// <param name="fileName">The name of the resource.</param>
		/// <returns>A Stream for fileName.</returns>
		/// <exception cref="System.ArgumentException">If the resource does not exit.</exception>
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

		/// <summary>
		/// Gets the setting given by key out of the settings file.
		/// </summary>
		/// <param name="key">The key of the setting to get.</param>
		/// <returns>The value of the setting given by key.</returns>
		public static string LoadSetting(string key)
		{
			XElement xSetting =  Tools.LoadSettings ().Element (key);

			string value;

			if (xSetting == null)
			{
				value = null;
			}
			else
			{
				value = xSetting.Value;
			}
			
			return value;
		}

		/// <summary>
		/// Saves the setting given by key with value value in the settings file.
		/// </summary>
		/// <param name="key">The key of the setting to save.</param>
		/// <param name="value">The value of the setting to save.</param>
		public static void SaveSetting(string key, string value)
		{
			XElement xSettings = Tools.LoadSettings ();
			XElement xSetting = xSettings.Element (key);

			if (xSetting == null)
			{
				xSettings.Add (new XElement(key, value));
			}
			else
			{
				xSetting.SetValue (value);
			}

			Tools.SaveSettings (xSettings);
		}

		/// <summary>
		/// Loads the settings out of the settings file.
		/// </summary>
		/// <returns>The XElement containing the settings.</returns>
		private static XElement LoadSettings()
		{
			XElement xSettings;

			if (!File.Exists (Tools.settingsFile))
			{
				xSettings = new XElement ("settings");
			}
			else
			{
				using (StreamReader streamReader = new StreamReader (Tools.settingsFile))
				{
					using (XmlReader xmlReader = XmlReader.Create (streamReader))
					{
						xSettings = XElement.Load (xmlReader);
					}
				}
			}

			return xSettings;
		}

		/// <summary>
		/// Saves the settings to the settings file. Note that all settings will be erased before
		/// writing to the file, so this method should only be called with all the settings included
		/// in xSettings.
		/// </summary>
		/// <param name="xSettings">The settings to save.</param>
		private static void SaveSettings(XElement xSettings)
		{
			XmlWriterSettings xmlSettings = new XmlWriterSettings ()
			{
				Indent = true,
				NewLineOnAttributes = true,
			};

			Tools.CreateDirectoryIfNotExist (Tools.settingsFile);

			using (StreamWriter streamWriter = new StreamWriter (Tools.settingsFile))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create (streamWriter, xmlSettings))
				{
					xSettings.WriteTo (xmlWriter);
				}
			}
		}

		/// <summary>
		/// Logs message> to the error log file and throws an new Exception with message as its message.
		/// </summary>
		/// <param name="message">The message of the error.</param>
		/// <exception cref="System.Exception">Always.</exception>
		public static void Error(string message)
		{
			Tools.Error (new System.Exception (message));
		}


		/// <summary>
		/// Logs exception to the error log file and throws it.
		/// </summary>
		/// <param name="exception">The Exception to log and throw.</param>
		/// <exception cref="System.Exception">Always.</exception>
		public static void Error(System.Exception exception)
		{
			Tools.LogException (exception);
			throw exception;
		}

		/// <summary>
		/// Logs Exception to the error log file. The type, message, source and stack trace are all
		/// logged, as well as all the internal exceptions.
		/// </summary>
		/// <param name="exception">The Exception to log.</param>
		public static void LogException(System.Exception exception)
		{
			string error = "========= New entry =========";

			for (System.Exception e = exception; e != null; e = e.InnerException)
			{
				error = string.Format ("{0}\n\nType: {1}\nMessage: {2}\nSource: {3}\nStack trace: {4}",
					error,
					e.GetType (),
					e.Message,
					e.Source,
					e.StackTrace
				);

			}

			Tools.LogError (error);
		}

		/// <summary>
		/// Logs message to the error log file.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogError(string message)
		{
			Tools.Log (message, Tools.errorFile);
		}

		/// <summary>
		/// Logs message to the log file.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogMessage(string message)
		{
			Tools.Log (message, Tools.logFile);
		}

		/// <summary>
		/// Logs message to file.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="file">The file to use as log file.</param>
		private static void Log(string message, string file)
		{
			Tools.CreateDirectoryIfNotExist (file);

			using (StreamWriter streamWriter = File.AppendText (file))
			{
				System.Array.ForEach (
					message.Split ('\n'),
					line => streamWriter.WriteLine (string.Format ("[{0}]\t{1}", System.DateTime.Now, line))
				);

				streamWriter.WriteLine ();
			}
		}

		/// <summary>
		/// Creates the parent directory of file if it does not exist.
		/// </summary>
		/// <param name="file">The file whose parent directory to create.</param>
		public static void CreateDirectoryIfNotExist(string file)
		{
			DirectoryInfo directory = Directory.GetParent (file);

			if (!directory.Exists)
			{
				directory.Create ();
			}
		}


		/// <summary>
		/// The executing assembly.
		/// </summary>
		private static readonly Assembly assembly = Assembly.GetExecutingAssembly ();


		/// <summary>
		/// The path to the resources of the program.
		/// </summary>
		private static readonly string resourcesLocation = "Epsitec.App.BanquePiguet.Resources";


		/// <summary>
		/// The path to the file which stores the settings.
		/// </summary>
		private static readonly string settingsFile = string.Format (@"{0}\Settings\settings.xml", Globals.Directories.UserAppData);

		/// <summary>
		/// The path to the file used to log messages.
		/// </summary>
		private static readonly string logFile = string.Format (@"{0}\Logs\logs.txt", Globals.Directories.UserAppData);

		/// <summary>
		/// The path to the file used to log errors.
		/// </summary>
		private static readonly string errorFile = string.Format (@"{0}\Logs\errors.txt", Globals.Directories.UserAppData);

	}

}
