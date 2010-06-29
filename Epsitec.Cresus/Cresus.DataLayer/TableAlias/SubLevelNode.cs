namespace Epsitec.Cresus.DataLayer.TableAlias
{


	/// <summary>
	/// The <c>SubLevelNode</c> class represent the <see cref="Node"/> that cannot have any children.
	/// </summary>
	internal class SubLevelNode : Node
	{


		/// <summary>
		/// Creates a new instance of <see cref="SubLevelNode"/>.
		/// </summary>
		/// <param name="parentNode">The parent of the node.</param>
		/// <param name="name">The SLQ name of the table of the node.</param>
		/// <param name="alias">The alias of the node.</param>
		public SubLevelNode(TopLevelNode parentNode, string name, string alias)
			: base (parentNode, name, alias)
		{
		}


	}


}
