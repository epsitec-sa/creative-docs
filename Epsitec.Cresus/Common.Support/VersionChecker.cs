//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
namespace Epsitec.Common.Support
{
	/// <summary>
	/// Summary description for VersionChecker.
	/// </summary>
	public class VersionChecker
	{
		public static VersionChecker CheckUpdate(string productName, string productVersion)
		{
			var url = "http://www.epsitec.ch/dynamics/check.php?software={0}&version={1}";
			var checker = new VersionChecker (productVersion);

			checker.StartCheck (string.Format (url, productName, productVersion));

			return checker;
		}

		public static VersionChecker CheckUpdate(System.Reflection.Assembly assembly, string urlFormat, string productName)
		{
			var checker = new VersionChecker (assembly);

			checker.StartCheck (string.Format (urlFormat, productName, checker.CurrentVersion));

			return checker;
		}


		private VersionChecker()
		{
			this.context = System.Threading.SynchronizationContext.Current ?? new System.Threading.SynchronizationContext ();
			System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += this.HandleNetworkAvailabilityChanged;
		}

		private VersionChecker(System.Reflection.Assembly assembly)
			: this (assembly.FullName.Split (',')[1].Split ('=')[1])
		{
		}

		private VersionChecker(string version)
			: this ()
		{
			VersionChecker.SplitVersionString (version, out this.currentMajor, out this.currentMinor, out this.currentBuild, out this.currentRevision);
		}

		public bool IsReady
		{
			get
			{
				lock (this.exclusion)
				{
					if (this.readerResult != null)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
		}

		public bool IsCheckSuccessful
		{
			get
			{
				lock (this.exclusion)
				{
					return !string.IsNullOrEmpty (this.readerResult);
				}
			}
		}

		public bool FoundNewerVersion
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

		public bool? IsNetworkAvailable
		{
			get
			{
				lock (this.exclusion)
				{
					return this.isNetworkAvailable;
				}
			}
			private set
			{
				bool change = false;

				lock (this.exclusion)
				{
					if (this.isNetworkAvailable != value)
					{
						this.isNetworkAvailable = value;
						change = true;
					}
				}

				if (change)
				{
					this.OnNetworkAvailabilityChanged ();
				}
			}
		}

		public string CurrentVersion
		{
			get
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}.{2:###}.{3}", this.currentMajor, this.currentMinor, this.currentBuild, this.currentRevision);
			}
		}

		public string NewerVersion
		{
			get
			{
				return this.foundVersion;
			}
		}

		public string NewerVersionUrl
		{
			get
			{
				return this.foundUrl;
			}
		}


		/// <summary>
		/// Formats a version number using a <c>#.#.###</c> pattern. Extra zeroes will
		/// be removed.
		/// </summary>
		/// <param name="format">The format pattern.</param>
		/// <param name="version">The version string.</param>
		/// <returns>The formatted version string.</returns>
		public static string PrettyPrint(string format, string version)
		{
			var buffer = new System.Text.StringBuilder ();
			int pos    = 0;

			foreach (var item in (version + "....").Split ('.'))
			{
				var digits = item;

				while (digits.Length > 1)
				{
					if (digits[0] == '0')
					{
						digits = digits.Substring (1);
					}
					else
					{
						break;
					}
				}

				int len = 0;
				bool inserted = false;

				while (true)
				{
					if ((pos < format.Length) &&
						(format[pos] == '#'))
					{
						//	Only count # on the first iteration; once we have done a substitution, we
						//	stop when we see a new #, since it belongs to the next group of digits.
						
						if (inserted)
						{
							break;
						}

						pos++;
						len++;

						continue;
					}
					
					if (len > 0)
                    {
						//	We have accumulated a series of # and we will have to produce at least
						//	as many digits in the output buffer.
						
						if (digits.Length < len)
                        {
							buffer.Append (new string ('0', len - digits.Length));
                        }
						
						buffer.Append (digits);
						
						len      = 0;
						inserted = true;
                    }

					//	Copy non-formatting characters to the output.

					while ((pos < format.Length) &&
						   (format[pos] != '#'))
					{
						buffer.Append (format[pos++]);
					}

					if (pos == format.Length)
					{
						return buffer.ToString ();
					}
				}
			}

			return buffer.ToString ();
		}


		private void StartCheck(string url)
		{
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (url));

			lock (this.exclusion)
			{
				if (this.isAsyncCheckRunning)
				{
					return;
				}
				this.isAsyncCheckRunning = true;
			}


			System.Threading.WaitCallback callback =
                delegate
				{
					try
					{
						this.ReadVersionInfoFromWebServer (url);
					}
					finally
					{
						lock (this.exclusion)
						{
							this.isAsyncCheckRunning = false;
						}
					}
				};

			System.Threading.ThreadPool.QueueUserWorkItem (callback);
		}

