//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (HintListController))]

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>HintListHeaderWidget</c> class represents the header of a hint list
	/// (see <see cref="HintListWidget"/> and <see cref="HintListController"/>).
	/// </summary>
	public sealed class HintListHeaderWidget : FrameBox
	{
		public HintListHeaderWidget()
		{
			this.searchWidget = new HintListSearchWidget ()
			{
				Embedder = this,
				Dock = DockStyle.Top,
				PreferredHeight = 26,
				Margins = new Drawing.Margins (2, 2, 1, 1),
				Name = "Search"
			};

			this.topHalf = new FrameBox ()
			{
				Embedder = this,
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredHeight = 52,
				Name = "TopHalf"
			};

			this.bottomHalf = new FrameBox ()
			{
				Embedder = this,
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Name = "BottomHalf"
			};

			this.image = new StaticImage ()
			{
				Embedder = this.topHalf,
				Dock = DockStyle.StackBegin,
				VerticalAlignment = VerticalAlignment.Top,
				Margins = new Drawing.Margins (0, 4, 0, 0),
				PreferredSize = new Drawing.Size (64, 48),
				ImageSize = new Drawing.Size (64, 48),
				Name = "Icon"
			};

			this.title = new StaticText ()
			{
				Embedder = this.topHalf,
				Dock = DockStyle.StackFill,
				TextBreakMode = Drawing.TextBreakMode.Ellipsis,
				Name = "Title"
			};

			this.toolBar = new HToolBar ()
			{
				Embedder = this.bottomHalf,
				Dock = DockStyle.Fill,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				PreferredHeight = 20
			};
		}


		public HintListContentType				ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				if (this.contentType != value)
				{
					this.contentType = value;
					this.OnContentTypeChanged ();
				}
			}
		}

		public AbstractToolBar					ToolBar
		{
			get
			{
				return this.toolBar;
			}
		}

		
		private void OnContentTypeChanged()
		{
			switch (this.contentType)
			{
				case HintListContentType.Default:
					this.image.Visibility =	false;
					break;

				case HintListContentType.Catalog:
					this.image.Visibility =	true;
					this.image.ImageName = @"manifest:Epsitec.Common.Dialogs.Images.box-64x48.png";
					break;

				case HintListContentType.Suggestions:
					this.image.Visibility = true;
					this.image.ImageName = @"manifest:Epsitec.Common.Dialogs.Images.binocular-64x48.png";
					break;
			}
			
			this.title.Text = string.Format (this.GetTitleTextFormat (), "Adresse de facturation");
		}

		private string GetTitleTextFormat()
		{
			switch (this.contentType)
			{
				case HintListContentType.Suggestions:
					return @"Suggestions pour<br/><font size=""120%""><b>«{0}»</b></font>";

				case HintListContentType.Catalog:
					return @"Liste des fiches<br/><font size=""120%""><b>«{0}»</b></font>";
			}

			return "{0}";
		}

		
		private readonly Widget					topHalf;
		private readonly Widget					bottomHalf;
		private readonly StaticImage			image;
		private readonly StaticText				title;
		private readonly HToolBar				toolBar;
		private readonly HintListSearchWidget	searchWidget;

		private HintListContentType				contentType;
	}
}
