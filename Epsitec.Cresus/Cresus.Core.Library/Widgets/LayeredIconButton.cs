//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>LayeredIconButton</c> class is a specialized <see cref="IconButton"/>
	/// with other icons layered on top of it.
	/// </summary>
	public class LayeredIconButton : IconButton
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LayeredIconButton"/> class.
		/// </summary>
		public LayeredIconButton()
		{
			this.overlays = new List<StaticImage> ();
		}

		/// <summary>
		/// Adds an overlay icon on top of the <c>IconButton</c>.
		/// </summary>
		/// <param name="iconUri">The icon URI.</param>
		/// <param name="preferredSize">Preferred size (if any).</param>
		public void AddOverlay(string iconUri, Size preferredSize = new Size ())
		{
			var image = new StaticImage
			{
				Parent = this,
				Anchor = AnchorStyles.All,
				ContentAlignment = ContentAlignment.MiddleCenter,
				ImageSize = preferredSize,
				ImageName = iconUri,
			};

			image.SetFrozen (true);
			
			this.overlays.Add (image);
		}

		/// <summary>
		/// Removes all overlay icons.
		/// </summary>
		public void ClearOverlays()
		{
			this.overlays.ForEach (overlay => overlay.Dispose ());
			this.overlays.Clear ();
		}

		
		protected override void OnEngaged()
		{
			base.OnEngaged ();
			this.overlays.ForEach (overlay => overlay.Margins = new Margins (1, -1, 1, -1));
		}

		protected override void OnDisengaged()
		{
			this.overlays.ForEach (overlay => overlay.Margins = Margins.Zero);
			base.OnDisengaged ();
		}

		
		private readonly List<StaticImage> overlays;
	}
}
