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
		public AbstractFieldController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> setFocusAction = null, System.Action contentChangedAction = null)
		{
			this.controller           = controller;
			this.line                 = line;
			this.columnMapper         = columnMapper;
			this.setFocusAction       = setFocusAction;
			this.contentChangedAction = contentChangedAction;
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		public EditionData EditionData
		{
			get
			{
				return this.editionData;
			}
			set
			{
				this.editionData = value;
			}
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

		public Widget EditWidget
		{
			get
			{
				return this.editWidget;
			}
		}


		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool HasFocus
		{
			get
			{
				return this.hasFocus;
			}
		}

		public virtual void SetFocus()
		{
		}

		public virtual void EditionDataToWidget()
		{
		}

		public virtual void WidgetToEditionData()
		{
		}

		public virtual void Validate()
		{
		}


		protected void SetFocusAction()
		{
			if (this.setFocusAction != null && this.columnMapper != null)
			{
				this.setFocusAction (this.line, this.columnMapper.Column);
			}
		}

		protected void ContentChangedAction()
		{
			if (this.contentChangedAction != null)
			{
				this.contentChangedAction ();
			}
		}


		protected readonly AbstractController					controller;
		protected readonly int									line;
		protected readonly ColumnMapper							columnMapper;
		protected readonly System.Action<int, ColumnType>		setFocusAction;
		protected readonly System.Action						contentChangedAction;

		protected EditionData									editionData;
		protected FrameBox										box;
		protected FrameBox										container;
		protected Widget										editWidget;
		protected bool											hasFocus;
		protected bool											ignoreChange;
	}
}
