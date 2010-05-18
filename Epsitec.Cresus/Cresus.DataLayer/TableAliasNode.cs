namespace Epsitec.Cresus.DataLayer
{


	abstract class TableAliasNode
	{


		public string Name
		{
			get;
			private set;
		}
		
		
		public string Alias
		{
			get;
			private set;
		}


		public TableAliasNode(string name, string alias)
		{
			this.Name = name;
			this.Alias = alias;
		}


	}


}