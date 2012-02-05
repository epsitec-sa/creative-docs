//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Contrôleur générique permettant l'édition d'un champ.
	/// </summary>
	public abstract class AbstractFieldController
	{
		public AbstractFieldController(AbstractController controller, EditionData editionData, FormattedText description, System.Action contentChangedAction = null, System.Action<EditionData> validateAction = null, System.Func<FormattedText, FormattedText, string> adjustHintFunction = null)
		{
			this.controller           = controller;
			this.editionData          = editionData;
			this.description          = description;
			this.contentChangedAction = contentChangedAction;
			this.validateAction       = validateAction;
			this.adjustHintFunction   = adjustHintFunction;
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		public FrameBox Box
		{
			get
			{
				return this.box;
			}
		}

		public Widget Container
		{
			get
			{
				return this.container;
			}
		}

		public Widget Field
		{
			get
			{
				return this.field;
			}
		}


		public FormattedText Text
		{
			get
			{
				return this.editionData.Text;
			}
			set
			{
				this.editionData.Text = value;
			}
		}


		protected void ContentChangedAction()
		{
			if (this.contentChangedAction != null)
			{
				this.contentChangedAction ();
			}
		}

		protected void ValidateAction()
		{
			if (this.validateAction != null)
			{
				this.validateAction (this.editionData);
			}
		}

		protected string AdjustHintFunction()
		{
			if (this.adjustHintFunction == null)
			{
				return null;
			}
			else
			{
				return this.adjustHintFunction (this.field.FormattedText, this.editionData.Text);
			}
		}


		protected readonly AbstractController					controller;
		protected readonly EditionData							editionData;
		protected readonly FormattedText						description;
		protected readonly System.Action						contentChangedAction;
		protected readonly System.Action<EditionData>			validateAction;
		protected readonly System.Func<FormattedText, FormattedText, string> adjustHintFunction;

		protected FrameBox										box;
		protected FrameBox										container;
		protected Widget										field;
	}
}
