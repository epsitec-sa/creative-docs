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
		public FrameList(TextFitter fitter)
		{
			this.fitter = fitter;
			this.frames = new System.Collections.ArrayList ();
		}
		
		
		public ITextFrame						this[int index]
		{
			get
			{
				return this.frames[index] as ITextFrame;
			}
		}
		
		public int								Count
		{
			get
			{
				return this.frames.Count;
			}
		}
		
		public TextFitter						TextFitter
		{
			get
			{
				return this.fitter;
			}
		}
		
		public IPageCollection					PageCollection
		{
			get
			{
				return this.fitter.PageCollection;
			}
		}
		
		
		public int IndexOf(ITextFrame frame)
		{
			return this.frames.IndexOf (frame);
		}
		
		
		public void InsertAt(int index, ITextFrame new_frame)
		{
			this.frames.Insert (index, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		public void InsertBefore(ITextFrame existing_frame, ITextFrame new_frame)
		{
			Debug.Assert.IsFalse (this.frames.Contains (new_frame));
			Debug.Assert.IsTrue (this.frames.Contains (existing_frame));
			
			this.frames.Insert (this.frames.IndexOf (existing_frame)+0, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		public void InsertAfter(ITextFrame existing_frame, ITextFrame new_frame)
		{
			Debug.Assert.IsFalse (this.frames.Contains (new_frame));
			Debug.Assert.IsTrue (this.frames.Contains (existing_frame));
			
			this.frames.Insert (this.frames.IndexOf (existing_frame)+1, new_frame);
			this.HandleInsertion (new_frame);
		}
		
		
		public void Remove(ITextFrame frame)
		{
			this.frames.Remove (frame);
			this.HandleRemoval (frame);
		}
		
		
		private void HandleInsertion(ITextFrame frame)
		{
		}
		
		private void HandleRemoval(ITextFrame frame)
		{
		}
		
		
		
		private TextFitter						fitter;
		private System.Collections.ArrayList	frames;
	}
}
