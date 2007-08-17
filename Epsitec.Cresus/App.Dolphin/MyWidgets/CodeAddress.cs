//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public int ArrowMaxLevel
		{
			get
			{
				int max = 0;

				foreach (Address address in this.addresses)
				{
					max = System.Math.Max(max, address.Level);
				}

				return max;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;
			double y = System.Math.Floor(rect.Center.Y);

			foreach (Address address in this.addresses)
			{
				Path path = new Path();
				double x1 = rect.Left + (address.Error ? 12:0);
				double x2 = System.Math.Floor(rect.Right - (address.Level+2)*6);
				double r = 6;
				double k = r*Path.Kappa;
				
				if (address.Error)
				{
					Rectangle box = new Rectangle(rect.Left-1, rect.Bottom, x1-rect.Left, rect.Height);
					box.Deflate(0.5);

					graphics.AddFilledRectangle(box);
					graphics.RenderSolid(Color.FromRgb(1.0, 0.8, 0.0));  // orange

					graphics.AddRectangle(box);
					graphics.RenderSolid(Color.FromBrightness(0.41));  // gris foncé

					graphics.Color = Color.FromBrightness(0);
					graphics.PaintText(box.Left, box.Bottom+1, box.Width, box.Height, "!", Font.GetFont(Font.DefaultFontFamily, "Bold"), 14, ContentAlignment.MiddleCenter);
				}

				switch (address.AddressType)
				{
					case Address.Type.StartToUp:
						path.MoveTo(x1, y+4);
						path.LineTo(x2-r, y+4);
						path.CurveTo(x2-r+k, y+4, x2, rect.Top-k, x2, rect.Top);
						break;

					case Address.Type.StartToDown:
						path.MoveTo(x1, y+4);
						path.LineTo(x2-r, y+4);
						path.CurveTo(x2-r+k, y+4, x2, y+4-r+k, x2, y+4-r);
						path.LineTo(x2, rect.Bottom);
						break;

					case Address.Type.Line:
						path.MoveTo(x2 ,rect.Bottom);
						path.LineTo(x2, rect.Top);
						break;

					case Address.Type.ArrowFromUp:
						path.MoveTo(x2, rect.Top);
						path.LineTo(x2, y-4+r);
						path.CurveTo(x2, y-4+r-k, x2-r+k, y-4, x2-r, y-4);
						path.LineTo(x1, y-4);

						path.MoveTo(x1, y-4);
						path.LineTo(x1+10, y-4-5);
						path.MoveTo(x1, y-4);
						path.LineTo(x1+10, y-4+5);
						break;

					case Address.Type.ArrowFromDown:
						path.MoveTo(x2, rect.Bottom);
						path.CurveTo(x2, rect.Bottom+k, x2-r+k, y-4, x2-r, y-4);
						path.LineTo(x1, y-4);

						path.MoveTo(x1, y-4);
						path.LineTo(x1+10, y-4-5);
						path.MoveTo(x1, y-4);
						path.LineTo(x1+10, y-4+5);
						break;

					case Address.Type.Arrow:
						path.MoveTo(x2, y-4);
						path.LineTo(x1, y-4);

						path.MoveTo(x1, y-4);
						path.LineTo(x1+10, y-4-5);
						path.MoveTo(x1, y-4);
						path.LineTo(x1+10, y-4+5);
						break;

					case Address.Type.Fear:
						path.MoveTo(x1, y+4);
						path.LineTo(rect.Right, y+4);

						path.MoveTo(rect.Right, y+4);
						path.LineTo(rect.Right-10, y+4-5);
						path.MoveTo(rect.Right, y+4);
						path.LineTo(rect.Right-10, y+4+5);
						break;
				}

				Color color = CodeAddress.colors[address.BaseAddress/3%4];

				if (address.Error)
				{
					rect.Right = x2;
					graphics.Rasterizer.AddOutline(path, 2);
					Geometry.RenderHorizontalGradient(graphics, rect, Color.FromAlphaRgb(0, color.R, color.G, color.B), color);
				}
				else
				{
					graphics.Rasterizer.AddOutline(path, 2);
					graphics.RenderSolid(color);
				}

				path.Dispose();
			}
		}


		protected static readonly Color[] colors =
		{
			Color.FromBrightness(0.6),
			Color.FromBrightness(0.4),
			Color.FromBrightness(0.2),
			Color.FromBrightness(0.0),
		};


		public struct Address
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
				Fear,
			}

			public Address(Type type, int baseAddress, int level, bool error)
			{
				this.type = type;
				this.baseAddress = baseAddress;
				this.level = level;
				this.error = error;
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

			private Type type;
			private int baseAddress;
			private int level;
			private bool error;
		}


		protected List<Address> addresses;
	}
}
