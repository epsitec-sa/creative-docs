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
	public class AssetsGraphicViewController : AbstractGraphicViewController<CumulNode>
	{
		public AssetsGraphicViewController(DataAccessor accessor, BaseType baseType, AbstractToolbarTreeTableController<CumulNode> treeTableController)
			: base (accessor, baseType, treeTableController)
		{
			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode -> CumulNode
			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.graphicViewState = new GraphicViewState ();

			this.graphicViewState.Fields.Add (this.accessor.GetMainStringField (this.baseType));
			this.graphicViewState.Fields.AddRange (this.UserFields
				.Where (x => x.Type == FieldType.AmortizedAmount || x.Type == FieldType.ComputedAmount)
				.Select (x => x.Field));

			for (int i=0; i<this.graphicViewState.Fields.Count; i++)
			{
				this.graphicViewState.FontFactors.Add ((i == 0) ? 1.0 : 0.7);
			}
		}


		public override void CompactOrExpand(Guid guid)
		{
			int index = this.NodeGetter.SearchBestIndex (guid);
			this.NodeGetter.CompactOrExpand (index);

			this.UpdateData ();
		}

		public override void SetParams(Timestamp? timestamp, Guid rootGuid, SortingInstructions instructions)
		{
			this.timestamp = timestamp;
			this.NodeGetter.SetParams (timestamp, rootGuid, this.graphicViewState.SortingInstructions);

			this.UpdateData ();
		}

		public override void UpdateData()
		{
			if (this.graphicViewState == null || this.scrollable == null)
			{
				return;
			}

			this.scrollable.Viewport.Children.Clear ();

			var parents = new List<Widget> ();
			parents.Add (this.scrollable.Viewport);

			var assetFields = this.GetFieds ();
			var groupFields = this.GroupFields;
			var fontFactors = this.GetFontFactors ();

			foreach (var node in this.NodeGetter.Nodes)
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

				var texts = this.GetTexts (node.BaseType, node, fields);
				var w = this.CreateNode (parent, node.Guid, node.Level, node.Type, texts, fontFactors);

				if (parents.Count <= level+1)
				{
					parents.Add (null);
				}

				parents[level+1] = w;
			}
		}

		private string[] GetTexts(BaseType baseType, CumulNode node, ObjectField[] fields)
		{
			var list = new List<string> ();
			var obj = this.accessor.GetObject (baseType, node.Guid);

			foreach (var field in fields)
			{
				var text = this.GetText (obj, node, field);
				list.Add (text);
			}

			return list.ToArray ();
		}

		private string GetText(DataObject obj, CumulNode node, ObjectField field)
		{
			var type = this.accessor.GetFieldType (field);

			if (type == FieldType.ComputedAmount)
			{
				//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
				//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
				var v = this.NodeGetter.GetValue (obj, node, field);
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
				var v = this.NodeGetter.GetValue (obj, node, field);
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

		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}


		private Timestamp? timestamp;
	}
}
