using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	/// <summary>
	/// Générateur de masques de saisie.
	/// </summary>
	public class Engine
	{
		public Engine(ResourceManager resourceManager)
		{
			//	Constructeur.
			this.resourceManager = resourceManager;
			this.entityContext = new EntityContext (this.resourceManager, EntityLoopHandlingMode.Skip);
		}

		public UI.Panel CreateForm(FormDescription form)
		{
			//	Crée un masque de saisie pour une entité donnée.
			//	La liste de FieldDescription doit être plate (pas de Node).
			string err = Arrange.Check(form.Fields);
			if (err != null)
			{
				UI.Panel container = new UI.Panel();

				StaticText warning = new StaticText(container);
				warning.Text = string.Concat("<i>", err, "</i>");
				warning.ContentAlignment = ContentAlignment.MiddleCenter;
				warning.Dock = DockStyle.Fill;

				return container;
			}

			Caption entityCaption = this.resourceManager.GetCaption(form.EntityId);
			StructuredType entity = TypeRosetta.GetTypeObject(entityCaption) as StructuredType;
			if (entity == null)
			{
				return null;
			}

			AbstractEntity entityData = null;

			EntityContext.Push(this.entityContext);
			
			try
			{
				entityData = entityContext.CreateEntity(form.EntityId);
			}
			finally
			{
				EntityContext.Pop();
			}

			//	Crée le panneau racine, le seul à définir DataSource. Les autres panneaux
			//	enfants héritent de cette propriété.
			UI.Panel root = new UI.Panel();
			root.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			root.ResourceManager = this.resourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource("Data", entityData);

			this.CreateFormBox(root, form.Fields, 0);

			return root;
		}


		private void CreateFormBox(UI.Panel root, List<FieldDescription> fields, int index)
		{
			//	Crée tous les champs dans une boîte.
			//	Cette méthode est appelée récursivement pour chaque BoxBegin/BoxEnd.

			//	Première passe pour déterminer quelles colonnes contiennent des labels.
			int column = 0, row = 0;
			int level = 0;
			List<int> labelsId = new List<int>();
			int labelId = 1;
			for (int i=index; i<fields.Count; i++)
			{
				FieldDescription field = fields[i];

				bool isGlueAfter = false;
				if (i <fields.Count-1)
				{
					if (fields[i+1].Type == FieldDescription.FieldType.Glue)
					{
						isGlueAfter = true;
					}
				}

				//	Assigne l'identificateur unique, qui ira dans la propriété Index des widgets.
				//	La valeur -1 par défaut indique un widget non identifié.
				field.UniqueId = i;

				if (field.Type == FieldDescription.FieldType.BoxBegin)
				{
					if (level == 0)
					{
						this.PreprocessBoxBegin(field, labelsId, ref labelId, ref column, isGlueAfter);
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
						this.PreprocessField(field, labelsId, ref labelId, ref column, isGlueAfter);
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
			int lastLabelId = int.MinValue;
			for (int i=0; i<labelsId.Count; i++)
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
						for (int j=i; j<labelsId.Count; j++)
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

				bool isGlueAfter = false;
				if (i <fields.Count-1)
				{
					if (fields[i+1].Type == FieldDescription.FieldType.Glue)
					{
						isGlueAfter = true;
					}
				}

				if (field.Type == FieldDescription.FieldType.BoxBegin)
				{
					if (level == 0)
					{
						UI.Panel box = this.CreateBox(root, grid, field, labelsId, ref column, ref row, isGlueAfter);
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
						this.CreateField(root, grid, path, field, labelsId, ref column, ref row, isGlueAfter);
					}
				}
				else if (field.Type == FieldDescription.FieldType.Title ||
						 field.Type == FieldDescription.FieldType.Line  )  // séparateur ?
				{
					if (level == 0)
					{
						FieldDescription next = Engine.SearchNextField(fields, i);  // cherche le prochain champ
						this.CreateSeparator(root, grid, field, next, labelsId, ref column, ref row, isGlueAfter, ref lastTitle);
					}
				}
			}
		}


		private void PreprocessBoxBegin(FieldDescription field, List<int> labelsId, ref int labelId, ref int column, bool isGlueAfter)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
			//	Un BoxBegin ne contient jamais de label, mais il faut tout de même faire évoluer
			//	le numéro de la colonne.
			int columnsRequired = field.ColumnsRequired;

			Engine.LabelIdUse(labelsId, labelId++, column, columnsRequired);

			if (isGlueAfter)
			{
				column += columnsRequired;
			}
			else
			{
				column = 0;
			}
		}

		private void PreprocessField(FieldDescription field, List<int> labelsId, ref int labelId, ref int column, bool isGlueAfter)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
			int columnsRequired = field.ColumnsRequired;

			Engine.LabelIdUse(labelsId, -(labelId++), column, 1);
			Engine.LabelIdUse(labelsId, labelId++, column+1, columnsRequired-1);

			if (isGlueAfter)
			{
				column += columnsRequired;
			}
			else
			{
				column = 0;
			}
		}

		static private void LabelIdUse(List<int> labelsId, int labelId, int column, int count)
		{
			//	Indique que les colonnes comprises entre column et column+count-1 ont un contenu commun,
			//	c'est-à-dire qui ne nécessite qu'une colonne physique dans GridLayoutEngine, si cela est
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
			count = System.Math.Max(count, 1);

			int n = (column+count)-labelsId.Count;
			for (int i=0; i<n; i++)
			{
				labelsId.Add(0);
			}

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

					if (last < labelsId.Count-1 && labelsId[last+1] == labelsId[last])
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

		static private int GetColumnIndex(List<int> labelsId, int column)
		{
			//	Conversion d'un numéro de colonne virtuelle (0..9) en un index pour une colonne physique.
			//	Les colonnes physiques peuvent être moins nombreuses que les virtuelles.
			int index = column;
			int last = int.MinValue;
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


		private UI.Panel CreateBox(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, List<int> labelsId, ref int column, ref int row, bool isGlueAfter)
		{
			//	Crée les widgets pour une boîte dans la grille, lors de la deuxième passe.
			UI.Panel box = new UI.Panel(root);
			box.ContainerLayoutMode = field.ContainerLayoutMode;
			box.DrawFrameState = FrameState.All;
			box.Margins = field.ContainerMargins;
			box.Padding = field.ContainerPadding;
			box.BackColor = FieldDescription.GetRealColor(field.BackColor);
			box.DrawFrameState = field.ContainerFrameState;
			box.DrawFrameWidth = field.ContainerFrameWidth;
			box.Index = field.UniqueId;
			
			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			int columnsRequired = field.ColumnsRequired;

			int i = Engine.GetColumnIndex(labelsId, column);
			int j = Engine.GetColumnIndex(labelsId, column+columnsRequired-1)+1;
			Widgets.Layouts.GridLayoutEngine.SetColumn(box, i);
			Widgets.Layouts.GridLayoutEngine.SetRow(box, row);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan(box, j-i);

			if (isGlueAfter)
			{
				column += columnsRequired;
			}
			else
			{
				row++;
				column = 0;
			}

			return box;
		}

		private void CreateField(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, string path, FieldDescription field, List<int> labelsId, ref int column, ref int row, bool isGlueAfter)
		{
			//	Crée les widgets pour un champ dans la grille, lors de la deuxième passe.
			UI.Placeholder placeholder = new UI.Placeholder(root);
			placeholder.SetBinding(UI.Placeholder.ValueProperty, new Binding(BindingMode.TwoWay, path));
			placeholder.BackColor = FieldDescription.GetRealColor(field.BackColor);
			placeholder.TabIndex = grid.RowDefinitions.Count;
			placeholder.Index = field.UniqueId;

			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			int columnsRequired = field.ColumnsRequired;

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

			if (field.RowsRequired > 1)
			{
				placeholder.PreferredHeight = field.RowsRequired*20;
			}

			int i = Engine.GetColumnIndex(labelsId, column);
			int j = Engine.GetColumnIndex(labelsId, column+columnsRequired-1)+1;
			Widgets.Layouts.GridLayoutEngine.SetColumn(placeholder, i);
			Widgets.Layouts.GridLayoutEngine.SetRow(placeholder, row);
			Widgets.Layouts.GridLayoutEngine.SetColumnSpan(placeholder, j-i);

			if (isGlueAfter)
			{
				column += columnsRequired;
			}
			else
			{
				row++;
				column = 0;
			}
		}

		private void CreateSeparator(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, FieldDescription nextField, List<int> labelsId, ref int column, ref int row, bool isGlueAfter, ref List<Druid> lastTitle)
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
					text.Index = field.UniqueId;

					int i = Engine.GetColumnIndex(labelsId, 0);
					int j = Engine.GetColumnIndex(labelsId, labelsId.Count-1)+1;
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
				sep.Index = field.UniqueId;

				int i = Engine.GetColumnIndex(labelsId, 0);
				int j = Engine.GetColumnIndex(labelsId, labelsId.Count-1)+1;
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

		protected readonly ResourceManager resourceManager;
		protected readonly EntityContext entityContext;
	}
}
