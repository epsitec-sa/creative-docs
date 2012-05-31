//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Controllers
{
	public class SoldesColumnController
	{
		public SoldesColumnController(AbstractController controller)
		{
			this.controller = controller;

			this.soldesColumn = new SoldesColumn ();
			this.ignoreChanges = new SafeCounter ();
		}


		public SoldesColumn SoldesColumn
		{
			get
			{
				return this.soldesColumn;
			}
			set
			{
				this.NuméroCompte           = value.NuméroCompte;
				this.soldesColumn.DateDébut = value.DateDébut;

				if (this.soldesColumn.DateDébut.Year == 1)
				{
					this.soldesColumn.DateDébut = this.controller.MainWindowController.Période.DateDébut;
				}

				this.UpdateWidgets ();
			}
		}

		public int Index
		{
			get;
			private set;
		}

		public bool Visibility
		{
			get
			{
				return this.frame.Visibility;
			}
			set
			{
				this.frame.Visibility = value;
			}
		}

		public FrameBox CreateUI(FrameBox parent, int index, System.Action<int> addSubClicked, System.Action<int> dataChanged)
		{
			this.Index = index;
			this.dataChanged = dataChanged;

			this.frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 0),
			};

			new StaticText
			{
				Parent           = this.frame,
				FormattedText    = "Compte",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = 40,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			this.compteController = UIBuilder.CreateAutoCompleteField (this.controller, this.frame, this.NuméroCompte, "Compte", this.ValidateCompteAction, this.CompteChangedAction);
			this.compteController.Box.Dock = DockStyle.Left;

			new StaticText
			{
				Parent           = this.frame,
				FormattedText    = "Début",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = 40,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			this.dateField = new TextFieldEx
			{
				Parent                       = this.frame,
				PreferredWidth               = 90,
				PreferredHeight              = 20,
				Dock                         = DockStyle.Left,
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				TabIndex                     = 2,
			};

			this.addSubButton = new GlyphButton
			{
				Parent          = this.frame,
				GlyphShape      = this.Index == 0 ? GlyphShape.Plus : GlyphShape.Minus,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (5, 0, 0, 0),
			};

			this.dateField.EditionAccepted += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					var date = Converters.ParseDate (this.dateField.Text);
					if (date.HasValue)
					{
						this.soldesColumn.DateDébut = date.Value;
						this.UpdateWidgets ();
						this.dataChanged (this.Index);
					}
				}
			};

			this.addSubButton.Clicked += delegate
			{
				addSubClicked (this.Index);
			};

			this.UpdateComptes ();
			this.UpdateWidgets ();

			return this.frame;
		}

		private void UpdateComptes()
		{
			var comptes = this.controller.ComptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal || x.Type == TypeDeCompte.TVA);
			UIBuilder.UpdateAutoCompleteTextField (this.compteController.EditWidget as AutoCompleteTextField, comptes);
		}

		private void UpdateWidgets()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.compteController.EditionData.Text = this.NuméroCompte;
				this.compteController.EditionDataToWidget ();

				this.dateField.Text = Converters.DateToString (this.soldesColumn.DateDébut);
			}
		}


		private FormattedText NuméroCompte
		{
			get
			{
				return this.soldesColumn.NuméroCompte.ToSimpleText ();
			}
			set
			{
				this.soldesColumn.NuméroCompte = value;
			}
		}

		private void ValidateCompteAction(EditionData data)
		{
			data.ClearError ();
		}

		private void CompteChangedAction(int line, ColumnType columnType)
		{
			if (this.ignoreChanges.IsZero)
			{
				this.NuméroCompte = this.compteController.EditionData.Text;
				this.UpdateWidgets ();
				this.dataChanged (this.Index);
			}
		}


		private readonly AbstractController				controller;
		private readonly SoldesColumn					soldesColumn;
		private readonly SafeCounter					ignoreChanges;

		private System.Action<int>						dataChanged;
		private FrameBox								frame;
		private AutoCompleteFieldController				compteController;
		private TextFieldEx								dateField;
		private GlyphButton								addSubButton;
	}
}
