//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Permet d'afficher une ligne pour une adresse.
	/// </summary>
	public class CodeAddress : Widget
	{
		public CodeAddress() : base()
		{
			this.addresses = new List<Address>();
		}

		public CodeAddress(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public void ArrowClear()
		{
			this.addresses.Clear();
			this.Invalidate();
		}

		public void ArrowAdd(Address.Type type, int baseAddress, int level, bool error)
		{
			this.addresses.Add(new Address(type, baseAddress, level, error));
			this.Invalidate();
		}

		public int DetectAddress(Point pos)
		{
			//	Détecte la flèche visée par la souris.
			//	Retourne l'adresse de base ou Misc.undefined.
			if (!this.Client.Bounds.Contains(pos))
			{
				return Misc.undefined;
			}

			foreach (Address address in this.addresses)
			{
				Path path = this.GetArrowPath(address);
				bool detect = Geometry.DetectOutline(path, 4, pos);
				path.Dispose();

				if (detect)
				{
					return address.BaseAddress;
				}
			}

			return Misc.undefined;
		}

		public void HiliteBaseAddress(int baseAddress)
		{
			//	Met en évidence les fragments de flèches utilisant une adresse de base donnée.
			bool changed = false;

			for (int i=0; i<this.addresses.Count; i++)
			{
				Address address = this.addresses[i];

				if (address.Hilite != (baseAddress == address.BaseAddress))
				{
					address.Hilite = (baseAddress == address.BaseAddress);
					changed = true;
				}
			}

			if (changed)
			{
				this.Invalidate();
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;

			foreach (Address address in this.addresses)
			{
				double x1 = rect.Left + (address.Error ? 12:0);
				double x2 = System.Math.Floor(rect.Right - (address.Level+2)*6);
				
				if (address.Error)
				{
					Rectangle box = new Rectangle(rect.Left-1, rect.Bottom, x1-rect.Left, rect.Height);
					box.Deflate(0.5);

					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(Color.FromRgb(1.0, 0.8, 0.0));  // orange

					graphics.AddRectangle(box);
					graphics.RenderSolid(DolphinApplication.FromBrightness(0.41));  // gris foncé

					graphics.Color = DolphinApplication.FromBrightness(0);
					graphics.PaintText(box.Left, box.Bottom+1, box.Width, box.Height, "!", Font.GetFont(Font.DefaultFontFamily, "Bold"), 14, ContentAlignment.MiddleCenter);
				}

				Path path = this.GetArrowPath(address);

				if (address.Hilite)
				{
					graphics.Rasterizer.AddOutline(path, 6);
					graphics.RenderSolid(DolphinApplication.FromBrightness(0.8));  // fond gris clair, utile si MarkPC
				}

				Color color = address.Hilite ? DolphinApplication.ColorHilite : CodeAddress.colors[address.BaseAddress/3%4];

				if (address.Error)
				{
					Rectangle box = rect;
					box.Right = x2;
					graphics.Rasterizer.AddOutline(path, 2);
					Geometry.RenderHorizontalGradient(graphics, box, Color.FromAlphaRgb(0, color.R, color.G, color.B), color);
				}
				else
				{
					graphics.Rasterizer.AddOutline(path, 2);
					graphics.RenderSolid(color);
				}

				path.Dispose();
			}
		}

		protected Path GetArrowPath(Address address)
		{
			Rectangle rect = this.Client.Bounds;

			double x1 = rect.Left + (address.Error ? 12:0);
			double x2 = System.Math.Floor(rect.Right - (address.Level+2)*6);
			double d = 3;
			double r = rect.Height/2-d;
			double k = r*Path.Kappa;
			double ax = 8;
			double ay = 4;
			double y = System.Math.Floor(rect.Center.Y);
			
			Path path = new Path();
			switch (address.AddressType)
			{
				case Address.Type.StartToUp:
					path.MoveTo(x1, y+d);
					path.LineTo(x2-r, y+d);
					path.CurveTo(x2-r+k, y+d, x2, rect.Top-k, x2, rect.Top);
					break;

				case Address.Type.StartToDown:
					path.MoveTo(x1, y+d);
					path.LineTo(x2-r, y+d);
					path.CurveTo(x2-r+k, y+d, x2, y+d-r+k, x2, y+d-r);
					path.LineTo(x2, rect.Bottom);
					break;

				case Address.Type.Line:
					path.MoveTo(x2 ,rect.Bottom);
					path.LineTo(x2, rect.Top);
					break;

				case Address.Type.ArrowFromUp:
					path.MoveTo(x2, rect.Top);
					path.LineTo(x2, y-d+r);
					path.CurveTo(x2, y-d+r-k, x2-r+k, y-d, x2-r, y-d);
					path.LineTo(x1, y-d);

					path.MoveTo(x1, y-d);
					path.LineTo(x1+ax, y-d-ay);
					path.MoveTo(x1, y-d);
					path.LineTo(x1+ax, y-d+ay);
					break;

				case Address.Type.ArrowFromDown:
					path.MoveTo(x2, rect.Bottom);
					path.CurveTo(x2, rect.Bottom+k, x2-r+k, y-d, x2-r, y-d);
					path.LineTo(x1, y-d);

					path.MoveTo(x1, y-d);
					path.LineTo(x1+ax, y-d-ay);
					path.MoveTo(x1, y-d);
					path.LineTo(x1+ax, y-d+ay);
					break;

				case Address.Type.Arrow:
					path.MoveTo(x2, y-d);
					path.LineTo(x1, y-d);

					path.MoveTo(x1, y-d);
					path.LineTo(x1+ax, y-d-ay);
					path.MoveTo(x1, y-d);
					path.LineTo(x1+ax, y-d+ay);
					break;

				case Address.Type.Far:
					path.MoveTo(x1, y+d);
					path.LineTo(rect.Right, y+d);

					path.MoveTo(rect.Right, y+d);
					path.LineTo(rect.Right-ax, y+d-ay);
					path.MoveTo(rect.Right, y+d);
					path.LineTo(rect.Right-ax, y+d+ay);
					break;
			}

			return path;
		}


		protected static readonly Color[] colors =
		{
			DolphinApplication.FromBrightness(0.6),
			DolphinApplication.FromBrightness(0.4),
			DolphinApplication.FromBrightness(0.2),
			DolphinApplication.FromBrightness(0.0),
		};


		/// <summary>
		/// Cette classe correspond à un fragment de flèche.
		/// </summary>
		public class Address
		{
			public enum Type
			{
				None,
				StartToUp,
				StartToDown,
				Line,
				ArrowFromUp,
				ArrowFromDown,
				Arrow,
				Far,
			}

			public Address(Type type, int baseAddress, int level, bool error)
			{
				this.type = type;
				this.baseAddress = baseAddress;
				this.level = level;
				this.error = error;
				this.hilite = false;
			}

			public Type AddressType
			{
				get
				{
					return this.type;
				}
			}

			public int BaseAddress
			{
				get
				{
					return this.baseAddress;
				}
			}

			public int Level
			{
				get
				{
					return this.level;
				}
			}

			public bool Error
			{
				get
				{
					return this.error;
				}
			}

			public bool Hilite
			{
				get
				{
					return this.hilite;
				}
				set
				{
					this.hilite = value;
				}
			}

			private Type type;
			private int baseAddress;
			private int level;
			private bool error;
			private bool hilite;
		}


		protected List<Address> addresses;
	}
}
