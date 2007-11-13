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

		public UI.Panel CreateForm(Druid entityId, List<FieldDescription> fields)
		{
			//	Crée un masque de saisie pour une entité donnée.
			//	La liste de FieldDescription doit être plate (pas de Node).
			Caption entityCaption = this.resourceManager.GetCaption(entityId);
			StructuredType entity = TypeRosetta.GetTypeObject(entityCaption) as StructuredType;

			StructuredData entityData = new StructuredData(entity);
			entityData.UndefinedValueMode = UndefinedValueMode.Default;

			//	Crée le panneau racine, le seul à définir DataSource. Les autres panneaux
			//	enfants héritent de cette propriété.
			UI.Panel root = new UI.Panel();
			root.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource("Data", entityData);

			this.CreateFormBox(root, fields, 0);

			return root;
		}


		private void CreateFormBox(UI.Panel root, List<FieldDescription> fields, int index)
		{
			//	Crée tous les champs dans une boîte.
			//	Cette méthode est appelée récursivement pour chaque BoxBegin/BoxEnd.

			//	Première passe pour déterminer quelles colonnes contiennent des labels.
			int column = 0, row = 0;
			int level = 0;
			int[] labelsId = new int[FormEngine.MaxColumnsRequired];
			int labelId = 1;
			for (int i=index; i<fields.Count; i++)
			{
				FieldDescription field = fields[i];

				if (field.Type == FieldDescription.FieldType.BoxBegin)
				{
					if (level == 0)
					{
						this.PreprocessBoxBegin(field, labelsId, ref labelId, ref column);
					}

					level++;
				}
				else if (field.Type == FieldDescription.FieldType.BoxEnd)
				{
					level--;

					if (level < 0)
					{
						break;
					}
				}
				else if (field.Type == FieldDescription.FieldType.Field)  // champ ?
				{
					if (level == 0)
					{
						this.PreprocessField(field, labelsId, ref labelId, ref column);
					}
				}
				else if (field.Type == FieldDescription.FieldType.Node ||
						 field.Type == FieldDescription.FieldType.Hide)
				{
					throw new System.InvalidOperationException("Type incorrect (la liste de FieldDescription devrait être aplatie).");
				}
			}

			//	Crée les différentes colonnes, en fonction des résultats de la première passe.
			Widgets.Layouts.GridLayoutEngine grid = new Widgets.Layouts.GridLayoutEngine();
			int lastLabelId = -1;
			for (int i=0; i<FormEngine.MaxColumnsRequired; i++)
			{
				if (lastLabelId != labelsId[i])
				{
					lastLabelId = labelsId[i];

					if (labelsId[i] < 0)  // est-ce que cette colonne contient un label ?
					{
						//	Largeur automatique selon la taille minimale du contenu.
						grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition());
						System.Console.WriteLine(string.Format("Column {0}: automatic", i));
					}
					else
					{
						//	Largeur de 10%, 10 pixels au minimum, pas de maximum (par colonne virtuelle).
						double relWidth = 0;
						double minWidth = 0;
						for (int j=i; j<FormEngine.MaxColumnsRequired; j++)
						{
							if (lastLabelId == labelsId[j])
							{
								relWidth += 10;  // largeur relative en %
								minWidth += 10;  // largeur minimale
							}
							else
							{
								break;
							}
						}

						grid.ColumnDefinitions.Add(new Widgets.Layouts.ColumnDefinition(new Widgets.Layouts.GridLength(relWidth, Widgets.Layouts.GridUnitType.Proportional), minWidth, double.PositiveInfinity));
						System.Console.WriteLine(string.Format("Column {0}: relWidth={1}%", i, relWidth));
					}
				}
			}
			System.Console.WriteLine(string.Format("GridLayoutEngine with {0} columns", grid.ColumnDefinitions.Count));
			grid.ColumnDefinitions[0].RightBorder = 1;

			Widgets.Layouts.LayoutEngine.SetLayoutEngine(root, grid);

			//	Deuxième passe pour générer le contenu.
			column = 0;
			row = 0;
			level = 0;
			List<Druid> lastTitle = null;
			for (int i=index; i<fields.Count; i++)
			{
				FieldDescription field = fields[i];

				if (field.Type == FieldDescription.FieldType.BoxBegin)
				{
					if (level == 0)
					{
						UI.Panel box = this.CreateBox(root, grid, field, labelsId, ref column, ref row);
						this.CreateFormBox(box, fields, i+1);
					}

					level++;
				}
				else if (field.Type == FieldDescription.FieldType.BoxEnd)
				{
					level--;

					if (level < 0)
					{
						break;
					}
				}
				else if (field.Type == FieldDescription.FieldType.Field)  // champ ?
				{
					if (level == 0)
					{
						string path = field.GetPath("Data");
						this.CreateField(root, grid, path, field, labelsId, ref column, ref row);
					}
				}
				else if (field.Type == FieldDescription.FieldType.Title ||
						 field.Type == FieldDescription.FieldType.Line  )  // séparateur ?
				{
					if (level == 0)
					{
						FieldDescription next = FormEngine.SearchNextField(fields, i);  // cherche le prochain champ
						this.CreateSeparator(root, grid, field, next, labelsId, ref column, ref row, ref lastTitle);
					}
				}
			}
		}


		private void PreprocessBoxBegin(FieldDescription field, int[] labelsId, ref int labelId, ref int column)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
			//	Un BoxBegin ne contient jamais de label, mais il faut tout de même faire évoluer
			//	le numéro de la colonne.
			int columnsRequired = System.Math.Min(field.ColumnsRequired, FormEngine.MaxColumnsRequired);

			if (column+columnsRequired > FormEngine.MaxColumnsRequired)  // dépasse à droite ?
			{
				column = 0;
			}

			FormEngine.LabelIdUse(labelsId, labelId++, column, columnsRequired);

			column += columnsRequired;
		}

		private void PreprocessField(FieldDescription field, int[] labelsId, ref int labelId, ref int column)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
			int columnsRequired = System.Math.Min(field.ColumnsRequired, FormEngine.MaxColumnsRequired);

			if (column+columnsRequired > FormEngine.MaxColumnsRequired)  // dépasse à droite ?
			{
				column = 0;
			}

			FormEngine.LabelIdUse(labelsId, -(labelId++), column, 1);
			FormEngine.LabelIdUse(labelsId, labelId++, column+1, columnsRequired-1);

			if (field.Separator == FieldDescription.SeparatorType.Append)
			{
				column += columnsRequired;
			}
			else
			{
				column = 0;
			}
		}

		static private void LabelIdUse(int[] labelsId, int labelId, int column, int count)
		{
			//	Indique que les colonnes comprises entre column et column+count-1 ont un contenu commun,
			//	c'est-à-dire qui ne nécessique qu'une colonne physique dans GridLayoutEngine, si cela est
			//	en accord avec les autres lignes.
			//
			//	Contenu initial:				0 0 0 0 0 0 0 0 0 0
			//	labelId=1, column=0, count=1:	1 0 0 0 0 0 0 0 0 0  (cas I)
			//	labelId=2, column=1, count=9:	1 2 2 2 2 2 2 2 2 2  (cas I)
			//	labelId=3, column=1, count=2:	1 3 3 2 2 2 2 2 2 2  (cas I)
			//	labelId=5, column=1, count=5:	1 3 3 5 5 5 2 2 2 2  (cas R)
			//	labelId=6, column=1, count=2:	1 6 6 4 4 4 2 2 2 2  (cas I)
			//	labelId=7, column=3, count=1:	1 6 6 7 4 4 2 2 2 2  (cas I)
			//	labelId=8, column=4, count=6:	1 6 6 7 4 4 2 2 2 2  (cas N)
			//
			//	Après cette initialisation, il faudra créer 5 colonnes physiques:
			//	1) 1
			//	2) 6 6
			//	3) 7
			//	4) 4 4
			//	5) 2 2 2 2
			int last = column+count-1;
			int id = labelsId[column];
			for (int i=column+1; i<=last; i++)
			{
				if (labelsId[i] != id)
				{
					if (column > 0 && labelsId[column-1] == labelsId[column])
					{
						//	Cas L:
						int m = labelsId[column];
						for (int j=column; j<=last; j++)
						{
							if (labelsId[j] != m)
							{
								break;
							}
							labelsId[j] = labelId;
						}
					}

					if (last < labelsId.Length-1 && labelsId[last+1] == labelsId[last])
					{
						//	Cas R:
						int m = labelsId[last];
						for (int j=last; j>=column; j--)
						{
							if (labelsId[j] != m)
							{
								break;
							}
							labelsId[j] = labelId;
						}
					}

					//	Cas N:
					return;
				}
			}

			//	Cas I:
			for (int i=column; i<=last; i++)
			{
				labelsId[i] = labelId;
			}
		}

		static private int GetColumnIndex(int[] labelsId, int column)
		{
			//	Conversion d'un numéro de colonne virtuelle (0..9) en un index pour une colonne physique.
			//	Les colonnes physiques peuvent être moins nombreuses que les virtuelles.
			int index = column;
			int last = -1;
			for (int i=0; i<=column; i++)
			{
				if (last == labelsId[i])
				{
					index--;
				}
				else
				{
					last = labelsId[i];
				}
			}

			return index;
		}


		private UI.Panel CreateBox(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, int[] labelsId, ref int column, ref int row)
		{
			//	Crée les widgets pour une boîte dans la grille, lors de la deuxième passe.
			UI.Panel box = new UI.Panel(root);
			box.ContainerLayoutMode = field.ContainerLayoutMode;
			box.DrawFrameState = FrameState.All;
			box.Margins = field.ContainerMargins;
			box.Padding = field.ContainerPadding;
			box.BackColor = field.ContainerBackColor;
			box.DrawFrameState = field.ContainerFrameState;
			box.DrawFrameWidth = field.ContainerFrameWidth;
			
			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			int columnsRequired = System.Math.Min(field.ColumnsRequired, FormEngine.MaxColumnsRequired);

			if (column+columnsRequired > FormEngine.MaxColumnsRequired)  // dépasse à droite ?
			{
				row++;
				column = 0;
			}

			int i = FormEngine.GetColumnIndex(labelsId, column);
			int j = FormEngine.GetColumnIndex(labelsId, column+columnsRequired-1)+1;
			Widgets.Layouts.GridLayoutEngine.SetColumn(box, i);
			Widgets.Layouts.GridLayoutEngine.SetRow(box, row);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan(box, j-i);

			column += columnsRequired;

			if (column >= FormEngine.MaxColumnsRequired)  // bord droite atteint ?
			{
				row++;
				column = 0;
			}

			return box;
		}

		private void CreateField(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string path, FieldDescription field, int[] labelsId, ref int column, ref int row)
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

			int columnsRequired = System.Math.Min(field.ColumnsRequired, FormEngine.MaxColumnsRequired);

			if (column+columnsRequired > FormEngine.MaxColumnsRequired)  // dépasse à droite ?
			{
				row++;
				column = 0;
			}

			if (field.RowsRequired > 1)
			{
				placeholder.PreferredHeight = field.RowsRequired*20;
			}

			int i = FormEngine.GetColumnIndex(labelsId, column);
			int j = FormEngine.GetColumnIndex(labelsId, column+columnsRequired-1)+1;
			Widgets.Layouts.GridLayoutEngine.SetColumn(placeholder, i);
			Widgets.Layouts.GridLayoutEngine.SetRow(placeholder, row);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan(placeholder, j-i);

			if (field.Separator == FieldDescription.SeparatorType.Append)
			{
				column += columnsRequired;
			}
			else
			{
				row++;
				column = 0;
			}
		}

		private void CreateSeparator(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, FieldDescription nextField, int[] labelsId, ref int column, ref int row, ref List<Druid> lastTitle)
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

					int i = FormEngine.GetColumnIndex(labelsId, 0);
					int j = FormEngine.GetColumnIndex(labelsId, FormEngine.MaxColumnsRequired-1)+1;
					Widgets.Layouts.GridLayoutEngine.SetColumn(text, i);
					Widgets.Layouts.GridLayoutEngine.SetRow(text, row);
					Widgets.Layouts.GridLayoutEngine.SetColumnSpan(text, j-i);

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

				int i = FormEngine.GetColumnIndex(labelsId, 0);
				int j = FormEngine.GetColumnIndex(labelsId, FormEngine.MaxColumnsRequired-1)+1;
				Widgets.Layouts.GridLayoutEngine.SetColumn(sep, i);
				Widgets.Layouts.GridLayoutEngine.SetRow(sep, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(sep, j-i);

				row++;
			}
		}


		static private int CountFields(List<FieldDescription> fields, int index)
		{
			//	Compte le nombre de descriptions de types champ, séparateur ou titre.
			int count = 0;

			for (int i=index; i<fields.Count; i++)
			{
				if (fields[i].Type == FieldDescription.FieldType.Field ||
					fields[i].Type == FieldDescription.FieldType.Line  ||
					fields[i].Type == FieldDescription.FieldType.Title )
				{
					count++;
				}
				else
				{
					break;
				}
			}

			return count;
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

		protected ResourceManager resourceManager;
	}
}
