using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;
using Nancy.Extensions;
using Nancy.Helpers;
using Nancy.IO;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Globalization;

using System.Linq;

using System.Net;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{


	internal sealed class NancyServer : IDisposable
	{


		// This class has been largely inspired by the source code of the official Nancy self host.


		public NancyServer(CoreServer coreServer, Uri uri)
		{
			this.baseUri = uri;
			this.httpServer = new HttpServer (uri, this.ProcessRequest);

			var bootStrapper = new CoreServerBootstrapper (coreServer);

			bootStrapper.Initialise ();
			this.engine = bootStrapper.GetEngine ();
		}


		public void Dispose()
		{
			this.httpServer.Dispose ();
		}


		private void ProcessRequest(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
		{
			try
			{
				Tools.LogMessage ("Received http request: " + httpRequest.Url);

				var stopwatch = Stopwatch.StartNew ();

				var nancyRequest = this.ConvertHttpRequestToNancyRequest (httpRequest);

				using (var nancyContext = this.engine.HandleRequest (nancyRequest))
				{
					var nancyResponse = nancyContext.Response;

					NancyServer.ConvertNancyResponseToHttpResponse (nancyResponse, httpResponse);
				}

				stopwatch.Stop ();

				var message = "Answered http request: " + httpRequest.Url
					+ " in " + stopwatch.ElapsedMilliseconds + " ms";

				Tools.LogMessage (message);
			}
			catch (Exception e)
			{
				var message = "Uncaught exception while processing http request :\n"
					+ e.GetFullText ();

				Tools.LogError (message);
			}
		}


		private static Uri GetUrlAndPathComponents(Uri uri)
		{
			return new Uri (uri.GetComponents (UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped));
		}


		private Request ConvertHttpRequestToNancyRequest(HttpListenerRequest request)
		{
			if (this.baseUri == null)
			{
				throw new InvalidOperationException ("Unable to locate base URI for request: " + request.Url);
			}

			var expectedRequestLength = NancyServer.GetExpectedRequestLength (request.Headers.ToDictionary ());

			var baseUrl = NancyServer.GetUrlAndPathComponents (this.baseUri);
			var requestUrl = NancyServer.GetUrlAndPathComponents (request.Url);
			var relativeUrl = baseUrl.MakeRelativeUri (requestUrl);

			var nancyUrl = new Url
			{
				Scheme = request.Url.Scheme,
				HostName = request.Url.Host,
				Port = request.Url.IsDefaultPort ? null : (int?) request.Url.Port,
				BasePath = this.baseUri.AbsolutePath.TrimEnd ('/'),
				Path = string.Concat ("/", HttpUtility.UrlDecode (relativeUrl.ToString ())),
				Query = request.Url.Query,
				Fragment = request.Url.Fragment,
			};

			return new Request
			(
				request.HttpMethod,
				nancyUrl,
				RequestStream.FromStream (request.InputStream, expectedRequestLength, true),
				request.Headers.ToDictionary (),
				(request.RemoteEndPoint != null) ? request.RemoteEndPoint.Address.ToString () : null
			);
		}

		
		private static long GetExpectedRequestLength(IDictionary<string, IEnumerable<string>> incomingHeaders)
		{
			if (incomingHeaders == null)
			{
				return 0;
			}

			if (!incomingHeaders.ContainsKey ("Content-Length"))
			{
				return 0;
			}

			var headerValue = incomingHeaders["Content-Length"].SingleOrDefault ();

			if (headerValue == null)
			{
				return 0;
			}

			long contentLength;
			if (!long.TryParse (headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength))
			{
				return 0;
			}

			return contentLength;
		}


		private static void ConvertNancyResponseToHttpResponse(Response nancyResponse, HttpListenerResponse response)
		{
			foreach (var header in nancyResponse.Headers)
			{
				response.AddHeader (header.Key, header.Value);
			}

			foreach (var nancyCookie in nancyResponse.Cookies)
			{
				response.Headers.Add (HttpResponseHeader.SetCookie, nancyCookie.ToString ());
			}

			response.ContentType = nancyResponse.ContentType;
			response.StatusCode = (int) nancyResponse.StatusCode;

			using (var output = response.OutputStream)
			{
				nancyResponse.Contents.Invoke (output);
			}
		}


		private readonly HttpServer httpServer;


		private readonly INancyEngine engine;


		private readonly Uri baseUri;


	}


}