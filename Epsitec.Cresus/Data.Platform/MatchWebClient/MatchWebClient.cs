//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

		public string GetMatchSortFileFromWebsite()
		{
			var response = this.DoMatchLoginRequest ();
			var uri = this.FindProductUri ();
			return this.DownloadFileToTemp (uri);
		}

		private string FindProductUri()
		{
			var page = this.DownloadString ("https://match.post.ch/downloadCenter?product=4");
			var startIndex = page.IndexOf ("/download");
			var length = page.IndexOf ("Bestand") - 2 - startIndex;
			return "https://match.post.ch" + page.Substring (startIndex, length);
		}

		private string DoMatchLoginRequest()
		{
			System.Console.WriteLine ("Login to Mat[CH] Downloadcenter...");
			var hiddenFieldValue = this.FindHiddenFormField ();
			var values = this.BuildLoginFormPostData (hiddenFieldValue);
			var response = this.UploadValues ("https://match.post.ch/downloadCenter?login=match", values);
			System.Console.WriteLine ("Done");
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

		private string DownloadFileToTemp(string uri)
		{
			var filename = Path.GetTempFileName ();
			System.Console.WriteLine ("Downloading Mat[CH]Sort file...");

			using (var stream = this.OpenRead (uri))
			{
				try
				{
					System.Console.WriteLine ("Done");
					var zipFile = new Epsitec.Common.IO.ZipFile ();
					System.Console.WriteLine ("Unzipping file...");
					zipFile.LoadFile (stream);
					System.Console.WriteLine ("Done");
					var zipEntry = zipFile.Entries.First ();
					System.Console.WriteLine ("Writing file on {0}...", filename);
					using (StreamWriter sw = new StreamWriter (filename))
					{
						sw.Write (System.Text.Encoding.Default.GetString (zipEntry.Data));
					}
					System.Console.WriteLine ("Done");
				}
				catch
				{
					throw new System.Exception ("Error during file download");
				}
			}
			return filename;
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
	}
}
