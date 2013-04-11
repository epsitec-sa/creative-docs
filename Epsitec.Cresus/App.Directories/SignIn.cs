using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;

namespace App.Directories
{
	public partial class SignIn : Form
	{

		private NativeApplicationClient  Provider = new NativeApplicationClient (GoogleAuthenticationServer.Description)
		{
			ClientIdentifier = "393028213266.apps.googleusercontent.com",
			ClientSecret = "k8YeDLjupHQPDu2uQZnw0YgH"

		};

		private IAuthorizationState State = null;
		public  string Auth = null;

		public SignIn()
		{
			InitializeComponent ();
		}

		private void SignIn_Load(object sender, EventArgs e)
		{
			this.State=new AuthorizationState ()
			{
				Callback=new Uri (NativeApplicationClient.OutOfBandCallbackUrl)
			};
			this.State.Scope.Add ("https://www.googleapis.com/auth/userinfo.email");


			Uri authUri = Provider.RequestUserAuthorization (this.State);

			// Request authorization from the user (by opening a browser window):
			//Process.Start (authUri.ToString ());
			this.Browser.Url=authUri;
		}


		private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var authCode = this.Browser.DocumentTitle.Split(' ');


			switch (authCode[0])
			{
				case "Success":
					this.Auth = authCode[1].Replace("code=","");

					//GET ACCESS TOKEN IN EXCHANGE OF AUTH
					UriTemplate Template = new UriTemplate ("token?code={c}&client_id={cid}&client_secret={cs}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&grant_type=authorization_code");
					Uri Prefix = new Uri ("https://accounts.google.com/o/oauth2/");
					NameValueCollection Parameters = new NameValueCollection ();
					Parameters.Add ("c", this.Auth);
					Parameters.Add ("cid", this.Provider.ClientIdentifier);
					Parameters.Add ("cs", this.Provider.ClientSecret);
					Uri ForgedUri = Template.BindByName (Prefix, Parameters);

					HttpWebRequest Request = WebRequest.Create (ForgedUri) as HttpWebRequest;
					Request.Method = WebRequestMethods.Http.Post;
					Request.ContentType = "application/x-www-form-urlencoded";


					try
					{
						using (HttpWebResponse Response = Request.GetResponse () as HttpWebResponse)
						{
							XmlReader JsonReader = JsonReaderWriterFactory.CreateJsonReader (Response.GetResponseStream (), new XmlDictionaryReaderQuotas ());
							XElement Root = XElement.Load (JsonReader);
						}


					}
					catch (WebException we)
					{

					}
					this.Close ();
					break;
				case "Denied":
					this.Close ();
					break;
			}

		}
	}
}
