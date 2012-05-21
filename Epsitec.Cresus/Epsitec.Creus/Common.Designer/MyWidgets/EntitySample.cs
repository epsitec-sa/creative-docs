using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Designer.EntitiesEditor;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant de dessiner un échantillon d'entité (boîte vide).
	/// </summary>
	public class EntitySample : Widget
	{
		public EntitySample() : base()
		{
			this.mainColor = AbstractObject.MainColor.Blue;
		}

		public EntitySample(Widget embedder)
			: this ()
		{
			this.SetEmbedder(embedder);
		}


		public AbstractObject.MainColor MainColor
		{
			get
			{
				return this.mainColor;
			}
			set
			{
				if (this.mainColor != value)
				{
					this.mainColor = value;
					this.Invalidate ();
				}
			}
		}

		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				if (this.title != value)
				{
					this.title = value;
					this.Invalidate ();
				}
			}
		}

		public string Subtitle
		{
			get
			{
				return this.subtitle;
			}
			set
			{
				if (this.subtitle != value)
				{
					this.subtitle = value;
					this.Invalidate ();
				}
			}
		}

		public DataLifetimeExpectancy DataLifetimeExpectancy
		{
			get
			{
				return this.dataLifetimeExpectancy;
			}
			set
			{
				if (this.dataLifetimeExpectancy != value)
				{
					this.dataLifetimeExpectancy = value;
					this.Invalidate ();
				}
			}
		}

		public StructuredTypeFlags StructuredTypeFlags
		{
			get
			{
				return this.structuredTypeFlags;
			}
			set
			{
				if (this.structuredTypeFlags != value)
				{
					this.structuredTypeFlags = value;
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;
			rect.Deflate (10);

			if (!rect.IsSurfaceZero)
			{
				ObjectBox.DrawFrame (graphics, rect, this.mainColor, false, true, false, this.title, this.subtitle, this.dataLifetimeExpectancy, this.structuredTypeFlags);
			}
		}


		private AbstractObject.MainColor mainColor;
		private string title;
		private string subtitle;
		private DataLifetimeExpectancy dataLifetimeExpectancy;
		private StructuredTypeFlags structuredTypeFlags;
	}
}
