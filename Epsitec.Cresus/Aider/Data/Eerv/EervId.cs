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


		public readonly string Id;
		public readonly string Name;
		public readonly EervKind Kind;
	
	
	}


}
