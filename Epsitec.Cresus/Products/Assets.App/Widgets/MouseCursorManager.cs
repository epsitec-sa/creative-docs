//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class MouseCursorManager
	{
		public static void SetWindow(Window window)
		{
			MouseCursorManager.window = window;
		}

		public static void Clear()
		{
			MouseCursorManager.SetMouseCursor (MouseCursorType.Default);
		}

		public static void SetMouseCursor(MouseCursorType cursor)
		{
			switch (cursor)
			{
				case MouseCursorType.HorizontalMoveSeparator:
					MouseCursorManager.MouseCursorImage (ref MouseCursorManager.imageCursorHorizontalMoveSeparator, MouseCursorManager.Icon ("Cursor.HorizontalMoveSeparator"));
					break;

				case MouseCursorType.VerticalMoveSeparator:
					MouseCursorManager.MouseCursorImage (ref MouseCursorManager.imageCursorVerticalMoveSeparator, MouseCursorManager.Icon ("Cursor.VerticalMoveSeparator"));
					break;

				case MouseCursorType.HorizontalMoveRectangle:
					MouseCursorManager.MouseCursorImage (ref MouseCursorManager.imageCursorHorizontalMoveRectangle, MouseCursorManager.Icon ("Cursor.HorizontalMoveRectangle"));
					break;

				case MouseCursorType.VerticalMoveRectangle:
					MouseCursorManager.MouseCursorImage (ref MouseCursorManager.imageCursorVerticalMoveRectangle, MouseCursorManager.Icon ("Cursor.VerticalMoveRectangle"));
					break;

				default:
					MouseCursorManager.window.MouseCursor = MouseCursor.AsArrow;
					break;
			}
		}

		private static void MouseCursorImage(ref Image image, string name)
		{
			//	Choix du sprite de la souris.
			if (image == null)
			{
				image = ImageProvider.Instance.GetImage (name, Resources.DefaultManager);
			}

			MouseCursorManager.window.MouseCursor = MouseCursor.FromImage (image);
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


		private static Window window;

		private static Image imageCursorHorizontalMoveSeparator;
		private static Image imageCursorVerticalMoveSeparator;
		private static Image imageCursorHorizontalMoveRectangle;
		private static Image imageCursorVerticalMoveRectangle;
	}
}
