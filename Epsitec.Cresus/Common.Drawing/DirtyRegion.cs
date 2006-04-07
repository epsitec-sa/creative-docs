//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe DirtyRegion permet de représenter une région simple qui
	/// consiste en un petit nombre de rectangles.
	/// </summary>
	public class DirtyRegion
	{
		public DirtyRegion() : this (DirtyRegion.DefaultCount)
		{
		}
		
		public DirtyRegion(int n)
		{
			System.Diagnostics.Debug.Assert (n > 1);
			System.Diagnostics.Debug.Assert (n < 10);
			
			this.rectangles = new Rectangle[n];
			
			for (int i = 0; i < n; i++)
			{
				this.rectangles[i] = Rectangle.Empty;
			}
			
			this.used = 0;
		}
		
		
		public Rectangle[]						Rectangles
		{
			get
			{
				Rectangle[] rects = new Rectangle[this.used];
				
				for (int i = 0; i < this.used; i++)
				{
					rects[i] = this.rectangles[i];
				}
				
				return rects;
			}
		}
		
		
		public void Add(Rectangle rectangle)
		{
			//	Le rectangle est-il déjà entièrement contenu dans la région ?
			
			for (int i = 0; i < this.used; i++)
			{
				if (this.rectangles[i].Contains (rectangle))
				{
					return;
				}
			}
			
			//	Vérifie si notre rectangle couvre complètement un des éléments
			//	composant la région :
			
			int j = 0;
			
			for (int i = 0; i < this.used; i++)
			{
				if (rectangle.Contains (this.rectangles[i]))
				{
					//	On n'a plus besoin de ce rectangle, car le nouveau va l'englober.
				}
				else
				{
					if (j != i)
					{
						this.rectangles[j] = this.rectangles[i];
					}
					j++;
				}
			}
			
			int n = this.rectangles.Length;
			
			if (j == n)
			{
				//	Problème: il n'y a plus de place dans la table des rectangles pour
				//	stocker ce rectangle. Il faut par conséquent fusionner deux rectangles
				//	pour faire de la place.
				
				Rectangle[] rects = new Rectangle[n+1];
				
				this.rectangles.CopyTo (rects, 0);
				rects[n] = rectangle;
				
				this.rectangles = DirtyRegion.MergeSmallestPair (rects);
				this.used       = this.rectangles.Length;
				
				System.Diagnostics.Debug.Assert (this.rectangles.Length == n);
			}
			else
			{
				//	Ajoute simplement le rectangle en fin de table :
				
				this.rectangles[j++] = rectangle;
				
				int first_empty = this.used;
				
				this.used = j;
				
				//	Si la table des rectangles a rétréci, il faut encore vider les rectangles
				//	qui traînent :
				
				while (j < first_empty)
				{
					this.rectangles[j++] = Rectangle.Empty;
				}
			}
		}
		
		public Rectangle[] GenerateStrips()
		{
			return DirtyRegion.GenerateHorizontalStrips (this.Rectangles);
		}
		
		
		public static Rectangle[] MergeSmallestPair(Rectangle[] rectangles)
		{
			int n = rectangles.Length;
			
			if (n < 2)
			{
				return new Rectangle[0];
			}
			
			int index_a = 0;
			int index_b = 1;
			
			double    surface;
			Rectangle union       = Rectangle.MaxValue;
			double    min_surface = Rectangle.MaxValue.Surface;
			
			for (int a = 0; a < n-1; a++)
			{
				for (int b = a+1; b < n; b++)
				{
					union   = Rectangle.Union (rectangles[a], rectangles[b]);
					surface = union.Surface;
					
					if (surface < min_surface)
					{
						index_a = a;
						index_b = b;
						min_surface = surface;
					}
				}
			}
			
			//	Il faut fusionner les rectangles 'A' et 'B', car ce sont eux qui couvrent
			//	la plus petite surface commune.
			
			Rectangle[] result = new Rectangle[n-1];
			
			int j = 0;
			
			for (int i = 0; i < n; i++)
			{
				if ((i == index_a) ||
					(i == index_b))
				{
					//	Saute les rectangles fusionnés.
				}
				else
				{
					result[j++] = rectangles[i];
				}
			}
			
			System.Diagnostics.Debug.Assert (j == n-2);
			
			//	Comme nous avons fusionné deux rectangles de manière à couvrir le moins de
			//	surface possible, on a l'assurance qu'aucun autre rectangle n'est couvert
			//	entièrement par leur union; on peut donc économiser ici une deuxième passe :
			
			result[j] = Rectangle.Union (rectangles[index_a], rectangles[index_b]);
			
			return result;
		}
		
		public static Rectangle[] GenerateHorizontalStrips(Rectangle[] rectangles)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			int n = rectangles.Length;
			
			if (n < 2)
			{
				return rectangles;
			}
			
			//	Détermine la position des frontières horizontales :
			
			double[] ys = new double[n*2];
			
			for (int i = 0; i < n; i++)
			{
				ys[i*2+0] = rectangles[i].Bottom;
				ys[i*2+1] = rectangles[i].Top;
			}
			
			//	Trie les frontières horizontales de manière ascendante :
			
			System.Array.Sort (ys);
			
			//	Trie les rectangles de manière à avoir les bords gauches triés par
			//	coordonnées croissantes :
			
			System.Array.Sort (rectangles, new DirtyRegion.LeftXSorter ());
			
			Segment[] segments = new DirtyRegion.Segment[n];
			Segment[] h_segs   = new DirtyRegion.Segment[n];
			
			for (int i = 0; i < n; i++)
			{
				segments[i] = new DirtyRegion.Segment (rectangles[i].Left, rectangles[i].Right);
				
				System.Diagnostics.Debug.Assert ((i == 0) || (segments[i-1].Left <= segments[i].Left));
			}
			
			for (int i = 0; i < n*2-1; i++)
			{
				//	Génère les rectangles compris entre deux horizontales :
				
				double y_bot = ys[i+0];
				double y_top = ys[i+1];
				
				if (y_top == y_bot)
				{
					continue;
				}
				
				//	Commence par déterminer les segments horizontaux qui sont couverts
				//	par des rectangles, en profitant de l'occasion pour fusionner deux
				//	segments adjacents :
				
				int h_seg_count = 0;
				
				for (int j = 0; j < n; j++)
				{
					if ((rectangles[j].Top > y_bot) &&
						(rectangles[j].Bottom < y_top))
					{
						if (h_seg_count == 0)
						{
							h_segs[h_seg_count++] = segments[j];
						}
						else
						{
							if (DirtyRegion.Segment.Overlap (h_segs[h_seg_count-1], segments[j]))
							{
								h_segs[h_seg_count-1] = DirtyRegion.Segment.Union (h_segs[h_seg_count-1], segments[j]);
							}
							else
							{
								h_segs[h_seg_count++] = segments[j];
							}
						}
					}
				}
				
				for (int j = 0; j < h_seg_count; j++)
				{
					list.Add (Rectangle.FromPoints (h_segs[j].Left, y_bot, h_segs[j].Right, y_top));
				}
			}
			
			rectangles = new Rectangle[list.Count];
			list.CopyTo (rectangles);
			
			return rectangles;
		}
		
		
		#region LeftXSorter Class
		class LeftXSorter : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object a, object b)
			{
				double ax = ((Rectangle)a).Left;
				double bx = ((Rectangle)b).Left;
				
				if (ax == bx)
				{
					return 0;
				}
				if (ax > bx)
				{
					return 1;
				}
				else
				{
					return -1;
				}
			}
			#endregion
		}
		#endregion
		
		#region Segment Struct
		struct Segment
		{
			public Segment(double left, double right)
			{
				this.Left  = left;
				this.Right = right;
			}
			
			
			public static bool Overlap(Segment a, Segment b)
			{
				if ((a.Right >= b.Left) &&
					(a.Left <= b.Right))
				{
					return true;
				}
				
				return false;
			}
			
			public static Segment Union(Segment a, Segment b)
			{
				return new Segment (System.Math.Min  (a.Left, b.Left), System.Math.Max (a.Right, b.Right));
			}
			
			
			public double						Left;
			public double						Right;
		}
		#endregion
		
		protected const int						DefaultCount = 8;
		
		protected Rectangle[]					rectangles;
		protected int							used;
	}
}
