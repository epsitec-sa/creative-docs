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

		public void ArrowAdd(Address.Type type, int level)
		{
			this.addresses.Add(new Address(type, level));
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
				double x = System.Math.Floor(rect.Right - address.Level*6)-1;
				
				switch (address.AddressType)
				{
					case Address.Type.StartToUp:
						path.MoveTo(rect.Left, y);
						path.LineTo(x, y);
						path.LineTo(x, rect.Top);
						break;

					case Address.Type.StartToDown:
						path.MoveTo(rect.Left, y);
						path.LineTo(x, y);
						path.LineTo(x, rect.Bottom);
						break;

					case Address.Type.Line:
						path.MoveTo(x ,rect.Bottom);
						path.LineTo(x, rect.Top);
						break;

					case Address.Type.ArrowFromUp:
						path.MoveTo(x, rect.Top);
						path.LineTo(x, y);
						path.LineTo(rect.Left, y);
						path.LineTo(rect.Left+10, y-5);
						path.MoveTo(rect.Left, y);
						path.LineTo(rect.Left+10, y+5);
						break;

					case Address.Type.ArrowFromDown:
						path.MoveTo(x, rect.Bottom);
						path.LineTo(x, y);
						path.LineTo(rect.Left, y);
						path.LineTo(rect.Left+10, y-5);
						path.MoveTo(rect.Left, y);
						path.LineTo(rect.Left+10, y+5);
						break;

					case Address.Type.Arrow:
						path.MoveTo(x, y);
						path.LineTo(rect.Left, y);
						path.LineTo(rect.Left+10, y-5);
						path.MoveTo(rect.Left, y);
						path.LineTo(rect.Left+10, y+5);
						break;
				}

				graphics.Rasterizer.AddOutline(path, 2);
				graphics.RenderSolid(CodeAddress.colors[address.Level%7]);

				path.Dispose();
			}
		}


		protected static readonly Color[] colors =
		{
			Color.FromRgb(0.0, 0.0, 0.0),  // noir
			Color.FromRgb(0.7, 0.0, 0.0),  // rouge
			Color.FromRgb(0.0, 0.7, 0.0),  // vert
			Color.FromRgb(0.0, 0.0, 0.7),  // bleu
			Color.FromRgb(0.7, 0.7, 0.0),  // jaune
			Color.FromRgb(0.7, 0.0, 0.7),  // magenta
			Color.FromRgb(0.0, 0.7, 0.7),  // cyan
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
			}

			public Address(Type type, int level)
			{
				this.type = type;
				this.level = level;
			}

			public Type AddressType
			{
				get
				{
					return this.type;
				}
			}

			public int Level
			{
				get
				{
					return this.level;
				}
			}

			private Type type;
			private int level;
		}


		protected List<Address> addresses;
	}
}
