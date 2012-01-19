﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class PlanComptableFooterController : AbstractFooterController
	{
		public PlanComptableFooterController(CoreApp app, BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity, AbstractDataAccessor dataAccessor, List<ColumnMapper> columnMappers, AbstractController abstractController, ArrayController arrayController)
			: base (app, businessContext, comptabilitéEntity, dataAccessor, columnMappers, abstractController, arrayController)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.footerBoxes.Clear ();
			this.footerContainers.Clear ();
			this.footerFields.Clear ();

			this.CreateLineUI (parent);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			this.footerBoxes.Add (new List<FrameBox> ());
			this.footerContainers.Add (new List<FrameBox> ());
			this.footerFields.Add (new List<AbstractTextField> ());

			var footerFrame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Bottom,
				Margins = new Margins (0, 0, 1, 0),
			};

			this.linesFrames.Add (footerFrame);
			int line = this.linesFrames.Count - 1;

			int columnCount = this.columnMappers.Count;

			for (int column = 0; column < columnCount; column++)
			{
				var box = new FrameBox
				{
					Parent        = footerFrame,
					DrawFullFrame = true,
					Dock          = DockStyle.Left,
					Margins       = new Margins (0, 1, 0, 0),
					TabIndex      = column+1,
				};

				var mapper = this.columnMappers[column];

				FrameBox container;
				AbstractTextField field;

				if (mapper.Column == ColumnType.Catégorie)
				{
					IEnumerable<EnumKeyValues<CatégorieDeCompte>> possibleItems = EnumKeyValues.FromEnum<CatégorieDeCompte> ();

					UIBuilder.CreateAutoCompleteTextField<CatégorieDeCompte> (box, possibleItems, out container, out field);
					field.Name = this.GetWidgetName (column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else if (mapper.Column == ColumnType.Type)
				{
					IEnumerable<EnumKeyValues<TypeDeCompte>> possibleItems = EnumKeyValues.FromEnum<TypeDeCompte> ();

					UIBuilder.CreateAutoCompleteTextField<TypeDeCompte> (box, possibleItems, out container, out field);
					field.Name = this.GetWidgetName (column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
#if false
				else if (mapper.Column == ColumnType.TVA)
				{
					IEnumerable<EnumKeyValues<VatCode>> possibleItems = EnumKeyValues.FromEnum<VatCode> ();

					UIBuilder.CreateAutoCompleteTextField<VatCode> (box, possibleItems, out container, out field);
					field.Name = this.GetWidgetName (column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
#endif
				else if (mapper.Column == ColumnType.Groupe)
				{
					var comptes = this.comptabilitéEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Groupe).OrderBy (x => x.Numéro);
					UIBuilder.CreateAutoCompleteTextField (box, comptes, out container, out field);
					field.Name = this.GetWidgetName (column, line);

					field.TextChanged += delegate
					{
						this.FooterTextChanged (field);
					};
				}
				else if (mapper.Column == ColumnType.CompteOuvBoucl)
				{
					var comptes = this.comptabilitéEntity.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal && x.Catégorie == CatégorieDeCompte.Exploitation).OrderBy (x => x.Numéro);
					UIBuilder.CreateAutoCompleteTextField (box, comptes, out container, out field);
					field.Name = this.GetWidgetName (column, line);

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
						Name     = this.GetWidgetName (column, line),
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
			}
		}

		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'un compte :" : "Création d'un compte :";
		}


		public override void UpdateFooterContent()
		{
			this.EditionDataToWidgets ();
			this.FooterValidate ();
		}
	}
}
