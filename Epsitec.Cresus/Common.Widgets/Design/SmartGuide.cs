namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe SmartGuide propose un service d'alignement au moyen de guides
	/// verticaux et horizontaux, basé sur la connaissance du widget en cours de
	/// déplacement et du widget cible (conteneur, futur parent).
	/// </summary>
	public class SmartGuide
	{
		public SmartGuide()
		{
		}
		
		public SmartGuide(Widget widget) : this()
		{
			this.widget  = widget;
		}
		
		public SmartGuide(Widget widget, Drawing.GripId id) : this(widget)
		{
			this.grip_id = id;
		}
		
		public SmartGuide(Widget widget, Drawing.GripId id, Widget target) : this(widget, id)
		{
			this.target = target;
		}
		
		
		public Drawing.GripId			GripId
		{
			get { return this.grip_id; }
			set { this.grip_id = value; }
		}
		
		public Widget					Widget
		{
			get { return this.widget; }
			set { this.widget = value; }
		}
		
		public Widget					Target
		{
			get { return this.target; }
			set { this.target = value; }
		}
		
		
		public Drawing.Point ConstrainCursor(Drawing.Point cursor, Drawing.Rectangle bounds)
		{
			double x = this.ConstrainCursorX (cursor.X, bounds);
			double y = this.ConstrainCursorY (cursor.Y, bounds);
			
			return cursor;
		}
		
		public double ConstrainCursorX(double x, Drawing.Rectangle bounds)
		{
			return x;
		}
		
		public double ConstrainCursorY(double y, Drawing.Rectangle bounds)
		{
			return y;
		}
		
		
		
		protected Drawing.GripId		grip_id;
		protected Widget				widget;
		protected Widget				target;
	}
}
