﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsTreeGraphicController : AbstractTreeGraphicController<CumulNode>
	{
		public AssetsTreeGraphicController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.treeGraphicViewState = new TreeGraphicState ();

			this.treeGraphicViewState.Fields.Add (this.accessor.GetMainStringField (this.baseType));
			this.treeGraphicViewState.Fields.AddRange (this.UserFields
				.Where (x => x.Type == FieldType.AmortizedAmount || x.Type == FieldType.ComputedAmount)
				.Select (x => x.Field));

			for (int i=0; i<this.treeGraphicViewState.Fields.Count; i++)
			{
				this.treeGraphicViewState.FontFactors.Add ((i == 0) ? 1.0 : 1.5);
			}

			this.treeGraphicViewState.ColumnWidth = 100;

			this.treeGraphicViewMode = TreeGraphicMode.FixedWidth;
		}


		public override void UpdateController(AbstractNodeGetter<CumulNode> nodeGetter, Guid selectedGuid, bool crop = true)
		{
			if (this.treeGraphicViewState == null || this.scrollable == null)
			{
				return;
			}

			this.scrollable.Viewport.Children.Clear ();

			var parents = new List<Widget> ();
			parents.Add (this.scrollable.Viewport);

			var assetFields = this.GetFieds ();
			var groupFields = this.GroupFields.ToArray ();
			var fontFactors = this.GetFontFactors ();

			var ng = nodeGetter as ObjectsNodeGetter;
			int deep = this.GetDeep (ng);
			this.InitializeMinMax (ng);

			foreach (var node in ng.Nodes)
			{
				var level = node.Level;
				var parent = parents[level];

				ObjectField[] fields;
				if (node.BaseType == BaseType.Groups)
				{
					fields = groupFields;
				}
				else
				{
					fields = assetFields;
				}

				double fontSize = AbstractTreeGraphicController<CumulNode>.GetFontSize (deep, level);

				var values = this.GetValues (ng, node.BaseType, node, fields);
				var w = this.CreateTile (parent, node.Guid, level, fontSize, node.Type, values, fontFactors);

				if (parents.Count <= level+1)
				{
					parents.Add (null);
				}

				parents[level+1] = w;
			}

			this.UpdateSelection (selectedGuid, crop);
		}


		private int GetDeep(ObjectsNodeGetter nodeGetter)
		{
			return nodeGetter.Nodes.Max (x => x.Level) + 1;
		}

		private void InitializeMinMax(ObjectsNodeGetter nodeGetter)
		{
			this.minAmount = 0.0m;
			this.maxAmount = 0.0m;

			var assetFields = this.GetFieds ();
			var groupFields = this.GroupFields.ToArray ();

			foreach (var node in nodeGetter.Nodes)
			{
				ObjectField[] fields;
				if (node.BaseType == BaseType.Groups)
				{
					fields = groupFields;
				}
				else
				{
					fields = assetFields;
				}

				var values = this.GetValues (nodeGetter, node.BaseType, node, fields);

				foreach (var value in values)
				{
					if (value.IsAmount)
					{
						this.minAmount = System.Math.Min (this.minAmount, value.Amount.Value);
						this.maxAmount = System.Math.Max (this.maxAmount, value.Amount.Value);
					}
				}
			}
		}


		private TreeGraphicValue[] GetValues(ObjectsNodeGetter nodeGetter, BaseType baseType, CumulNode node, ObjectField[] fields)
		{
			var list = new List<TreeGraphicValue> ();
			var obj = this.accessor.GetObject (baseType, node.Guid);

			foreach (var field in fields)
			{
				var text = this.GetValue (nodeGetter, obj, node, field);
				list.Add (text);
			}

			//	Supprime les lignes vides à la fin, pour que les éventuelles tuiles filles
			//	soient positionnées plus haut.
			while (list.Count > 0 && list.Last ().IsEmpty)
			{
				list.RemoveAt (list.Count-1);
			}

			return list.ToArray ();
		}

		private TreeGraphicValue GetValue(ObjectsNodeGetter nodeGetter, DataObject obj, CumulNode node, ObjectField field)
		{
			var type = this.accessor.GetFieldType (field);

			if (type == FieldType.ComputedAmount)
			{
				//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
				//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
				var v = nodeGetter.GetValue (obj, node, field);
				if (v.HasValue)
				{
					var ca = new ComputedAmount (v);
					return TreeGraphicValue.CreateAmount (ca.FinalAmount);
				}
			}
			else if (type == FieldType.AmortizedAmount)
			{
				//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
				//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
				var v = nodeGetter.GetValue (obj, node, field);
				if (v.HasValue)
				{
					var aa = new AmortizedAmount (null)
					{
						InitialAmount = v,
					};
					return TreeGraphicValue.CreateAmount (aa.FinalAmortizedAmount);
				}
			}
			else
			{
				ObjectProperties.GetObjectPropertyAmortizedAmount (obj, null, field);
				var text = ObjectProperties.GetObjectPropertyString (obj, null, field);
				return TreeGraphicValue.CreateText (text);
			}

			return TreeGraphicValue.Empty;
		}


		private IEnumerable<ObjectField> GroupFields
		{
			get
			{
				yield return ObjectField.Name;

				foreach (var field in this.UserFields
					.Where (x => x.Type == FieldType.AmortizedAmount || x.Type == FieldType.ComputedAmount)
					.Select (x => x.Field))
				{
					yield return field;
				}
			}
		}

		private IEnumerable<UserField> UserFields
		{
			get
			{
				return AssetsLogic.GetUserFields (this.accessor);
			}
		}
	}
}
