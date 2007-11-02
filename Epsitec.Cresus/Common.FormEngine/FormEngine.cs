using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// G�n�rateur de masques de saisie.
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
			//	Cr�e un masque de saisie pour une entit� donn�e.
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

			foreach (FieldDescription field in fields)
			{
				string path = field.GetPath("Data");
				this.CreateField(root, grid, path, field);
			}

			return root;
		}

		private void CreateField(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string path, FieldDescription field)
		{
			//	Cr�e les widgets pour un champ dans la grille.
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
		public static readonly int MaxRowsRequired = 20;

		ResourceManager resourceManager;
	}
}
