namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public class Message
	{
		public Message()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		public bool					FilterNoChildren
		{
			get { return this.filter_no_children; }
		}
		
		public bool					FilterOnlyFocused
		{
			get { return this.filter_only_focused; }
		}
		
		public bool					FilterOnlyOnHit
		{
			get { return this.filter_only_on_hit; }
		}
		
		public bool					Processed
		{
			get { return this.is_processed; }
			set { this.is_processed = value; }
		}
		
		protected bool				filter_no_children;
		protected bool				filter_only_focused;
		protected bool				filter_only_on_hit;
		protected bool				is_processed;
	}
}
