using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ZoomHistory permet de mémoriser l'historique des zooms.
	/// </summary>
	public class ZoomHistory
	{
		public class ZoomElement
		{
			public double		Zoom;
			public Point		Center;
		}

		public ZoomHistory()
		{
		}
		
		public void Clear()
		{
			this.list.Clear();
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public void Add(ZoomElement item)
		{
			int total = this.list.Count;
			if ( total > 0 )
			{
				ZoomElement last = this.list[total-1] as ZoomElement;
				if ( System.Math.Abs(last.Zoom-item.Zoom) < 0.001 )  return;
			}
			this.list.Add(item);
		}

		public ZoomElement Remove()
		{
			int total = this.list.Count;
			if ( total == 0 )  return null;
			ZoomElement item = this.list[total-1] as ZoomElement;
			this.list.RemoveAt(total-1);
			return item;
		}


		protected System.Collections.ArrayList	list = new System.Collections.ArrayList();
	}
}
