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
			this.QueryParameters = new List<DirectoriesQueryParameter> ();
			this.Authentication = Authentication;
			this.Paging = Paging;
		}

		private DirectoriesAuthentication Authentication;
		private DirectoriesPaging Paging;
		private List<DirectoriesQueryParameter> QueryParameters;

		public void AddFirstNameParameter(string Value,bool UsePhonetic,string DirectoriesPrecisionCode)
		{
            DirectoriesQueryParameter Param = new DirectoriesQueryParameter(2, "FirstName", Value, UsePhonetic, DirectoriesPrecisionCode);
			this.QueryParameters.Add (Param);
		}

		public void AddLastNameParameter(string Value, bool UsePhonetic, string DirectoriesPrecisionCode)
		{
            DirectoriesQueryParameter Param = new DirectoriesQueryParameter(1, "LastName", Value, UsePhonetic, DirectoriesPrecisionCode);
            this.QueryParameters.Add(Param);
		}

        public void AddLocationParameter(string Value, bool UsePhonetic)
        {
            DirectoriesQueryParameter Param = new DirectoriesQueryParameter(6, "Location", Value, UsePhonetic);
            this.QueryParameters.Add(Param);
        }

		public void AddPhoneParameter(string Value)
		{
            DirectoriesQueryParameter Param = new DirectoriesQueryParameter(8, "Phone", Value);
            this.QueryParameters.Add(Param);
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
            var OrderedParameters = this.QueryParameters.OrderBy(q => q.Sequence).ToList();
            foreach (DirectoriesQueryParameter qp in OrderedParameters)
            {
                AddressParam.Add(qp.GetParameter());
            }
			

			AddressParam.Add (TypeCode);
			
			
			return AddressParam;
		}
	}
}
