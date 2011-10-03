//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Epsitec.DebugService
{
	/// <summary>
	/// The <c>WebListener</c> class implements a Web server which listens to HTTP POST
	/// requests and processes them.
	/// </summary>
	public static class WebListener
	{
		public static string StoragePath
		{
			get;
			set;
		}

		public static void RunServer(string prefixes, string storagePath)
		{
			System.Console.Title = string.Format ("DebugService {1} running since {0:dd.MM.yyyy} - {0:HH:mm}", System.DateTime.Now, typeof (Program).Assembly.FullName.Split (',')[1].Split ('=')[1]);
			System.Console.Clear ();
			System.Console.ForegroundColor = System.ConsoleColor.Green;
			System.Console.WriteLine ("Listening on {0}", prefixes);
			System.Console.ForegroundColor = System.ConsoleColor.White;

			try
			{
				WebListener.StoragePath = storagePath;
				WebListener.Listen (prefixes);
			}
			catch (System.Exception ex)
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
				System.Console.WriteLine ("Exception: {0}", ex.Message);
				System.Console.WriteLine (ex.StackTrace);
				System.Console.WriteLine ();
				System.Console.ForegroundColor = System.ConsoleColor.White;
			}

			System.Console.WriteLine ("Exited... Press RETURN to quit.");
			System.Console.ReadLine ();
		}


		private static string ProcessRequest(HttpListenerRequest request)
		{
			if (request.HttpMethod != "POST")
			{
				return "Error: not an HTTP POST request";
			}
			if (request.HasEntityBody == false)
			{
				return "Error: no body";
			}

			string urlPath  = request.Url.LocalPath;
			string urlQuery = request.Url.Query.Substring (1);

			byte[] data = new byte[request.ContentLength64];

			WebListener.ReadAllBytes (request.InputStream, data);

			var keyValues = urlQuery.Split ('&').Select (x => x.Split ('=')).ToDictionary (x => x[0], x => x.Length == 2 ? System.Web.HttpUtility.UrlDecode (x[1]) : null);

			string argSession;
			string argFile;

			keyValues.TryGetValue ("session", out argSession);
			keyValues.TryGetValue ("file", out argFile);

			if ((urlPath == "/debugservice/log") &&
				(string.IsNullOrEmpty (argSession) == false) &&
				(string.IsNullOrEmpty (argFile) == false))
			{
				WebListener.ExecuteLog (argSession, argFile, data);
				return "OK";
			}

			return "Error: unknown action " + urlPath;
		}

		private static void ExecuteLog(string argSession, string argFile, byte[] data)
		{
			if (string.IsNullOrEmpty (WebListener.StoragePath))
			{
				return;
			}
			if (System.IO.Directory.Exists (WebListener.StoragePath) == false)
			{
				return;
			}

			var dir = System.IO.Path.Combine (WebListener.StoragePath, argSession);

			if (System.IO.Directory.Exists (dir) == false)
			{
				System.IO.Directory.CreateDirectory (dir);
			}

			var path = System.IO.Path.Combine (dir, argFile);
			System.IO.File.WriteAllBytes (path, data);
		}

		private static void ReadAllBytes(System.IO.Stream stream, byte[] data)
		{
			int offset = 0;
			int more   = data.Length;

			while (more > 0)
			{
				int read = stream.Read (data, offset, more);

				if (read == 0)
				{
					throw new System.IO.EndOfStreamException (string.Format ("Missing {0} bytes", more));
				}

				more   -= read;
				offset += read;
			}

			stream.Close ();
		}


		private static void Listen(string prefixes)
		{
			if (!HttpListener.IsSupported)
			{
				return;
			}

			//	URI prefixes are required, for example "http://+:8081/debugservice/".
			//	The server must have the ACLs properly cofigured for this to work:
			//	-------------------------------------------------------------------------------
			//	CMD> netsh http add urlacl url=http://+:8081/debugservice user=administrator
			//	-------------------------------------------------------------------------------

			if (string.IsNullOrEmpty (prefixes))
			{
				throw new System.ArgumentException ("No prefixes specified", "prefixes");
			}

			using (var listener = new HttpListener ())
			{
				listener.Prefixes.Add (prefixes);
				listener.Start ();

				while (true)
				{
					//	Note: the GetContext method blocks while waiting for a request.

					HttpListenerContext context = listener.GetContext ();

					try
					{
						var listenerRequest = context.Request;
						var now = System.DateTime.Now;

						System.Console.WriteLine ("{0:dd.MM}-{0:HH:mm} {1}", now, WebListener.Truncate (listenerRequest.RawUrl, 64));

						var responseString = WebListener.ProcessRequest (listenerRequest);
						var responseData   = System.Text.Encoding.UTF8.GetBytes (responseString);

						var listenerResponse = context.Response;

						listenerResponse.ContentLength64 = responseData.Length;

						WebListener.WriteToOutputStream (listenerResponse, responseData);
					}
					catch (System.Exception ex)
					{
						System.Console.ForegroundColor = System.ConsoleColor.Red;
						System.Console.WriteLine ("Exception: {0}", ex.Message);
						System.Console.WriteLine (ex.StackTrace);
						System.Console.WriteLine ();
						System.Console.ForegroundColor = System.ConsoleColor.White;
					}
				}
			}
		}

		private static void WriteToOutputStream(HttpListenerResponse listenerResponse, byte[] data)
		{
			var outputStream = listenerResponse.OutputStream;
			outputStream.Write (data, 0, data.Length);
			outputStream.Close ();
		}
		
		private static string Truncate(string text, int length)
		{
			if ((string.IsNullOrEmpty (text)) ||
				(text.Length <= length))
			{
				return text;
			}
			else
			{
				return text.Substring (0, length-2) + "..";
			}
		}
	}
}