		private void ReadVersionInfoFromWebServer(string url)
		{
			string result = "";

			try
			{
				//  Wait until the network is available : it is useless to try
				//  to download the info from the web if we know for sure that
				//  there is no connectivity !

				while (this.IsNetworkAvailable == false)
				{
					System.Threading.Thread.Sleep (1000);
				}

				System.Diagnostics.Debug.WriteLine ("Checking for updates at URL " + url);

				System.Net.WebRequest  request  = System.Net.HttpWebRequest.Create (new System.Uri (url));
				System.Net.WebResponse response = request.GetResponse ();

				System.IO.Stream       raw      = response.GetResponseStream ();
				System.IO.StreamReader reader   = new System.IO.StreamReader (raw);

				result = reader.ReadToEnd ();

				reader.Close ();
				response.Close ();

				lock (this.exclusion)
				{
					this.readerResult = result;

					if ((result.Length > 0) &&
                        (result.IndexOf ('|') > 0))
					{
						string[] args = result.Split ('|');

						this.foundVersion = args[0];
						this.foundUrl = args[1];

						VersionChecker.SplitVersionString (this.foundVersion, out this.foundMajor, out this.foundMinor, out this.foundBuild, out this.foundRevision);
					}
				}

				this.OnVersionInformationChanged ();
				this.IsNetworkAvailable = true;

				System.Diagnostics.Debug.WriteLine ("Update result : " + result);
			}
			catch (System.Exception)
			{
				if (this.IsNetworkAvailable.HasValue == false)
				{
					this.OnVersionInformationChanged ();
					this.IsNetworkAvailable = false;
					this.ReadVersionInfoFromWebServer (url);
				}
			}
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
						minor = System.Int32.Parse (args[1], System.Globalization.CultureInfo.InvariantCulture);

						if (args.Length > 2)
						{
							build = System.Int32.Parse (args[2].Split (' ')[0], System.Globalization.CultureInfo.InvariantCulture);

							if (args.Length > 3)
							{
								revision = System.Int32.Parse (args[3], System.Globalization.CultureInfo.InvariantCulture);
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		private void HandleNetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
		{
			this.IsNetworkAvailable = e.IsAvailable;
		}

		private void OnVersionInformationChanged()
		{
			var handler = this.VersionInformationChanged;

			if ((handler != null) &&
				(this.context != null))
			{
				this.context.Post (x => handler (this), null);
			}
		}

		private void OnNetworkAvailabilityChanged()
		{
			var handler = this.NetworkAvailabilityChanged;

			if ((handler != null) &&
				(this.context != null))
			{
				this.context.Post (x => handler (this), null);
			}
		}



		public event EventHandler NetworkAvailabilityChanged;
		public event EventHandler VersionInformationChanged;

		readonly object                         exclusion = new object ();
		readonly System.Threading.SynchronizationContext context;
		string							        readerResult;

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

		private bool                            isAsyncCheckRunning;
		private bool?                           isNetworkAvailable;
	}
}
