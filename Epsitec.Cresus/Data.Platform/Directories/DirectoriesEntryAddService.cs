using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesEntryAddService
	{
		public DirectoriesEntryAddService(XElement Service)
		{
			this.SortNo = Service.Attribute ("SortNo").Value;
			this.TypeGrpCode = Service.Attribute ("TypeGrpCode").Value;
			this.Value = Service.Attribute ("Value").Value;
			this.IsNoAdvert = Service.Attribute ("IsNoAdvert").Value;
			this.ValidFrom = Service.Attribute ("ValidFrom").Value;
			this.ValidTo = Service.Attribute ("ValidTo").Value;
		}

		public string SortNo;
		public string TypeGrpCode;
		public string Value;
		public string IsNoAdvert;
		public string ValidFrom;
		public string ValidTo;

		///<Service SortNo="0" TypeGrpCode="TEL" Value="044 860 00 82" IsNoAdvert="1" ValidFrom="1986-09-25" ValidTo="2099-01-01" />
	}
}
