//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	/// The <c>HintListHeaderWidget</c> class ...
	/// </summary>
	public sealed class HintListHeaderWidget : FrameBox
	{
		public HintListHeaderWidget()
		{
			this.topHalf = new Widget ()
			{
				Embedder = this,
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredHeight = 52
			};

			this.image = new StaticImage ()
			{
				Embedder = this.topHalf,
				Dock = DockStyle.StackBegin,
				VerticalAlignment = VerticalAlignment.Top,
				Margins = new Drawing.Margins (0, 4, 0, 0),
				PreferredSize = new Drawing.Size (64, 48),
				ImageSize = new Drawing.Size (64, 48)
			};

			this.title = new StaticText ()
			{
				Embedder = this.topHalf,
				Dock = DockStyle.StackFill,
				TextBreakMode = Drawing.TextBreakMode.Ellipsis
			};
		}


		public HintListContentType ContentType
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

		private void OnContentTypeChanged()
		{
			switch (this.contentType)
			{
				case HintListContentType.Default:
					this.image.Visibility =	false;
					break;

				case HintListContentType.Catalog:
					this.image.Visibility =	true;
					this.image.ImageName = @"manifest:Epsitec.Common.Dialogs.Images.binocular-64x48.png";
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

		private readonly Widget topHalf;
		private readonly StaticImage image;
		private readonly StaticText title;

		private HintListContentType contentType;
	}
}
