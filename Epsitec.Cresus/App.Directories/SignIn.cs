using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace App.Directories
{
	public partial class SignIn : Form
	{
		private string Auth = null;
        public string AccessToken = null;
        public string RefreshToken = null;
        public string ClientIdentifier = "482676398973.apps.googleusercontent.com";
        public string ClientSecret = "TFHSDYXJcP2wyOkLVjmZ-I8-";
		public SignIn()
		{
			InitializeComponent ();
		}

		private void SignIn_Load(object sender, EventArgs e)
		{
            UriTemplate Template = new UriTemplate("auth?scope={scope}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&response_type=code&client_id={clientid}");
            Uri Prefix = new Uri("https://accounts.google.com/o/oauth2/");
            NameValueCollection Parameters = new NameValueCollection();
            Parameters.Add("clientid", this.ClientIdentifier);
            Parameters.Add("scope", "https://www.googleapis.com/auth/plus.me");

            Uri AuthUri = Template.BindByName(Prefix, Parameters);

			// Request authorization from the user (by opening a browser window):
			//Process.Start (authUri.ToString ());
			this.Browser.Url=AuthUri;
		}


		private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var authCode = this.Browser.DocumentTitle.Split(' ');


			switch (authCode[0])
			{
				case "Success":
					this.Auth = authCode[1].Replace("code=","");

					//GET ACCESS TOKEN IN EXCHANGE OF AUTH
                    Uri ForgedUri = new Uri("https://accounts.google.com/o/oauth2/token?code=" + this.Auth + "&client_id=" + this.ClientIdentifier + "&client_secret=" + this.ClientSecret + "&redirect_uri=urn:ietf:wg:oauth:2.0:oob&grant_type=authorization_code");
					string Content = "code=" + this.Auth + "&client_id=" + this.ClientIdentifier + "&client_secret=" + this.ClientSecret + "&redirect_uri=urn:ietf:wg:oauth:2.0:oob&grant_type=authorization_code";

					HttpWebRequest Request = WebRequest.Create (ForgedUri) as HttpWebRequest;
					Request.Method = WebRequestMethods.Http.Post;
					Request.ContentType = "application/x-www-form-urlencoded";
                    
                    
                    ASCIIEncoding encoding = new ASCIIEncoding ();
                    byte[] byte1 = encoding.GetBytes(Content);
                    // Set the content length of the string being posted.
                    Request.ContentLength = byte1.Length;
                    Stream newStream = Request.GetRequestStream ();
                    newStream.Write (byte1, 0, byte1.Length);
                    // Close the Stream object.
                    newStream.Close ();

					try
					{
						using (HttpWebResponse Response = Request.GetResponse () as HttpWebResponse)
						{
							XmlReader JsonReader = JsonReaderWriterFactory.CreateJsonReader (Response.GetResponseStream (), new XmlDictionaryReaderQuotas ());
							XElement Root = XElement.Load (JsonReader);
                            this.AccessToken = Root.Element("access_token").Value;
                            this.RefreshToken = Root.Element("refresh_token").Value;
                            
                       /*<root type="object">
                          <access_token type="string">ya29.AHES6ZTOCLTxYqy-lfoecfHBuzaPhXD8I9woS_6NgxT_3jAt</access_token>
                          <token_type type="string">Bearer</token_type>
                          <expires_in type="number">3600</expires_in>
                          <id_token type="string">eyJhbGciOiJSUzI1NiIsImtpZCI6ImQzOGM1M2JkNDJiNTYxMTVhYjJlNjcyNTAzYjI3ZTZhNDU1NDg1ZTgifQ.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwidG9rZW5faGFzaCI6IjdIa2VuZXZUOWhuREREZ0Rvdk1YUGciLCJhdF9oYXNoIjoiN0hrZW5ldlQ5aG5ERERnRG92TVhQZyIsInZlcmlmaWVkX2VtYWlsIjoidHJ1ZSIsImVtYWlsX3ZlcmlmaWVkIjoidHJ1ZSIsImlkIjoiMTA1MDI2NjIwNjY3ODM4MTU1MTYzIiwic3ViIjoiMTA1MDI2NjIwNjY3ODM4MTU1MTYzIiwiYXVkIjoiMzkzMDI4MjEzMjY2LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiY2lkIjoiMzkzMDI4MjEzMjY2LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXpwIjoiMzkzMDI4MjEzMjY2LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiZW1haWwiOiJzYW11ZWwubG91cEBnbWFpbC5jb20iLCJpYXQiOjEzNjU3NDM2NzQsImV4cCI6MTM2NTc0NzU3NH0.ous4R2bLyWfcYKL1A6bRuGRDWMrkdbD0sWhbteqEK4EgYStRriDj7TnBA0SpdejJ3y11e628OGBs5he_QlOizEKeuBSHT96nlTy-xT-vQvQTwMo4L5qnOh5lABtHulhMPWABWfY7mkTYhKhSrOTTMrAySlOqJtiDAd-4NRT57J0</id_token>
                          <refresh_token type="string">1/EyOCjCX3JNysyIGihkuj0AGDhhd0A98evjFGxBdWFxg</refresh_token>
                        </root>*/
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
