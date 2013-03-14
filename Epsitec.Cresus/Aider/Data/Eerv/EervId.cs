using System;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervId
	{


		public EervId(string id, string name, EervKind kind)
		{
			this.Id = id;
			this.Name = name;
			this.Kind = kind;
		}


		public string GetFileName()
		{
			switch (this.Kind)
			{
				case EervKind.Canton:
					return "fichier du canton (" + this.Name + ")";

				case EervKind.Region:
					return "fichier de la région " + this.GetRegionCode ();

				case EervKind.Parish:
					return "fichier de la paroisse " + this.Name;

				default:
					throw new NotImplementedException ();
			}
		}


		public int GetRegionCode()
		{
			if (this.Kind != EervKind.Region && this.Kind != EervKind.Parish)
			{
				throw new InvalidOperationException ();
			}

			var digits = this.Id.Substring (0, 2);

			return int.Parse (digits);
		}


		public readonly string Id;
		public readonly string Name;
		public readonly EervKind Kind;
	
	
	}


}
