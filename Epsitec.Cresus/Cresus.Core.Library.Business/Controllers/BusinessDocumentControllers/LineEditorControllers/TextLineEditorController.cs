//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class TextLineEditorController : AbstractLineEditorController
	{
		public TextLineEditorController(AccessData accessData)
			: base (accessData)
		{
		}

		protected override void CreateUI(UIBuilder builder)
		{
			var leftFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
				TabIndex = this.GetNextTabIndex (),
			};

			var rightFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
				PreferredWidth = 300,
				Padding = new Margins (10),
				TabIndex = this.GetNextTabIndex (),
			};

			var separator = new Separator
			{
				IsVerticalLine = true,
				PreferredWidth = 1,
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
			};

			//	Tient compte de la logique d'entreprise.
			bool myEyesOnly = this.Entity.Attributes.HasFlag (DocumentItemAttributes.MyEyesOnly);

			if (!this.accessData.DocumentLogic.IsTextEditionEnabled)
			{
				leftFrame.Enable = false;
				rightFrame.Enable = false;
			}

			if (this.accessData.DocumentLogic.IsMyEyesOnlyEditionEnabled)
			{
				leftFrame.Enable = myEyesOnly;
				rightFrame.Enable = false;
			}

			//	Partie gauche.
			var textField = builder.CreateTextFieldMulti (null, DockStyle.None, 0, false, Marshaler.Create (() => this.SimpleText, x => this.SimpleText = x));
			this.PlaceLabelAndField (leftFrame, 50, 0, this.TileTitle, textField);

			this.firstFocusedWidget = textField;

			//	Partie droite.
			var check = new CheckButton
			{
				Text = "Uniquement sur les documents internes",
				ActiveState = myEyesOnly ? ActiveState.Yes : ActiveState.No,
				Parent = rightFrame,
				Dock = DockStyle.Top,
				TabIndex = this.GetNextTabIndex (),
			};

			check.ActiveStateChanged += delegate
			{
				if (check.ActiveState == ActiveState.Yes)
				{
					this.Entity.Attributes = DocumentItemAttributes.MyEyesOnly;
				}
				else
				{
					this.Entity.Attributes = DocumentItemAttributes.None;
				}
			};
		}

		public override FormattedText TileTitle
		{
			get
			{
				return this.IsTitle ? "Titre" : "Texte";
			}
		}


		private FormattedText SimpleText
		{
			get
			{
				return LineEngine.TitleToSimpleText (this.Entity.Text);
			}
			set
			{
				if (value.IsNullOrEmpty ())
				{
					this.Entity.Text = null;
				}
				else
				{
					if (this.IsTitle)
					{
						this.Entity.Text = LineEngine.SimpleTextToTitle (value);
					}
					else
					{
						this.Entity.Text = value;
					}
				}
			}
		}

		private bool IsTitle
		{
			get
			{
				return LineEngine.IsTitle (this.Entity.Text);
			}
		}


		private TextDocumentItemEntity Entity
		{
			get
			{
				return this.entity as TextDocumentItemEntity;
			}
		}
	}
}
