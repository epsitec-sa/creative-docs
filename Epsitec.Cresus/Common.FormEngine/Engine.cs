using System.Collections.Generic;

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
	public delegate FormDescription FormDescriptionFinder(Druid id);


	/// <summary>
	/// Générateur de masques de saisie.
	/// </summary>
	public sealed class Engine
	{
		public Engine(IFormResourceProvider resourceProvider)
		{
			//	Constructeur.
			//	FindFormDescription permet de retrouver le FormDescription correspondant à un Druid,
			//	lorsque les ressources ne sont pas sérialisées. Pour un usage hors de Designer, avec
			//	des ressources sérialisées, ce paramètre peut être null.
			this.resourceProvider = resourceProvider;

			this.arrange = new Arrange(this.resourceProvider);
			this.entityContext = new EntityContext(this.resourceProvider, EntityLoopHandlingMode.Skip);
		}

		public Arrange Arrange
		{
			get
			{
				return this.arrange;
			}
		}

		public UI.Panel CreateForm(FormDescription form)
		{
			return this.CreateForm(form, false);
		}

		public UI.Panel CreateForm(FormDescription form, bool forDesigner)
		{
			//	Crée un masque de saisie pour une entité donnée.
			//	La liste de FieldDescription doit être plate (pas de Node).
			this.forDesigner = forDesigner;
			this.resourceProvider.ClearCache();

			List<FieldDescription> fields0 = form.Fields;

			if (form.IsPatch)
			{
				string xml = this.resourceProvider.GetFormXmlSource(form.FormIdToPatch);
				if (!string.IsNullOrEmpty(xml))
				{
					FormDescription refForm = Serialization.DeserializeForm(xml);
					fields0 = this.arrange.Merge(refForm.Fields, fields0);
				}
			}

			string err = this.arrange.Check(fields0);
			if (err != null)
			{
				UI.Panel container = new UI.Panel();

				StaticText warning = new StaticText(container);
				warning.Text = string.Concat("<i>", err, "</i>");
				warning.ContentAlignment = ContentAlignment.MiddleCenter;
				warning.Dock = DockStyle.Fill;

				return container;
			}

			List<FieldDescription> fields1 = this.arrange.DevelopSubForm(fields0);
			List<FieldDescription> fields2 = this.arrange.Organize(fields1);

			if (this.GetEntityDefinition(form.EntityId) == null)
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
			root.ResourceManager = this.resourceProvider.ResourceManager;
			root.DataSource = new UI.DataSource();
			root.DataSource.AddDataSource(UI.DataSource.DataName, entityData);

			this.CreateFormBox(root, form.EntityId, fields2, 0);

			return root;
		}

		private StructuredType GetEntityDefinition(Druid entityId)
		{
			//	Trouve la définition de l'entité spécifiée par son id.
			return this.resourceProvider.GetStructuredType(entityId);
		}

		private enum FieldEditionMode
		{
			Unknown,
			Data,							//	le champ contient des données
			Search							//	le champ sert à réaliser des recherches
		}

		private FieldEditionMode GetFieldEditionMode(Druid entityId, IList<Druid> fieldIds)
		{
			//	Détermine comment un champ doit être traité. Il peut soit être
			//	considéré comme une donnée, soit comme un critère de recherche.
			foreach (Druid fieldId in fieldIds)
			{
				StructuredType entityDef = this.GetEntityDefinition(entityId);
				if (entityDef == null)
				{
					return FieldEditionMode.Unknown;
				}

				StructuredTypeField fieldDef = entityDef.GetField(fieldId.ToString());
				if (fieldDef == null)
				{
					return FieldEditionMode.Unknown;
				}

				if (fieldDef.Relation == FieldRelation.None)
				{
					return FieldEditionMode.Data;
				}

				if (fieldDef.IsSharedRelation)
				{
					return FieldEditionMode.Search;
				}
				
				entityId = fieldDef.TypeId;
			}

			return FieldEditionMode.Data;
		}

		private void CreateFormBox(UI.Panel root, Druid entityId, List<FieldDescription> fields, int index)
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

				if (field.PatchHidden)
				{
					continue;
				}

				bool isGlueAfter = false;
				FieldDescription nextField = Engine.SearchNextElement(fields, i);
				if (nextField != null && nextField.Type == FieldDescription.FieldType.Glue)
				{
					isGlueAfter = true;
				}

				if (field.Type == FieldDescription.FieldType.BoxBegin ||  // début de boîte ?
					field.Type == FieldDescription.FieldType.SubForm)
				{
					if (level == 0)
					{
						this.PreprocessBoxBegin(field, labelsId, ref labelId, ref column, isGlueAfter);
					}

					level++;
				}
				else if (field.Type == FieldDescription.FieldType.BoxEnd)  // fin de boîte ?
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
				else if (field.Type == FieldDescription.FieldType.Glue)  // colle ?
				{
					if (level == 0)
					{
						this.PreprocessGlue(field, labelsId, ref labelId, ref column, isGlueAfter);
					}
				}
				else if (field.Type == FieldDescription.FieldType.Node)
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

			if (grid.ColumnDefinitions.Count != 0)
			{
				grid.ColumnDefinitions[0].RightBorder = 1;
			}

			Widgets.Layouts.LayoutEngine.SetLayoutEngine(root, grid);

			//	Deuxième passe pour générer le contenu.
			column = 0;
			row = 0;
			level = 0;
			List<Druid> lastTitle = null;
			for (int i=index; i<fields.Count; i++)
			{
				FieldDescription field = fields[i];

				if (field.PatchHidden)
				{
					continue;
				}

				bool isGlueAfter = false;
				FieldDescription nextField = Engine.SearchNextElement(fields, i);
				if (nextField != null && nextField.Type == FieldDescription.FieldType.Glue)
				{
					isGlueAfter = true;
				}

				//	Assigne l'identificateur unique, qui ira dans la propriété Index des widgets.
				//	La valeur -1 par défaut indique un widget non identifié.
				System.Guid guid;
				if (field.Source == null)
				{
					guid = field.Guid;
				}
				else
				{
					//	Un champ d'un sous-masque reçoit l'identificateur du SubForm qui l'a initié,
					//	afin que sa sélection dans l'éditeur sélectionne le SubForm dans la liste.
					guid = field.Source.Guid;
				}

				if (field.Type == FieldDescription.FieldType.BoxBegin ||  // début de boîte ?
					field.Type == FieldDescription.FieldType.SubForm)
				{
					if (level == 0)
					{
						UI.Panel box = this.CreateBox(root, grid, field, guid, labelsId, ref column, ref row, isGlueAfter);
						this.CreateFormBox(box, entityId, fields, i+1);
					}

					level++;
				}
				else if (field.Type == FieldDescription.FieldType.BoxEnd)  // fin de boîte ?
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
						this.CreateField(root, entityId, grid, field, guid, labelsId, ref column, ref row, isGlueAfter);
					}
				}
				else if (field.Type == FieldDescription.FieldType.Glue)  // colle ?
				{
					if (level == 0)
					{
						this.CreateGlue(root, grid, field, guid, labelsId, ref column, ref row, isGlueAfter);
					}
				}
				else if (field.Type == FieldDescription.FieldType.Title ||
						 field.Type == FieldDescription.FieldType.Line)  // séparateur ?
				{
					if (level == 0)
					{
						FieldDescription next = Engine.SearchNextField(fields, i);  // cherche le prochain champ
						this.CreateSeparator(root, grid, field, guid, next, labelsId, ref column, ref row, isGlueAfter, ref lastTitle);
					}
				}
			}
		}


		private void PreprocessBoxBegin(FieldDescription field, List<int> labelsId, ref int labelId, ref int column, bool isGlueAfter)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
			//	Un BoxBegin ne contient jamais de label, mais il faut tout de même faire évoluer
			//	le numéro de la colonne.
			int columnsRequired = System.Math.Max(field.ColumnsRequired, 1);

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
			int columnsRequired = System.Math.Max(field.ColumnsRequired, 1);

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

		private void PreprocessGlue(FieldDescription field, List<int> labelsId, ref int labelId, ref int column, bool isGlueAfter)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
			int columnsRequired = field.ColumnsRequired;

			for (int i=0; i<columnsRequired; i++)
			{
				Engine.LabelIdUse(labelsId, labelId++, column+i, 1);
			}

			column += columnsRequired;
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
				if (i >= labelsId.Count)
				{
					break;
				}

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


		private UI.Panel CreateBox(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, List<int> labelsId, ref int column, ref int row, bool isGlueAfter)
		{
			//	Crée les widgets pour une boîte dans la grille, lors de la deuxième passe.
			UI.Panel box = new UI.Panel(root);
			box.DrawFrameState = FrameState.All;
			box.Padding = FieldDescription.GetRealBoxPadding(field.BoxPadding);
			box.BackColor = FieldDescription.GetRealColor(field.BackColor);
			box.DrawFrameState = field.BoxFrameState;
			box.DrawFrameWidth = field.BoxFrameWidth;
			box.Name = guid.ToString();
			
			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			int columnsRequired = System.Math.Max(field.ColumnsRequired, 1);

			grid.RowDefinitions[row].BottomBorder = FieldDescription.GetRealSeparator(field.SeparatorBottom);

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

		private void CreateField(UI.Panel root, Druid entityId, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, List<int> labelsId, ref int column, ref int row, bool isGlueAfter)
		{
			//	Crée les widgets pour un champ dans la grille, lors de la deuxième passe.
			UI.Placeholder placeholder = new UI.Placeholder(root);
			placeholder.SetBinding(UI.Placeholder.ValueProperty, new Binding(BindingMode.TwoWay, field.GetPath(UI.DataSource.DataName)));
			placeholder.BackColor = FieldDescription.GetRealColor(field.BackColor);
			placeholder.TabIndex = grid.RowDefinitions.Count;
			placeholder.Name = guid.ToString();

			//	Détermine si le placeholder doit être utilisé pour saisir du texte ou pour
			//	saisir un critère de recherche et le configure en conséquence.
			FieldEditionMode editionMode = this.GetFieldEditionMode(entityId, field.FieldIds);
			switch (editionMode)
			{
				case FieldEditionMode.Data:
					placeholder.SuggestionMode = Epsitec.Common.UI.PlaceholderSuggestionMode.None;
					break;
				case FieldEditionMode.Search:
					placeholder.SuggestionMode = Epsitec.Common.UI.PlaceholderSuggestionMode.DisplayHint;
					break;
				default:
					throw new System.InvalidOperationException(string.Format("Invalid edition mode {0}", editionMode));
			}

			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			int columnsRequired = System.Math.Max(field.ColumnsRequired, 1);

			if (columnsRequired == 1)  // tout sur une seule colonne ?
			{
				placeholder.Controller = "*";
				placeholder.ControllerParameters = UI.Controllers.AbstractController.NoLabelsParameter;  // cache le label
			}

			grid.RowDefinitions[row].BottomBorder = FieldDescription.GetRealSeparator(field.SeparatorBottom);

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

		private void CreateGlue(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, List<int> labelsId, ref int column, ref int row, bool isGlueAfter)
		{
			//	Crée les widgets pour un collage dans la grille, lors de la deuxième passe.
			int columnsRequired = field.ColumnsRequired;

			if (this.forDesigner)
			{
				FrameBox glue = new FrameBox(root);
				glue.BackColor = FieldDescription.GetRealColor(field.BackColor);
				glue.Name = guid.ToString();

				if (columnsRequired == 0)
				{
					glue.Index = Engine.GlueNull;  // pour feinter les dimensions lors des détections et du dessin de la sélection
					glue.PreferredWidth = 0; // pour ne pas perturber le calcul de la largeur d'une colonne contenant un label

					int i = Engine.GetColumnIndex(labelsId, column);
					Widgets.Layouts.GridLayoutEngine.SetColumn(glue, i);
					Widgets.Layouts.GridLayoutEngine.SetRow(glue, row);
				}
				else
				{
					grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

					int i = Engine.GetColumnIndex(labelsId, column);
					int j = Engine.GetColumnIndex(labelsId, column+columnsRequired-1)+1;
					Widgets.Layouts.GridLayoutEngine.SetColumn(glue, i);
					Widgets.Layouts.GridLayoutEngine.SetRow(glue, row);
					Widgets.Layouts.GridLayoutEngine.SetColumnSpan(glue, j-i);
				}
			}

			column += columnsRequired;
		}

		private void CreateSeparator(UI.Panel root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, FieldDescription nextField, List<int> labelsId, ref int column, ref int row, bool isGlueAfter, ref List<Druid> lastTitle)
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

					builder.Append(this.GetCaptionDefaultLabel(druid));
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
					text.Name = guid.ToString();

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
				sep.Name = guid.ToString();

				int i = Engine.GetColumnIndex(labelsId, 0);
				int j = Engine.GetColumnIndex(labelsId, labelsId.Count-1)+1;
				Widgets.Layouts.GridLayoutEngine.SetColumn(sep, i);
				Widgets.Layouts.GridLayoutEngine.SetRow(sep, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(sep, j-i);

				row++;
			}
		}

		private string GetCaptionDefaultLabel(Druid druid)
		{
			Caption caption = this.resourceProvider.GetCaption(druid);
			return caption == null ? "" : caption.DefaultLabel;
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

		static private FieldDescription SearchNextElement(List<FieldDescription> fields, int index)
		{
			//	Cherche le prochain élément.
			if (fields[index].Type == FieldDescription.FieldType.BoxBegin ||
				fields[index].Type == FieldDescription.FieldType.SubForm)
			{
				int level = 0;

				for (int i=index; i<fields.Count; i++)
				{
					if (fields[i].Type == FieldDescription.FieldType.BoxBegin ||
						fields[i].Type == FieldDescription.FieldType.SubForm)
					{
						level++;
					}
					else if (fields[i].Type == FieldDescription.FieldType.BoxEnd)
					{
						level--;

						if (level == 0)
						{
							index = i;
							break;
						}
					}
				}
			}

			index++;
			if (index < fields.Count)
			{
				return fields[index];
			}
			else
			{
				return null;
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
		public static readonly int GlueNull = 1;

		private readonly IFormResourceProvider resourceProvider;
		private readonly EntityContext entityContext;
		private Arrange arrange;
		private bool forDesigner;
	}
}
