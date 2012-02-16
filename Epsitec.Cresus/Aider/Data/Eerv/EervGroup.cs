namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroup
	{


		public EervGroup(string id, string definitionId, string name)
		{
			this.Id = id;
			this.DefinitionId = definitionId;
			this.Name = name;
		}


		public readonly string Id;
		public readonly string DefinitionId;
		public readonly string Name;


	}


}
