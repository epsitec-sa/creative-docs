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
			//	Ajoute un élément à la fin de la liste.
			int total = this.list.Count;
			if ( total > 0 )
			{
				ZoomElement last = this.list[total-1] as ZoomElement;
				if (ZoomHistory.IsNearlyEqual(last, item))
				{
					return;
				}
			}
			this.list.Add(item);
		}

		public ZoomElement Remove()
		{
			//	Enlève et retourne le dernier élément de la liste.
			//	Retourne null s'il n'y en a plus.
			int total = this.list.Count;
			if (total == 0)
			{
				return null;
			}

			ZoomElement item = this.list[total-1] as ZoomElement;
			this.list.RemoveAt(total-1);
			return item;
		}


		static public bool IsNearlyEqual(ZoomElement a, ZoomElement b)
		{
			//	Retourne true si deux éléments sont presque égaux.
			if (System.Math.Abs(a.Zoom-b.Zoom) > 0.001)
			{
				return false;
			}

			if (System.Math.Abs(a.Center.X-b.Center.X) > 0.01)
			{
				return false;
			}

			if (System.Math.Abs(a.Center.Y-b.Center.Y) > 0.01)
			{
				return false;
			}

			return true;
		}


		protected System.Collections.ArrayList	list = new System.Collections.ArrayList();
	}
}
