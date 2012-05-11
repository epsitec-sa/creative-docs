//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Fields.Controllers
{
	/// <summary>
	/// Contrôleur générique permettant l'édition d'un champ.
	/// </summary>
	public abstract class AbstractFieldController
	{
		public AbstractFieldController(AbstractController controller, int line, ColumnMapper columnMapper, System.Action<int, ColumnType> clearFocusAction, System.Action<int, ColumnType> setFocusAction, System.Action<int, ColumnType> contentChangedAction)
		{
			this.controller           = controller;
			this.line                 = line;
			this.columnMapper         = columnMapper;
			this.clearFocusAction     = clearFocusAction;
			this.setFocusAction       = setFocusAction;
			this.contentChangedAction = contentChangedAction;

			this.ignoreChanges = new SafeCounter ();
		}


		public virtual void CreateUI(Widget parent)
		{
		}

		protected void CreateForegroundUI()
		{
			this.foregroundFrame = new CustomFrameBox
			{
				Parent     = this.editWidget,
				Anchor     = AnchorStyles.All,
			};
		}

		public bool Furtive
		{
			get
			{
				return this.furtive;
			}
			set
			{
				if (this.furtive != value)
				{
					this.furtive = value;
					this.UpdateFurtive ();
				}
			}
		}

		public ColumnMapper ColumnMapper
		{
			get
			{
				return this.columnMapper;
			}
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

		public bool EmptyLineAdorner
		{
			//	Détermine si le champ a un fond hachuré.
			get
			{
				if (this.foregroundFrame == null)
				{
					return false;
				}
				else
				{
					return this.foregroundFrame.EmptyLineAdorner;
				}
			}
			set
			{
				if (this.foregroundFrame != null && this.foregroundFrame.EmptyLineAdorner != value)
				{
					this.foregroundFrame.EmptyLineAdorner = value;
					this.UpdateForegroundFrame ();
				}
			}
		}

		public bool BaseTVAAdorner
		{
			//	Dessine la première partie d'une flèche de haut en bas.
			get
			{
				if (this.foregroundFrame == null)
				{
					return false;
				}
				else
				{
					return this.foregroundFrame.BaseTVAAdorner;
				}
			}
			set
			{
				if (this.foregroundFrame != null && this.foregroundFrame.BaseTVAAdorner != value)
				{
					this.foregroundFrame.BaseTVAAdorner = value;
					this.UpdateForegroundFrame ();
				}
			}
		}

		public bool CodeTVAAdorner
		{
			//	Dessine la seconde partie d'une flèche de haut en bas.
			get
			{
				if (this.foregroundFrame == null)
				{
					return false;
				}
				else
				{
					return this.foregroundFrame.CodeTVAAdorner;
				}
			}
			set
			{
				if (this.foregroundFrame != null && this.foregroundFrame.CodeTVAAdorner != value)
				{
					this.foregroundFrame.CodeTVAAdorner = value;
					this.UpdateForegroundFrame ();
				}
			}
		}

		private void UpdateForegroundFrame()
		{
#if false
			if (this.foregroundFrame != null)
			{
				this.foregroundFrame.Visibility = this.foregroundFrame.EmptyLineAdorner ||
												  this.foregroundFrame.BaseTVAAdorner ||
												  this.foregroundFrame.CodeTVAAdorner ||
												  !this.foregroundFrame.OverlayText.IsNullOrEmpty;
			}
#endif
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


		protected void ClearFocusAction()
		{
			if (this.clearFocusAction != null && this.columnMapper != null)
			{
				int line   = this.line;
				var column = this.columnMapper.Column;
				var action = this.clearFocusAction;

				Application.QueueAsyncCallback
				(
					delegate
					{
						action (line, column);
					}
				);

				//this.clearFocusAction (this.line, this.columnMapper.Column);
			}
		}

		protected void SetFocusAction()
		{
			if (this.setFocusAction != null && this.columnMapper != null)
			{
				int line   = this.line;
				var column = this.columnMapper.Column;
				var action = this.setFocusAction;

				Application.QueueAsyncCallback
				(
					delegate
					{
						action (line, column);
					}
				);

				//this.setFocusAction (this.line, this.columnMapper.Column);
			}
		}

		protected void ContentChangedAction()
		{
			if (this.contentChangedAction != null)
			{
				this.contentChangedAction (this.line, this.columnMapper.Column);
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

		protected void CreateBoxUI(Widget parent)
		{
			this.box = new FrameBox
			{
				Parent          = parent,
				DrawFullFrame   = true,
				PreferredHeight = 20,
				Margins         = this.BoxMargins,
			};

			if (this.HasRightEditor)
			{
				this.box.Dock = DockStyle.Top;
			}
			else
			{
				this.box.Anchor = AnchorStyles.Left;
			}
		}

		protected virtual void UpdateFurtive()
		{
			if (this.Furtive)
			{
				this.box.DrawFullFrame = false;
				this.box.BackColor = Color.Empty;
			}
			else
			{
				this.box.DrawFullFrame = true;
				this.box.BackColor = Color.FromName ("White");
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

		protected void UpdateTooltip()
		{
			if (this.editionData.HasError)
			{
				ToolTip.Default.SetToolTip (this.editWidget, this.editionData.Error);
			}
			else
			{
				if (this.columnMapper != null)
				{
					ToolTip.Default.SetToolTip (this.editWidget, this.columnMapper.Tooltip);
				}
			}

			if (this.foregroundFrame != null)
			{
				this.foregroundFrame.OverlayText = this.editionData.OverlayText;
				this.foregroundFrame.OverlayTextColor = this.editionData.HasError ? UIBuilder.FieldOverlayTextErrorColor : UIBuilder.FieldOverlayTextColor;
			}
		}


		protected readonly AbstractController					controller;
		protected readonly int									line;
		protected readonly ColumnMapper							columnMapper;
		protected readonly System.Action<int, ColumnType>		clearFocusAction;
		protected readonly System.Action<int, ColumnType>		setFocusAction;
		protected readonly System.Action<int, ColumnType>		contentChangedAction;
		protected SafeCounter									ignoreChanges;

		protected EditionData									editionData;
		protected FrameBox										box;
		protected FrameBox										container;
		protected Widget										editWidget;
		protected CustomFrameBox								foregroundFrame;
		protected bool											hasFocus;
		protected bool											furtive;
	}
}
