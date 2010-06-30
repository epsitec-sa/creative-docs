namespace Epsitec.Cresus.DataLayer.TableAlias
{


	/// <summary>
	/// The <c>AbstractNode</c> class is the base class for all the alias nodes in the tree of aliases.
	/// </summary>
	internal abstract class AbstractNode
	{


		/// <summary>
		/// Gets or sets the SQL name of the table of this node.
		/// </summary>
		/// <value>The name of the SQL table.</value>
		public string Name
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets or sets the alias of this node.
		/// </summary>
		/// <value>The alias of this node.</value>
		public string Alias
		{
			get;
			private set;
		}


		/// <summary>
		/// Creates a new instance of <c>AbstractNode</c>.
		/// </summary>
		/// <param name="parentNode">The parent of the node.</param>
		/// <param name="name">The SLQ name of the table of the node.</param>
		/// <param name="alias">The alias of the node.</param>
		protected AbstractNode(TopLevelNode parentNode, string name, string alias)
		{
			this.parentNode = parentNode;
			
			this.Name = name;
			this.Alias = alias;
		}


		/// <summary>
		/// Gets the parent of this node.
		/// </summary>
		/// <returns>The parent of this node.</returns>
		public TopLevelNode GetParentNode()
		{
			return this.parentNode;
		}


		/// <summary>
		/// The reference to the parent of this node.
		/// </summary>
		private TopLevelNode parentNode;


	}


}