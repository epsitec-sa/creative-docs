namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroupDefinition
	{


		public EervGroupDefinition(string id, string name, string parentId)
		{
			this.Id = id;
			this.Name = name;
			this.ParentId = parentId;
		}


		public readonly string Id;
		public readonly string Name;
		public readonly string ParentId;


	}


}
