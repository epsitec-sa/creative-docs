﻿//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			var credentials = Convert.ToBase64String (Encoding.ASCII.GetBytes ("TU_26036_001:L8qUmdpU"));
			this.Headers.Add (HttpRequestHeader.Authorization, $"Basic {credentials}");
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
			if (this.aValidFileIsAvailable)
			{
				return MatchWebClient.GetLocalMatchSortDataPath ();
			}

			this.ProductUri = this.ServiceUri ();
			
			if (this.MustUpdateOrCreate ())
			{
				var fileName = this.DownloadFile (this.ProductUri);
				/*
				if (fileName == null)
				{
					this.IsANewRelease = false;
				}
				else
				{
					var release = this.GetMatchSortFileReleaseDate ();
					MatchWebClient.WriteLocalMetaData (release);
					this.IsANewRelease = true;
				}
				*/
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
			return true;
			/*
			var swissPostMeta = MatchWebClient.GetLocalMetaDataPath ();
			if (System.IO.File.Exists (swissPostMeta))
			{
				try
				{
					var currentRelease = MatchWebClient.ReadLocalMetaData ();
					var lastRelease    = this.GetMatchSortFileReleaseDate ();
					int result = System.DateTime.Compare (currentRelease, lastRelease);
					if (result < 0)
					{
						System.Diagnostics.Trace.WriteLine ("Outdated local MAT[CH] file detected");
						return true;
					}
					else
					{
						System.Diagnostics.Trace.WriteLine ("No update required");
						return false;
					}
				}
				catch
				{
					System.Diagnostics.Trace.WriteLine ("Cannot update - reverting back to local MAT[CH] file");
					return false;
				}
			}
			else
			{
				System.Diagnostics.Trace.WriteLine ("No local MAT[CH] file found, download required");
				return true;
			}*/
		}

		private string ServiceUri()
		{
			return "https://webservices.post.ch:17017/IN_ZOPAxFILES/v1/groups/1062/versions/latest/file/gateway";
		}

		private string DownloadFile(string uri)
		{
			var filename = MatchWebClient.GetLocalMatchSortDataPath ();
			System.Diagnostics.Trace.WriteLine ("Downloading MAT[CH]Sort file...");

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
				catch (System.Exception ex)
				{
					System.Diagnostics.Trace.WriteLine (ex.Message);
					
					if (System.IO.File.Exists (filename))
					{
						return filename;
					}
					
					return null;
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
