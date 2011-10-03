//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Epsitec.DebugService
{
	public static class WebListener
	{
		public static string StoragePath
		{
			get;
			set;
		}

		public static string ProcessRequest(HttpListenerRequest request)
		{
			if (!request.HasEntityBody)
			{
				return "Error: no body";
			}

			string urlPath = request.Url.LocalPath;
			string urlQuery = request.Url.Query.Substring (1);

			byte[] data = new byte[request.ContentLength64];

			WebListener.Read (request.InputStream, data);

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

		private static void Read(System.IO.Stream stream, byte[] data)
		{
			int offset = 0;
			int more = data.Length;

			while (more > 0)
			{
				int read = stream.Read (data, offset, more);

				if (read == 0)
				{
					throw new System.IO.EndOfStreamException (string.Format ("Missing {0} bytes", more));
				}

				more  -= read;
				offset += read;
			}

			stream.Close ();
		}


		public static void Listen(string prefixes)
		{
			if (!HttpListener.IsSupported)
			{
				return;
			}
			// URI prefixes are required,
			// for example "http://+:8081/debugservice/".
			if (string.IsNullOrEmpty (prefixes))
			{
				throw new System.ArgumentException ("prefixes");
			}

			// Create a listener.
			HttpListener listener = new HttpListener ();
			
			listener.Prefixes.Add (prefixes);
			listener.Start ();
			
			// Note: The GetContext method blocks while waiting for a request. 

			while (true)
			{
				HttpListenerContext context = listener.GetContext ();
				
				try
				{
					HttpListenerRequest request = context.Request;
					var responseString = WebListener.ProcessRequest (request);

					// Obtain a response object.
					HttpListenerResponse response = context.Response;
					// Construct a response.
					byte[] buffer = System.Text.Encoding.UTF8.GetBytes (responseString);
					// Get a response stream and write the response to it.
					response.ContentLength64 = buffer.Length;
					System.IO.Stream output = response.OutputStream;
					output.Write (buffer, 0, buffer.Length);
					// You must close the output stream.
					output.Close ();
				}
				catch (System.Exception ex)
				{
					System.Console.WriteLine ("Exception: {0}", ex.Message);
					System.Console.WriteLine (ex.StackTrace);
					System.Console.WriteLine ();
				}
			}

			listener.Stop ();
		}
	}
}
