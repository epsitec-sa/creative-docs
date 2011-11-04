//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Nancy;

using Nancy.Bootstrapper;

using Nancy.IO;

using Nancy.Cookies;

using Nancy.Extensions;

using System;

using System.Collections.Generic;

using System.Globalization;

using System.Linq;

using System.Net;


namespace Epsitec.Cresus.Core.Server.NancyComponents
{
	

	internal sealed class NancyServer : IDisposable
	{


		// This class has been largely inspired by the source code found here :
		// https://github.com/NancyFx/Nancy/blob/09a5c3f8f79d5986a04973b0371e52f4f596a600/src/Nancy.Hosting.Self/NancyHost.cs


		public NancyServer(Uri uri, int nbThreads)
		{
			this.baseUri = uri;
			this.nbThreads = nbThreads;
			this.httpServer = new HttpServer.HttpServer (uri);

			NancyBootstrapperLocator.Bootstrapper.Initialise ();
			this.engine = NancyBootstrapperLocator.Bootstrapper.GetEngine ();
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


		private Request ConvertHttpRequestToNancyRequest(HttpListenerRequest httpRequest)
		{
			var expectedRequestLength = NancyServer.GetExpectedRequestLength (httpRequest.Headers.ToDictionary ());

			var relativeUrl = NancyServer.GetUrlAndPathComponents (this.baseUri).MakeRelativeUri (GetUrlAndPathComponents (httpRequest.Url));

			var nancyUrl = new Url
			{
				Scheme = httpRequest.Url.Scheme,
				HostName = httpRequest.Url.Host,
				Port = httpRequest.Url.IsDefaultPort ? null : (int?) httpRequest.Url.Port,
				BasePath = this.baseUri.AbsolutePath.TrimEnd ('/'),
				Path = string.Concat ("/", relativeUrl),
				Query = httpRequest.Url.Query,
				Fragment = httpRequest.Url.Fragment,
			};

			return new Request
			(
				httpRequest.HttpMethod,
				nancyUrl,
				RequestStream.FromStream (httpRequest.InputStream, expectedRequestLength, true),
				httpRequest.Headers.ToDictionary ()
			);
		}


		private static Uri GetUrlAndPathComponents(Uri uri)
		{
			return new Uri (uri.GetComponents (UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped));
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


		private static void ConvertNancyResponseToHttpResponse(Response nancyResponse, HttpListenerResponse httpResponse)
		{
			foreach (var header in nancyResponse.Headers)
			{
				httpResponse.AddHeader (header.Key, header.Value);
			}

			foreach (var nancyCookie in nancyResponse.Cookies)
			{
				var cookie = NancyServer.ConvertNancyCookieToHttpCookie (nancyCookie);

				httpResponse.Cookies.Add (cookie);
			}

			httpResponse.ContentType = nancyResponse.ContentType;
			httpResponse.StatusCode = (int) nancyResponse.StatusCode;

			using (var output = httpResponse.OutputStream)
			{
				nancyResponse.Contents (output);
			}
		}


		private static Cookie ConvertNancyCookieToHttpCookie(INancyCookie nancyCookie)
		{
			var cookie = new Cookie (nancyCookie.EncodedName, nancyCookie.EncodedValue, nancyCookie.Path, nancyCookie.Domain);

			if (nancyCookie.Expires.HasValue)
			{
				cookie.Expires = nancyCookie.Expires.Value;
			}

			return cookie;
		}


		private readonly HttpServer.HttpServer httpServer;


		private readonly INancyEngine engine;


		private readonly int nbThreads;


		private readonly Uri baseUri;


	}


}