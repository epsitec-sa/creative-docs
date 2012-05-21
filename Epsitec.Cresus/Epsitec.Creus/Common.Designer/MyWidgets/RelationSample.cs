using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant de dessiner un échantillon de relation (flèche).
	/// </summary>
	public class RelationSample : Widget
	{
		public RelationSample() : base()
		{
			this.relation = FieldRelation.None;
		}

		public RelationSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public FieldRelation Relation
		{
			get
			{
				return this.relation;
			}
			set
			{
				if (this.relation != value)
				{
					this.relation = value;
					this.Invalidate();
				}
			}
		}

		public bool IsPrivateRelation
		{
			get
			{
				return this.isPrivateRelation;
			}
			set
			{
				if (this.isPrivateRelation != value)
				{
					this.isPrivateRelation = value;
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			Point start = new Point(rect.Left, rect.Center.Y);
			Point end = new Point(rect.Right, rect.Center.Y);

			if (this.relation != FieldRelation.None)
			{
				graphics.LineWidth = 2;
				graphics.AddLine(start, end);
				EntitiesEditor.AbstractObject.DrawEndingArrow(graphics, start, end, this.relation, this.isPrivateRelation);
				graphics.RenderSolid(Color.FromBrightness(0));
				graphics.LineWidth = 1;
			}
		}



		protected FieldRelation relation;
		protected bool isPrivateRelation;
	}
}
