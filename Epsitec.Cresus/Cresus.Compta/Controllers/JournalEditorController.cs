//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Dialogs;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class JournalEditorController : AbstractEditorController
	{
		public JournalEditorController(AbstractController controller)
			: base (controller)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			var editor = this.CreateEditorUI (parent);

			this.footer = new CustomFrameBox
			{
				Parent = editor,
				Dock   = DockStyle.Fill,
			};

			//	Crée les boîtes.
			this.infoFrameBox = new FrameBox
			{
				Parent          = this.footer,
				PreferredHeight = 39,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 0, 0),
			};

			this.infoFrameSeparator = new FrameBox
			{
				Parent          = this.footer,
				PreferredHeight = 5,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 0, 0),
			};

			{
				var linesBox = new FrameBox
				{
					Parent              = this.footer,
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					Dock                = DockStyle.Bottom,
				};

				this.linesContainer = new TabCatcherFrameBox
				{
					Parent  = linesBox,
					Dock    = DockStyle.Fill,
				};

				this.linesContainer.TabPressed += new TabCatcherFrameBox.TabPressedEventHandler (this.HandleLinesContainerTabPressed);

				this.scroller = new VScroller
				{
					Parent     = linesBox,
					IsInverted = true,  // zéro en haut
					Dock       = DockStyle.Right,
				};

				this.scroller.ValueChanged += delegate
				{
					this.ChangeScroller ();
				};
			}

			//	Crée les lignes éditables.
			this.CreateLineUI (this.linesContainer);

			//	Crée les informations.
			this.débitInfoSeparator = new Separator
			{
				Parent          = this.infoFrameSeparator,
				IsVerticalLine  = true,
				DrawFrameWidth  = 3,
				PreferredHeight = 5,
				Dock            = DockStyle.Left,
			};

			this.créditInfoSeparator = new Separator
			{
				Parent          = this.infoFrameSeparator,
				IsVerticalLine  = true,
				DrawFrameWidth  = 3,
				PreferredHeight = 5,
				Dock            = DockStyle.Left,
			};

			this.débitInfoFrame = new FrameBox
			{
				Parent        = this.infoFrameBox,
				DrawFullFrame = true,
				BackColor     = UIBuilder.InfoColor,
				Dock          = DockStyle.Left,
				Margins       = new Margins (0, 1, 0, 0),
			};

			this.créditInfoFrame = new FrameBox
			{
				Parent        = this.infoFrameBox,
				DrawFullFrame = true,
				BackColor     = UIBuilder.InfoColor,
				Dock          = DockStyle.Left,
				Margins       = new Margins (0, 1, 0, 0),
			};

			UIBuilder.CreateInfoCompte (this.débitInfoFrame);
			UIBuilder.CreateInfoCompte (this.créditInfoFrame);

			base.CreateUI (this.footer, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var editorFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 1, 0),
			};

			this.linesFrames.Add (editorFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			editorFrame.TabIndex = line+1;

			//	Colle la ligne à la précédente s'il y a lieu.
			var type = this.GetTypeEcriture (line);
			if (type == TypeEcriture.CodeTVA)
			{
				editorFrame.Margins = new Margins (0, 0, -1, 2);  // la hauteur totale doit rester identique (1+0 = -1+2) !
			}

			var comptes = this.compta.PlanComptable.Where (x => x.Type != TypeDeCompte.Groupe);

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				AbstractFieldController field;

				if (mapper.Column == ColumnType.Date)
				{
					field = new DateFieldController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else if (mapper.Column == ColumnType.Débit ||
						 mapper.Column == ColumnType.Crédit)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else if (mapper.Column == ColumnType.Libellé)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
					(field.EditWidget as AutoCompleteTextField).AcceptFreeText = true;

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, this.compta.GetLibellésDescriptions (this.période).ToArray ());

					this.CreateButtonModèleUI (field, line);
				}
				else if (mapper.Column == ColumnType.LibelléTVA)
				{
					field = new StaticTextController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else if (mapper.Column == ColumnType.CodeTVA)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else if (mapper.Column == ColumnType.TauxTVA)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else if (mapper.Column == ColumnType.Journal)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);

					var journaux = this.compta.Journaux.Select (x => x.Nom);
					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, journaux.ToArray ());
				}
				else if (mapper.Column == ColumnType.Type)
				{
					field = new FixedTextController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);

					(field as FixedTextController).TextConverter = x => ComptaEcritureEntity.GetShortType (Converters.StringToTypeEcriture (x));
				}
				else if (mapper.Column == ColumnType.OrigineTVA)
				{
					field = new FixedTextController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleClearFocus, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}

				if (mapper.Column == ColumnType.Montant   ||
					mapper.Column == ColumnType.MontantTTC)
				{
					field.EditWidget.ContentAlignment = ContentAlignment.MiddleRight;
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);
			}
		}

		private void ResetLineUI()
		{
			//	Recrée une seule ligne.
			using (this.controller.IgnoreChanges.Enter ())
			{
				this.linesContainer.Children.Clear ();
				this.fieldControllers.Clear ();
				this.linesFrames.Clear ();

				this.CreateLineUI (this.linesContainer);
			}
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'une écriture :" : "Création d'une écriture :";
		}


		public override void AcceptAction()
		{
			if (!this.controller.GetCommandEnable (Res.Commands.Edit.Accept))
			{
				return;
			}

			//	Avant de créer les lignes de l'écriture, met le type 'Normal' partout où
			//	cela est possible.
			for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
			{
				var type = this.GetTypeEcriture (line);

				if ((type == TypeEcriture.Vide || type == TypeEcriture.Nouveau) &&
					!(this.dataAccessor.EditionLine[line] as JournalEditionLine).IsEmptyLine)
				{
					this.SetTypeEcriture (line, TypeEcriture.Normal);
				}
			}

			base.AcceptAction();

			this.UpdateLibellés ();
		}

		private void UpdateLibellés()
		{
			//	Met à jour les libellés usuels dans les widgets combo pour les libellés.
			var libellés = this.compta.GetLibellésDescriptions (this.période).ToArray ();

			for (int line = 0; line < this.fieldControllers.Count; line++)
			{
				var field = this.GetFieldController (ColumnType.Libellé, line);
				UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, libellés);
			}
		}


		private void PrepareFirstMulti()
		{
			//	Met à jour l'interface pour permettre de créer une écriture multiple, lorsqu'on passe de 1 ligne à 3.
			bool isDébitMulti = this.IsDébitMulti (0);
			var multiActiveColumn   =  isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;
			var multiInactiveColumn = !isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;

			int indexVide, indexCP;

			if (this.settingsList.GetBool (SettingsType.EcritureProposeVide))
			{
				this.dataAccessor.InsertEditionLine (-1);  // 2ème ligne
				indexVide = 1;
				indexCP   = 2;
			}
			else
			{
				indexVide = -1;
				indexCP   = 1;
			}

			this.dataAccessor.InsertEditionLine (-1);  // contrepartie

			//	Met à jour les données de la 1ère ligne.
			this.SetTypeEcriture (0, TypeEcriture.Nouveau);

			//	Met à jour les données de la 2ème ligne (vide).
			if (indexVide != -1)
			{
				this.SetTypeEcriture (indexVide, TypeEcriture.Nouveau);
				this.dataAccessor.EditionLine[indexVide].SetText (ColumnType.Date,             this.dataAccessor.EditionLine[0].GetText (ColumnType.Date));
				//?this.dataAccessor.EditionLine[indexVide].SetText (multiInactiveColumn,         JournalDataAccessor.multi);
				this.dataAccessor.EditionLine[indexVide].SetText (ColumnType.Montant,          Converters.MontantToString (0));
				this.dataAccessor.EditionLine[indexVide].SetText (ColumnType.Journal,          this.dataAccessor.EditionLine[0].GetText (ColumnType.Journal));
			}
																				   
			//	Met à jour les données de la contrepartie.						   
			this.SetTypeEcriture (indexCP, TypeEcriture.Nouveau);
			this.dataAccessor.EditionLine[indexCP].SetText (ColumnType.Date,             this.dataAccessor.EditionLine[0].GetText (ColumnType.Date));
			//?this.dataAccessor.EditionLine[indexCP].SetText (multiActiveColumn,           JournalDataAccessor.multi);
			//?this.dataAccessor.EditionLine[indexCP].SetText (ColumnType.Libellé,          "Total");
			this.dataAccessor.EditionLine[indexCP].SetText (ColumnType.Journal,          this.dataAccessor.EditionLine[0].GetText (ColumnType.Journal));
			this.dataAccessor.EditionLine[indexCP].SetText (ColumnType.TotalAutomatique, "1");

			if (this.PlusieursPièces)
			{
				var nomJournal = this.dataAccessor.EditionLine[0].GetText (ColumnType.Journal);
				var journal = this.compta.Journaux.Where (x => x.Nom == nomJournal).FirstOrDefault ();

				if (indexVide != -1)
				{
					this.dataAccessor.EditionLine[indexVide].SetText (ColumnType.Pièce, this.controller.MainWindowController.PiècesGenerator.GetProchainePièce (journal, indexVide));
				}

				this.dataAccessor.EditionLine[indexCP].SetText (ColumnType.Pièce, this.controller.MainWindowController.PiècesGenerator.GetProchainePièce (journal, indexCP));
			}
			else
			{
				if (indexVide != -1)
				{
					this.dataAccessor.EditionLine[indexVide].SetText (ColumnType.Pièce, this.dataAccessor.EditionLine[0].GetText (ColumnType.Pièce));
				}

				this.dataAccessor.EditionLine[indexCP].SetText (ColumnType.Pièce, this.dataAccessor.EditionLine[0].GetText (ColumnType.Pièce));
			}
		}

		private int ExplodeForTVA(int line, bool defaultDébit = true)
		{
			bool isDébitMulti  = this.IsCréditTVA (line);
			bool isCréditMulti = this.IsDébitTVA  (line);

			if (!isDébitMulti && !isCréditMulti)
			{
				isDébitMulti  =  defaultDébit;
				isCréditMulti = !defaultDébit;
			}

			var multiActiveColumn   =  isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;
			var multiInactiveColumn = !isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;

			var p  = this.dataAccessor.EditionLine[line].GetText (multiActiveColumn);
			var cp = this.dataAccessor.EditionLine[line].GetText (multiInactiveColumn);
			var compteP  = this.compta.PlanComptable.Where (x => x.Numéro == p ).FirstOrDefault ();
			var compteCP = this.compta.PlanComptable.Where (x => x.Numéro == cp).FirstOrDefault ();

			//	Cherche le code TVA à utiliser.
			var codeTVA = compteP.CodeTVAParDéfaut;
			bool hasDefaultCodeTVA = true;

			if (codeTVA == null)
			{
				codeTVA = this.compta.CodesTVA.FirstOrDefault ();
				hasDefaultCodeTVA = false;
			}
			if (codeTVA == null)  // garde-fou
			{
				return 1;
			}

			var taux = codeTVA.DefaultTauxValue.GetValueOrDefault ();

			var montantTTC = Converters.ParseMontant (this.dataAccessor.EditionLine[line].GetText (ColumnType.MontantTTC)).GetValueOrDefault ();
			var montantHT  = Converters.ParseMontant (this.dataAccessor.EditionLine[line].GetText (ColumnType.Montant   )).GetValueOrDefault ();

			if (!hasDefaultCodeTVA)
			{
				montantTTC = montantHT;
				montantHT  = TVA.CalculeHT (montantHT, taux);
			}
			else if (montantTTC == 0)
			{
				montantTTC  = TVA.CalculeTTC (montantHT, taux);
			}
			else
			{
				montantHT  = TVA.CalculeHT (montantTTC, taux);
			}

			var montantTVA = TVA.CalculeTVA (montantHT, taux);

			int total = (this.dataAccessor.CountEditedRow == 1) ? 3 : 2;

			this.dataAccessor.InsertEditionLine (line+1);  // insère la 2ème ligne après l'actuelle

			if (total == 3)
			{
				this.dataAccessor.InsertEditionLine (line+2);  // insère la 3ème ligne après la 2ème
			}

			//	Met à jour les données de la 1ère ligne (BaseTVA).
			this.SetTypeEcriture (line+0, TypeEcriture.BaseTVA);
			this.dataAccessor.EditionLine[line+0].SetText (multiInactiveColumn,         JournalDataAccessor.multi);
			this.dataAccessor.EditionLine[line+0].SetText (ColumnType.OrigineTVA,       (multiActiveColumn == ColumnType.Débit) ? "D" : "C");
			this.dataAccessor.EditionLine[line+0].SetText (ColumnType.MontantTTC,       Converters.MontantToString (montantTTC));
			this.dataAccessor.EditionLine[line+0].SetText (ColumnType.Montant,          Converters.MontantToString (montantHT));

			//	Met à jour les données de la 2ème ligne (CodeTVA).
			this.SetTypeEcriture (line+1, TypeEcriture.CodeTVA);
			this.dataAccessor.EditionLine[line+1].SetText (ColumnType.Date,             this.dataAccessor.EditionLine[line].GetText (ColumnType.Date));
			this.dataAccessor.EditionLine[line+1].SetText (multiActiveColumn,           codeTVA.Compte.Numéro);
			this.dataAccessor.EditionLine[line+1].SetText (multiInactiveColumn,         JournalDataAccessor.multi);
			this.dataAccessor.EditionLine[line+1].SetText (ColumnType.OrigineTVA,       (multiActiveColumn == ColumnType.Débit) ? "D" : "C");
			this.dataAccessor.EditionLine[line+1].SetText (ColumnType.CodeTVA,          codeTVA.Code);
			this.dataAccessor.EditionLine[line+1].SetText (ColumnType.TauxTVA,          Converters.PercentToString (codeTVA.DefaultTauxValue));
			this.dataAccessor.EditionLine[line+1].SetText (ColumnType.Montant,          Converters.MontantToString (montantTVA));
			this.dataAccessor.EditionLine[line+1].SetText (ColumnType.Journal,          this.dataAccessor.EditionLine[line].GetText (ColumnType.Journal));

			this.UpdateAfterCompteOrigineTVAChanged (line);
			this.UpdateAfterCodeTVAChanged (line+1);

			if (total == 3)
			{
				//	Met à jour les données de la contrepartie.						   
				this.SetTypeEcriture (line+2, TypeEcriture.Normal);
				this.dataAccessor.EditionLine[line+2].SetText (ColumnType.Date,             this.dataAccessor.EditionLine[line+0].GetText (ColumnType.Date));
				this.dataAccessor.EditionLine[line+2].SetText (multiActiveColumn,           JournalDataAccessor.multi);
				this.dataAccessor.EditionLine[line+2].SetText (multiInactiveColumn,         (compteCP == null) ? null : compteCP.Numéro);
				this.dataAccessor.EditionLine[line+2].SetText (ColumnType.Libellé,          this.dataAccessor.EditionLine[line+0].GetText (ColumnType.Libellé));
				this.dataAccessor.EditionLine[line+2].SetText (ColumnType.Journal,          this.dataAccessor.EditionLine[line+0].GetText (ColumnType.Journal));
				this.dataAccessor.EditionLine[line+2].SetText (ColumnType.TotalAutomatique, "1");
				this.dataAccessor.EditionLine[line+2].SetText (ColumnType.IsAutoLibellé,    "1");
			}

			if (this.PlusieursPièces)
			{
				var nomJournal = this.dataAccessor.EditionLine[line].GetText (ColumnType.Journal);
				var journal = this.compta.Journaux.Where (x => x.Nom == nomJournal).FirstOrDefault ();

				this.dataAccessor.EditionLine[line+1].SetText (ColumnType.Pièce, this.controller.MainWindowController.PiècesGenerator.GetProchainePièce (journal, 1));

				if (total == 3)
				{
					this.dataAccessor.EditionLine[line+2].SetText (ColumnType.Pièce, this.controller.MainWindowController.PiècesGenerator.GetProchainePièce (journal, 2));
				}
			}
			else
			{
				this.dataAccessor.EditionLine[line+1].SetText (ColumnType.Pièce, this.dataAccessor.EditionLine[line].GetText (ColumnType.Pièce));

				if (total == 3)
				{
					this.dataAccessor.EditionLine[line+2].SetText (ColumnType.Pièce, this.dataAccessor.EditionLine[line].GetText (ColumnType.Pièce));
				}
			}

			return total;
		}

		public override void MultiInsertLineAction()
		{
			//	Insère une nouvelle ligne après la ligne courante.
			bool isDébitMulti = this.IsDébitMulti (this.selectedLine);
			var multiActiveColumn   =  isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;
			var multiInactiveColumn = !isDébitMulti ? ColumnType.Crédit : ColumnType.Débit;

			this.selectedLine++;
			this.dataAccessor.InsertEditionLine (this.selectedLine);

			this.dataAccessor.EditionLine[this.selectedLine].SetText (multiInactiveColumn, JournalDataAccessor.multi);
			this.SetTypeEcriture (this.selectedLine, TypeEcriture.Nouveau);

			int cp = this.IndexTotalAutomatique;
			if (cp != -1)
			{
				this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Date,    this.dataAccessor.EditionLine[cp].GetText (ColumnType.Date));
				this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Journal, this.dataAccessor.EditionLine[cp].GetText (ColumnType.Journal));

				if (this.PlusieursPièces)
				{
					var nomJournal = this.dataAccessor.EditionLine[cp].GetText (ColumnType.Journal);
					var journal = this.compta.Journaux.Where (x => x.Nom == nomJournal).FirstOrDefault ();

					this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Pièce, this.controller.MainWindowController.PiècesGenerator.GetProchainePièce (journal, this.dataAccessor.EditionLine.Count-1));
				}
				else
				{
					this.dataAccessor.EditionLine[this.selectedLine].SetText (ColumnType.Pièce, this.dataAccessor.EditionLine[cp].GetText (ColumnType.Pièce));
				}
			}

			this.dirty = true;
			this.UpdateEditorContent ();
			this.SelectedLineChanged ();
			this.EditorSelect (multiActiveColumn, this.selectedLine);
		}

		public override void MultiInsertTVALineAction()
		{
			//	Insère une nouvelle ligne de TVA après la ligne courante.
			var débit  = this.GetCompteDébit  (this.selectedLine);
			var crédit = this.GetCompteCrédit (this.selectedLine);

			bool defaultDébit = false;

			if (débit == null || crédit == null)  // est-ce une écriture multiple ?
			{
				defaultDébit = (débit == null);
			}
			else
			{
				bool débitSansCodeTVA  = débit .CodeTVAParDéfaut == null;
				bool créditSansCodeTVA = crédit.CodeTVAParDéfaut == null;

				if ((débitSansCodeTVA && créditSansCodeTVA) || (!débitSansCodeTVA && !créditSansCodeTVA))
				{
					//	Demande s'il faut passer la TVA sur le compte au débit ou au crédit.
					var dialog = new DébitCréditDialog (this.controller, débit, crédit);
					dialog.Show ();

					if (!dialog.IsDébit && !dialog.IsCrédit)  // fermé sans choisir ?
					{
						return;
					}

					defaultDébit = dialog.IsCrédit;
				}
			}

			this.ExplodeForTVA (this.selectedLine, defaultDébit: defaultDébit);

			this.dirty = true;
			this.UpdateEditorContent ();

			this.selectedLine = this.selectedLine+1;
			this.EditorSelect (ColumnType.CodeTVA);
		}

		public override void MultiDeleteLineAction()
		{
			//	Supprime la ligne courante.
			var type = this.GetTypeEcriture (this.selectedLine);

			if (type == TypeEcriture.BaseTVA)
			{
				this.dataAccessor.RemoveAtEditionLine (this.selectedLine);

				if (this.GetTypeEcriture (this.selectedLine) == TypeEcriture.CodeTVA)
				{
					this.dataAccessor.RemoveAtEditionLine (this.selectedLine);
				}
			}
			else if (type == TypeEcriture.CodeTVA)
			{
				if (this.GetTypeEcriture (this.selectedLine-1) == TypeEcriture.BaseTVA)
				{
					//	Transforme la ligne précédente 'BaseTVA' en une ligne normale avec le montant TTC.
					this.SetTypeEcriture (this.selectedLine-1, TypeEcriture.Normal);

					var montantTTC = this.dataAccessor.EditionLine[this.selectedLine-1].GetText (ColumnType.MontantTTC);
					this.dataAccessor.EditionLine[this.selectedLine-1].SetText (ColumnType.Montant, montantTTC);
				}

				this.dataAccessor.RemoveAtEditionLine (this.selectedLine);
			}
			else
			{
				this.dataAccessor.RemoveAtEditionLine (this.selectedLine);
			}

			this.selectedLine = System.Math.Min (this.selectedLine, this.dataAccessor.CountEditedRow-1);

			this.dirty = true;
			this.UpdateEditorContent ();
			this.SelectedLineChanged ();
			this.EditorSelect (this.selectedColumn, this.selectedLine);
		}

		public override void MultiMoveLineAction(int direction)
		{
			//	Monte ou descend la ligne courante.
			this.SwapLine (this.selectedLine, this.selectedLine+direction);
			this.selectedLine += direction;

			this.dirty = true;
			this.UpdateEditorContent ();
			this.SelectedLineChanged ();
			this.EditorSelect (this.selectedColumn, this.selectedLine);
		}

		public override void MultiLineSwapAction()
		{
			//	Permute le débit et le crédit dans la ligne courante.
			var type = this.GetTypeEcriture (this.selectedLine);

			this.SwapDébitCrédit (this.selectedLine);

			if (type == TypeEcriture.BaseTVA &&
				this.GetTypeEcriture (this.selectedLine+1) == TypeEcriture.CodeTVA)
			{
				this.SwapDébitCrédit (this.selectedLine+1);
			}

			if (type == TypeEcriture.CodeTVA &&
				this.GetTypeEcriture (this.selectedLine-1) == TypeEcriture.BaseTVA)
			{
				this.SwapDébitCrédit (this.selectedLine-1);
			}

			this.dirty = true;
			this.UpdateEditorContent ();
		}

		private void SwapDébitCrédit(int line)
		{
			var débit  = this.dataAccessor.EditionLine[line].GetText (ColumnType.Débit);
			var crédit = this.dataAccessor.EditionLine[line].GetText (ColumnType.Crédit);

			this.dataAccessor.EditionLine[line].SetText (ColumnType.Débit, crédit);
			this.dataAccessor.EditionLine[line].SetText (ColumnType.Crédit, débit);
		}

		public override void MultiLineAutoAction()
		{
			//	Met le total automatique dans la ligne courante.
			for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.TotalAutomatique, (line == this.selectedLine) ? "1" : "0");
			}

			this.dirty = true;
			this.UpdateEditorContent ();
		}

		private void SwapLine(int line1, int line2)
		{
			//	Permute deux lignes (pour les opérations monte/descend).
			var t                                = this.dataAccessor.EditionLine[line2];
			this.dataAccessor.EditionLine[line2] = this.dataAccessor.EditionLine[line1];
			this.dataAccessor.EditionLine[line1] = t;
		}

		public override void InsertModèle(int n)
		{
			RaccourciModèle rm = RaccourciModèle.Ctrl0 + n;
			string srm = Converters.RaccourciToString (rm);
			var modèle = this.compta.Modèles.Where (x => x.Raccourci == srm).FirstOrDefault ();

			if (modèle != null)
			{
				this.InsertModèle (modèle, this.selectedLine);
			}
		}


		protected override void HandleSetFocus(int line, ColumnType columnType)
		{
			//	Appelé lorsqu'un champ prend le focus. C'est ici qu'on complète l'écriture, lorsqu'on crée
			//	une écriture multiple, qu'on a donné un compte avec code TVA, etc.
			base.HandleSetFocus (line, columnType);

			bool changed = false;

			//	Création d'une écriture multiple ?
			if (this.dataAccessor.EditionLine.Count != 0 && !this.isMulti && !this.IsTVA (0) && !this.dataAccessor.IsModification && this.dataAccessor.CountEditedRow == 1)
			{
				if (this.IsDébitMulti (0) || this.IsCréditMulti (0))
				{
					this.PrepareFirstMulti ();
					changed = true;
				}
			}

			//	Compte avec code TVA ?
			int i = 0;
			while (i < this.dataAccessor.CountEditedRow)
			{
				var type = this.GetTypeEcriture (i);

				if (type == TypeEcriture.Nouveau ||  // ligne fraichement créée ?
					type == TypeEcriture.Vide)
				{
					var débit  = this.GetCompteDébit  (i);
					var crédit = this.GetCompteCrédit (i);

					if (this.isMulti)
					{
						if ((débit  != null && débit .CodeTVAParDéfaut != null) ||
							(crédit != null && crédit.CodeTVAParDéfaut != null))  // code TVA au débit ou au crédit ?
						{
							i += this.ExplodeForTVA (i);
							changed = true;
							continue;
						}
					}
					else
					{
						if (débit != null && crédit != null)
						{
							if ((débit.CodeTVAParDéfaut != null && crédit.CodeTVAParDéfaut == null) ||
								(débit.CodeTVAParDéfaut == null && crédit.CodeTVAParDéfaut != null))  // code TVA au débit ou au crédit, mais pas aux deux ?
							{
								i += this.ExplodeForTVA (i);
								changed = true;
								continue;
							}
						}

						var numéroDébit  = this.dataAccessor.EditionLine[i].GetText (ColumnType.Débit);
						var numéroCrédit = this.dataAccessor.EditionLine[i].GetText (ColumnType.Crédit);

						if ((débit  != null && débit .CodeTVAParDéfaut != null && numéroCrédit == JournalDataAccessor.multi) ||
							(crédit != null && crédit.CodeTVAParDéfaut != null && numéroDébit  == JournalDataAccessor.multi))
						{
							i += this.ExplodeForTVA (i);
							changed = true;
							continue;
						}
					}
				}

				i++;
			}

			//	Crée une ligne vide s'il n'y en a plus ?
			if (this.CreateEmptyLine ())
			{
				changed = true;
			}

			//	Met à jour s'il y a eu un changement.
			if (changed)
			{
				this.UpdateEditorContent ();

				this.AdjustLineColumn (ref line, ref columnType);
				this.selectedLine = line;
				this.EditorSelect (columnType);
			}
		}

		private bool CreateEmptyLine()
		{
			//	Crée une ligne vide s'il n'en existe aucune.
			if (this.isMulti && this.dataAccessor.CountEmptyRow == 0 && this.settingsList.GetBool (SettingsType.EcritureProposeVide))
			{
				(this.dataAccessor as JournalDataAccessor).CreateEmptyLine ();
				return true;
			}

			return false;
		}

		protected override void UpdateAfterValidate()
		{
			//	Met à jour les décorations des champs (hachures grises aux champs d'une ligne vide et flèches BaseTVA -> CodeTVA).
			int count = System.Math.Min (this.fieldControllers.Count, this.dataAccessor.EditionLine.Count);

			for (int line = 0; line < count; line++)
			{
				for (int column = 0; column < this.fieldControllers[line].Count; column++)
				{
					this.fieldControllers[line][column].EmptyLineAdorner = false;
					this.fieldControllers[line][column].BaseTVAAdorner   = false;
					this.fieldControllers[line][column].CodeTVAAdorner   = false;
				}
			}

			for (int line = 0; line < count; line++)
			{
				bool emptyLine = (this.dataAccessor.EditionLine[line] as JournalEditionLine).IsEmptyLine;

				for (int column = 0; column < this.fieldControllers[line].Count; column++)
				{
					var columnType = this.GetMapperColumnType (column);
					this.fieldControllers[line][column].EmptyLineAdorner = emptyLine;

					var type = this.GetTypeEcriture (line);

					if (type == TypeEcriture.BaseTVA)
					{
						var origineTVA = this.dataAccessor.EditionLine[line].GetText (ColumnType.OrigineTVA);
						if ((columnType == ColumnType.Débit  && origineTVA == "D") ||
							(columnType == ColumnType.Crédit && origineTVA == "C"))
						{
							this.fieldControllers[line][column].BaseTVAAdorner = true;
						}
					}

					if (type == TypeEcriture.CodeTVA)
					{
						if (columnType == ColumnType.Débit || columnType == ColumnType.Crédit)
						{
							this.fieldControllers[line][column].CodeTVAAdorner = true;
						}
					}
				}
			}
		}

		protected override void UpdateEditionWidgets(int line, ColumnType columnType)
		{
			//	Met à jour toutes les données en édition.
			if (this.controller.IgnoreChanges.IsNotZero || !this.dataAccessor.EditionLine.Any ())
			{
				return;
			}

			this.UpdateEditorGeometry ();  // pour montrer/cacher les champs libellé/CodeTVA/TauxTVA

			var type = this.GetTypeEcriture (line);

			if (this.isMulti)
			{
				if (columnType == ColumnType.Débit ||
					columnType == ColumnType.Crédit)
				{
					this.MultiDébitCréditChanged (line, columnType);
				}
			}

			if ((type == TypeEcriture.Nouveau || type == TypeEcriture.Vide) && this.IsTVA (line))
			{
				this.NouveauMontant_TTC_HT_Changed (line, columnType);
			}

			if (columnType == ColumnType.Libellé)
			{
				this.LibelléChanged (line);
			}

			if (type == TypeEcriture.BaseTVA && line < this.dataAccessor.EditionLine.Count-1)
			{
				if (columnType == ColumnType.Débit ||
					columnType == ColumnType.Crédit)
				{
					this.BaseTVADébitCréditChanged (line);
				}

				if (columnType == ColumnType.MontantTTC)
				{
					this.MontantTTCChanged (line);
				}

				if (columnType == ColumnType.Montant)
				{
					this.MontantHTChanged (line);
				}
			}

			if (type == TypeEcriture.CodeTVA && line > 0)
			{
				if (columnType == ColumnType.CodeTVA)
				{
					this.CodeChanged (line-1);
				}

				if (columnType == ColumnType.TauxTVA)
				{
					this.TauxChanged (line-1);
				}

				if (columnType == ColumnType.Montant)
				{
					this.MontantTVAChanged (line-1);
				}
			}

			bool éditeCompteTVA  = this.settingsList.GetBool (SettingsType.EcritureEditeCompteTVA);
			bool éditeMontantHT  = this.settingsList.GetBool (SettingsType.EcritureEditeMontantHT);
			bool éditeCodeTVA    = this.settingsList.GetBool (SettingsType.EcritureEditeCodeTVA);
			bool éditeTauxTVA    = this.settingsList.GetBool (SettingsType.EcritureEditeTauxTVA);
			bool éditeMontantTVA = this.settingsList.GetBool (SettingsType.EcritureEditeMontantTVA);

			for (int i = 0; i < this.dataAccessor.EditionLine.Count; i++)
			{
				type = this.GetTypeEcriture (i);

				if (this.isMulti)
				{
					this.SetWidgetVisibility (ColumnType.Débit,  i, !this.IsDébitMulti  (i));
					this.SetWidgetVisibility (ColumnType.Crédit, i, !this.IsCréditMulti (i));

					bool totalAutomatique = (this.dataAccessor.EditionLine[i].GetText (ColumnType.TotalAutomatique) == "1");

					this.SetWidgetVisibility (ColumnType.Date,    i, totalAutomatique);
					this.SetWidgetVisibility (ColumnType.Journal, i, totalAutomatique);

					if (!this.PlusieursPièces)
					{
						this.SetWidgetVisibility (ColumnType.Pièce, i, totalAutomatique);
					}


					this.SetWidgetVisibility (ColumnType.CodeTVA, i, !totalAutomatique);
					this.SetWidgetVisibility (ColumnType.TauxTVA, i, !totalAutomatique);

					if (type == TypeEcriture.BaseTVA)
					{
						this.dataAccessor.GetEditionData (i, ColumnType.Débit).Enable = true;
						this.dataAccessor.GetEditionData (i, ColumnType.Crédit).Enable = true;
						this.dataAccessor.GetEditionData (i, ColumnType.MontantTTC).Enable = true;
						this.dataAccessor.GetEditionData (i, ColumnType.Montant).Enable = éditeMontantHT;
						this.dataAccessor.GetEditionData (i, ColumnType.CodeTVA).Enable = false;
						this.dataAccessor.GetEditionData (i, ColumnType.TauxTVA).Enable = false;
					}
					else if (type == TypeEcriture.CodeTVA)
					{
						this.dataAccessor.GetEditionData (i, ColumnType.Débit).Enable = éditeCompteTVA;
						this.dataAccessor.GetEditionData (i, ColumnType.Crédit).Enable = éditeCompteTVA;
						this.dataAccessor.GetEditionData (i, ColumnType.MontantTTC).Enable = false;
						this.dataAccessor.GetEditionData (i, ColumnType.Montant).Enable = éditeMontantTVA;
						this.dataAccessor.GetEditionData (i, ColumnType.CodeTVA).Enable = éditeCodeTVA;
						this.dataAccessor.GetEditionData (i, ColumnType.TauxTVA).Enable = éditeTauxTVA;
					}
					else
					{
						this.dataAccessor.GetEditionData (i, ColumnType.Débit).Enable = true;
						this.dataAccessor.GetEditionData (i, ColumnType.Crédit).Enable = true;
						this.dataAccessor.GetEditionData (i, ColumnType.MontantTTC).Enable = false;
						this.dataAccessor.GetEditionData (i, ColumnType.Montant).Enable = !totalAutomatique;
						this.dataAccessor.GetEditionData (i, ColumnType.CodeTVA).Enable = false;
						this.dataAccessor.GetEditionData (i, ColumnType.TauxTVA).Enable = false;
					}
				}
				else
				{
					this.SetWidgetVisibility (ColumnType.Date,    i, true);
					this.SetWidgetVisibility (ColumnType.Débit,   i, true);
					this.SetWidgetVisibility (ColumnType.Crédit,  i, true);
					this.SetWidgetVisibility (ColumnType.Pièce,   i, true);
					this.SetWidgetVisibility (ColumnType.Journal, i, true);
				}

				//	Décide de la visibilité du champ 'Montant TTC'.
				bool showTTC = false;

				if (type == TypeEcriture.BaseTVA)
				{
					showTTC = true;
				}
				else if ((type == TypeEcriture.Nouveau || type == TypeEcriture.Vide) && this.IsTVA (i))
				{
					showTTC = true;
				}

				//?if (this.GetWidgetVisibility (ColumnType.MontantTTC, i) != showTTC)
				{
					this.SetWidgetVisibility (ColumnType.MontantTTC, i, showTTC);

					if (showTTC)
					{
						var montantTTC = Converters.ParseMontant (this.dataAccessor.EditionLine[i].GetText (ColumnType.MontantTTC));
						if (!montantTTC.HasValue)
						{
							this.dataAccessor.EditionLine[i].SetText (ColumnType.MontantTTC, Converters.MontantToString (0));
						}
					}
				}
			}

			this.UpdateMultiEditionData ();  // recalcule le total

			for (int i = 0; i < this.dataAccessor.EditionLine.Count; i++)
			{
				type = this.GetTypeEcriture (i);

				if (type == TypeEcriture.BaseTVA)
				{
					this.UpdateAfterCompteOrigineTVAChanged (i);
					this.UpdateAfterCodeTVAChanged (i);
				}
			}
		}

		private void UpdateAfterCompteOrigineTVAChanged(int line)
		{
			//	Appelé lorsque le compte à l'origine de la TVA a changé, pour mettre à jour les codes TVA dans le menu.
			var field = this.GetFieldController (ColumnType.CodeTVA, line+1);

			var origineTVA = this.dataAccessor.EditionLine[line].GetText (ColumnType.OrigineTVA);
			ComptaCompteEntity compte = null;
			if (origineTVA == "D")
			{
				compte = this.GetCompteDébit (line);
			}
			if (origineTVA == "C")
			{
				compte = this.GetCompteCrédit (line);
			}

			if (field != null && compte != null)
			{
				var codesTVA = compte.CodesTVAMenuDescription;

				//	Si le compte ne définit aucun code TVA, on les propose tous.
				if (!codesTVA.Any ())
				{
					codesTVA = this.compta.CodesTVAMenuDescription;
				}

				UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, '#', codesTVA);
			}
		}

		private void UpdateAfterCodeTVAChanged(int line)
		{
			//	Appelé lorsque le code TVA a changé, pour mettre à jour les taux de TVA dans le menu.
			if (line > 0 && line < this.dataAccessor.EditionLine.Count)
			{
				(this.dataAccessor.EditionLine[line] as JournalEditionLine).UpdateCodeTVAParameters ();
			}
		}

		private void MultiDébitCréditChanged(int line, ColumnType columnType)
		{
			var débit  = this.dataAccessor.EditionLine[line].GetText (ColumnType.Débit);
			var crédit = this.dataAccessor.EditionLine[line].GetText (ColumnType.Crédit);

			if (columnType == ColumnType.Débit)
			{
				if (débit.IsNullOrEmpty)
				{
					this.dataAccessor.EditionLine[line].SetText (ColumnType.Crédit, FormattedText.Empty);
				}
				else
				{
					if (débit == JournalDataAccessor.multi)
					{
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Crédit, FormattedText.Empty);
					}
					else
					{
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Crédit, JournalDataAccessor.multi);
					}
				}
			}
			else if (columnType == ColumnType.Crédit)
			{
				if (crédit.IsNullOrEmpty)
				{
					this.dataAccessor.EditionLine[line].SetText (ColumnType.Débit, FormattedText.Empty);
				}
				else
				{
					if (crédit == JournalDataAccessor.multi)
					{
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Débit, FormattedText.Empty);
					}
					else
					{
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Débit, JournalDataAccessor.multi);
					}
				}
			}
		}

		private void NouveauMontant_TTC_HT_Changed(int line, ColumnType columnType)
		{
			//	Lorsqu'on crée une nouvelle ligne avec TVA, il faut permettre d'éditer soit le montant TTC,
			//	soit le montant HT, mais pas les deux.
			var montantTTC = Converters.ParseMontant (this.dataAccessor.EditionLine[line].GetText (ColumnType.MontantTTC));
			var montantHT  = Converters.ParseMontant (this.dataAccessor.EditionLine[line].GetText (ColumnType.Montant));

			if (columnType == ColumnType.MontantTTC)  // montant TTC ?
			{
				if (montantTTC.HasValue)
				{
					this.dataAccessor.EditionLine[line].SetText (ColumnType.Montant, FormattedText.Empty);  // vide le montant HT
				}
			}
			else if (columnType == ColumnType.Montant)  // montant HT ?
			{
				if (montantHT.HasValue)
				{
					this.dataAccessor.EditionLine[line].SetText (ColumnType.MontantTTC, FormattedText.Empty);  // vide le montant TTC
				}
			}
			else
			{
				if (montantTTC.GetValueOrDefault () == 0 && montantHT.GetValueOrDefault () == 0)
				{
					this.dataAccessor.EditionLine[line].SetText (ColumnType.Montant,    Converters.MontantToString (0));
					this.dataAccessor.EditionLine[line].SetText (ColumnType.MontantTTC, Converters.MontantToString (0));
				}
			}
		}

		private void LibelléChanged(int line)
		{
			var libellé = this.dataAccessor.EditionLine[line].GetText (ColumnType.Libellé);
			var auto = libellé.IsNullOrEmpty ? "1" : "0";
			this.dataAccessor.EditionLine[line].SetText (ColumnType.IsAutoLibellé, auto);

			if (this.UpdateMultiLibelléTVA ())
			{
				int count = this.dataAccessor.EditionLine.Count;
				this.GetFieldController (ColumnType.Libellé, count-1).EditionDataToWidget ();
			}
		}

		private void BaseTVADébitCréditChanged(int line)
		{
			var débit  = this.GetCompteDébit  (line);
			var crédit = this.GetCompteCrédit (line);

			var compte = (débit == null) ? crédit : débit;

			if (compte != null && compte.CodeTVAParDéfaut != null)
			{
				var codeTVA = this.dataAccessor.EditionLine[line+1].GetText (ColumnType.CodeTVA);

				if (!compte.CodesTVAPossibles.Where (x => x.Code == codeTVA).Any ())
				{
					this.dataAccessor.EditionLine[line+0].SetText (ColumnType.OrigineTVA, (compte == débit) ? "D" : "C");
					this.dataAccessor.EditionLine[line+1].SetText (ColumnType.OrigineTVA, (compte == débit) ? "D" : "C");
					this.dataAccessor.EditionLine[line+1].SetText (ColumnType.CodeTVA,    compte.CodeTVAParDéfaut.Code);
					this.CodeChanged (line);
					this.UpdateAfterCompteOrigineTVAChanged (line);
				}
			}
		}

		private void CodeChanged(int line)
		{
			var code = this.dataAccessor.EditionLine[line+1].GetText (ColumnType.CodeTVA).ToSimpleText ();
			var codeTVA = this.compta.CodesTVA.Where (x => x.Code == code).FirstOrDefault ();

			if (codeTVA == null)
			{
				this.dataAccessor.EditionLine[line+1].SetText (ColumnType.TauxTVA, Converters.PercentToString (0));
			}
			else
			{
				this.dataAccessor.EditionLine[line+1].SetText (ColumnType.TauxTVA, Converters.PercentToString (codeTVA.DefaultTauxValue));
			}

			this.TauxChanged (line);
		}

		private void TauxChanged(int line)
		{
			//	Si on a édité le montant HT, on recalcule le montant TTC, et inversément.
			//	Si on ne sait pas (modification d'une écriture existante), on recalcule
			//	toujours le montant HT.
			if (this.GetTypeMontantEdité (line) == "HT")
			{
				this.UpdateMontantTTC (line);
			}
			else
			{
				this.UpdateMontantHT (line);
			}

			this.UpdateMontantTVA (line);
		}

		private void MontantTVAChanged(int line)
		{
			var montantTTC = Converters.ParseMontant (this.dataAccessor.EditionLine[line+0].GetText (ColumnType.MontantTTC));
			var montantTVA = Converters.ParseMontant (this.dataAccessor.EditionLine[line+1].GetText (ColumnType.Montant));

			if (montantTTC.HasValue && montantTVA.HasValue)
			{
				var montantHT = montantTTC.Value - montantTVA.Value;
				this.dataAccessor.EditionLine[line+0].SetText (ColumnType.Montant, Converters.MontantToString (montantHT));
			}
		}

		private void MontantHTChanged(int line)
		{
			this.SetTypeMontantEdité (line, "HT");
			this.UpdateMontantTTC (line);
			this.UpdateMontantTVA (line);
		}

		private void MontantTTCChanged(int line)
		{
			this.SetTypeMontantEdité (line, "TTC");
			this.UpdateMontantHT (line);
			this.UpdateMontantTVA (line);
		}

		private void UpdateMontantHT(int line)
		{
			var montantTTC = Converters.ParseMontant (this.dataAccessor.EditionLine[line+0].GetText (ColumnType.MontantTTC));
			var tauxTVA    = Converters.ParsePercent (this.dataAccessor.EditionLine[line+1].GetText (ColumnType.TauxTVA));

			if (montantTTC.HasValue && tauxTVA.HasValue)
			{
				var montantHT = TVA.CalculeHT (montantTTC.Value, tauxTVA.Value);
				this.dataAccessor.EditionLine[line+0].SetText (ColumnType.Montant, Converters.MontantToString (montantHT));
			}
		}

		private void UpdateMontantTTC(int line)
		{
			var montantHT = Converters.ParseMontant (this.dataAccessor.EditionLine[line+0].GetText (ColumnType.Montant));
			var tauxTVA   = Converters.ParsePercent (this.dataAccessor.EditionLine[line+1].GetText (ColumnType.TauxTVA));

			if (montantHT.HasValue && tauxTVA.HasValue)
			{
				var montantTTC = TVA.CalculeTTC (montantHT.Value, tauxTVA.Value);
				this.dataAccessor.EditionLine[line+0].SetText (ColumnType.MontantTTC, Converters.MontantToString (montantTTC));
			}
		}

		private void UpdateMontantTVA(int line)
		{
			var montantHT  = Converters.ParseMontant (this.dataAccessor.EditionLine[line+0].GetText (ColumnType.Montant));
			var montantTTC = Converters.ParseMontant (this.dataAccessor.EditionLine[line+0].GetText (ColumnType.MontantTTC));

			if (montantHT.HasValue && montantTTC.HasValue)
			{
				var montantTVA = montantTTC.Value - montantHT.Value;
				this.dataAccessor.EditionLine[line+1].SetText (ColumnType.Montant, Converters.MontantToString (montantTVA));
			}
		}


		private void UpdateMultiEditionData()
		{
			//	Recalcule le total de l'écriture multiple.
			if (!this.isMulti)
			{
				return;
			}

			int cp = this.IndexTotalAutomatique;
			if (cp != -1)
			{
				decimal totalDébit  = 0;
				decimal totalCrédit = 0;

				for (int line = 0; line < this.dataAccessor.EditionLine.Count; line++ )
				{
					var type  = this.GetTypeEcriture (line);
					var type2 = this.GetTypeEcriture (line+1);

					if (type == TypeEcriture.BaseTVA && type2 == TypeEcriture.CodeTVA)  // ligne BaseTVA suivie d'une CodeTVA ?
					{
						var code = this.dataAccessor.EditionLine[line+1].GetText (ColumnType.CodeTVA).ToSimpleText ();
						var taux = this.dataAccessor.EditionLine[line+1].GetText (ColumnType.TauxTVA);

						this.dataAccessor.EditionLine[line+0].SetText (ColumnType.CodeTVA, code);
						this.dataAccessor.EditionLine[line+0].SetText (ColumnType.TauxTVA, taux);
						this.dataAccessor.EditionLine[line+0].SetText (ColumnType.MontantComplément, this.dataAccessor.EditionLine[line+1].GetText (ColumnType.Montant));
						this.dataAccessor.EditionLine[line+1].SetText (ColumnType.MontantComplément, this.dataAccessor.EditionLine[line+0].GetText (ColumnType.Montant));

						//	Le libellé de la ligne CodeTVA reprend toujours celui de la ligne BaseTVA. Selon les présentations,
						//	on affichera directement ce libellé, ou autre chose de calculé. Par exemple, dans un journal, on
						//	désire voir un texte qui résumé la TVA (code et taux), alors que dans un extrait de compte, le
						//	libellé natif convient parfaitement.
						this.dataAccessor.EditionLine[line+1].SetText (ColumnType.Libellé, this.dataAccessor.EditionLine[line+0].GetText (ColumnType.Libellé));
					}

					if (line != cp)
					{
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Date,    this.dataAccessor.EditionLine[cp].GetText (ColumnType.Date));
						this.dataAccessor.EditionLine[line].SetText (ColumnType.Journal, this.dataAccessor.EditionLine[cp].GetText (ColumnType.Journal));

						if (!this.PlusieursPièces)
						{
							this.dataAccessor.EditionLine[line].SetText (ColumnType.Pièce, this.dataAccessor.EditionLine[cp].GetText (ColumnType.Pièce));
						}

						decimal montant = Converters.ParseMontant (this.dataAccessor.EditionLine[line].GetText (ColumnType.Montant)).GetValueOrDefault ();

						if (this.IsDébitMulti (line))
						{
							totalCrédit += montant;
						}

						if (this.IsCréditMulti (line))
						{
							totalDébit += montant;
						}
					}
				}

				decimal? total = 0;

				if (this.IsDébitMulti (cp))
				{
					total = totalDébit - totalCrédit;
				}

				if (this.IsCréditMulti (cp))
				{
					total = totalCrédit - totalDébit;
				}

				this.dataAccessor.EditionLine[cp].SetText (ColumnType.Montant, Converters.MontantToString (total));
				this.dataAccessor.EditionLine[cp].SetText (ColumnType.CodeTVA, null);
			}

			this.UpdateMultiLibelléTVA ();
		}

		private bool UpdateMultiLibelléTVA()
		{
			//	Si on est dans de cas d'une écriture multiple avec TVA, recopie le libellé de la première ligne
			//	dans celui de la dernière.
			int count = this.dataAccessor.EditionLine.Count;

			if (this.dataAccessor.CountEditedRowWithoutEmpty == 3 &&
				this.GetTypeEcriture (0) == TypeEcriture.BaseTVA &&
				this.GetTypeEcriture (1) == TypeEcriture.CodeTVA &&
				this.dataAccessor.EditionLine[count-1].GetText (ColumnType.IsAutoLibellé) == "1")  // libellé jamais entré manuellement ?
			{
				this.dataAccessor.EditionLine[count-1].SetText (ColumnType.Libellé, this.dataAccessor.EditionLine[0].GetText (ColumnType.Libellé));
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool IsDébitMulti(int line)
		{
			return this.dataAccessor.EditionLine[line].GetText (ColumnType.Débit) == JournalDataAccessor.multi;
		}

		private bool IsCréditMulti(int line)
		{
			return this.dataAccessor.EditionLine[line].GetText (ColumnType.Crédit) == JournalDataAccessor.multi;
		}

		private bool IsTVA(int line)
		{
			//	Retourne true si le compte au débit ou au crédit a un code TVA, mais pas les deux.
			return this.IsDébitTVA (line) ^ this.IsCréditTVA (line);
		}

		private bool IsDébitTVA(int line)
		{
			//	Retourne true si le compte au débit a un code TVA.
			var compte = this.GetCompteDébit (line);
			return compte != null && compte.CodeTVAParDéfaut != null;
		}

		private bool IsCréditTVA(int line)
		{
			//	Retourne true si le compte au crédit a un code TVA.
			var compte = this.GetCompteCrédit (line);
			return compte != null && compte.CodeTVAParDéfaut != null;
		}

		private int IndexTotalAutomatique
		{
			//	Retourne l'index de la ligne qui contient le total automatique.
			get
			{
				return this.dataAccessor.EditionLine.FindIndex (x => x.GetText (ColumnType.TotalAutomatique) == "1");
			}
		}


		protected override void UpdateAfterSelectedLineChanged()
		{
#if false
			int sel = this.multiFirstRow + this.selectedLine;

			if (this.arrayController.SelectedRow != sel)
			{
				this.controller.IgnoreChanged = true;
				this.arrayController.SelectedRow = sel;
				this.controller.IgnoreChanged = false;
			}
#endif

			this.UpdateToolbar ();
			this.UpdateInsertionRow ();
			this.UpdateEditorInfo ();
		}


		public override void UpdateEditorContent()
		{
			this.UpdateArrayColumns ();

			int count = this.dataAccessor.CountEditedRow;
			this.selectedLine = System.Math.Min (this.selectedLine, count-1);
			this.isMulti = (count > 1);

			base.UpdateEditorContent ();
		}


		public override void UpdateEditorGeometry()
		{
			base.UpdateEditorGeometry ();

			double w1 = this.arrayController.GetColumnsAbsoluteWidth (0);
			double w2 = this.arrayController.GetColumnsAbsoluteWidth (1);
			double w3 = this.arrayController.GetColumnsAbsoluteWidth (2);

			this.débitInfoSeparator.Margins  = new Margins (w1, 0, 0, 0);
			this.débitInfoSeparator.PreferredWidth  = w2-1;
			this.créditInfoSeparator.PreferredWidth = w3-1;

			this.débitInfoFrame.PreferredWidth  = w1+w2;
			this.créditInfoFrame.PreferredWidth = w1+w2-1;
		}

		protected override bool GetColumnGeometry(int line, ColumnType columnType, out double left, out double width)
		{
			left = width = 0;

			var type = this.GetTypeEcriture (line);

			if (type == TypeEcriture.CodeTVA)
			{
				if (columnType == ColumnType.Libellé)
				{
					return false;  // cache le libellé
				}
				else if (columnType == ColumnType.LibelléTVA ||
						 columnType == ColumnType.CodeTVA    ||
						 columnType == ColumnType.TauxTVA    )
				{
					this.arrayController.GetColumnGeometry (ColumnType.Libellé, out left, out width);

					double codeWidth = System.Math.Min (100, System.Math.Floor (width/3));
					double tauxWidth = System.Math.Min ( 55, System.Math.Floor (width/3));

					if (columnType == ColumnType.LibelléTVA)
					{
						width -= codeWidth + tauxWidth;
					}
					else if (columnType == ColumnType.CodeTVA)
					{
						left += width - tauxWidth - codeWidth;
						width = codeWidth;
					}
					else
					{
						left += width - tauxWidth;
						width = tauxWidth;
					}

					return true;
				}
				else
				{
					return this.arrayController.GetColumnGeometry (columnType, out left, out width);
				}
			}
			else
			{
				if (columnType == ColumnType.LibelléTVA ||
					columnType == ColumnType.CodeTVA    ||
					columnType == ColumnType.TauxTVA    )
				{
					return false;  // cache les champs pour la TVA
				}
				else
				{
					return this.arrayController.GetColumnGeometry (columnType, out left, out width);
				}
			}
		}

		protected override void UpdateArrayColumns()
		{
			//	Si nécessaire, adapte l'interface pour accueillir le nombre de lignes et de colonnes requis.
			int count = this.dataAccessor.CountEditedRow;

			if (this.linesFrames.Count != count ||
				this.fieldControllers[0].Count != this.columnMappers.Where (x => x.Show).Count ())
			{
				this.ResetLineUI ();  // crée la première ligne

				for (int i = 0; i < count-1; i++)
				{
					this.CreateLineUI (this.linesContainer);  // crée les lignes suivantes
				}

				this.UpdateEditorGeometry ();
			}

			this.UpdateAfterFirstLineChanged ();
		}

		protected override void SelectedLineChanged()
		{
			//	Appelé lorsque la ligne sélectionnée a changé. On détermine ici la première ligne affichée,
			//	afin de montrer la ligne sélectionnée au mieux.
			int visibleLines = System.Math.Min (this.dataAccessor.CountEditedRow, this.maxLines);

			int first = this.selectedLine;
			first = System.Math.Min (first + visibleLines/2, this.dataAccessor.CountEditedRow-1);
			first = System.Math.Max (first - (visibleLines-1), 0);

			if (this.firstLine != first)
			{
				this.firstLine = first;
				this.UpdateAfterFirstLineChanged ();
			}
		}

		private void ChangeScroller()
		{
			//	Appelé lorsque l'ascenseur a été bougé.
			int value = (int) this.scroller.Value;

			if (this.firstLine != value)
			{
				this.firstLine = value;
				this.UpdateAfterFirstLineChanged ();
			}
		}

		protected override void UpdateAfterFirstLineChanged()
		{
			//	Met à jour les lignes visibles.
			this.firstLine = System.Math.Min (this.firstLine, this.dataAccessor.CountEditedRow - this.maxLines);
			this.firstLine = System.Math.Max (this.firstLine, 0);

			for (int i = 0; i < this.linesFrames.Count; i++)
			{
				this.linesFrames[i].Visibility = (i >= this.firstLine && i < this.firstLine+this.maxLines);
			}

			//	Met à jour l'ascenseur.
			if (this.dataAccessor.CountEditedRow > this.maxLines)
			{
				this.scroller.Visibility = true;

				decimal totalHeight = this.dataAccessor.CountEditedRow;
				decimal areaHeight = this.maxLines;

				this.scroller.MaxValue          = totalHeight-areaHeight;
				this.scroller.VisibleRangeRatio = areaHeight/totalHeight;
				this.scroller.Value             = this.firstLine;
				this.scroller.SmallChange       = 1;
				this.scroller.LargeChange       = areaHeight/2;
			}
			else
			{
				this.scroller.Visibility = false;
			}
		}


		protected override void UpdateEditorInfo()
		{
			this.infoFrameSeparator.Visibility = this.ShowInfoPanel;
			this.infoFrameBox.Visibility       = this.ShowInfoPanel;

			if (this.ShowInfoPanel)
			{
				this.UpdateEditorInfo (this.dataAccessor.GetEditionText (this.selectedLine, ColumnType.Débit ), isDébit: true);
				this.UpdateEditorInfo (this.dataAccessor.GetEditionText (this.selectedLine, ColumnType.Crédit), isDébit: false);
			}
		}

		private void UpdateEditorInfo(FormattedText numéro, bool isDébit)
		{
			FormattedText title;
			decimal? solde;

			this.GetInfoCompte (numéro, out title, out solde);
			UIBuilder.UpdateInfoCompte (isDébit ? this.débitInfoFrame : this.créditInfoFrame, title, solde);
		}

		public override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			bool enable = this.dataAccessor.IsActive;
			int count = this.linesFrames.Count;

			this.controller.SetCommandEnable (Res.Commands.Multi.Insert,    this.IsCommandInsertEnable);
			this.controller.SetCommandEnable (Res.Commands.Multi.InsertTVA, this.IsCommandInsertTVAEnable);
			this.controller.SetCommandEnable (Res.Commands.Multi.Delete,    this.IsCommandDeleteEnable);
			this.controller.SetCommandEnable (Res.Commands.Multi.Up,        enable && count >  1 && this.selectedLine > 0);
			this.controller.SetCommandEnable (Res.Commands.Multi.Down,      enable && count >  1 && this.selectedLine < count-1);
			this.controller.SetCommandEnable (Res.Commands.Multi.Swap,      enable && count != 0 && this.selectedLine != -1);
			this.controller.SetCommandEnable (Res.Commands.Multi.Auto,      this.IsCommandAutoEnable);
		}

		private bool IsCommandInsertEnable
		{
			get
			{
				if (!this.dataAccessor.IsActive)
				{
					return false;
				}

				if (this.dataAccessor.CountEditedRowWithoutEmpty <= 1)
				{
					return false;
				}

				if (this.GetTypeEcriture (this.selectedLine) == TypeEcriture.BaseTVA ||
					this.GetTypeEcriture (this.selectedLine) == TypeEcriture.CodeTVA)
				{
					return false;
				}

				return true;
			}
		}

		private bool IsCommandInsertTVAEnable
		{
			get
			{
				if (!this.dataAccessor.IsActive)
				{
					return false;
				}

#if false
				if (this.dataAccessor.CountEditedRowWithoutEmpty <= 1)
				{
					return false;
				}
#endif

				if (this.GetTypeEcriture (this.selectedLine) != TypeEcriture.Normal)
				{
					return false;
				}

				if (this.selectedLine == this.IndexTotalAutomatique)
				{
					return false;
				}

				return true;
			}
		}

		private bool IsCommandDeleteEnable
		{
			get
			{
				if (!this.dataAccessor.IsActive)
				{
					return false;
				}

				if (this.dataAccessor.CountEditedRowWithoutEmpty <= 2)
				{
					return false;
				}

				var type = this.GetTypeEcriture (this.selectedLine);

				if (type == TypeEcriture.Vide   ||
					type == TypeEcriture.CodeTVA)
				{
					return true;
				}

				if (this.dataAccessor.CountEditedRowWithoutEmpty == 3 &&
					this.GetTypeEcriture(0) == TypeEcriture.BaseTVA   &&
					this.GetTypeEcriture(1) == TypeEcriture.CodeTVA   )
				{
					return false;
				}

				if (this.selectedLine == this.IndexTotalAutomatique)
				{
					return false;
				}

				return true;
			}
		}

		private bool IsCommandAutoEnable
		{
			get
			{
				if (!this.dataAccessor.IsActive)
				{
					return false;
				}

				if (this.dataAccessor.CountEditedRowWithoutEmpty <= 1)
				{
					return false;
				}

				if (this.GetTypeEcriture (this.selectedLine) != TypeEcriture.Normal)
				{
					return false;
				}

				if (this.selectedLine == this.IndexTotalAutomatique)
				{
					return false;
				}

				return true;
			}
		}


		protected override FormattedText EditionDescription
		{
			get
			{
				int n = this.dataAccessor.CountEditedRow;

				if (n <= 1)
				{
					return FormattedText.Empty;
				}
				else
				{
					return FormattedText.Concat (n.ToString (), " lignes");
				}
			}
		}


		private void SetTypeEcriture(int line, TypeEcriture type)
		{
			if (line >= 0 && line < this.dataAccessor.EditionLine.Count)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Type, Converters.TypeEcritureToString (type));
			}
		}

		private TypeEcriture GetTypeEcriture(int line)
		{
			var type = TypeEcriture.Normal;

			if (line >= 0 && line < this.dataAccessor.EditionLine.Count)
			{
				type = Converters.StringToTypeEcriture (this.dataAccessor.EditionLine[line].GetText (ColumnType.Type));
			}

			return type;
		}


		private void SetTypeMontantEdité(int line, string type)
		{
			if (line >= 0 && line < this.dataAccessor.EditionLine.Count)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.MontantEdité, type);
			}
		}

		private string GetTypeMontantEdité(int line)
		{
			if (line >= 0 && line < this.dataAccessor.EditionLine.Count)
			{
				return this.dataAccessor.EditionLine[line].GetText (ColumnType.MontantEdité).ToString ();
			}
			else
			{
				return null;
			}
		}


		private ComptaCompteEntity GetCompteDébit(int line)
		{
			return this.compta.PlanComptable.Where (x => x.Numéro == this.dataAccessor.EditionLine[line].GetText (ColumnType.Débit)).FirstOrDefault ();
		}

		private ComptaCompteEntity GetCompteCrédit(int line)
		{
			return this.compta.PlanComptable.Where (x => x.Numéro == this.dataAccessor.EditionLine[line].GetText (ColumnType.Crédit)).FirstOrDefault ();
		}

		private void GetInfoCompte(FormattedText numéro, out FormattedText titre, out decimal? solde)
		{
			var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();

			if (compte == null)
			{
				titre = FormattedText.Empty;
				solde = null;
			}
			else
			{
				titre = compte.Titre;
				solde = this.dataAccessor.SoldesJournalManager.GetSolde (compte);
			}
		}

		private bool PlusieursPièces
		{
			//	Retourne true si les écritures multiples peuvent avoir une pièce par ligne.
			get
			{
				return this.settingsList.GetBool (SettingsType.EcriturePlusieursPièces);
			}
		}


		#region Menu des écritures modèles
		private void CreateButtonModèleUI(AbstractFieldController fieldController, int line)
		{
			if (this.compta.Modèles.Any ())
			{
				var button = new Button
				{
					Parent          = fieldController.Box,
					ButtonStyle     = ButtonStyle.Icon,
					Text            = "M",
					Index           = line,
					PreferredWidth  = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock            = DockStyle.Right,
					Margins         = new Margins (-1, 0, 0, 0),
				};

				button.Clicked += delegate
				{
					this.ShowMenuModèles (button, button.Index);
				};

				ToolTip.Default.SetToolTip (button, "Choix d'une écriture modèle");
			}
		}

		private void ShowMenuModèles(Widget parentButton, int line)
		{
			//	Affiche le menu permettant de choisir le mode.
			var menu = new VMenu ();

			int index = 0;
			foreach (var modèle in this.compta.Modèles)
			{
				var item = new MenuItem ()
				{
					FormattedText = modèle.ShortSummary,
					Index         = index++,
				};

				item.Clicked += delegate
				{
					this.InsertModèle (this.compta.Modèles[item.Index], line);
				};

				menu.Items.Add (item);
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void InsertModèle(ComptaModèleEntity modèle, int line)
		{
			if (modèle.Débit != null)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Débit, modèle.Débit.Numéro);
			}

			if (modèle.Crédit != null)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Crédit, modèle.Crédit.Numéro);
			}

			if (!modèle.Pièce.IsNullOrEmpty)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Pièce, modèle.Pièce);
			}

			int cursor = -1;
			if (!modèle.Libellé.IsNullOrEmpty)
			{
				cursor = modèle.Libellé.ToString ().IndexOf ("@");
				var m = modèle.Libellé.ToString ().Replace ("@", "");
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Libellé, m);
			}

			if (modèle.Montant.HasValue)
			{
				this.dataAccessor.EditionLine[line].SetText (ColumnType.Montant, Converters.MontantToString (modèle.Montant));
			}

			this.UpdateEditorContent ();
			this.EditorSelect (ColumnType.Libellé);

			var fc = this.GetFieldController (ColumnType.Libellé, line);
			var field = fc.EditWidget as AbstractTextField;
			field.Focus ();
			field.Cursor = (cursor == -1) ? field.Text.Length : cursor;
		}
		#endregion


		private CustomFrameBox						footer;
		private TabCatcherFrameBox					linesContainer;
		private FrameBox							infoFrameSeparator;
		private FrameBox							infoFrameBox;
		private Separator							débitInfoSeparator;
		private Separator							créditInfoSeparator;
		private FrameBox							débitInfoFrame;
		private FrameBox							créditInfoFrame;
		private VScroller							scroller;

		private bool								isMulti;
	}
}
