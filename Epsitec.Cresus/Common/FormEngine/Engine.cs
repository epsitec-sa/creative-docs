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
			this.defaultMode = FieldEditionMode.Data;
		}

		public Arrange Arrange
		{
			get
			{
				return this.arrange;
			}
		}

		/// <summary>
		/// Gets or sets the data to which the engine should bind the user
		/// interface to.
		/// </summary>
		/// <value>The entity data.</value>
		public AbstractEntity Data
		{
			get
			{
				return this.entityData;
			}
			set
			{
				if (this.entityData == null)
				{
					this.entityData = value;
				}
				else
				{
					throw new System.InvalidOperationException ();
				}
			}
		}

		public void EnableSearchMode()
		{
			this.defaultMode = FieldEditionMode.Search;
		}

		public UI.Panel CreateForm(Druid formId, ref Size defaultSize)
		{
			//	Crée un masque de saisie.
			//	Si le Druid correspond à un Form delta, il est fusionné jusqu'au Form de base parent.
			//	Cette méthode est utilisée par une application finale pour construire un masque.
			string xml = this.resourceProvider.GetFormXmlSource(formId);
			
			if (string.IsNullOrEmpty(xml))
			{
				return null;
			}

			FormDescription formDescription = Serialization.DeserializeForm(xml);

			if (!double.IsNaN(formDescription.DefaultSize.Width))
			{
				defaultSize.Width = formDescription.DefaultSize.Width;
			}
			if (!double.IsNaN(formDescription.DefaultSize.Height))
			{
				defaultSize.Height = formDescription.DefaultSize.Height;
			}

			return this.CreateForm(formDescription);
		}

		public UI.Panel CreateForm(FormDescription formDescription)
		{
			if (formDescription == null)
			{
				return null;
			}

			List<FieldDescription> baseFields, finalFields;
			Druid entityId;
			this.arrange.Build(formDescription, null, out baseFields, out finalFields, out entityId);

			UI.Panel panel = this.CreateForm(finalFields, entityId, false);
			return panel;
		}

		public UI.Panel CreateForm(List<FieldDescription> fields, Druid entityId, bool forDesigner)
		{
			//	Crée un masque de saisie.
			//	La liste de FieldDescription doit être plate (pas de Node).
			//	Cette méthode est utilisée par Designer pour construire un masque.
			this.forDesigner = forDesigner;
			this.resourceProvider.ClearCache();

			string err = this.arrange.Check(fields);
			if (err != null)
			{
				UI.Panel container = new UI.Panel();

				StaticText warning = new StaticText(container);
				warning.Text = string.Concat("<i>", err, "</i>");
				warning.ContentAlignment = ContentAlignment.MiddleCenter;
				warning.Dock = DockStyle.Fill;

				return container;
			}

			List<FieldDescription> fields1 = this.arrange.DevelopSubForm(fields);
			List<FieldDescription> fields2 = this.arrange.Organize(fields1);

			if (this.GetEntityDefinition(entityId) == null)
			{
				return null;
			}

			AbstractEntity entityData = null;

			if (this.entityData == null)
			{
				//	Personne n'a défini de données à associer avec l'interface utilisateur
				//	que nous allons créer, alors on crée nous-même une entité vide pour
				//	permettre d'utiliser correctement le binding par la suite.

				EntityContext.Push (this.entityContext);

				try
				{
					entityData = this.entityContext.CreateEntity (entityId);
				}
				finally
				{
					EntityContext.Pop ();
				}
			}
			else
			{
				//	Utilise les données de l'entité fournies par l'appelant pour réaliser
				//	le binding par la suite.

				entityData = this.entityData;
			}

			//	Crée le panneau racine, le seul à définir DataSource. Les autres panneaux
			//	enfants héritent de cette propriété.
			UI.Panel root = new UI.Panel();
			root.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			root.CaptionResolver = this.resourceProvider;
			root.DataSource = new DataSource();
			root.DataSource.AddDataSource(DataSource.DataName, entityData);

			//	Crée un gestionnaire de styles pour le panneau dans son entier; un tel
			//	gestionnaire doit être attaché au panneau racine au moment de sa création
			UI.TextStyleManager textStyleManager = new UI.TextStyleManager(root);
			textStyleManager.Attach(root);  // active les styles pour le panneau spécifié et tous ses enfants

			this.tabIndex = 1;
			this.CreateFormBox(root, entityId, fields2, 0);
			this.GenerateForwardTab(root, fields2);

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
					return this.defaultMode;
				}

				if (fieldDef.IsSharedRelation)
				{
					return FieldEditionMode.Search;
				}
				
				entityId = fieldDef.TypeId;
			}

			return this.defaultMode;
		}

		private void CreateFormBox(Widget root, Druid entityId, List<FieldDescription> fields, int index)
		{
			//	Crée tous les champs dans une boîte.
			//	Cette méthode est appelée récursivement pour chaque BoxBegin/BoxEnd.

			//	Première passe pour déterminer quelles colonnes contiennent des labels.
			int column = 0, row = 0;
			int level = 0;
			List<int> labelsId = new List<int>();
			int labelId = 1;
			Widgets.Layouts.GridLayoutEngine grid = null;
			int lastLabelId = int.MinValue;

			if (root is UI.Panel)
			{
				for (int i=index; i<fields.Count; i++)
				{
					FieldDescription field = fields[i];

					if (field.DeltaHidden)
					{
						continue;
					}

					bool isGlueAfter = false;
					FieldDescription nextField = Engine.SearchNextElement(fields, i);
					if (nextField != null && nextField.Type == FieldDescription.FieldType.Glue && !nextField.DeltaHidden)
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
					else if (field.Type == FieldDescription.FieldType.Command)  // commande ?
					{
						if (level == 0)
						{
							this.PreprocessCommand(field, labelsId, ref labelId, ref column, isGlueAfter);
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
				grid = new Widgets.Layouts.GridLayoutEngine();
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
			}

			//	Deuxième passe pour générer le contenu.
			column = 0;
			row = 0;
			level = 0;
			List<Druid> lastTitle = null;
			for (int i=index; i<fields.Count; i++)
			{
				FieldDescription field = fields[i];

				if (field.DeltaHidden)
				{
					continue;
				}

				FieldDescription nextField = Engine.SearchNextElement(fields, i);

				bool isGlueAfter = false;
				if (nextField != null && nextField.Type == FieldDescription.FieldType.Glue && !nextField.DeltaHidden)
				{
					isGlueAfter = true;
				}

				bool isLastOfBox = false;
				if (nextField != null && nextField.Type == FieldDescription.FieldType.BoxEnd && !nextField.DeltaHidden)
				{
					isLastOfBox = true;
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
						Widget box = this.CreateBox(root, grid, field, guid, labelsId, ref column, ref row, isGlueAfter, isLastOfBox);
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
						this.CreateField(root, entityId, grid, field, guid, labelsId, ref column, ref row, isGlueAfter, isLastOfBox);
					}
				}
				else if (field.Type == FieldDescription.FieldType.Command)  // commande ?
				{
					if (level == 0)
					{
						this.CreateCommand(root, entityId, grid, field, guid, labelsId, ref column, ref row, isGlueAfter, isLastOfBox);
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

			if (columnsRequired == 1)
			{
				Engine.LabelIdUse(labelsId, labelId++, column, 1);
			}
			else
			{
				Engine.LabelIdUse(labelsId, -(labelId++), column, 1);
				Engine.LabelIdUse(labelsId, labelId++, column+1, columnsRequired-1);
			}

			if (isGlueAfter)
			{
				column += columnsRequired;
			}
			else
			{
				column = 0;
			}
		}

		private void PreprocessCommand(FieldDescription field, List<int> labelsId, ref int labelId, ref int column, bool isGlueAfter)
		{
			//	Détermine quelles colonnes contiennent des labels, lors de la première passe.
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


		private Widget CreateBox(Widget root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, List<int> labelsId, ref int column, ref int row, bool isGlueAfter, bool isLastOfBox)
		{
			//	Crée les widgets pour une boîte dans la grille, lors de la deuxième passe.
			Widget box;

			if (field.BoxLayout == FieldDescription.BoxLayoutType.Grid)
			{
				box = new UI.Panel(root);
			}
			else
			{
				box = new FrameBox(root);

				switch (field.BoxLayout)
				{
					case FieldDescription.BoxLayoutType.HorizontalLeft:
						box.HorizontalAlignment = HorizontalAlignment.Left;
						break;

					case FieldDescription.BoxLayoutType.HorizontalRight:
						box.HorizontalAlignment = HorizontalAlignment.Right;
						break;
					
					default:
						box.HorizontalAlignment = HorizontalAlignment.Center;
						break;
				}
			}

			double mLeft   = ((field.BoxFrameState & FrameState.Left  ) == 0) ? 0 : (field.BoxFrameWidth-1)/2;
			double mRight  = ((field.BoxFrameState & FrameState.Right ) == 0) ? 0 : (field.BoxFrameWidth-1)/2;
			double mTop    = ((field.BoxFrameState & FrameState.Top   ) == 0) ? 0 : (field.BoxFrameWidth-1)/2;
			double mBottom = ((field.BoxFrameState & FrameState.Bottom) == 0) ? 0 : (field.BoxFrameWidth-1)/2;

			Margins padding = FieldDescription.GetRealBoxPadding(field.BoxPadding);
			
			padding.Left   += mLeft;
			padding.Right  += mRight;
			padding.Top    += mTop;
			padding.Bottom += mBottom;

			box.DrawFrameState = FrameState.All;
			box.Padding = padding;
			box.BackColor = FieldDescription.GetRealBackColor(field.BackColor);
			box.DrawFrameState = field.BoxFrameState;
			box.DrawFrameWidth = field.BoxFrameWidth;
			box.PreferredWidth = field.PreferredWidth;
			box.Margins = new Margins(mLeft, mRight, mTop, mBottom);
			box.TabIndex = this.tabIndex;  // pas besoin d'incrémenter, pour que le groupe ne fasse pas perdre un numéro
			box.Name = guid.ToString();
			this.ApplyTextStyle(box, field);

			grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

			int columnsRequired = System.Math.Max(field.ColumnsRequired, 1);

			grid.RowDefinitions[row].BottomBorder = FieldDescription.GetRealSeparator(field.SeparatorBottom, isLastOfBox);

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

		private void CreateField(Widget root, Druid entityId, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, List<int> labelsId, ref int column, ref int row, bool isGlueAfter, bool isLastOfBox)
		{
			//	Crée les widgets pour un champ dans la grille, lors de la deuxième passe.
			UI.Placeholder placeholder = this.CreatePlaceholder(root, entityId, field);
			placeholder.BackColor = FieldDescription.GetRealBackColor(field.BackColor);
			placeholder.TabIndex = this.tabIndex++;
			placeholder.Name = guid.ToString();
			placeholder.PreferredWidth = field.PreferredWidth;
			placeholder.LabelReplacement = field.LabelReplacement;
			placeholder.Verbosity = field.Verbosity;
			this.ApplyTextStyle(placeholder, field);

			//	Détermine si le placeholder doit être utilisé pour saisir du texte ou pour
			//	saisir un critère de recherche et le configure en conséquence.
			FieldEditionMode editionMode = this.GetFieldEditionMode(entityId, field.FieldIds);
			switch (editionMode)
			{
				case FieldEditionMode.Data:
					placeholder.SuggestionMode = Epsitec.Common.UI.PlaceholderSuggestionMode.None;
					break;
				case FieldEditionMode.Search:
					placeholder.SuggestionMode = Epsitec.Common.UI.PlaceholderSuggestionMode.DisplayPassiveHint;
					break;
				default:
					throw new System.InvalidOperationException(string.Format("Invalid edition mode {0}", editionMode));
			}

			if (grid == null)
			{
				placeholder.Dock = DockStyle.Left;
			}
			else
			{
				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

				int columnsRequired = System.Math.Max(field.ColumnsRequired, 1);

				if (columnsRequired == 1)  // tout sur une seule colonne ?
				{
					placeholder.Controller = "*";
					placeholder.ControllerParameters = UI.Controllers.AbstractController.NoLabelsParameter;  // cache le label
				}

				grid.RowDefinitions[row].BottomBorder = FieldDescription.GetRealSeparator(field.SeparatorBottom, isLastOfBox);

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
		}

		private UI.Placeholder CreatePlaceholder(Widget root, Druid entityId, FieldDescription field)
		{
			//	Crée le bon type de placeholder, en fonction du champ qui doit être
			//	représenté. Les champs normaux sont gérés par la classe Placeholder
			//	alors que les références sont gérées par la classe ReferencPlaceholder.

			EntityFieldPath fieldPath = field.GetFieldPath ();
			Druid  leafEntityId;
			string leafFieldId;
			fieldPath.NavigateSchema (this.entityContext, entityId, out leafEntityId, out leafFieldId);

			StructuredType entityDef = this.GetEntityDefinition (leafEntityId);
			StructuredTypeField fieldDef = entityDef.GetField (leafFieldId);

			UI.Placeholder placeholder = null;

			switch (fieldDef.Relation)
			{
				case FieldRelation.None:
					placeholder = new UI.Placeholder ()
					{
						Embedder = root
					};
					placeholder.SetBinding (UI.Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, field.GetPath (DataSource.DataName)));
					break;

				case FieldRelation.Reference:
					placeholder = new UI.ReferencePlaceholder ()
					{
						Embedder = root,
						EntityType = fieldDef.Type as StructuredType,
						EntityFieldPath = fieldPath
					};
					placeholder.SetBinding (UI.Placeholder.ValueProperty, new Binding (BindingMode.TwoWay, field.GetPath (DataSource.DataName)));
					break;

				case FieldRelation.Collection:
					placeholder = new UI.CollectionPlaceholder ()
					{
						Embedder = root,
						EntityType = fieldDef.Type as StructuredType,
						EntityFieldPath = fieldPath
					};
					placeholder.SetBinding (UI.CollectionPlaceholder.CollectionProperty, new Binding (BindingMode.OneWay, field.GetPath (DataSource.DataName)));
					break;
			}


			return placeholder;
		}

		private void CreateCommand(Widget root, Druid entityId, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, List<int> labelsId, ref int column, ref int row, bool isGlueAfter, bool isLastOfBox)
		{
			//	Crée les widgets pour une commande dans la grille, lors de la deuxième passe.
			UI.MetaButton button = new UI.MetaButton();
			button.SetParent(root);
			button.TabIndex = this.tabIndex++;
			button.Name = guid.ToString();
			button.CommandId = field.FieldIds[0];
			button.PreferredWidth = field.PreferredWidth;
			button.CaptionId = field.LabelReplacement;
			this.ApplyCommandButtonClass(button, field);
			this.ApplyTextStyle(button, field);

			if (grid == null)
			{
				button.Dock = DockStyle.Left;
			}
			else
			{
				grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());

				int columnsRequired = System.Math.Max(field.ColumnsRequired, 1);

				grid.RowDefinitions[row].BottomBorder = FieldDescription.GetRealSeparator(field.SeparatorBottom, isLastOfBox);

				int i = Engine.GetColumnIndex(labelsId, column);
				int j = Engine.GetColumnIndex(labelsId, column+columnsRequired-1)+1;
				Widgets.Layouts.GridLayoutEngine.SetColumn(button, i);
				Widgets.Layouts.GridLayoutEngine.SetRow(button, row);
				Widgets.Layouts.GridLayoutEngine.SetColumnSpan(button, j-i);

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
		}

		private void CreateGlue(Widget root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, List<int> labelsId, ref int column, ref int row, bool isGlueAfter)
		{
			//	Crée les widgets pour un collage dans la grille, lors de la deuxième passe.
			int columnsRequired = field.ColumnsRequired;

			if (grid == null)
			{
				FrameBox glue = new FrameBox(root);
				glue.BackColor = FieldDescription.GetRealBackColor(field.BackColor);
				glue.Name = guid.ToString();
				glue.Dock = DockStyle.Left;

				if (root is FrameBox)
				{
					glue.PreferredWidth = field.PreferredWidth;
				}
			}
			else
			{
				if (this.forDesigner)
				{
					FrameBox glue = new FrameBox(root);
					glue.BackColor = FieldDescription.GetRealBackColor(field.BackColor);
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
			}

			column += columnsRequired;
		}

		private void CreateSeparator(Widget root, Widgets.Layouts.GridLayoutEngine grid, FieldDescription field, System.Guid guid, FieldDescription nextField, List<int> labelsId, ref int column, ref int row, bool isGlueAfter, ref List<Druid> lastTitle)
		{
			//	Crée les widgets pour un séparateur dans la grille, lors de la deuxième passe.
			FieldDescription.FieldType type = field.Type;

			double m = FieldDescription.GetRealSeparator(field.SeparatorBottom, true)+2;
			double w = field.LineWidth;

			if (nextField == null || grid == null)
			{
				type = FieldDescription.FieldType.Line;
			}

			if (type == FieldDescription.FieldType.Title)
			{
				List<Druid> druids = nextField.FieldIds;
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				if (field.LabelReplacement.IsEmpty)
				{
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
				}
				else
				{
					builder.Append(this.GetCaptionDefaultLabel(field.LabelReplacement));
				}

				if (builder.Length == 0)  // titre sans texte ?
				{
					type = FieldDescription.FieldType.Line;  // il faudra mettre une simple ligne
				}
				else
				{
					if (grid != null)
					{
						grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
						grid.RowDefinitions[row].TopBorder = m;
						grid.RowDefinitions[row].BottomBorder = 0;
					}

					double size = System.Math.Max(200-(druids.Count-2)*25, 100);

					StaticText text = new StaticText(root);
					text.Text = string.Concat("<font size=\"", size.ToString(System.Globalization.CultureInfo.InvariantCulture), "%\"><b>", builder.ToString(), "</b></font>");
					text.PreferredHeight = size/100*16;
					text.Name = guid.ToString();
					this.ApplyTextStyle(text, field);

					if (grid == null)
					{
						text.Dock = DockStyle.Left;
					}
					else
					{
						int i = Engine.GetColumnIndex(labelsId, 0);
						int j = Engine.GetColumnIndex(labelsId, labelsId.Count-1)+1;
						Widgets.Layouts.GridLayoutEngine.SetColumn(text, i);
						Widgets.Layouts.GridLayoutEngine.SetRow(text, row);
						Widgets.Layouts.GridLayoutEngine.SetColumnSpan(text, j-i);

						row++;
					}
				}

				lastTitle = druids;  // pour se rappeler du titre précédent
			}

			if (type == FieldDescription.FieldType.Line ||
				type == FieldDescription.FieldType.Title)
			{
				if (grid != null)
				{
					grid.RowDefinitions.Add(new Widgets.Layouts.RowDefinition());
					grid.RowDefinitions[row].TopBorder = (type == FieldDescription.FieldType.Title) ? 0 : m+1;
					grid.RowDefinitions[row].BottomBorder = m;
				}

				Separator sep = new Separator(root);
				sep.DrawFrameWidth = w;
				sep.PreferredHeight = w;
				sep.Name = guid.ToString();

				if (type == FieldDescription.FieldType.Title)
				{
					sep.Color = FieldDescription.GetRealFontColor(field.LabelFontColor);
				}

				if (root is FrameBox)
				{
					sep.PreferredWidth = field.PreferredWidth;
				}

				if (grid == null)
				{
					sep.IsVerticalLine = true;
					sep.Dock = DockStyle.Left;
				}
				else
				{
					sep.IsHorizontalLine = true;

					int i = Engine.GetColumnIndex(labelsId, 0);
					int j = Engine.GetColumnIndex(labelsId, labelsId.Count-1)+1;
					Widgets.Layouts.GridLayoutEngine.SetColumn(sep, i);
					Widgets.Layouts.GridLayoutEngine.SetRow(sep, row);
					Widgets.Layouts.GridLayoutEngine.SetColumnSpan(sep, j-i);

					row++;
				}
			}
		}


		private void ApplyTextStyle(Widget widget, FieldDescription field)
		{
			//	Applique les différents styles de texte définis, s'ils existent, pour le widget et ses enfants.
			if (!field.HasTextStyle)
			{
				return;
			}

			UI.TextStyleManager textStyleManager = new UI.TextStyleManager();

			if (field.HasLabelTextStyle)
			{
				TextStyle style = new TextStyle();

				if (field.LabelFontColor != FieldDescription.FontColorType.Default)
				{
					style.FontColor = FieldDescription.GetRealFontColor(field.LabelFontColor);
				}

				this.ApplyFontTextStyle(style, field.LabelFontFace, field.LabelFontStyle);

				if (field.LabelFontSize != FieldDescription.FontSizeType.Normal)
				{
					style.FontSize = FieldDescription.GetRealFontSize(field.LabelFontSize);
				}

				textStyleManager.StaticTextStyle = style;
			}

			if (field.HasFieldTextStyle)
			{
				TextStyle style = new TextStyle();

				if (field.FieldFontColor != FieldDescription.FontColorType.Default)
				{
					style.FontColor = FieldDescription.GetRealFontColor(field.FieldFontColor);
				}

				this.ApplyFontTextStyle(style, field.FieldFontFace, field.FieldFontStyle);

				if (field.FieldFontSize != FieldDescription.FontSizeType.Normal)
				{
					style.FontSize = FieldDescription.GetRealFontSize(field.FieldFontSize);
				}

				textStyleManager.TextFieldStyle = style;
			}

			//	Active les styles pour le widget spécifié et tous ses enfants.
			textStyleManager.Attach(widget);
		}

		private void ApplyCommandButtonClass(UI.MetaButton button, FieldDescription field)
		{
			//	Applique le type pour le bouton d'une commande.
			switch (field.CommandButtonClassValue)
			{
				case FieldDescription.CommandButtonClass.DialogButton:
					button.ButtonClass = ButtonClass.DialogButton;
					break;

				case FieldDescription.CommandButtonClass.RichDialogButton:
					button.ButtonClass = ButtonClass.RichDialogButton;
					break;

				case FieldDescription.CommandButtonClass.FlatButton:
					button.ButtonClass = ButtonClass.FlatButton;
					break;

				default:
					button.ButtonClass = ButtonClass.None;
					break;
			}
		}

		private void ApplyFontTextStyle(TextStyle textStyle, FieldDescription.FontFaceType face, FieldDescription.FontStyleType style)
		{
			if (face != FieldDescription.FontFaceType.Default || style != FieldDescription.FontStyleType.Normal)
			{
				string faceName, styleName;
				FieldDescription.GetRealFontStrings(face, style, out faceName, out styleName);
				textStyle.Font = Font.GetFont(faceName, styleName);
			}
		}


		private string GetCaptionDefaultLabel(Druid druid)
		{
			Caption caption = this.resourceProvider.GetCaption(druid);
			return caption == null ? "" : caption.DefaultLabel;
		}


		private void GenerateForwardTab(Widget root, List<FieldDescription> fields)
		{
			//	Initialise les propriétés ForwardTabOverride et BackwardTabOverride aux widgets du masque
			//	qui sont définis par des exceptions FieldDescription.ForwardTabGuid pour la navigation.
			foreach (FieldDescription srcField in fields)
			{
				if (srcField.ForwardTabGuid != System.Guid.Empty)  // définition d'une exception pour la navigation ?
				{
					FieldDescription dstField = this.SearchField(fields, srcField.ForwardTabGuid);
					if (dstField != null)
					{
						Widget srcWidget = this.SearchWidget(root, srcField.Guid);
						Widget dstWidget = this.SearchWidget(root, dstField.Guid);

						if (srcWidget != null && dstWidget != null)
						{
							srcWidget.ForwardTabOverride  = dstWidget;
							dstWidget.BackwardTabOverride = srcWidget;
						}
					}
				}
			}
		}

		private Widget SearchWidget(Widget parent, System.Guid guid)
		{
			//	Cherche un widget défini par son System.Guid dans tout le masque.
			foreach (Widget widget in parent.Children.Widgets)
			{
				if (!string.IsNullOrEmpty(widget.Name))
				{
					System.Guid current = new System.Guid(widget.Name);
					if (current == guid)
					{
						return widget;
					}
				}

				Widget search = this.SearchWidget(widget, guid);
				if (search != null)
				{
					return search;
				}
			}

			return null;
		}

		private FieldDescription SearchField(List<FieldDescription> fields, System.Guid guid)
		{
			//	Cherche un champ défini par son System.Guid dans la liste des champs.
			foreach (FieldDescription field in fields)
			{
				if (field.Guid == guid)
				{
					return field;
				}
			}

			return null;
		}


		static private int CountFields(List<FieldDescription> fields, int index)
		{
			//	Compte le nombre de descriptions de types champ, séparateur ou titre.
			int count = 0;

			for (int i=index; i<fields.Count; i++)
			{
				if (fields[i].Type == FieldDescription.FieldType.Field    ||
					fields[i].Type == FieldDescription.FieldType.Command  ||
					fields[i].Type == FieldDescription.FieldType.Line     ||
					fields[i].Type == FieldDescription.FieldType.Title    )
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
		private AbstractEntity entityData;
		private Arrange arrange;
		private bool forDesigner;
		private FieldEditionMode defaultMode;
		private int tabIndex;
	}
}
