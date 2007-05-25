using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer graphiquement des entités.
	/// </summary>
	public class EntityEditor : Widget
	{
		public EntityEditor()
		{
			this.boxes = new List<MyWidgets.EntityBox>();
			this.links = new List<MyWidgets.EntityLink>();
		}

		public EntityEditor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public void AddBox(MyWidgets.EntityBox box)
		{
			box.GeometryChanged += new EventHandler(this.HandleBoxGeometryChanged);
			box.SetParent(this);
			box.SetManualBounds(new Rectangle(20+(180+40)*this.boxes.Count, 1000-20-100, 180, 100));

			this.boxes.Add(box);
		}

		public void AddLink(MyWidgets.EntityLink link)
		{
			link.SetParent(this);
			this.links.Add(link);
		}


		public void UpdateGeometry()
		{
			//	Met à jour la géométrie de toutes les boîtes et de toutes les liaisons.
			foreach (MyWidgets.EntityBox box in this.boxes)
			{
				Rectangle bounds = box.ActualBounds;
				double top = bounds.Top;
				double h = box.GetBestHeight();
				bounds.Bottom = top-h;
				bounds.Height = h;
				box.SetManualBounds(bounds);
			}

			this.UpdateLink(this.links[0], this.boxes[0], 2, this.boxes[1], FieldRelation.Reference);  // lien client
			this.UpdateLink(this.links[1], this.boxes[0], 3, this.boxes[2], FieldRelation.Collection);  // lien articles
			this.UpdateLink(this.links[2], this.boxes[0], 5, this.boxes[3], FieldRelation.Inclusion);  // lien rabais
		}

		protected void UpdateLink(MyWidgets.EntityLink link, MyWidgets.EntityBox src, int srcRank, MyWidgets.EntityBox dst, FieldRelation relation)
		{
			//	Met à jour la géométrie d'une liaison.
			link.SetManualBounds(this.Client.Bounds);
			link.Relation = relation;

			Rectangle srcBounds = src.ActualBounds;
			Rectangle dstBounds = dst.ActualBounds;

			Rectangle srcBoundsLittle = srcBounds;
			Rectangle dstBoundsLittle = dstBounds;
			srcBoundsLittle.Deflate(2);
			dstBoundsLittle.Deflate(2);

			double v = src.GetLinkVerticalPosition(srcRank);
			if (double.IsNaN(v))
			{
				link.Visibility = false;
			}
			else
			{
				link.Visibility = true;

				Point p = new Point(0, v);
				p = src.MapClientToParent(p);

				double min = double.MaxValue;
				MyWidgets.EntityBox.LinkAnchor best = EntityBox.LinkAnchor.Left;
				bool right = true;

				foreach (MyWidgets.EntityBox.LinkAnchor anchor in System.Enum.GetValues(typeof(MyWidgets.EntityBox.LinkAnchor)))
				{
					Point end = dst.GetLinkDestination(p.Y, anchor);
					Point start;
					double d;

					start = new Point(srcBounds.Right-1, p.Y);
					d = Point.Distance(start, end);
					if (Geometry.IsOver(srcBoundsLittle, start, end, 20))  d *= 20;
					if (Geometry.IsOver(dstBoundsLittle, start, end, 20))  d *= 10;
					if (d < min)
					{
						min = d;
						best = anchor;
						right = true;
					}

					start = new Point(srcBounds.Left+1, p.Y);
					d = Point.Distance(start, end);
					if (Geometry.IsOver(srcBoundsLittle, start, end, 20))  d *= 20;
					if (Geometry.IsOver(dstBoundsLittle, start, end, 20))  d *= 10;
					if (d < min)
					{
						min = d;
						best = anchor;
						right = false;
					}
				}

				link.Source = new Point(right ? srcBounds.Right-1 : srcBounds.Left+1, p.Y);
				link.Destination = dst.GetLinkDestination(p.Y, best);
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					if (this.isDragging)
					{
						this.MouseDraggingMove(pos);
					}
					else
					{
						this.MouseHilite(pos);
					}
					message.Consumer = this;
					break;

				case MessageType.MouseDown:
					this.MouseDraggingBegin(pos);
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.MouseDraggingEnd(pos);
					message.Consumer = this;
					break;
			}
		}

		protected void MouseHilite(Point pos)
		{
			foreach (Widget widget in this.Children)
			{
				if (widget is MyWidgets.EntityBox)
				{
					MyWidgets.EntityBox box = widget as MyWidgets.EntityBox;
					box.Hilite(pos);
				}
			}
		}

		protected MyWidgets.EntityBox DetectBox(Point pos)
		{
			foreach (Widget widget in this.Children)
			{
				if (widget is MyWidgets.EntityBox)
				{
					MyWidgets.EntityBox box = widget as MyWidgets.EntityBox;
					if (box.ActualBounds.Contains(pos))
					{
						return box;
					}
				}
			}

			return null;
		}

		protected void MouseDraggingBegin(Point pos)
		{
			MyWidgets.EntityBox box = this.DetectBox(pos);

			if (box != null && box.IsHilited)
			{
				this.draggingBox = box;
				this.draggingPos = pos;
				this.isDragging = true;
			}
		}

		protected void MouseDraggingMove(Point pos)
		{
			Rectangle bounds = this.draggingBox.ActualBounds;

			bounds.Offset(pos-this.draggingPos);
			this.draggingPos = pos;

			this.draggingBox.SetManualBounds(bounds);
			this.UpdateGeometry();
		}

		protected void MouseDraggingEnd(Point pos)
		{
			this.draggingBox = null;
			this.isDragging = false;
		}


		private void HandleBoxGeometryChanged(object sender)
		{
			//	Appelé lorsque la géométrie d'une boîte a changé (changement compact/étendu).
			this.UpdateGeometry();
		}



		protected List<MyWidgets.EntityBox> boxes;
		protected List<MyWidgets.EntityLink> links;
		protected bool isDragging;
		protected Point draggingPos;
		protected MyWidgets.EntityBox draggingBox;
	}
}
