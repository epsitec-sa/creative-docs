using System.Collections.Generic;
using System.Linq;
using System;


namespace Epsitec.Cresus.DataLayer.TableAlias
{


	/// <summary>
	/// The <c>TopLevelNode</c> class represents the <see cref="Node"/> that can have two kind of
	/// children, other <c>TopLevelNode</c> and <c>SubLevelNode</c>.
	/// </summary>
	internal class TopLevelNode : Node
	{


		/// <summary>
		/// Creates a new instance of <see cref="TopLevelNode"/>.
		/// </summary>
		/// <param name="parentNode">The parent of the node.</param>
		/// <param name="name">The SLQ name of the table of the node.</param>
		/// <param name="alias">The alias of the node.</param>
		public TopLevelNode(TopLevelNode parentNode, string name, string alias)
			: base (parentNode, name, alias)
		{
			this.topLevelNodes = new List<TopLevelNode> ();
			this.subLevelNodes = new List<SubLevelNode> ();
		}


		/// <summary>
		/// Creates a new <see cref="TopLevelNode"/> and adds it in the list of the children of this
		/// instance.
		/// </summary>
		/// <param name="name">The SQL name of the new <see cref="TopLevelNode"/>.</param>
		/// <param name="alias">The alias of the new <see cref="TopLevelNode"/>.</param>
		/// <returns>The new <see cref="TopLevelNode"/>.</returns>
		public TopLevelNode CreateTopLevelNode(string name, string alias)
		{
			TopLevelNode node = new TopLevelNode (this, name, alias);

			this.topLevelNodes.Add (node);

			return node;
		}


		/// <summary>
		/// Creates a new <see cref="SubLevelNode"/> and adds it in the list of the children of this
		/// instance.
		/// </summary>
		/// <param name="name">The SQL name of the new <see cref="SubLevelNode"/>.</param>
		/// <param name="alias">The alias of the new <see cref="SubLevelNode"/>.</param>
		/// <returns>The new <see cref="SubLevelNode"/>.</returns>
		public SubLevelNode CreateSubLevelNode(string name, string alias)
		{
			SubLevelNode node = new SubLevelNode (this, name, alias);

			this.subLevelNodes.Add (node);

			return node;
		}


		/// <summary>
		/// Gets the first <see cref="TopLevelNode"/> child of this instance whose name is
		/// <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The SQL name of the <see cref="TopLevelNode"/> to get.</param>
		/// <returns>The <see cref="TopLevelNode"/>.</returns>
		public TopLevelNode GetTopLevelNode(string name)
		{
			return this.GetTopLevelNode (name, 0);
		}


		/// <summary>
		/// Gets the first <see cref="SubLevelNode"/> child of this instance whose name is
		/// <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The SQL name of the <see cref="SubLevelNode"/> to get.</param>
		/// <returns>The <see cref="SubLevelNode"/>.</returns>
		public SubLevelNode GetSubLevelNode(string name)
		{
			return this.subLevelNodes.Where (n => n.Name == name).FirstOrDefault ();
		}


		/// <summary>
		/// Gets the <see cref="TopLevelNode"/> child of this instance whose name is
		/// <paramref name="name"/> and whose index among its sibling is <see cref="index"/>.
		/// </summary>
		/// <param name="name">The SQL name of the <see cref="TopLevelNode"/>.</param>
		/// <param name="index">
		/// The index of the <see cref="TopLevelNode"/> within the <see cref="TopLevelNode"/> children
		/// that have the name <paramref name="name"/>.
		/// </param>
		/// <returns>The <see cref="TopLevelNode"/></returns>
		public TopLevelNode GetTopLevelNode(string name, int index)
		{
			return this.topLevelNodes.Where (n => n.Name == name).ElementAtOrDefault (index);
		}


		private List<TopLevelNode> topLevelNodes;


		private List<SubLevelNode> subLevelNodes;


	}


}
