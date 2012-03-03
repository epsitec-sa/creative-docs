//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Fields.Controllers
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

			this.ignoreChanges = new SafeCounter ();
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
			//	Attention: Le setter est protégé. Pour modifier le mode IsReadOnly, il faut soit utiliser
			//	ColumnMapper.Enable, soit EditionData.Enable.
			get
			{
				return false;
			}
			protected set
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


		protected void CreateLabelUI(Widget parent)
		{
			if (this.HasRightEditor && !this.columnMapper.Description.IsNullOrEmpty)
			{
				new StaticText
				{
					Parent        = parent,
					FormattedText = this.columnMapper.Description + " :",
					Dock          = DockStyle.Top,
					Margins       = new Margins (0, 0, 0, 2),
				};
			}
		}

		protected Margins BoxMargins
		{
			get
			{
				double left   = 0;
				double right  = 0;
				double top    = 0;
				double bottom = 0;

				if (this.HasRightEditor)
				{
					if (this.columnMapper.Description.IsNullOrEmpty)
					{
						top = -6;
					}

					bottom = 5;
				}
				else
				{
					right = 1;
				}

				return new Margins (left, right, top, bottom);
			}
		}

		protected bool HasRightEditor
		{
			get
			{
				return this.controller.HasRightEditor;
			}
		}


		protected readonly AbstractController					controller;
		protected readonly int									line;
		protected readonly ColumnMapper							columnMapper;
		protected readonly System.Action<int, ColumnType>		setFocusAction;
		protected readonly System.Action						contentChangedAction;
		protected SafeCounter									ignoreChanges;

		protected EditionData									editionData;
		protected FrameBox										box;
		protected FrameBox										container;
		protected Widget										editWidget;
		protected bool											hasFocus;
	}
}
