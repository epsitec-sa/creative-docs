using Epsitec.Common.Support;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.IO;

using System.Net;

using System.Threading;
using System.Collections;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public class UnitTestHttpServer
	{


		[TestMethod]
		public void SimpleTest()
		{
			var uri = this.GetLocalUriWithRandomPort ();

			Action<HttpListenerRequest, HttpListenerResponse> action = (request, response) =>
			{
				var text = this.ReadHttpRequest (request);

				this.WriteHttpResponse (response, text);
			};

			using (var httpServer = new HttpServer (uri, action))
			{
				var text = "Hello, you beautiful http server !";
				var response = this.MakeHttpRequest (uri, text);

				Assert.AreEqual (text, response);
			}
		}


		[TestMethod]
		public void ConcurrencyTest()
		{
			var uri = this.GetLocalUriWithRandomPort ();

			Action<HttpListenerRequest, HttpListenerResponse> action = (request, response) =>
			{
				var text = this.ReadHttpRequest (request);

				Thread.Sleep (100);

				this.WriteHttpResponse (response, text);
			};

			using (var httpServer = new HttpServer (uri, action))
			{
				var startTime = DateTime.Now;

				var threads = new List<Thread> ();
				var times = new List<List<DateTime>> ();

				for (int i = 0; i < 10; i++)
				{
					var iCaptured = i;

					var thread = new Thread (() =>
					{
						while (DateTime.Now - startTime < TimeSpan.FromSeconds (1))
						{
							this.MakeHttpRequest (uri, "Hi !");

							times[iCaptured].Add (DateTime.Now);
						}
					});

					threads.Add (thread);
					times.Add (new List<DateTime> ());
				}

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}

				foreach (var list in times)
				{
					Assert.IsTrue (list.Count >= 4);
				}
			}
		}


		[TestMethod]
		public void DisposeWhileProcessing()
		{
			// Why does this test fail? It should not, because the server HttpServer waits to close
			// that the request is handled. But still, the client cannot read it. Strange...

			var uri = this.GetLocalUriWithRandomPort ();

			Action<HttpListenerRequest, HttpListenerResponse> action = (req, res) =>
			{
				var t = this.ReadHttpRequest (req);

				Thread.Sleep (1000);
				
				this.WriteHttpResponse (res, t);
			};

			var thread = new Thread (() =>
			{
				using (var httpServer = new HttpServer (uri, action))
				{
					Thread.Sleep (500);
				}
			});

			thread.Start ();

			var text = "Hello, you closing http server !";
			var response = this.MakeHttpRequest (uri, text);

			Assert.AreEqual (text, response);

			thread.Join ();
		}


		private string MakeHttpRequest(Uri uri, string text)
		{
			var request = WebRequest.Create (uri);

			request.Method = "POST";
			request.ContentType = "text/html";

			this.WriteHttpRequest (request, text);

			var response = request.GetResponse ();

			return this.ReadHttpResponse (response);
		}


		private void WriteHttpRequest(WebRequest request, string sentText)
		{
			using (var outputStream = request.GetRequestStream ())
			using (var streamWriter = new StreamWriter (outputStream))
			{
				streamWriter.Write (sentText);
			}
		}


		private string ReadHttpResponse(WebResponse response)
		{
			using (var inputStream = response.GetResponseStream ())
			using (var streamReader = new StreamReader (inputStream))
			{
				return streamReader.ReadToEnd ();
			}
		}


		private string ReadHttpRequest(HttpListenerRequest request)
		{	
			using (var inputStream = request.InputStream)
			using (var streamReader = new StreamReader (inputStream))
			{
				return streamReader.ReadToEnd ();
			}
		}


		private void WriteHttpResponse(HttpListenerResponse response, string text)
		{
			using (var outputStream = response.OutputStream)
			using (var streamWriter = new StreamWriter (outputStream))
			{
				streamWriter.Write (text);
			}
		}


		private Uri GetLocalUriWithRandomPort()
		{
			var port = dice.Next (0, 65535);

			return new Uri ("http://localhost:" + port + "/");
		}


		private readonly Random dice = new Random ();


	}


}
