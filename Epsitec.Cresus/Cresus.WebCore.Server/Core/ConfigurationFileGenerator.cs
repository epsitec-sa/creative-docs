using System.Collections.Generic;

using System.IO;

using System.Text;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	internal sealed class ConfigurationFileGenerator
	{


		public ConfigurationFileGenerator(FileInfo configurationFile, string configurationName)
		{
			this.configurationFile = configurationFile;
			this.configurationName = configurationName;

			this.configuration = new Dictionary<string, string> ();
		}


		public void Set(string key, string value)
		{
			this.SetValue (key, "'" + value + "'");
		}


		public void Set(string key, bool value)
		{
			this.SetValue (key, value ? "true" : "false");
		}


		private void SetValue(string key, string value)
		{
			this.configuration[key] = value;
		}


		public void Write()
		{
			using (var stream = this.configurationFile.Open (FileMode.Create, FileAccess.Write))
			using (var streamWriter = new StreamWriter (stream, Encoding.UTF8))
			{
				streamWriter.WriteLine ("var " + this.configurationName + " = {");

				int currentIndex = 0;
				int lastindex = this.configuration.Count - 1;

				foreach (var item in this.configuration)
				{
					var line = "  " + item.Key + ": " + item.Value;

					if (currentIndex < lastindex)
					{
						line += ",";
					}

					streamWriter.WriteLine (line);

					currentIndex++;
				}

				streamWriter.WriteLine ("};");
			}
		}


		private readonly FileInfo configurationFile;


		private readonly string configurationName;


		private readonly Dictionary<string, string> configuration;


	}


}
