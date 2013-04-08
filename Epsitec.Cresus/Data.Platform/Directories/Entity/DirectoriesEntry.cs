using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories.Entity
{
	public class DirectoriesEntry
	{

		public DirectoriesEntry(XElement Entry)
		{
			this.EntryId = Entry.Attribute ("EntryId").Value;
			this.TypeCode = Entry.Attribute ("TypeCode").Value;
			this.EntryAdds = new List<DirectoriesEntryAdd> ();

			foreach (XElement elem in Entry.Elements ("EntryAdds").Elements("EntryAdd"))
			{
				this.EntryAdds.Add (new DirectoriesEntryAdd (elem));
			}
		}

		public string EntryId;
		public string TypeCode;
		public List<DirectoriesEntryAdd> EntryAdds;

		/*<Entry EntryId="257620" TypeCode="RES">
		  <Hierarchies></Hierarchies>
		  <EntryRefs />
		  <EntryAdds>
			<EntryAdd SortNo="0" LastName="à Porta" FemaleName="Fisler" FirstName="Roger" Profession="chef de cuisine" StreetName="Brestenbühlstrasse" HouseNo="31" Zip="8182" LocaPostId="818200" LocaPostName="Hochfelden" StateId="27" StateCode="ZH" CountryCode="CHE" ValidFrom="1986-09-25" ValidTo="2099-01-01">
			  <Services>
				<Service SortNo="0" TypeGrpCode="TEL" Value="044 860 00 82" IsNoAdvert="1" ValidFrom="1986-09-25" ValidTo="2099-01-01" />
			  </Services>
			</EntryAdd>
		  </EntryAdds>
		  <LocaDirs>
			<LocaDir Name="Hochfelden" />
		  </LocaDirs>
		  <Cates />
	   </Entry>*/



	}
}
