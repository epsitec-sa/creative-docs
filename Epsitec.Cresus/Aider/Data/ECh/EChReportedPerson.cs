using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Xml.Linq;


namespace Epsitec.Aider.Data.ECh
{

    [System.Serializable]
	internal sealed class EChReportedPerson
	{


		// NOTE: Here we discard the field cantonalStreetNumber.


		public EChReportedPerson(EChPerson adult1, EChPerson adult2, IEnumerable<EChPerson> children, EChAddress address)
		{
			this.Adult1 = adult1;
			this.Adult2 = adult2;
			this.Children = children.ToList ().AsReadOnly ();
			this.Address = address;
		}

		/// <summary>
		/// Constructor used by EChDataComparer
		/// </summary>
		public EChReportedPerson(EChPerson adult1, EChPerson adult2, IEnumerable<EChPerson> children, EChAddress address,XElement Xml)
		{
			this.Adult1 = adult1;
			this.Adult2 = adult2;
			this.Children = children.ToList ().AsReadOnly ();
			this.Address = address;

			//Calculate a surrogate familyKey for traking slow change in entity
			List<string> keysToOrder = new List<string> ();
			keysToOrder.Add (this.Adult1.Id);

			if (this.Adult2 != null)
			{
				keysToOrder.Add (this.Adult2.Id);
			}

			//Adding Childs to surrogate key
			foreach (EChPerson c in this.Children)
			{
				keysToOrder.Add (c.Id);
			}

			keysToOrder.Sort ();

			this.FamilyKey = string.Concat (keysToOrder);

			this.Xml = Xml;
		}


		public IEnumerable<EChPerson> GetAdults()
		{
			if (this.Adult1 != null)
			{
				yield return this.Adult1;
			}

			if (this.Adult2 != null)
			{
				yield return this.Adult2;
			}
		}


		public IEnumerable<EChPerson> GetMembers()
		{
			return this.GetAdults ().Concat (this.Children);
		}

		public bool CheckData(string hn, string cc, string al, string s, int szc, int szca, int szci, string t)
		{
			return this.Address.HouseNumber == hn && this.Address.CountryCode == cc && this.Address.AddressLine1 == al && this.Address.Street == s && this.Address.SwissZipCode == szc && this.Address.SwissZipCodeAddOn == szca && this.Address.SwissZipCodeId == szci && this.Address.Town == t;
		}
		
		public XElement GetXml()
		{
			return new XElement (this.Xml);
		}

		public readonly EChPerson Adult1;
		public readonly EChPerson Adult2;
		public readonly ReadOnlyCollection<EChPerson> Children;
		public readonly EChAddress Address;

		//Used by DataComparer
		public readonly string FamilyKey;

        [System.NonSerialized]
		private XElement Xml;


	}


}
