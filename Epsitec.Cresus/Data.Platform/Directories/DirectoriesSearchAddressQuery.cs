using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesSearchAddressQuery
	{
		public DirectoriesSearchAddressQuery(DirectoriesAuthentication Authentication,DirectoriesPaging Paging )
		{
			this.QueryParameters = new List<XElement> ();
			this.Authentication = Authentication;
			this.Paging = Paging;
		}

		private DirectoriesAuthentication Authentication;
		private DirectoriesPaging Paging;
		private List<XElement> QueryParameters;

		public void AddFirstNameParameter(string Value,bool UsePhonetic,string DirectoriesPrecisionCode)
		{
			XElement FirstNameElement = new XElement ("FirstName");
			FirstNameElement.SetAttributeValue ("Value", Value);
			FirstNameElement.SetAttributeValue ("Phonetic", UsePhonetic == true ? "1" : "0");
			FirstNameElement.SetAttributeValue ("PrecisionCode", DirectoriesPrecisionCode);

			this.QueryParameters.Add (FirstNameElement);
		}

		public void AddLastNameParameter(string Value, bool UsePhonetic, string DirectoriesPrecisionCode)
		{
			XElement LastNameElement = new XElement ("LastName");
			LastNameElement.SetAttributeValue ("Value", Value);
			LastNameElement.SetAttributeValue ("Phonetic", UsePhonetic == true ? "1" : "0");
			LastNameElement.SetAttributeValue ("PrecisionCode", DirectoriesPrecisionCode);

			this.QueryParameters.Add (LastNameElement);
		}

        public void AddLocationParameter(string Value, bool UsePhonetic)
        {
            XElement LastNameElement = new XElement("Location");
            LastNameElement.SetAttributeValue("Value", Value);
            LastNameElement.SetAttributeValue("Phonetic", UsePhonetic == true ? "1" : "0");

            this.QueryParameters.Add(LastNameElement);
        }

		public void AddPhoneParameter(string Value)
		{
			XElement Phone=new XElement ("Phone")
			{
				Value=Value
			};

			this.QueryParameters.Add (Phone);
		}

		public XElement ForgeRequest()
		{
			XElement AddressParam = new XElement ("AddressParam");
			XElement TypeCode=new XElement ("TypeCode")
			{
				Value="ALL"
			};


			//Add Authentication XElement
			AddressParam.Add (this.Authentication.GetAuthenticationElement ());
			//Add Paging XElement
			AddressParam.Add (this.Paging.GetPagingElement ());

			//Add Query Parameters
			AddressParam.Add (this.QueryParameters);

			AddressParam.Add (TypeCode);
			
			
			return AddressParam;
		}
	}
}
