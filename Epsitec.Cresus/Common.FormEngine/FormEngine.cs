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
			List<string> containers = new List<string>();
			foreach (FieldDescription field in fields)
			{
				if (!containers.Contains(field.Container))
				{
					containers.Add(field.Container);
				}
			}

			if (containers.Count == 1)
			{
				return this.CreateFormContainer(entityId, fields, containers[0]);
			}
			else
			{
				Widget root = new FrameBox();
				root.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

				for (int i=0; i<containers.Count; i++)
				{
					Widget container = this.CreateFormContainer(entityId, fields, containers[i]);
					container.SetParent(root);
					container.Dock = DockStyle.StackFill;

					if (i < containers.Count-1)
					{
						container.Margins = new Margins(0, 10, 0, 0);
					}
				}

				return root;
			}
		}

		private Widget CreateFormContainer(Druid entityId, List<FieldDescription> fields, string container)
		{
			//	Crée un conteneur d'un masque de saisie pour une entité donnée.
			Caption entityCaption = this.resourceManager.GetCaption(entityId);
			StructuredType entity = TypeRosetta.GetTypeObject(entityCaption) as StructuredType;

			StructuredData entityData = new StructuredData(entity);
			entityData.UndefinedValueMode = UndefinedValueMode.Default;

			UI.Panel root = new UI.Panel();
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource("Data", entityData);

			int column = 0, row = 0;
			bool[] isLabel = new bool[FormEngine.MaxColumnsRequired];
			foreach (FieldDescription field in fields)
			{
				if (field.Container == container)
				{
					this.PrecreateField(field, isLabel, ref column, ref row);
				}
			}

			Widgets.Layouts.GridLayoutEngine grid = new Widgets.Layouts.GridLayoutEngine();
			for (int i=0; i<FormEngine.MaxColumnsRequired; i++)
			{
				if (isLabel[i])
				{
					grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition());
				}
				else
				{
					grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition(new Widgets.Layouts.GridLength(i, Widgets.Layouts.GridUnitType.Proportional), 20, double.PositiveInfinity));
				}
			}
			grid.ColumnDefinitions[0].RightBorder = 1;

			Widgets.Layouts.LayoutEngine.SetLayoutEngine(root, grid);

			column = 0;
			row = 0;
			foreach (FieldDescription field in fields)
			{
				if (field.Container == container)
				{
					string path = field.GetPath("Data");
					this.CreateField(root, grid, path, field, ref column, ref row);
				}
			}

			return root;
		}

		private void PrecreateField(FieldDescription field, bool[] isLabel, ref int column, ref int row)
		{
			int columnsRequired = System.Math.Min(field.ColumnsRequired, FormEngine.MaxColumnsRequired-1);

			if (column+1+columnsRequired > FormEngine.MaxColumnsRequired)  // dépasse à droite ?
			{
				row++;
				column = 0;
			}

			isLabel[column] = true;

			if (field.BottomSeparator == FieldDescription.SeparatorType.Append)
			{
				column += 1+columnsRequired;
			}
			else
			{
				row++;
				column = 0;
			}
		}

		private void CreateField(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string path, FieldDescription field, ref int column, ref int row)
		{
			//	Crée les widgets pour un champ dans la grille.
			if (field.TopSeparator == FieldDescription.SeparatorType.Title)
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				List<Druid> druids = field.FieldIds;
				for (int i=0; i<druids.Count-1; i++)
				{
					if (i > 0)
					{
						builder.Append(".");
					}

					Druid druid = druids[i];
					Caption caption = this.resourceManager.GetCaption(druid);
					builder.Append(caption.Name);
				}

				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
				grid.RowDefinitions[row].TopBorder = 5;
				grid.RowDefinitions[row].BottomBorder = 5;

				StaticText text = new StaticText(root);
				text.Text = string.Concat("<font size=\"125%\"><b>", builder.ToString(), "</b></font>");

				Widgets.Layouts.GridLayoutEngine.SetColumn(text, 0);
				Widgets.Layouts.GridLayoutEngine.SetRow(text, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(text, 1+FormEngine.MaxColumnsRequired);

				row++;
			}

			if (field.TopSeparator == FieldDescription.SeparatorType.Line ||
				field.TopSeparator == FieldDescription.SeparatorType.Title)
			{
				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
				grid.RowDefinitions[row].TopBorder = (field.TopSeparator == FieldDescription.SeparatorType.Title) ? 0 : 10;
				grid.RowDefinitions[row].BottomBorder = 10;

				Separator sep = new Separator(root);
				sep.PreferredHeight = 1;

				Widgets.Layouts.GridLayoutEngine.SetColumn(sep, 0);
				Widgets.Layouts.GridLayoutEngine.SetRow(sep, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(sep, 1+FormEngine.MaxColumnsRequired);

				row++;
			}

			UI.Placeholder placeholder = new Epsitec.Common.UI.Placeholder(root);
			placeholder.SetBinding(UI.Placeholder.ValueProperty, new Binding(BindingMode.TwoWay, path));
			placeholder.BackColor = field.BackColor;
			placeholder.TabIndex = grid.RowDefinitions.Count;

			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			double m = 2;
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
			grid.RowDefinitions[row].BottomBorder = m;

			int columnsRequired = System.Math.Min(field.ColumnsRequired, FormEngine.MaxColumnsRequired-1);

			if (column+1+columnsRequired > FormEngine.MaxColumnsRequired)  // dépasse à droite ?
			{
				row++;
				column = 0;
			}

			if (field.RowsRequired > 1)
			{
				placeholder.PreferredHeight = field.RowsRequired*20;
			}

			Widgets.Layouts.GridLayoutEngine.SetColumn(placeholder, column);
			Widgets.Layouts.GridLayoutEngine.SetRow(placeholder, row);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan(placeholder, 1+columnsRequired);

			if (field.BottomSeparator == FieldDescription.SeparatorType.Append)
			{
				column += 1+columnsRequired;
			}
			else
			{
				row++;
				column = 0;
			}

			if (field.BottomSeparator == FieldDescription.SeparatorType.Line)
			{
				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
				grid.RowDefinitions[row].TopBorder = 10;
				grid.RowDefinitions[row].BottomBorder = 10;

				Separator sep = new Separator(root);
				sep.PreferredHeight = 1;

				Widgets.Layouts.GridLayoutEngine.SetColumn(sep, 0);
				Widgets.Layouts.GridLayoutEngine.SetRow(sep, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(sep, 1+FormEngine.MaxColumnsRequired);

				row++;
			}
		}


		public static readonly int MaxColumnsRequired = 10;
		public static readonly int MaxRowsRequired = 20;

		ResourceManager resourceManager;
	}
}
