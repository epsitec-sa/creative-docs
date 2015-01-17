using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Epsitec.Data.Platform.Directories.Helpers;

namespace Epsitec.Data.Platform.Directories.Entity
{
	public class DirectoriesEntryAddService
	{
		public DirectoriesEntryAddService(XElement Service)
		{
			this.SortNo = Service.Attribute ("SortNo").Value;
			this.TypeGrpCode = DirectoriesValueHelper.TryGetFromXElement ("TypeGrpCode", Service);
			this.Value = DirectoriesValueHelper.TryGetFromXElement ("Value", Service);
			this.IsNoAdvert = DirectoriesValueHelper.TryGetFromXElement ("IsNoAdvert", Service);
			this.ValidFrom = DirectoriesValueHelper.TryGetFromXElement ("ValidFrom", Service);
			this.ValidTo = DirectoriesValueHelper.TryGetFromXElement ("ValidTo", Service);
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
