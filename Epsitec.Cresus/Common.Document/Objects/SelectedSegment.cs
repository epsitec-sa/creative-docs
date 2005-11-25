using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe SelectedSegment représente un segment sélectionné par le modeleur.
	/// </summary>
	public class SelectedSegment
	{
		public enum Type
		{
			None,
			Line,
			Curve,
		}

		public SelectedSegment(Document document, Objects.Abstract obj, int rank, Point pos)
		{
			this.document = document;
			this.rank = rank;
			this.scale = 0;

			Path path = obj.GetMagnetPath();
			Point p1, s1, s2, p2;
			this.type = Geometry.PathExtract(path, rank, out p1, out s1, out s2, out p2);

			if ( this.type == Type.Line )
			{
				pos = Point.Projection(p1, p2, pos);
				if ( !Point.Equals(p1, p2) )
				{
					this.scale = Point.Distance(p1, pos) / Point.Distance(p1, p2);
				}
				pos = Point.Scale(p1, p2, this.scale);
			}

			if ( this.type == Type.Curve )
			{
				this.scale = Point.FindBezierParameter(p1, s1, s2, p2, pos);
				pos = Point.FromBezier(p1, s1, s2, p2, this.scale);
			}

			this.handle = new Handle(this.document);
			this.handle.Position = pos;
			this.handle.IsVisible = true;
			this.handle.Type = HandleType.Add;
		}

		// Rang du segment dans le chemin simplifié (GetMagnetPath) de l'objet.
		public int Rank
		{
			get
			{
				return this.rank;
			}
		}

		// Position de la poignée sur le segment.
		public Point Position
		{
			get
			{
				return this.handle.Position;
			}

			set
			{
				this.handle.Position = value;
			}
		}

		// Dessine la poignée sur le segment.
		public void Draw(Graphics graphics, DrawingContext drawingContext)
		{
			this.handle.Draw(graphics, drawingContext);
		}


		// Cherche un segment sélectionné dans une liste.
		public static int Search(System.Collections.ArrayList list, int rank)
		{
			for ( int i=0 ; i<list.Count ; i++ )
			{
				SelectedSegment ss = list[i] as SelectedSegment;
				if ( ss.Rank == rank )  return i;
			}
			return -1;
		}

		// Mémorise pour le undo.
		public static void InsertOpletGeometry(System.Collections.ArrayList list, Objects.Abstract obj)
		{
		}

		// Mise à jour après un changement de géométrie.
		public static void Update(System.Collections.ArrayList list, Objects.Abstract obj)
		{
			Path path = obj.GetMagnetPath();

			for ( int i=0 ; i<list.Count ; i++ )
			{
				SelectedSegment ss = list[i] as SelectedSegment;

				Point p1, s1, s2, p2;
				Geometry.PathExtract(path, ss.rank, out p1, out s1, out s2, out p2);

				if ( ss.type == Type.Line )
				{
					ss.Position = Point.Scale(p1, p2, ss.scale);
				}

				if ( ss.type == Type.Curve )
				{
					ss.Position = Point.FromBezier(p1, s1, s2, p2, ss.scale);
				}
			}
		}


		protected Document					document;
		protected int						rank;
		protected Type						type;
		protected double					scale;
		protected Handle					handle;
	}
}
