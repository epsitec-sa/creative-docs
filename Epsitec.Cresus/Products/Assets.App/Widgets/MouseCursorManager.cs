//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class MouseCursorManager
	{
		public MouseCursorManager(Widget parent)
		{
			this.parent = parent;
		}


		public void Clear()
		{
			this.SetMouseCursor (MouseCursorType.Default);
		}

		public void SetMouseCursor(MouseCursorType cursor)
		{
			this.mouseCursorType = cursor;

			switch (this.mouseCursorType)
			{
				case MouseCursorType.HorizontalMove:
					this.MouseCursorImage (ref this.mouseCursorHorizontalMove, MouseCursorManager.Icon ("Cursor.HorizontalMove"));
					break;

				case MouseCursorType.VerticalMove:
					this.MouseCursorImage (ref this.mouseCursorVerticalMove, MouseCursorManager.Icon ("Cursor.VerticalMove"));
					break;

				default:
					this.parent.Window.MouseCursor = MouseCursor.AsArrow;
					break;
			}
		}

		private void MouseCursorImage(ref Image image, string name)
		{
			//	Choix du sprite de la souris.
			if (image == null)
			{
				image = ImageProvider.Instance.GetImage (name, Resources.DefaultManager);
			}

			this.parent.Window.MouseCursor = MouseCursor.FromImage (image);
		}

		private static string Icon(string icon)
		{
			//	Retourne le nom complet d'une icône.
			if (string.IsNullOrEmpty (icon))
			{
				return null;
			}

			System.Diagnostics.Debug.Assert (icon.StartsWith ("manifest:") == false);
			System.Diagnostics.Debug.Assert (icon.EndsWith (".icon") == false);

			return string.Format ("manifest:Epsitec.Cresus.Assets.App.Images.{0}.icon", FormattedText.Escape (icon));
		}


		private readonly Widget parent;

		private MouseCursorType mouseCursorType;
		private Image mouseCursorHorizontalMove;
		private Image mouseCursorVerticalMove;
	}
}
