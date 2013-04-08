using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Epsitec.Data.Platform.Directories.Helpers;

namespace Epsitec.Data.Platform.Directories.Entity
{
	public class DirectoriesEntryAdd
	{
		public DirectoriesEntryAdd(XElement EntryAdd)
		{
			this.SortNo = EntryAdd.Attribute("SortNo").Value;
			this.LastName = DirectoriesValueHelper.TryGetFromXElement ("LastName", EntryAdd);
			this.FemaleName = DirectoriesValueHelper.TryGetFromXElement ("FemaleName", EntryAdd);
			this.FirstName = DirectoriesValueHelper.TryGetFromXElement ("FirstName", EntryAdd);
			this.Profession = DirectoriesValueHelper.TryGetFromXElement ("Profession", EntryAdd);
			this.StreetName = DirectoriesValueHelper.TryGetFromXElement ("StreetName", EntryAdd);
			this.HouseNo = DirectoriesValueHelper.TryGetFromXElement ("HouseNo", EntryAdd);
			this.Zip = DirectoriesValueHelper.TryGetFromXElement ("Zip", EntryAdd);
			this.LocaPostId = DirectoriesValueHelper.TryGetFromXElement ("LocaPostId", EntryAdd);
			this.LocaPostName = DirectoriesValueHelper.TryGetFromXElement ("LocaPostName", EntryAdd);
			this.StateId = DirectoriesValueHelper.TryGetFromXElement ("StateId", EntryAdd);
			this.StateCode = DirectoriesValueHelper.TryGetFromXElement ("StateCode", EntryAdd);
			this.CountryCode = DirectoriesValueHelper.TryGetFromXElement ("CountryCode", EntryAdd);
			this.ValidFrom = DirectoriesValueHelper.TryGetFromXElement ("ValidFrom", EntryAdd);
			this.ValidTo = DirectoriesValueHelper.TryGetFromXElement ("ValidTo", EntryAdd);

			this.Services = new List<DirectoriesEntryAddService> ();

			foreach (XElement elem in EntryAdd.Elements ("Services").Elements("Service"))
			{
				this.Services.Add (new DirectoriesEntryAddService (elem));
			}
		}

		public string SortNo;
		public string LastName;
		public string FemaleName;
		public string FirstName;
		public string Profession;
		public string StreetName;
		public string HouseNo;
		public string Zip;
		public string LocaPostId;
		public string LocaPostName;
		public string StateId;
		public string StateCode;
		public string CountryCode;
		public string ValidFrom;
		public string ValidTo;
		public List<DirectoriesEntryAddService> Services;

		/*
		 <EntryAdd SortNo="0" LastName="à Porta" FemaleName="Fisler" FirstName="Roger" Profession="chef de cuisine" StreetName="Brestenbühlstrasse" HouseNo="31" Zip="8182" LocaPostId="818200" LocaPostName="Hochfelden" StateId="27" StateCode="ZH" CountryCode="CHE" ValidFrom="1986-09-25" ValidTo="2099-01-01">
			  <Services>
				<Service SortNo="0" TypeGrpCode="TEL" Value="044 860 00 82" IsNoAdvert="1" ValidFrom="1986-09-25" ValidTo="2099-01-01" />
			  </Services>
		 */
	}
}
