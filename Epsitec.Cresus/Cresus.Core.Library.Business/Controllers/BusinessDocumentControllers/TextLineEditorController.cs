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
			var box = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
			};

			var textField = builder.CreateTextFieldMulti (null, DockStyle.None, 0, Marshaler.Create (() => this.SimpleText, x => this.SimpleText = x));
			this.PlaceLabelAndField (box, 50, 400, this.TitleTile, textField);

			this.firstFocusedWidget = textField;
		}

		public override FormattedText TitleTile
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
				return LinesEngine.TitleToSimpleText (this.Entity.Text);
			}
			set
			{
				if (value.IsNullOrEmpty)
				{
					this.Entity.Text = null;
				}
				else
				{
					if (this.IsTitle)
					{
						this.Entity.Text = LinesEngine.SimpleTextToTitle (value);
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
				return LinesEngine.IsTitle (this.Entity.Text);
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
