//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Permanents.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Permanents.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les paramètres d'affichage permanents génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractPermanentsController
	{
		public AbstractPermanentsController(AbstractController controller)
		{
			this.controller = controller;

			this.comptaEntity  = this.controller.ComptaEntity;
			this.périodeEntity = this.controller.PériodeEntity;
			this.permanents    = this.controller.DataAccessor.Permanents;

			this.ignoreChanges = new SafeCounter ();
		}


		public virtual void CreateUI(FrameBox parent, System.Action permanentsChanged)
		{
			this.permanentsChanged = permanentsChanged;

			this.toolbar = new FrameBox
			{
				Parent              = parent,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, 5),
			};
		}

		protected virtual void PermanentsChanged()
		{
			this.permanentsChanged ();
		}

		public void ClearAction()
		{
			this.permanents.Clear ();
			this.PermanentsChanged ();
		}

		protected virtual void UpdateWidgets()
		{
		}


		public virtual void UpdateContent()
		{
		}


		protected readonly AbstractController					controller;
		protected readonly ComptaEntity							comptaEntity;
		protected readonly ComptaPériodeEntity					périodeEntity;
		protected readonly AbstractPermanents					permanents;
		protected readonly SafeCounter							ignoreChanges;

		protected System.Action									permanentsChanged;

		protected int											tabIndex;
		protected FrameBox										toolbar;
	}
}
