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
						container.Margins = new Margins(0, 20, 0, 0);
					}
				}

				return root;
			}
		}

		private Widget CreateFormContainer(Druid entityId, List<FieldDescription> fields, string container)
		{
			//	Crée un conteneur (super-colonne) d'un masque de saisie pour une entité donnée.
			Caption entityCaption = this.resourceManager.GetCaption(entityId);
			StructuredType entity = TypeRosetta.GetTypeObject(entityCaption) as StructuredType;

			StructuredData entityData = new StructuredData(entity);
			entityData.UndefinedValueMode = UndefinedValueMode.Default;

			UI.Panel root = new UI.Panel();
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource("Data", entityData);

			//	Première passe pour déterminer quelles colonnes contiennent des labels.
			int column = 0, row = 0;
			bool[] isLabel = new bool[FormEngine.MaxColumnsRequired];
			foreach (FieldDescription field in fields)
			{
				if (field.Container == container)
				{
					this.PrecreateField(field, isLabel, ref column, ref row);
				}
			}

			//	Crée les différentes colonnes.
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

			//	Deuxième passe pour générer le contenu.
			column = 0;
			row = 0;
			List<Druid> lastTitle = null;
			foreach (FieldDescription field in fields)
			{
				if (field.Container == container)
				{
					string path = field.GetPath("Data");
					this.CreateField(root, grid, path, field, ref column, ref row, ref lastTitle);
				}
			}

			return root;
		}

		private void PrecreateField(FieldDescription field, bool[] isLabel, ref int column, ref int row)
		{
			//	Permière passe pour déterminer quelles colonnes contiennent des labels.
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

		private void CreateField(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string path, FieldDescription field, ref int column, ref int row, ref List<Druid> lastTitle)
		{
			//	Deuxième passe pour crée les widgets pour un champ dans la grille.
			FieldDescription.SeparatorType topSeparator = field.TopSeparator;

			if (topSeparator == FieldDescription.SeparatorType.Title)
			{
				List<Druid> druids = field.FieldIds;
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				for (int i=0; i<druids.Count-1; i++)
				{
					Druid druid = druids[i];

					if (lastTitle != null && i < lastTitle.Count && lastTitle[i] == druid)  // label déjà mis précédemment ?
					{
						continue;
					}

					if (builder.Length > 0)
					{
						builder.Append(", ");
					}

					Caption caption = this.resourceManager.GetCaption(druid);
					builder.Append(caption.DefaultLabel);
				}

				if (builder.Length == 0)  // titre sans texte ?
				{
					topSeparator = FieldDescription.SeparatorType.Line;  // il faudra mettre une simple ligne
				}
				else
				{
					grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
					grid.RowDefinitions[row].TopBorder = 5;
					grid.RowDefinitions[row].BottomBorder = 0;

					double size = System.Math.Max(200-(druids.Count-2)*25, 100);

					StaticText text = new StaticText(root);
					text.Text = string.Concat("<font size=\"", size.ToString(System.Globalization.CultureInfo.InvariantCulture), "%\"><b>", builder.ToString(), "</b></font>");
					text.PreferredHeight = size/100*16;

					Widgets.Layouts.GridLayoutEngine.SetColumn(text, 0);
					Widgets.Layouts.GridLayoutEngine.SetRow(text, row);
					Widgets.Layouts.GridLayoutEngine.SetColumnSpan(text, 1+FormEngine.MaxColumnsRequired);

					row++;
				}

				lastTitle = druids;  // pour se rappeler du titre précédent
			}

			if (topSeparator == FieldDescription.SeparatorType.Line ||
				topSeparator == FieldDescription.SeparatorType.Title)
			{
				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
				grid.RowDefinitions[row].TopBorder = (topSeparator == FieldDescription.SeparatorType.Title) ? 0 : 10;
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
