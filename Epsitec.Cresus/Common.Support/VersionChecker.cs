using System;

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
			VersionChecker.SplitVersionString (version, out this.current_major, out this.current_minor, out this.current_build, out this.current_revision);
		}		
		
		public VersionChecker(int major, int minor, int build, int revision)
		{
			this.current_major    = major;
			this.current_minor    = minor;
			this.current_build    = build;
			this.current_revision = revision;
		}
		
		
		public bool								IsReady
		{
			get
			{
				if (this.reader_result != null)
				{
					return true;
				}
				if ((this.reader_async != null) &&
					(this.reader_async.IsCompleted))
				{
					this.reader_result = this.reader.EndInvoke (this.reader_async);
					this.reader_async  = null;
					
					if ((this.reader_result.Length > 0) &&
						(this.reader_result.IndexOf ('|') > 0))
					{
						string[] args = this.reader_result.Split ('|');
						
						this.found_version = args[0];
						this.found_url     = args[1];
						
						VersionChecker.SplitVersionString (this.found_version, out this.found_major, out this.found_minor, out this.found_build, out this.found_revision);
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
				return this.IsReady && (this.reader_result.Length > 0);
			}
		}
		
		
		public bool								FoundNewerVersion
		{
			get
			{
				return this.IsCheckSuccessful &&
					((this.current_major < this.found_major) ||
					((this.current_major == this.found_major) && (this.current_minor < this.found_minor)) ||
					((this.current_major == this.found_major) && (this.current_minor == this.found_minor) && (this.current_build < this.found_build)) ||
					((this.current_major == this.found_major) && (this.current_minor == this.found_minor) && (this.current_build == this.found_build) && (this.current_revision < this.found_revision)));
			}
		}
		
		
		public string							CurrentVersion
		{
			get
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", this.current_major, this.current_minor, this.current_build, this.current_revision);
			}
		}
		
		public string							NewerVersion
		{
			get
			{
				return this.found_version;
			}
		}
		
		public string							NewerVersionUrl
		{
			get
			{
				return this.found_url;
			}
		}
		
		
		public void StartCheck(string url)
		{
			this.reader       = new ReadStringFromUrl (VersionChecker.Read);
			this.reader_async = reader.BeginInvoke (url, null, null);
		}
		
		
		private static string Read(string url)
		{
			string result = "";
			
			try
			{
				System.Net.WebRequest  request  = System.Net.HttpWebRequest.Create (new System.Uri (url));
				System.Net.WebResponse response = request.GetResponse ();
				
				System.IO.Stream       raw      = response.GetResponseStream ();
				System.IO.StreamReader reader   = new System.IO.StreamReader (raw);
				
				result = reader.ReadToEnd ();
				
				reader.Close ();
				response.Close ();
			}
			catch
			{
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
			
			if (args.Length > 0)
			{
				major = System.Int32.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
				
				if (args.Length > 1)
				{
					minor = System.Int32.Parse (args[1], System.Globalization.CultureInfo.InvariantCulture);
					
					if (args.Length > 2)
					{
						build = System.Int32.Parse (args[2], System.Globalization.CultureInfo.InvariantCulture);
						
						if (args.Length > 3)
						{
							revision = System.Int32.Parse (args[3], System.Globalization.CultureInfo.InvariantCulture);
						}
					}
				}
			}
		}
		
		
		private delegate string ReadStringFromUrl(string url);
		
		private ReadStringFromUrl				reader;
		private System.IAsyncResult				reader_async;
		private string							reader_result;
		
		private int								current_major;
		private int								current_minor;
		private int								current_build;
		private int								current_revision;
		
		private int								found_major;
		private int								found_minor;
		private int								found_build;
		private int								found_revision;
		
		private string							found_version;
		private string							found_url;
	}
}
