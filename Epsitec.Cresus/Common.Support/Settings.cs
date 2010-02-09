//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{

	/// <summary>
	/// The Settings class provides the tools to load, modify and save settings.
	/// </summary>
	public class Settings : Dictionary<string, string>
	{

		/// <summary>
		/// Initializes a new instance of the Settings class. The settings are loaded from
		/// file and if a setting does not exist in file, it is set to its default value
		/// taken from defaultValues.
		/// </summary>
		/// <param name="file">The file containing the settings to load.</param>
		/// <param name="defaultValues">The default values of the settings.</param>
		protected Settings(string file, Dictionary<string,string> defaultValues)
			: base (defaultValues)
		{
			this.File = file;
			
			if (System.IO.File.Exists (file))
			{
				XElement xSettings;

				using (StreamReader streamReader = new StreamReader (file))
				{
					using (XmlReader xmlReader = XmlReader.Create (streamReader))
					{
						xSettings = XElement.Load (xmlReader);
					}
				}

				foreach (XElement xSetting in xSettings.Elements ("entry"))
				{
					string key = xSetting.Element ("key").Value;
					string value = xSetting.Element ("value").Value;

					this[key] = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the file where the settings are stored.
		/// </summary>
		/// <value>The file where the settings are stored.</value>
		protected string File
		{
			get;
			set;
		}

		/// <summary>
		/// Saves the settings to their file, which is the file from where they where
		/// loaded.
		/// </summary>
		public void Save()
		{
			this.Save (this.File);
		}

		/// <summary>
		/// Saves the settings to file.
		/// </summary>
		/// <param name="file">The file.</param>
		public void Save(string file)
		{
			XElement xSettings = new XElement ("settings");

			foreach (string key in this.Keys)
			{
				xSettings.Add (new XElement (
					"entry",
					new XElement ("key", key),
					new XElement ("value", this[key])
				));
			}

			XmlWriterSettings xmlSettings = new XmlWriterSettings ()
			{
				Indent = true,
				NewLineOnAttributes = true,
			};
			
			DirectoryInfo directory = Directory.GetParent (file);

			if (!directory.Exists)
			{
				directory.Create ();
			}

			using (StreamWriter streamWriter = new StreamWriter (file))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create (streamWriter, xmlSettings))
				{
					xSettings.Save (xmlWriter);
				}
			}
		}

		/// <summary>
		/// Loads the settings stored in the default settings file.
		/// </summary>
		/// <returns>The loaded settings.</returns>
		public static Settings Load()
		{
			return Settings.Load (Settings.defaultSettingsFile);
		}

		/// <summary>
		/// Loads the settings stored in the default settings file and uses defaultValues
		/// to provide the values which are not present in the file.
		/// </summary>
		/// <param name="defaultValues">The default values.</param>
		/// <returns>The loaded settings.</returns>
		public static Settings Load(Dictionary<string, string> defaultValues)
		{
			return Settings.Load (Settings.defaultSettingsFile, defaultValues);
		}

		/// <summary>
		/// Loads the settings stored in file.
		/// </summary>
		/// <param name="file">The file to load.</param>
		/// <returns>The loaded settings.</returns>
		public static Settings Load(string file)
		{
			return Settings.Load (file, new Dictionary<string,string>());
		}

		/// <summary>
		/// Loads the settings stored in file and uses defaultValues to provide the
		/// values which are not present in the file.
		/// </summary>
		/// <param name="file">The file to load.</param>
		/// <param name="defaultValues">The default values.</param>
		/// <returns>The loaded settings.</returns>
		public static Settings Load(string file, Dictionary<string, string> defaultValues)
		{
			return new Settings (file, defaultValues);
		}

		/// <summary>
		/// The path of the default log file (UserAppData\Settings\settings.xml).
		/// </summary>
		/// <remarks>
		/// Cannot use Epsitec.Common.Support.Globals.Directories.UserAppData to build the path
		/// because Visual Studio cannot handle circular project references.
		/// </remarks>
		public static readonly string defaultSettingsFile = string.Format (@"{0}\Settings\settings.xml", System.IO.Path.GetDirectoryName (System.Windows.Forms.Application.UserAppDataPath));

	}

}
