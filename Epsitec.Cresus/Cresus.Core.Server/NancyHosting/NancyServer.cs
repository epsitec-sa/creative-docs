using Epsitec.Cresus.Core.Server.CoreServer;

using Nancy;

using Nancy.Bootstrapper;

using Nancy.IO;

using Nancy.Cookies;

using Nancy.Extensions;

using Nancy.Helpers;

using System;

using System.Collections.Generic;

using System.Globalization;

using System.Linq;

using System.Net;



namespace Epsitec.Cresus.Core.Server.NancyHosting
{


	internal sealed class NancyServer : IDisposable
	{


		// This class has been largely inspired by the source code of the official Nancy self host.


		public NancyServer(ServerContext serverContext, Uri uri, int nbThreads)
		{
			this.baseUri = uri;
			this.nbThreads = nbThreads;
			this.httpServer = new HttpServer (uri);

			var bootStrapper = new CoreServerBootstrapper (serverContext);

			bootStrapper.Initialise ();
			this.engine = bootStrapper.GetEngine ();
		}


		public void Start()
		{
			this.httpServer.Start (r => this.ProcessRequest (r), this.nbThreads);
		}


		public void Stop()
		{
			this.httpServer.Stop ();
		}


		public void Dispose()
		{
			this.httpServer.Dispose ();
		}


		private void ProcessRequest(HttpListenerContext requestContext)
		{
			HttpListenerRequest httpRequest = requestContext.Request;

			using (HttpListenerResponse httpResponse = requestContext.Response)
			{
				var nancyRequest = ConvertHttpRequestToNancyRequest (httpRequest);

				using (var nancyContext = this.engine.HandleRequest (nancyRequest))
				{
					var nancyResponse = nancyContext.Response;

					NancyServer.ConvertNancyResponseToHttpResponse (nancyResponse, httpResponse);
				}
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


		private readonly int nbThreads;


		private readonly Uri baseUri;


	}


}