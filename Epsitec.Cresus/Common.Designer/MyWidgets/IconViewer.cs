using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Ce widget sait afficher toutes les variantes d'une icône.
	/// Une variante correspond à une page dans CresusPictogrammes.
	/// </summary>
	public class IconViewer : Button, Widgets.Helpers.IToolTipHost
	{
		public IconViewer() : base()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;
			this.AutoEngage = false;
			this.AutoFocus = true;

			this.icons = new List<OneIcon> ();
		}

		public IconViewer(Widget embedder)
			: this ()
		{
			this.SetEmbedder(embedder);
		}



		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			this.UpdateIcons ();

			if (this.icons.Count == 0)
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

				double size = System.Math.Min (this.Client.Bounds.Width, this.Client.Bounds.Height) - 8;
				var rect = new Rectangle (this.Client.Bounds.Center.X-size/2, this.Client.Bounds.Center.Y-size/2, size, size);

				graphics.AddLine (rect.BottomLeft, rect.TopRight);
				graphics.AddLine (rect.BottomRight, rect.TopLeft);
				graphics.RenderSolid (adorner.ColorBorder);
			}
			else
			{
				foreach (var oneIcon in this.icons)
				{
					this.DrawOneIcon (graphics, oneIcon);
				}
			}
		}

		private void DrawOneIcon(Graphics graphics, OneIcon oneIcon)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			var bounds = oneIcon.Bounds;
			bounds.Inflate (0.5);

			graphics.AddRectangle (bounds);
			graphics.RenderSolid (adorner.ColorBorder);

			oneIcon.TextLayout.Paint (oneIcon.Bounds.BottomLeft, graphics);
		}


		#region IToolTipHost Members

		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			this.UpdateIcons ();

			foreach (var oneIcon in this.icons)
			{
				if (oneIcon.Bounds.Contains (pos))
				{
					return IconViewer.GetIconDescripton (oneIcon.IconKey);
				}
			}

			return null;  // pas de tooltip
		}

		#endregion


		private void UpdateIcons()
		{
			if (this.lastIconUri != this.IconUri)
			{
				this.lastIconUri = this.IconUri;
				this.icons.Clear ();

				if (!string.IsNullOrEmpty (this.IconUri))
				{
					Image image = ImageProvider.Default.GetImage (this.IconUri, Resources.DefaultManager);
					Canvas canvas = image as Canvas;

					if (canvas != null)
					{
						var list = canvas.IconKeys.ToList ();
						list.Sort (new IconKeyComparer ());  // de la plus petite icône à la plus grande

						double totalWidth = 0;
						double maxHeight = 0;

						foreach (var iconKey in list)
						{
							totalWidth += iconKey.Size.Width+3;
							maxHeight = System.Math.Max (maxHeight, iconKey.Size.Height);
						}

						double x = this.Client.Bounds.Left + System.Math.Floor ((this.Client.Bounds.Width-totalWidth)/2);

						foreach (var iconKey in list)
						{
							double y = this.Client.Bounds.Bottom + System.Math.Floor ((this.Client.Bounds.Height-iconKey.Size.Height)/2);
							Rectangle bounds = new Rectangle (x, y, iconKey.Size.Width, iconKey.Size.Height);

							this.icons.Add (new OneIcon (this.IconUri, iconKey, bounds));

							x += iconKey.Size.Width+3;
						}
					}
				}
			}
		}

		private class IconKeyComparer : IComparer<Canvas.IconKey>
		{
			public int Compare(Canvas.IconKey a, Canvas.IconKey b)
			{
				//	Trie de la plus petite icône à la plus grande.
				if (a.Size.Width != b.Size.Width)
				{
					return a.Size.Width.CompareTo (b.Size.Width);
				}

				if (a.Size.Height != b.Size.Height)
				{
					return a.Size.Height.CompareTo (b.Size.Height);
				}

				if (a.PageRank != b.PageRank)
				{
					return a.PageRank.CompareTo (b.PageRank);
				}

				return 0;
			}
		}

		private class OneIcon
		{
			public OneIcon(string iconUri, Canvas.IconKey iconKey, Rectangle bounds)
			{
				this.iconUri = iconUri;
				this.IconKey = iconKey;
				this.Bounds = bounds;

				this.textLayout = new TextLayout ();
				this.textLayout.Text = IconButton.GetSourceForIconText (this.iconUri, this.IconKey.Size, this.IconKey.Language, this.IconKey.Style);
				this.textLayout.LayoutSize = this.IconKey.Size;
			}

			public Canvas.IconKey IconKey
			{
				get;
				private set;
			}

			public Rectangle Bounds
			{
				get;
				private set;
			}

			public TextLayout TextLayout
			{
				get
				{
					return this.textLayout;
				}
			}

			private string iconUri;
			private TextLayout textLayout;
		}


		private static string GetIconDescripton(Canvas.IconKey iconKey)
		{
			var builder = new System.Text.StringBuilder ();

			builder.Append ("Taille: ");
			builder.Append (iconKey.Size.Width.ToString ());
			builder.Append ("×");
			builder.Append (iconKey.Size.Height.ToString ());

			if (!string.IsNullOrEmpty (iconKey.Language))
			{
				builder.Append ("<br/>");
				builder.Append ("Langue: ");
				builder.Append (iconKey.Language);
			}

			if (!string.IsNullOrEmpty (iconKey.Style))
			{
				builder.Append ("<br/>");
				builder.Append ("Style: ");
				builder.Append (iconKey.Style);
			}

			return builder.ToString ();
		}


		private string							lastIconUri;
		private readonly List<OneIcon>			icons;
	}
}
