//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Helpers;
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
				this.treeGraphicViewState.FontFactors.Add ((i == 0) ? 1.0 : 0.7);
			}

			this.treeGraphicViewMode = TreeGraphicMode.AutoWidthAllLines;
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
			var groupFields = this.GroupFields;
			var fontFactors = this.GetFontFactors ();

			var ng = nodeGetter as ObjectsNodeGetter;
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

				var texts = this.GetTexts (ng, node.BaseType, node, fields);
				var w = this.CreateTile (parent, node.Guid, node.Level, node.Type, texts, fontFactors);

				if (parents.Count <= level+1)
				{
					parents.Add (null);
				}

				parents[level+1] = w;
			}

			this.UpdateSelection (selectedGuid, crop);
		}

		private string[] GetTexts(ObjectsNodeGetter nodeGetter, BaseType baseType, CumulNode node, ObjectField[] fields)
		{
			var list = new List<string> ();
			var obj = this.accessor.GetObject (baseType, node.Guid);

			foreach (var field in fields)
			{
				var text = this.GetText (nodeGetter, obj, node, field);
				list.Add (text);
			}

			return list.ToArray ();
		}

		private string GetText(ObjectsNodeGetter nodeGetter, DataObject obj, CumulNode node, ObjectField field)
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
					if (ca != null)
					{
						return TypeConverters.AmountToString (ca.FinalAmount);
					}
				}
			}
			else if (type == FieldType.AmortizedAmount)
			{
				//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
				//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
				var v = nodeGetter.GetValue (obj, node, field);
				if (v.HasValue)
				{
					var aa = new AmortizedAmount (v);
					if (aa != null)
					{
						return TypeConverters.AmountToString (aa.FinalAmortizedAmount);
					}
				}
			}
			else
			{
				ObjectProperties.GetObjectPropertyAmortizedAmount (obj, this.timestamp, field);
				return ObjectProperties.GetObjectPropertyString (obj, this.timestamp, field);
			}

			return null;
		}


		private ObjectField[] GroupFields
		{
			get
			{
				return new ObjectField[1] { ObjectField.Name };
			}
		}

		private IEnumerable<UserField> UserFields
		{
			get
			{
				return AssetsLogic.GetUserFields (this.accessor);
			}
		}


		private Timestamp? timestamp;
	}
}
