namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervId
	{


		public EervId(string id, string name)
		{
			this.Id = id;
			this.Name = name;
		}


		public bool IsParish
		{
			get
			{
				// Names have the format RRPP000000 where RR is the region number and PP is the
				// parish number. If the parish number is 00, then we know that this is a region and
				// not a parish.

				return !this.Id.EndsWith ("00000000");
			}
		}


		public readonly string Id;
		public readonly string Name;
	
	
	}


}
