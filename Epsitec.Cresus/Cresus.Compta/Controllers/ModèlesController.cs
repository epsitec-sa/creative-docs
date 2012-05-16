﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les écritures modèles de la comptabilité.
	/// </summary>
	public class ModèlesController : AbstractController
	{
		public ModèlesController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new ModèlesDataAccessor (this);
		}


		protected override void UpdateTitle()
		{
			this.SetGroupTitle ("Journal");
			this.SetTitle ("Écritures modèles");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasFilterPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasInfoPanel
		{
			get
			{
				return true;
			}
		}


		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
		}


		protected override void CreateEditor(FrameBox parent)
		{
			this.editorController = new ModèlesEditorController (this);
			this.editorController.CreateUI (parent, this.UpdateArrayContent);
			this.editorController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Code,      0.20, ContentAlignment.MiddleLeft,  "Code",      "Code mnémotechnique");
				yield return new ColumnMapper (ColumnType.Raccourci, 0.20, ContentAlignment.MiddleLeft,  "Raccourci", "Raccourci clavier");
				yield return new ColumnMapper (ColumnType.Débit,     0.25, ContentAlignment.MiddleLeft,  "Débit",     "Numéro ou nom du compte à débiter (facultatif)");
				yield return new ColumnMapper (ColumnType.Crédit,    0.25, ContentAlignment.MiddleLeft,  "Crédit",    "Numéro ou nom du compte à créditer (facultatif)");
				yield return new ColumnMapper (ColumnType.Pièce,     0.20, ContentAlignment.MiddleLeft,  "Pièce",     "Numéro de la pièce comptable (facultatif)");
				yield return new ColumnMapper (ColumnType.Libellé,   0.80, ContentAlignment.MiddleLeft,  "Libellé",   "Libellé de l'écriture modèle<br/>Le caractère @ indique le point d'insertion");
				yield return new ColumnMapper (ColumnType.Montant,   0.25, ContentAlignment.MiddleRight, "Montant",   "Montant de l'écriture modèle (facultatif)");
			}
		}
	}
}
