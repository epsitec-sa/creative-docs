using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Epsitec.Data.Platform.Bings
{
	public class BingsWebService
	{
		/// <summary>
		/// Initiate a new Bings REST Web Service
		/// </summary>
		/// <param name="Key">Your API key (see https://www.bingmapsportal.com/Account )</param>
		public BingsWebService(string Key)
		{
			this.Key = Key;
		}

		private string Key;



		public Tuple<double,double> GetCoordinatesBySwissZip(string Zip)
		{
			UriTemplate Template = new UriTemplate ("Locations?countryRegion={cnty}&postalCode={zip}&maxResults={nb}&key={key}");
			Uri Prefix = new Uri ("http://dev.virtualearth.net/REST/v1/");
			NameValueCollection Parameters = new NameValueCollection ();
			Parameters.Add ("cnty", "CH");
			Parameters.Add ("zip", Zip);
			Parameters.Add ("nb", "1");
			Parameters.Add ("key", this.Key);

			Uri ForgedUri = Template.BindByName (Prefix, Parameters);

			HttpWebRequest Request = WebRequest.Create (ForgedUri) as HttpWebRequest;
			Request.Method = WebRequestMethods.Http.Get;

			try
			{
				using (HttpWebResponse Response = Request.GetResponse () as HttpWebResponse)
				{
					XmlReader JsonReader = JsonReaderWriterFactory.CreateJsonReader (Response.GetResponseStream (), new XmlDictionaryReaderQuotas ());
					XElement Root = XElement.Load (JsonReader);

					string MapX = Root.XPathSelectElement ("//coordinates/item[1]").Value;
					string MapY = Root.XPathSelectElement ("//coordinates/item[2]").Value;


					return Tuple.Create(Convert.ToDouble (MapX), Convert.ToDouble (MapY));
				}


			}
			catch (WebException we)
			{
				throw new Exception (we.Message);
			}
		}
	}
}
