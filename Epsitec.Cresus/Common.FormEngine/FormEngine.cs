using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Générateur de masques de saisie.
	/// </summary>
	public class FormEngine
	{
		public FormEngine(ResourceManager resourceManager)
		{
			//	Constructeur.
			this.resourceManager = resourceManager;
		}

		public Widget CreateForm(Druid entityId, List<FieldDescription> fields)
		{
			//	Crée un masque de saisie pour une entité donnée.
			Caption entityCaption = this.resourceManager.GetCaption(entityId);
			StructuredType entityType = TypeRosetta.GetTypeObject(entityCaption) as StructuredType;

			StructuredData entityData = new StructuredData(entityType);
			entityData.UndefinedValueMode = UndefinedValueMode.Default;

			UI.Panel root = new UI.Panel();
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource("Data", entityData);

			Widgets.Layouts.GridLayoutEngine grid = new Widgets.Layouts.GridLayoutEngine();
			grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition());
			grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition(new Widgets.Layouts.GridLength(1, Widgets.Layouts.GridUnitType.Proportional), 60, double.PositiveInfinity));
			grid.ColumnDefinitions[0].RightBorder = 1;

			Widgets.Layouts.LayoutEngine.SetLayoutEngine(root, grid);

			List<Druid> entitiesBlackList = new List<Druid>();
			this.CreateFormRows(entityType, root, grid, "Data.", 0, entitiesBlackList);

			return root;
		}

		private void CreateFormRows(StructuredType entityType, UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string prefix, int depth, List<Druid> entitiesBlackList)
		{
			if (depth > 3)
			{
				return;
			}

			//	A-t-on déjà traité cette entité ? Si oui, on ne génère pas de nouveaux
			//	champs pour éviter de tourner en rond avec des relations cycliques.
			if (entitiesBlackList.Contains(entityType.CaptionId))
			{
				return;
			}
			else
			{
				entitiesBlackList.Add(entityType.CaptionId);
			}

			foreach (string fieldId in entityType.GetFieldIds())
			{
				StructuredTypeField field = entityType.GetField(fieldId);

				if (field.Relation == FieldRelation.None)
				{
					this.CreateField(field, root, grid, prefix);
				}
				else if (field.Relation == FieldRelation.Reference)
				{
					this.CreateFormRows(field.Type as StructuredType, root, grid, string.Concat(prefix, field.Id, "."), depth+1, entitiesBlackList);
				}
			}
		}

		private void CreateField(StructuredTypeField field, UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string prefix)
		{
			int row = grid.RowDefinitions.Count;

			UI.Placeholder placeholder = new Epsitec.Common.UI.Placeholder(root);
			placeholder.SetBinding(UI.Placeholder.ValueProperty, new Binding(BindingMode.TwoWay, string.Concat(prefix, field.Id)));
			placeholder.TabIndex = row;

			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
			grid.RowDefinitions[row].BottomBorder = 2;

			Widgets.Layouts.GridLayoutEngine.SetColumn(placeholder, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow(placeholder, row);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan(placeholder, 2);
		}


		ResourceManager resourceManager;
	}
}
