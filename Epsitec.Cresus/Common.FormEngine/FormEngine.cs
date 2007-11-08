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
			List<FieldDescription> flat = Arrange.Develop(fields);

			Caption entityCaption = this.resourceManager.GetCaption(entityId);
			StructuredType entity = TypeRosetta.GetTypeObject(entityCaption) as StructuredType;

			StructuredData entityData = new StructuredData(entity);
			entityData.UndefinedValueMode = UndefinedValueMode.Default;

			Stack<UI.Panel> stack = new Stack<UI.Panel>();

			UI.Panel root = new UI.Panel();
			root.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource("Data", entityData);
			stack.Push(root);

			UI.Panel row = new UI.Panel(stack.Peek());
			row.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			row.Dock = DockStyle.Fill;
			stack.Push(row);

			int i = 0;
			while (i < flat.Count)
			{
				FieldDescription field = flat[i];

				switch (field.Type)
				{
					case FieldDescription.FieldType.Field:
					case FieldDescription.FieldType.Line:
					case FieldDescription.FieldType.Title:
						int count = 0;
						for (int j=i; j<flat.Count; j++)
						{
							if (flat[j].Type == FieldDescription.FieldType.Field ||
								flat[j].Type == FieldDescription.FieldType.Line  ||
								flat[j].Type == FieldDescription.FieldType.Title )
							{
								count++;
							}
							else
							{
								break;
							}
						}

						UI.Panel column = new UI.Panel(stack.Peek());
						column.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
						column.DrawFrameState = FrameState.All;
						column.Dock = DockStyle.Fill;
						column.Margins = new Margins(5, 5, 5, 5);
						column.Padding = new Margins(5, 5, 5, 5);
						//stack.Push(column);

						this.CreateFormBox(column, flat, i, count);

						//stack.Pop();
						i += count;
						break;

					case FieldDescription.FieldType.BoxBegin:
						UI.Panel box = new UI.Panel(stack.Peek());
						box.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
						box.DrawFrameState = FrameState.All;
						box.Dock = DockStyle.Fill;
						box.Margins = new Margins(5, 5, 5, 5);
						box.Padding = new Margins(5, 5, 5, 5);
						stack.Push(box);
						i++;
						break;

					case FieldDescription.FieldType.BoxEnd:
						stack.Pop();
						i++;
						break;

					case FieldDescription.FieldType.BreakColumn:
						i++;
						break;

					case FieldDescription.FieldType.BreakRow:
						stack.Pop();

						row = new UI.Panel(stack.Peek());
						row.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
						row.Dock = DockStyle.Fill;
						stack.Push(row);
						i++;
						break;

					default:
						i++;
						break;
				}
			}

			return root;
		}


		private void CreateFormBox(UI.Panel root, List<FieldDescription> fields, int index, int count)
		{
			//	Crée tous les champs dans une boîte.

			//	Première passe pour déterminer quelles colonnes contiennent des labels.
			int column = 0, row = 0;
			bool[] isLabel = new bool[FormEngine.MaxColumnsRequired];
			for (int i=index; i<index+count; i++)
			{
				FieldDescription field = fields[i];

				this.SearchFieldLabel(field, isLabel, ref column);
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
			for (int i=index; i<index+count; i++)
			{
				FieldDescription field = fields[i];

				if (field.Type == FieldDescription.FieldType.Field)  // champ ?
				{
					string path = field.GetPath("Data");
					this.CreateField(root, grid, path, field, ref column, ref row);
				}
				else  // séparateur ?
				{
					FieldDescription next = FormEngine.SearchNextField(fields, i);  // cherche le prochain champ
					this.CreateSeparator(root, grid, field, next, ref column, ref row, ref lastTitle);
				}
			}
		}


		private void SearchFieldLabel(FieldDescription field, bool[] isLabel, ref int column)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
			if (field.Type == FieldDescription.FieldType.Field)
			{
				int columnsRequired = System.Math.Min(field.ColumnsRequired, FormEngine.MaxColumnsRequired-1);

				if (column+1+columnsRequired > FormEngine.MaxColumnsRequired)  // dépasse à droite ?
				{
					column = 0;
				}

				isLabel[column] = true;

				if (field.Separator == FieldDescription.SeparatorType.Append)
				{
					column += 1+columnsRequired;
				}
				else
				{
					column = 0;
				}
			}
		}

		private void CreateSeparator(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, FieldDescription nextField, ref int column, ref int row, ref List<Druid> lastTitle)
		{
			//	Crée les widgets pour un séparateur dans la grille, lors de la deuxième passe.
			FieldDescription.FieldType type = field.Type;

			if (nextField == null)
			{
				type = FieldDescription.FieldType.Line;
			}

			if (type == FieldDescription.FieldType.Title)
			{
				List<Druid> druids = nextField.FieldIds;
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
					type = FieldDescription.FieldType.Line;  // il faudra mettre une simple ligne
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

			if (type == FieldDescription.FieldType.Line ||
				type == FieldDescription.FieldType.Title)
			{
				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
				grid.RowDefinitions[row].TopBorder = (type == FieldDescription.FieldType.Title) ? 0 : 10;
				grid.RowDefinitions[row].BottomBorder = 10;

				Separator sep = new Separator(root);
				sep.PreferredHeight = 1;

				Widgets.Layouts.GridLayoutEngine.SetColumn(sep, 0);
				Widgets.Layouts.GridLayoutEngine.SetRow(sep, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(sep, 1+FormEngine.MaxColumnsRequired);

				row++;
			}
		}

		private void CreateField(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string path, FieldDescription field, ref int column, ref int row)
		{
			//	Crée les widgets pour un champ dans la grille, lors de la deuxième passe.
			UI.Placeholder placeholder = new Epsitec.Common.UI.Placeholder(root);
			placeholder.SetBinding(UI.Placeholder.ValueProperty, new Binding(BindingMode.TwoWay, path));
			placeholder.BackColor = field.BackColor;
			placeholder.TabIndex = grid.RowDefinitions.Count;

			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			double m = 2;
			switch (field.Separator)
			{
				case FieldDescription.SeparatorType.Compact:
					m = -1;
					break;

				case FieldDescription.SeparatorType.Extend:
					m = 10;
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

			if (field.Separator == FieldDescription.SeparatorType.Append)
			{
				column += 1+columnsRequired;
			}
			else
			{
				row++;
				column = 0;
			}
		}

		static private FieldDescription SearchNextField(List<FieldDescription> fields, int index)
		{
			//	Cherche la prochaine description de champ (pas de séparateur).
			for (int i=index+1; i<fields.Count; i++)
			{
				if (fields[i].Type == FieldDescription.FieldType.Field)
				{
					return fields[i];
				}
			}

			return null;
		}


		public static readonly int MaxColumnsRequired = 10;
		public static readonly int MaxRowsRequired = 20;

		ResourceManager resourceManager;
	}
}
