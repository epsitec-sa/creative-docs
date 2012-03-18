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
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Fields.Controllers
{
	/// <summary>
	/// Contrôleur spécialisé permettant l'édition d'une liste de taux de TVA.
	/// </summary>
	public class ListeTVAController : AbstractFieldController
	{
		public ListeTVAController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> clearFocusAction, System.Action<int, ColumnType> setFocusAction, System.Action<int, ColumnType> contentChangedAction)
			: base (controller, line, columnMapper, clearFocusAction, setFocusAction, contentChangedAction)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateLabelUI (parent);
			this.CreateBoxUI (parent);
			this.box.DrawFullFrame = false;

			this.container = new FrameBox
			{
				Parent   = this.box,
				Dock     = DockStyle.Fill,
				TabIndex = 1,
			};

			var toolbar = UIBuilder.CreateMiniToolbar (this.container);

			this.addButton = new GlyphButton
			{
				Parent         = toolbar,
				GlyphShape     = GlyphShape.Plus,
				PreferredWidth = 30,
				Dock           = DockStyle.Left,
			};

			this.removeButton = new GlyphButton
			{
				Parent     = toolbar,
				GlyphShape = GlyphShape.Minus,
				Dock       = DockStyle.Left,
				Margins    = new Margins (1, 0, 0, 0),
			};

			this.scrollList = new ScrollList
			{
				Parent          = this.container,
				PreferredHeight = 200,
				RowHeight       = 20,
				Dock            = DockStyle.Top,
			};

			this.editionFrame = new FrameBox
			{
				Parent = this.container,
				Dock   = DockStyle.Top,
				Enable = false,
			};

			{
				var line = new FrameBox
				{
					Parent          = this.editionFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 5, 0),
				};

				this.defaultRadio = new RadioButton
				{
					Parent          = line,
					Text            = "Taux actuellement en vigueur",
					PreferredHeight = 20,
					AutoToggle      = false,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (3, 0, 0, 0),
				};
			}

			{
				var line = new FrameBox
				{
					Parent          = this.editionFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 5, 0),
				};

				new StaticText
				{
					Parent           = line,
					Text             = "Du",
					PreferredWidth   = 40,
					ContentAlignment = ContentAlignment.MiddleRight,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 5, 0, 0),
				};

				this.dateField = new TextFieldEx
				{
					Parent                       = line,
					PreferredWidth               = 100,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Left,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};
			}

			{
				var line = new FrameBox
				{
					Parent          = this.editionFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
					Margins          = new Margins (0, 0, -1, 0),
				};

				new StaticText
				{
					Parent           = line,
					Text             = "Taux",
					PreferredWidth   = 40,
					ContentAlignment = ContentAlignment.MiddleRight,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 5, 0, 0),
				};

				this.rateField = new TextFieldEx
				{
					Parent                       = line,
					PreferredWidth               = 100,
					PreferredHeight              = 20,
					Dock                         = DockStyle.Left,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};
			}

			ToolTip.Default.SetToolTip (this.addButton, "Ajoute un nouveau taux dans la liste");
			ToolTip.Default.SetToolTip (this.removeButton, "Supprime le taux sélectionné dans la liste");

			//	Connexion ds événements.
			this.addButton.Clicked += delegate
			{
				this.AddAction ();
			};

			this.removeButton.Clicked += delegate
			{
				this.RemoveAction ();
			};

			this.scrollList.SelectedItemChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.SelectList (this.scrollList.SelectedItemIndex);
					this.UpdateButtons ();
				}
			};

			this.defaultRadio.Clicked += delegate
			{
				this.ChangeDefault ();
			};

			this.dateField.EditionAccepted += delegate
			{
				this.ChangeDate ();
			};

			this.rateField.EditionAccepted += delegate
			{
				this.ChangeRate ();
			};

			base.CreateForegroundUI ();
		}


		public override void EditionDataToWidget()
		{
			if (this.editionData != null)
			{
				using (this.ignoreChanges.Enter ())
				{
					this.UpdateScrollList ();
				}
			}
		}

		public override void WidgetToEditionData()
		{
			if (this.editionData != null)
			{
			}
		}

		public override void Validate()
		{
			if (this.editionData == null)
			{
				return;
			}

			this.WidgetToEditionData ();
		}


		private void AddAction()
		{
			int row = this.Array.RowCount;
			this.Array.InsertRow (row);

			var date1 = new Date (Date.Today.Year+1, 1, 1);  // le premier janvier de l'année suivante
			var date2 = this.Array.GetDate (row-1, ColumnType.Date).GetValueOrDefault ();
			var date3 = new Date (date2.Year+1, 1, 1);
			var date = new Date (System.Math.Max (date1.Ticks, date3.Ticks));

			var rate = this.Array.GetPercent (row-1, ColumnType.Taux).GetValueOrDefault () + 0.01m;  // le taux précédent +1%

			this.Array.SetDate    (row, ColumnType.Date, date);
			this.Array.SetPercent (row, ColumnType.Taux, rate);

			this.UpdateScrollList (row);
			this.ContentChangedAction ();

			this.rateField.SelectAll ();  // sélectionne le taux, qu'il faudra assurément changer
			this.rateField.Focus ();
		}

		private void RemoveAction()
		{
			int sel = this.scrollList.SelectedItemIndex;

			this.Array.RemoveRow (sel);

			this.UpdateScrollList (sel);
			this.ContentChangedAction ();
		}

		private void ChangeDefault()
		{
			int sel = this.scrollList.SelectedItemIndex;

			for (int row = 0; row < this.Array.RowCount; row++)
			{
				this.Array.SetBool (row, ColumnType.ParDéfaut, row == sel);
			}

			this.UpdateScrollList ();
			this.ContentChangedAction ();
		}

		private void ChangeDate()
		{
			var date = Converters.ParseDate(this.dateField.Text);

			int sel = this.scrollList.SelectedItemIndex;

			Date? prev = new Date (1900, 1, 1);
			if (sel > 0)
			{
				prev = this.Array.GetDate (sel-1, ColumnType.Date);
			}

			Date? next = new Date (2099, 1, 1);
			if (sel < this.Array.RowCount-1)
			{
				next = this.Array.GetDate (sel+1, ColumnType.Date);
			}

			if (date.HasValue && date.Value > prev.Value && date.Value < next.Value)
			{
				this.Array.SetDate (sel, ColumnType.Date, date.Value);
				this.UpdateScrollList ();
				this.ContentChangedAction ();
			}
			else
			{
				this.dateField.SetError (true);
				ToolTip.Default.SetToolTip (this.dateField, "La date n'est pas correcte");
			}
		}

		private void ChangeRate()
		{
			var rate = Converters.ParsePercent (this.rateField.Text);

			if (rate.HasValue && rate.Value < 0.5m)  // le taux ne devrait jamais dépasser 50% !
			{
				int sel = this.scrollList.SelectedItemIndex;
				this.Array.SetPercent (sel, ColumnType.Taux, rate.Value);
				this.UpdateScrollList ();
				this.ContentChangedAction ();
			}
			else
			{
				this.rateField.SetError (true);
				ToolTip.Default.SetToolTip (this.rateField, "Le taux n'est pas correct");
			}
		}


		private void UpdateScrollList(int? sel = null)
		{
			if (!sel.HasValue)
			{
				sel = this.scrollList.SelectedItemIndex;
			}

			this.scrollList.Items.Clear ();

			for (int row = 0; row < this.Array.RowCount; row++)
			{
				this.scrollList.Items.Add (this.GetRowSummary (row));
			}

			using (this.ignoreChanges.Enter ())
			{
				if (sel.Value >= this.scrollList.Items.Count)
				{
					sel = this.scrollList.Items.Count-1;
				}

				this.scrollList.SelectedItemIndex = sel.Value;
				this.scrollList.ShowSelected (ScrollShowMode.Extremity);
			}

			this.SelectList (sel.Value);
			this.UpdateButtons ();
		}

		private void SelectList(int sel)
		{
			if (sel == -1)
			{
				this.editionFrame.Enable = false;
			}
			else
			{
				this.editionFrame.Enable = true;

				using (this.ignoreChanges.Enter ())
				{
					this.defaultRadio.ActiveState = this.Array.GetBool (sel, ColumnType.ParDéfaut) ? ActiveState.Yes : ActiveState.No;
					this.dateField.FormattedText = this.Array.GetText (sel, ColumnType.Date);
					this.rateField.FormattedText = this.Array.GetText (sel, ColumnType.Taux);
					this.dateField.SetError (false);
					this.rateField.SetError (false);

					ToolTip.Default.ClearToolTip (this.dateField);
					ToolTip.Default.ClearToolTip (this.rateField);
				}
			}
		}

		private void UpdateButtons()
		{
			int sel = this.scrollList.SelectedItemIndex;

			if (sel != -1 && this.Array.RowCount > 1 && !this.Array.GetBool (sel, ColumnType.ParDéfaut))
			{
				this.removeButton.Enable = true;
			}
			else
			{
				this.removeButton.Enable = false;
			}
		}


		private FormattedText GetRowSummary(int row)
		{
			//	Retourne le résumé d'une ligne pour peupler la ScrollList.
			string icon = UIBuilder.GetRadioStateIconUri (this.Array.GetBool (row, ColumnType.ParDéfaut));
			icon = string.Format (@"<img src=""{0}"" voff=""-5""/>", UIBuilder.GetResourceIconUri (icon));

			var date = this.Array.GetText (row, ColumnType.Date);
			var rate = this.Array.GetText (row, ColumnType.Taux);

			return FormattedText.Concat (icon, "     ", date, "     ", rate);
		}


		private DataArray Array
		{
			get
			{
				return this.editionData.Array;
			}
		}


		private GlyphButton						addButton;
		private GlyphButton						removeButton;
		private ScrollList						scrollList;
		private FrameBox						editionFrame;
		private RadioButton						defaultRadio;
		private TextFieldEx						dateField;
		private TextFieldEx						rateField;
	}
}
