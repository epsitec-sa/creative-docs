//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// Summary description for VersionChecker.
	/// </summary>
	public class VersionChecker
	{
		public VersionChecker(System.Reflection.Assembly assembly) : this (assembly.FullName.Split(',')[1].Split('=')[1])
		{
		}
		
		public VersionChecker(string version)
		{
			VersionChecker.SplitVersionString (version, out this.currentMajor, out this.currentMinor, out this.currentBuild, out this.currentRevision);
		}		
		
		public VersionChecker(int major, int minor, int build, int revision)
		{
			this.currentMajor    = major;
			this.currentMinor    = minor;
			this.currentBuild    = build;
			this.currentRevision = revision;
		}
		
		
		public bool								IsReady
		{
			get
			{
				if (this.readerResult != null)
				{
					return true;
				}
				if ((this.readerAsync != null) &&
					(this.readerAsync.IsCompleted))
				{
					this.readerResult = this.reader.EndInvoke (this.readerAsync);
					this.readerAsync  = null;
					
					if ((this.readerResult.Length > 0) &&
						(this.readerResult.IndexOf ('|') > 0))
					{
						string[] args = this.readerResult.Split ('|');
						
						this.foundVersion = args[0];
						this.foundUrl     = args[1];
						
						VersionChecker.SplitVersionString (this.foundVersion, out this.foundMajor, out this.foundMinor, out this.foundBuild, out this.foundRevision);
					}
					
					return true;
				}
				
				return false;
			}
		}
		
		public bool								IsCheckSuccessful
		{
			get
			{
				return this.IsReady && (this.readerResult.Length > 0);
			}
		}
		
		
		public bool								FoundNewerVersion
		{
			get
			{
				return this.IsCheckSuccessful &&
					((this.currentMajor < this.foundMajor) ||
					((this.currentMajor == this.foundMajor) && (this.currentMinor < this.foundMinor)) ||
					((this.currentMajor == this.foundMajor) && (this.currentMinor == this.foundMinor) && (this.currentBuild < this.foundBuild)) ||
					((this.currentMajor == this.foundMajor) && (this.currentMinor == this.foundMinor) && (this.currentBuild == this.foundBuild) && (this.currentRevision < this.foundRevision)));
			}
		}
		
		
		public string							CurrentVersion
		{
			get
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", this.currentMajor, this.currentMinor, this.currentBuild, this.currentRevision);
			}
		}
		
		public string							NewerVersion
		{
			get
			{
				return this.foundVersion;
			}
		}
		
		public string							NewerVersionUrl
		{
			get
			{
				return this.foundUrl;
			}
		}
		
		
		public void StartCheck(string url)
		{
			this.reader       = new ReadStringFromUrl (VersionChecker.Read);
			this.readerAsync = this.reader.BeginInvoke (url, null, null);
		}
		
		
		private static string Read(string url)
		{
			string result = "";
			
			try
			{
				System.Diagnostics.Debug.WriteLine ("Checking for updates at URL " + url);

				System.Net.WebRequest  request  = System.Net.HttpWebRequest.Create (new System.Uri (url));
				System.Net.WebResponse response = request.GetResponse ();
				
				System.IO.Stream       raw      = response.GetResponseStream ();
				System.IO.StreamReader reader   = new System.IO.StreamReader (raw);
				
				result = reader.ReadToEnd ();
				
				reader.Close ();
				response.Close ();

				System.Diagnostics.Debug.WriteLine ("Update result : " + result);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("No updates found : " + ex.Message);

				//	Mange toutes les exceptions. On retourne simple une chaîne
				//	vide si on n'a pas réussi à se connecter à l'URL spécifiée.
			}
			
			return result;
		}
		
		private static void SplitVersionString(string v, out int major, out int minor, out int build, out int revision)
		{
			string[] args = v.Split ('.');
			
			major    = 0;
			minor    = 0;
			build    = 0;
			revision = 0;

			try
			{
				if (args.Length > 0)
				{
					major = System.Int32.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);

					if (args.Length > 1)
					{
						minor = System.Int32.Parse(args[1], System.Globalization.CultureInfo.InvariantCulture);

						if (args.Length > 2)
						{
							build = System.Int32.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture);

							if (args.Length > 3)
							{
								revision = System.Int32.Parse(args[3], System.Globalization.CultureInfo.InvariantCulture);
							}
						}
					}
				}
			}
			catch
			{
			}
		}
		
		
		private delegate string ReadStringFromUrl(string url);
		
		private ReadStringFromUrl				reader;
		private System.IAsyncResult				readerAsync;
		private string							readerResult;
		
		private int								currentMajor;
		private int								currentMinor;
		private int								currentBuild;
		private int								currentRevision;
		
		private int								foundMajor;
		private int								foundMinor;
		private int								foundBuild;
		private int								foundRevision;
		
		private string							foundVersion;
		private string							foundUrl;
	}
}
