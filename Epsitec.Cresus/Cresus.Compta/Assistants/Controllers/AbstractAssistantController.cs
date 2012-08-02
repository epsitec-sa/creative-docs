//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Assistants.Controllers
{
	/// <summary>
	/// Contrôleur générique pour un assistant la comptabilité.
	/// </summary>
	public abstract class AbstractAssistantController
	{
		public AbstractAssistantController(AbstractController controller)
		{
			this.controller = controller;

			this.compta          = this.controller.ComptaEntity;
			this.période         = this.controller.PériodeEntity;
			this.dataAccessor    = this.controller.DataAccessor;
			this.arrayController = this.controller.ArrayController;
			this.businessContext = this.controller.BusinessContext;
			this.settingsList    = this.controller.SettingsList;

			this.columnMappers = new List<ColumnMapper> ();
			this.fieldControllers = new List<AbstractFieldController> ();

			this.InitializeColumnMappers ();
		}

		public virtual FrameBox CreateUI(Widget parent)
		{
			return null;
		}

		public void UpdateContent()
		{
			this.EditionDataToWidgets (ignoreFocusField: true);
			this.EditorValidate ();
		}

		public virtual void UpdateGeometry()
		{
			foreach (var mapper in this.columnMappers.Where (x => x.LineLayout == 0))
			{
				this.UpdateColumnGeometry (mapper.Column, mapper.Column);
			}
		}

		protected void UpdateColumnGeometry(ColumnType columnType, ColumnType columnTypeModel)
		{
			var fieldController = this.GetFieldController (columnType);

			if (fieldController != null)
			{
				double left, width;
				if (this.arrayController.GetColumnGeometry (columnTypeModel, out left, out width))
				{
					fieldController.Box.Visibility = true;
					fieldController.Box.Margins = new Margins (left+1, 0, 0, 0);
					fieldController.Box.PreferredWidth = width-1;
				}
				else
				{
					fieldController.Box.Visibility = false;
				}
			}
		}


		protected void EditorTextChanged()
		{
			this.controller.EditorController.Dirty = true;

			//?this.UpdateEditionWidgets ();
			this.EditionDataToWidgets (ignoreFocusField: true);  // nécessaire pour le feedback du travail de UpdateMultiWidgets !

			this.EditorValidate ();
			//?this.UpdateToolbar ();
			//?this.UpdateEditorInfo ();
			//?this.UpdateInsertionRow ();
		}

		private void EditionDataToWidgets(bool ignoreFocusField)
		{
			//	Effectue le transfert this.dataAccessor.EditionData -> widgets éditables.
			foreach (var mapper in this.columnMappers.Where (x => x.Show && x.Edition))
			{
				var controller = this.GetFieldController (mapper.Column);

				if (controller != null)
				{
					controller.EditionData = this.editionLine.GetData (mapper.Column);

					//	Le widget en cours d'édition ne doit absolument pas être modifié.
					//	Par exemple, s'il contient "123" et qu'on a tapé "4", la chaîne actuellement contenue
					//	est "1234". Si on le mettait à jour, il contiendrait "1234.00", ce qui serait une
					//	catastrophe !
					if (!ignoreFocusField || !controller.HasFocus)
					{
						controller.EditionDataToWidget ();
					}
				}
			}
		}

		private void EditorValidate()
		{
			this.hasError = false;

			foreach (var mapper in this.columnMappers.Where (x => x.Show && x.Edition))
			{
				var controller = this.GetFieldController (mapper.Column);

				if (controller != null)
				{
					controller.Validate ();

					if (controller.EditionData != null && controller.EditionData.HasError)
					{
						this.hasError = true;
					}
				}
			}
		}


		protected virtual void InitializeColumnMappers()
		{
		}


		private AbstractFieldController GetFieldController(ColumnType columnType)
		{
			return this.fieldControllers.Where (x => x.ColumnMapper.Column == columnType).FirstOrDefault ();
		}


		protected readonly AbstractController					controller;
		protected readonly ComptaEntity							compta;
		protected readonly ComptaPériodeEntity					période;
		protected readonly AbstractDataAccessor					dataAccessor;
		protected readonly ArrayController						arrayController;
		protected readonly BusinessContext						businessContext;
		protected readonly SettingsList							settingsList;
		protected readonly List<ColumnMapper>					columnMappers;
		protected readonly List<AbstractFieldController>		fieldControllers;

		protected AbstractEditionLine							editionLine;
		protected AbstractData									data;
		protected bool											hasError;
	}
}
