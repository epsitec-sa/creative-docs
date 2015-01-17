using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesSearchServiceQuery
	{
		public DirectoriesSearchServiceQuery(DirectoriesAuthentication Authentication,string ServiceCode)
		{
			this.QueryParameters = new List<DirectoriesQueryParameter> ();
			this.Authentication = Authentication;
			this.ServiceCode = ServiceCode;
		}

		private DirectoriesAuthentication Authentication;
		private List<DirectoriesQueryParameter> QueryParameters;
		private string ServiceCode;

		public void AddServiceParameter(string Value)
		{
			DirectoriesQueryParameter Param = new DirectoriesQueryParameter(1, "Service", Value);
			this.QueryParameters.Add(Param);
		}

		public XElement ForgeRequest()
		{
			XElement ServiceListParam = new XElement ("ServiceListParam");
			XElement ServiceCode=new XElement ("ServiceCode")
			{
				Value=this.ServiceCode
			};


			//Add Authentication XElement
			ServiceListParam.Add (this.Authentication.GetAuthenticationElement ());

			XElement Services = new XElement ("Services");
			//Add Service Parameters
			foreach (DirectoriesQueryParameter qp in this.QueryParameters)
			{
				Services.Add(qp.GetParameter());
			}

			ServiceListParam.Add (Services);
			ServiceListParam.Add (ServiceCode);
			
			
			return ServiceListParam;
		}
	}
}
