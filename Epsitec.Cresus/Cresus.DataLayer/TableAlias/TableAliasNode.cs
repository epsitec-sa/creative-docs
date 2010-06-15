namespace Epsitec.Cresus.DataLayer.TableAlias
{


	internal abstract class TableAliasNode
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


		protected TableAliasNode(string name, string alias)
		{
			this.Name = name;
			this.Alias = alias;
		}


	}


}