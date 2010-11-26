//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget affiche une icône, selon IconUri et IconPreferredSize, ou une image selon ImageEntity.
	/// </summary>
	public class UserButton : Button
	{
		public UserButton()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;

			this.iconLayout = new TextLayout ();
			this.iconLayout.Alignment = ContentAlignment.MiddleCenter;
			this.iconLayout.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split;
		}

		public UserButton(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public CoreData CoreData
		{
			get
			{
				return this.coreData;
			}
			set
			{
				this.coreData = value;
			}
		}

		public ImageEntity ImageEntity
		{
			get
			{
				return this.imageEntity;
			}
			set
			{
				if (this.imageEntity != value)
				{
					this.imageEntity = value;

					this.UpdateImage ();
					this.Invalidate ();
				}
			}
		}

		public Size IconPreferredSize
		{
			get
			{
				return this.iconPreferredSize;
			}
			set
			{
				this.iconPreferredSize = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.Text = null;
			base.PaintBackgroundImplementation(graphics, clipRect);

			if (this.imageEntity.IsNull ())
			{
				this.iconLayout.Text = IconButton.GetSourceForIconText (this.IconUri, this.iconPreferredSize, null, null);
				this.iconLayout.LayoutSize = this.Client.Bounds.Size;
				this.iconLayout.Paint (Point.Zero, graphics);
			}
			else
			{
				if (this.image == null)
				{
					graphics.AddLine (this.Client.Bounds.BottomLeft, this.Client.Bounds.TopRight);
					graphics.AddLine (this.Client.Bounds.TopLeft, this.Client.Bounds.BottomRight);
					graphics.RenderSolid (Color.FromBrightness (0));  // affiche un grand 'X'
				}
				else
				{
					Rectangle rect = this.Client.Bounds;
					rect.Deflate (1);  // laisse une place pour le cadre du hilite

					graphics.PaintImage (this.image, rect);
				}
			}
		}


		private void UpdateImage()
		{
			if (this.image != null)
			{
				this.image.Dispose ();
				this.image = null;
			}

			if (this.coreData != null && this.imageEntity.IsNotNull ())
			{
				var store = this.coreData.ImageDataStore;
				var data = store.GetImageData (this.imageEntity.ImageBlob.Code, 40);
				this.image = data.GetImage ();
			}
		}


		private CoreData			coreData;
		private ImageEntity			imageEntity;
		private Image				image;
		private Size				iconPreferredSize;
		private TextLayout			iconLayout;
	}
}
