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
	public class BudgetsFooterController : AbstractFooterController
	{
		public BudgetsFooterController(Application app, BusinessContext businessContext, ComptaEntity comptaEntity, AbstractDataAccessor dataAccessor, List<ColumnMapper> columnMappers, AbstractController abstractController, ArrayController arrayController)
			: base (app, businessContext, comptaEntity, dataAccessor, columnMappers, abstractController, arrayController)
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
			int tabIndex = 0;

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
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

					if (mapper.Column == ColumnType.Numéro ||
						mapper.Column == ColumnType.Titre  ||
						mapper.Column == ColumnType.Solde  )
					{
						field.IsReadOnly = true;
					}

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
			return modify ? "Modification du budget d'un compte :" : "";
		}


		public override void UpdateFooterContent()
		{
			foreach (var field in this.footerFields[0])
			{
				field.Visibility = this.dataAccessor.IsModification;
			}

			this.EditionDataToWidgets ();
			this.FooterValidate ();
		}
	}
}
