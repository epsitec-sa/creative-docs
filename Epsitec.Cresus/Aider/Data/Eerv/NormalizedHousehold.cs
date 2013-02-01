using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class NormalizedHousehold
	{


		public NormalizedHousehold()
		{
			this.Heads = new List<NormalizedPerson> ();
			this.Children = new List<NormalizedPerson> ();
		}


		public List<NormalizedPerson> Heads
		{
			get;
			set;
		}


		public List<NormalizedPerson> Children
		{
			get;
			set;
		}


		public NormalizedAddress Address
		{
			get;
			set;
		}


		public IEnumerable<NormalizedPerson> Members
		{
			get
			{
				return this.Heads.Concat (this.Children);
			}
		}


	}


}
