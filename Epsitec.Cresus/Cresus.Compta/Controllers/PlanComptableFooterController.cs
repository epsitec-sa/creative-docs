//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class PlanComptableFooterController : AbstractFooterController
	{
		public PlanComptableFooterController(AbstractController controller)
			: base (controller)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.fieldControllers.Clear ();

			this.CreateLineUI (parent);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var footerFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 1, 0),
			};

			this.linesFrames.Add (footerFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				AbstractFieldController field;

				if (mapper.Column == ColumnType.Catégorie)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, "Actif", "Passif", "Charge", "Produit", "Exploitation");
				}
				else if (mapper.Column == ColumnType.Type)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, "Normal", "Titre", "Groupe");
				}
				else if (mapper.Column == ColumnType.Groupe)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					var comptes = this.comptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Groupe);
					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else if (mapper.Column == ColumnType.CompteOuvBoucl)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					var comptes = this.comptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal && x.Catégorie == CatégorieDeCompte.Exploitation);
					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);

#if false
				var box = new FrameBox
				{
					Parent        = footerFrame,
					DrawFullFrame = true,
					Dock          = DockStyle.Left,
					Margins       = new Margins (0, 1, 0, 0),
					TabIndex      = ++tabIndex,
				};

				FrameBox container;
				AbstractTextField field;

				if (mapper.Column == ColumnType.Catégorie)
				{
					UIBuilder.CreateAutoCompleteTextField (box, out container, out field);
					UIBuilder.UpdateAutoCompleteTextField (field, "Actif", "Passif", "Charge", "Produit", "Exploitation");
					field.Name = this.GetWidgetName (mapper.Column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else if (mapper.Column == ColumnType.Type)
				{
					UIBuilder.CreateAutoCompleteTextField (box, out container, out field);
					UIBuilder.UpdateAutoCompleteTextField (field, "Normal", "Titre", "Groupe");
					field.Name = this.GetWidgetName (mapper.Column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else if (mapper.Column == ColumnType.Groupe)
				{
					var comptes = this.comptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Groupe);
					UIBuilder.CreateAutoCompleteTextField (box, out container, out field);
					UIBuilder.UpdateAutoCompleteTextField (field, comptes);
					field.Name = this.GetWidgetName (mapper.Column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else if (mapper.Column == ColumnType.CompteOuvBoucl)
				{
					var comptes = this.comptaEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal && x.Catégorie == CatégorieDeCompte.Exploitation);
					UIBuilder.CreateAutoCompleteTextField (box, out container, out field);
					UIBuilder.UpdateAutoCompleteTextField (field, comptes);
					field.Name = this.GetWidgetName (mapper.Column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else
				{
					container = new FrameBox
					{
						Parent   = box,
						Dock     = DockStyle.Fill,
						TabIndex = 1,
					};

					field = new TextField
					{
						Parent   = container,
						Dock     = DockStyle.Fill,
						Name     = this.GetWidgetName (mapper.Column, line),
						TabIndex = 1,
					};

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}

				this.footerBoxes     [line].Add (box);
				this.footerContainers[line].Add (container);
				this.footerFields    [line].Add (field);
#endif
			}
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'un compte :" : "Création d'un compte :";
		}
	}
}
