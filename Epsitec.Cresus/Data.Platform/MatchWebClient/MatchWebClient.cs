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
			var credentials = Convert.ToBase64String (Encoding.ASCII.GetBytes ("TU_26036_0001:L8qUmdpU"));
			this.Headers.Add (HttpRequestHeader.Authorization, "Basic " + credentials);
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

		public string ProductUri
		{
			get;
			internal set;
		}

		public string GetMatchSortFile()
		{
			var filename = MatchWebClient.GetLocalMatchSortDataPath ();
			
			if (this.aValidFileIsAvailable)
			{
				return filename;
			}

			if ((System.IO.File.Exists (filename) == false) ||
				(System.IO.File.GetLastWriteTime (filename).Date.Date != System.DateTime.Now.Date))
			{
				this.ProductUri = this.ServiceUri ();
				this.DownloadMatchFile (this.ProductUri, filename);
				this.IsANewRelease = this.VerifyNewRelease (filename);
			}
			this.aValidFileIsAvailable = true;
			
			return filename;
		}

		private bool VerifyNewRelease(string filename)
		{
			//	First line contains something like "00;20161205;34148"
			//	The 2nd argument encodes the date of the file...

			var firstLine = System.IO.File.ReadLines (filename).First ();
			var dateArg = firstLine.Split (';')[1];
			
			var dateYear  = int.Parse (dateArg.Substring (0, 4));
			var dateMonth = int.Parse (dateArg.Substring (4, 2));
			var dateDay   = int.Parse (dateArg.Substring (6, 2));

			var releaseDate = new System.DateTime (dateYear, dateMonth, dateDay);
			var currentDate = MatchWebClient.ReadLocalMetaData ();

			MatchWebClient.WriteLocalMetaData (releaseDate);

			return currentDate != releaseDate;
		}

		private string ServiceUri()
		{
			return "https://webservices.post.ch:17017/IN_ZOPAxFILES/v1/groups/1062/versions/latest/file/gateway";
		}

		private void DownloadMatchFile(string uri, string filename)
		{
			System.Diagnostics.Trace.WriteLine (string.Format ("Downloading MAT[CH]sort file from {0}", uri));
			
			using (var stream = this.OpenRead (uri))
			{
				try
				{
					var zipFile = new Epsitec.Common.IO.ZipFile ();
					zipFile.LoadFile (stream);
					var zipEntry = zipFile.Entries.First ();
					System.Diagnostics.Trace.WriteLine (string.Format ("Writing file {0}", filename));
					using (var sw = new StreamWriter (filename)) // output as UTF-8
					{
						sw.Write (System.Text.Encoding.Default.GetString (zipEntry.Data));
					}
					System.Diagnostics.Trace.WriteLine ("Done");
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Trace.WriteLine (ex.Message);
				}
			}
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

		private static System.DateTime? ReadLocalMetaData()
		{
			var swissPostStreetMeta = MatchWebClient.GetLocalMetaDataPath ();

			if (System.IO.File.Exists (swissPostStreetMeta))
			{
				string date = System.IO.File.ReadAllText (swissPostStreetMeta);
				return System.Convert.ToDateTime (date);
			}

			return null;
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
