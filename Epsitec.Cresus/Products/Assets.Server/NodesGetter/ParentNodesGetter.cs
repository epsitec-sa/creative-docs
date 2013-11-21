//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des données quelconques, enrichies du parent (Guid)
	/// et de la position (int).
	/// GuidNode -> ParentNode
	/// </summary>
	public class ParentNodesGetter : AbstractNodesGetter<ParentNode>  // outputNodes
	{
		public ParentNodesGetter(AbstractNodesGetter<GuidNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

			this.OrderField = ObjectField.Nom;
		}


		public Timestamp?						Timestamp;
		public ObjectField						OrderField;


		public override int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public override ParentNode this[int index]
		{
			get
			{
				var node       = this.inputNodes[index];
				var obj        = this.accessor.GetObject (this.baseType, node.Guid);
				var parent     = this.GetParent (obj);
				var orderValue = this.GetOrderValue (obj);

				return new ParentNode (node.Guid, parent, orderValue);
			}
		}


		private Guid GetParent(DataObject obj)
		{
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.Timestamp, ObjectField.Parent) as DataGuidProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return Guid.Empty;
		}

		private string GetOrderValue(DataObject obj)
		{
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.Timestamp, this.OrderField) as DataStringProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}


		private readonly AbstractNodesGetter<GuidNode>	inputNodes;
		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
	}
}
