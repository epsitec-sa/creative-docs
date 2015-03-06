//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Epsitec.Data.Platform
{
	public class MatchWebClient : WebClient
	{
		public MatchWebClient()
		{
			this.container = new CookieContainer ();
		}

		public CookieContainer CookieContainer
		{
			get
			{
				return container;
			}
			set
			{
				container= value;
			}
		}

		public bool IsANewRelease
		{
			get;
			internal set;
		}


		public bool IsLogged
		{
			get;
			internal set;
		}

		public string ProductUri
		{
			get;
			internal set;
		}

		public string GetMatchSortFile()
		{
			if (this.aValidFileIsAvailable)
			{
				return MatchWebClient.GetLocalMatchSortDataPath ();
			}

			if (!this.IsLogged)
			{
				this.DoMatchLoginRequest ();
			}

			this.ProductUri = this.FindProductUri ();
			
			if (this.MustUpdateOrCreate ())
			{
				this.DownloadFile (this.ProductUri);
				var release = this.GetMatchSortFileReleaseDate ();
				MatchWebClient.WriteLocalMetaData (release);
				this.IsANewRelease = true;
				this.aValidFileIsAvailable = true;
				return MatchWebClient.GetLocalMatchSortDataPath ();
			}
			else
			{
				this.IsANewRelease = false;
				this.aValidFileIsAvailable = true;
				return MatchWebClient.GetLocalMatchSortDataPath ();
			}
		}

		public bool MustUpdateOrCreate()
		{
			var swissPostMeta = MatchWebClient.GetLocalMetaDataPath ();
			if (System.IO.File.Exists (swissPostMeta))
			{
				var currentRelease = MatchWebClient.ReadLocalMetaData ();
				var lastRelease     = this.GetMatchSortFileReleaseDate ();
				int result = System.DateTime.Compare (currentRelease, lastRelease);
				if (result < 0)
				{
					System.Diagnostics.Trace.WriteLine ("Outdated local Mat[CH] file detected");
					return true;
				}
				else
				{
					System.Diagnostics.Trace.WriteLine ("No update required");
					return false;
				}
			}
			else
			{
				System.Diagnostics.Trace.WriteLine ("No local Mat[CH] file found, download required");
				return true;
			}
		}

		private DateTime GetMatchSortFileReleaseDate()
		{
			if (!this.IsLogged)
			{
				this.DoMatchLoginRequest ();
			}
			return FindProductReleaseDate ();
		}

		private string FindProductUri()
		{
			var page = this.DownloadString ("https://match.post.ch/downloadCenter?product=4");
			var startIndex = page.IndexOf ("/download");
			var length = page.IndexOf ("Bestand") - 2 - startIndex;
			return "https://match.post.ch" + page.Substring (startIndex, length);
		}

		private DateTime FindProductReleaseDate()
		{
			var currentYear = DateTime.UtcNow.Year.ToString ();
			var page = this.DownloadString ("https://match.post.ch/downloadCenter?product=4");
			var endIndex = page.IndexOf (currentYear + "</b>");
			var startIndex = endIndex - 6;
			var length = endIndex - startIndex;
			var date = page.Substring (startIndex, length) + currentYear;
			return Convert.ToDateTime (date);
		}

		private string DoMatchLoginRequest()
		{
			System.Diagnostics.Trace.WriteLine ("Login to Mat[CH] Downloadcenter...");
			var hiddenFieldValue = this.FindHiddenFormField ();
			var values = this.BuildLoginFormPostData (hiddenFieldValue);
			var response = this.UploadValues ("https://match.post.ch/downloadCenter?login=match", values);
			System.Diagnostics.Trace.WriteLine ("Done");
			this.IsLogged = true;
			return Encoding.Default.GetString (response);
		}

		private string FindHiddenFormField()
		{
			var page = this.DownloadString ("https://match.post.ch/downloadCenter?login=match");
			var startIndex = page.LastIndexOf ("type=\"hidden\" value=\"") + 21;
			var length = page.LastIndexOf ("\" name=\"fp_match\"") - startIndex;
			return page.Substring (startIndex, length);
		}

		private NameValueCollection BuildLoginFormPostData(string hiddenFieldName)
		{
			var userName = "arnaud@epsitec.ch";
			var password = "TADF8%PYC";
			var  loginFormData = new NameValueCollection ();
			loginFormData["benutzer"] = userName;
			loginFormData["passwort"] = password;
			loginFormData["fp_match"] = hiddenFieldName;
			return loginFormData;
		}

		private string DownloadFile(string uri)
		{
			var filename = MatchWebClient.GetLocalMatchSortDataPath ();
			System.Diagnostics.Trace.WriteLine ("Downloading Mat[CH]Sort file...");

			using (var stream = this.OpenRead (uri))
			{
				try
				{
					var zipFile = new Epsitec.Common.IO.ZipFile ();
					zipFile.LoadFile (stream);
					var zipEntry = zipFile.Entries.First ();
					System.Diagnostics.Trace.WriteLine ("Writing file on {0}...", filename);
					using (StreamWriter sw = new StreamWriter (filename))
					{
						sw.Write (System.Text.Encoding.Default.GetString (zipEntry.Data));
					}
					System.Diagnostics.Trace.WriteLine ("Done");
				}
				catch
				{
					throw new System.Exception ("Error during file download");
				}
			}
			return filename;
		}

		private static string GetLocalMetaDataPath()
		{
			string path1 = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			return System.IO.Path.Combine (path1, "Epsitec", "swisspost.meta");
		}

		private static string GetLocalMatchSortDataPath()
		{
			string path1 = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			return System.IO.Path.Combine (path1, "Epsitec", "swisspost.csv");
		}

		private static System.DateTime ReadLocalMetaData()
		{
			var swissPostStreetMeta = MatchWebClient.GetLocalMetaDataPath ();
			string date = System.IO.File.ReadAllText (swissPostStreetMeta);
			return System.Convert.ToDateTime (date);
		}

		private static void WriteLocalMetaData(System.DateTime releaseDate)
		{
			var date = releaseDate.ToShortDateString ();
			var swissPostStreetMeta = MatchWebClient.GetLocalMetaDataPath ();
			System.IO.File.WriteAllText (swissPostStreetMeta, date);
		}

		private CookieContainer container = new CookieContainer ();

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest r = base.GetWebRequest (address);
			var request = r as HttpWebRequest;
			if (request != null)
			{
				request.CookieContainer = container;
			}
			return r;
		}

		protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
		{
			WebResponse response = base.GetWebResponse (request, result);
			ReadCookies (response);
			return response;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = base.GetWebResponse (request);
			ReadCookies (response);
			return response;
		}

		private void ReadCookies(WebResponse r)
		{
			var response = r as HttpWebResponse;
			if (response != null)
			{
				CookieCollection cookies = response.Cookies;
				container.Add (cookies);
			}
		}

		private bool aValidFileIsAvailable = false;
	}
}
