//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe FrameList gère la liste des cadres utilisés pour couler le
	/// texte.
	/// </summary>
	public sealed class FrameList
	{
		public FrameList(Text.Context context)
		{
			this.context = context;
			this.list    = new System.Collections.ArrayList ();
		}
		
		
		public ITextFrame						this[int index]
		{
			get
			{
				return this.list[index] as ITextFrame;
			}
		}
		
		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}
		
		public Text.Context						TextContext
		{
			get
			{
				return this.context;
			}
		}
		
		public IPageCollection					PageCollection
		{
			get
			{
				return this.context.PageCollection;
			}
		}
		
		
		public int IndexOf(ITextFrame frame)
		{
			return this.list.IndexOf (frame);
		}
		
		
		public void InsertAt(int index, ITextFrame new_frame)
		{
			this.list.Insert (index, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		public void InsertBefore(ITextFrame existing_frame, ITextFrame new_frame)
		{
			Debug.Assert.IsFalse (this.list.Contains (new_frame));
			Debug.Assert.IsTrue (this.list.Contains (existing_frame));
			
			this.list.Insert (this.list.IndexOf (existing_frame)+0, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		public void InsertAfter(ITextFrame existing_frame, ITextFrame new_frame)
		{
			Debug.Assert.IsFalse (this.list.Contains (new_frame));
			Debug.Assert.IsTrue (this.list.Contains (existing_frame));
			
			this.list.Insert (this.list.IndexOf (existing_frame)+1, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		
		public void Remove(ITextFrame frame)
		{
			this.list.Remove (frame);
			this.HandleRemoval (frame);
		}
		
		
		private void HandleInsertion(ITextFrame frame)
		{
		}
		
		private void HandleRemoval(ITextFrame frame)
		{
		}
		
		
		
		private Text.Context					context;
		private System.Collections.ArrayList	list;
	}
}
