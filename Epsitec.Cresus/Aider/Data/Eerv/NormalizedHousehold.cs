using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class NormalizedHousehold
	{


		public NormalizedPerson Head1
		{
			get;
			set;
		}


		public NormalizedPerson Head2
		{
			get;
			set;
		}



		public List<NormalizedPerson> Children
		{
			get;
			set;
		}


		public IEnumerable<NormalizedPerson> Members
		{
			get
			{
				if (this.Head1 != null)
				{
					yield return this.Head1;
				}

				if (this.Head2 != null)
				{
					yield return this.Head2;
				}

				foreach (var child in this.Children)
				{
					yield return child;
				}
			}
		}



		public NormalizedAddress Address
		{
			get;
			set;
		}


	}


}
