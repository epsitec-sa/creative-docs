using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Aider.Data.ECh
{


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


		public readonly EChPerson Adult1;
		public readonly EChPerson Adult2;
		public readonly ReadOnlyCollection<EChPerson> Children;
		public readonly EChAddress Address;


	}


}
