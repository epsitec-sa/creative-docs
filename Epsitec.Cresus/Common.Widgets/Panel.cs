namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for Panel.
	/// </summary>
	public class Panel : AbstractGroup
	{
		public Panel()
		{
		}
		
		public Panel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public override Drawing.Rectangle GetClipBounds()
		{
			Drawing.Rectangle parent_clip = this.MapParentToClient (this.parent.GetClipBounds ());
			Drawing.Rectangle client_clip = base.GetClipBounds ();
			
			return Drawing.Rectangle.Intersection (parent_clip, client_clip);
		}
	}
}
