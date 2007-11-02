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
			StructuredType entity = TypeRosetta.GetTypeObject(entityCaption) as StructuredType;

			StructuredData entityData = new StructuredData(entity);
			entityData.UndefinedValueMode = UndefinedValueMode.Default;

			UI.Panel root = new UI.Panel();
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource("Data", entityData);

			Widgets.Layouts.GridLayoutEngine grid = new Widgets.Layouts.GridLayoutEngine();
			grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition());
			for (int i=0; i<FormEngine.MaxColumnsRequired; i++)
			{
				grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition(new Widgets.Layouts.GridLength(1+i, Widgets.Layouts.GridUnitType.Proportional), 20, double.PositiveInfinity));
			}
			grid.ColumnDefinitions[0].RightBorder = 1;

			Widgets.Layouts.LayoutEngine.SetLayoutEngine(root, grid);

#if false
			List<Druid> entitiesBlackList = new List<Druid>();
			this.CreateFormRows(entity, root, grid, "Data.", 0, entitiesBlackList);
#else
			foreach (FieldDescription field in fields)
			{
				string path = field.GetPath("Data");
				this.CreateField(root, grid, path, field);
			}
#endif

			return root;
		}

		private void CreateFormRows(StructuredType entity, UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string prefix, int depth, List<Druid> entitiesBlackList)
		{
			if (depth > 3)
			{
				return;
			}

			//	A-t-on déjà traité cette entité ? Si oui, on ne génère pas de nouveaux
			//	champs pour éviter de tourner en rond avec des relations cycliques.
			if (entitiesBlackList.Contains(entity.CaptionId))
			{
				return;
			}
			else
			{
				entitiesBlackList.Add(entity.CaptionId);
			}

			foreach (string fieldId in entity.GetFieldIds())
			{
				StructuredTypeField field = entity.GetField(fieldId);

				if (field.Relation == FieldRelation.None)
				{
					this.CreateField(root, grid, string.Concat(prefix, field.Id), null);
				}
				else if (field.Relation == FieldRelation.Reference)
				{
					this.CreateFormRows(field.Type as StructuredType, root, grid, string.Concat(prefix, field.Id, "."), depth+1, entitiesBlackList);
				}
			}
		}

		private void CreateField(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string path, FieldDescription field)
		{
			int row = grid.RowDefinitions.Count;

			UI.Placeholder placeholder = new Epsitec.Common.UI.Placeholder(root);
			placeholder.SetBinding(UI.Placeholder.ValueProperty, new Binding(BindingMode.TwoWay, path));
			placeholder.TabIndex = row;

			if (field != null)
			{
				placeholder.BackColor = field.BackColor;
			}

			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			double m = 2;
			if (field != null)
			{
				switch (field.BottomSeparator)
				{
					case FieldDescription.SeparatorType.Compact:
						m = -1;
						break;

					case FieldDescription.SeparatorType.Extend:
						m = 10;
						break;

					case FieldDescription.SeparatorType.Line:
						m = 0;
						break;
				}
			}
			grid.RowDefinitions[row].BottomBorder = m;

			Widgets.Layouts.GridLayoutEngine.SetColumn(placeholder, 0);
			Widgets.Layouts.GridLayoutEngine.SetRow(placeholder, row);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan(placeholder, 1+field.ColumnsRequired);
			Widgets.Layouts.GridLayoutEngine.SetRowSpan(placeholder, field.RowsRequired);

			if (field != null && field.BottomSeparator == FieldDescription.SeparatorType.Line)
			{
				row++;
				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
				grid.RowDefinitions[row].TopBorder = 10;
				grid.RowDefinitions[row].BottomBorder = 10;

				Separator sep = new Separator(root);
				sep.PreferredHeight = 1;

				Widgets.Layouts.GridLayoutEngine.SetColumn(sep, 0);
				Widgets.Layouts.GridLayoutEngine.SetRow(sep, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(sep, 1+FormEngine.MaxColumnsRequired);
			}
		}


		public static readonly int MaxColumnsRequired = 10;

		ResourceManager resourceManager;
	}
}
