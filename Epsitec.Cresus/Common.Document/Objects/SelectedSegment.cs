using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

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

			Path path = obj.GetShaperPath();
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

		public int Rank
		{
			//	Rang du segment dans le chemin simplifié (GetMagnetPath) de l'objet.
			get
			{
				return this.rank;
			}
		}

		public Point Position
		{
			//	Position de la poignée sur le segment.
			get
			{
				return this.handle.Position;
			}

			set
			{
				this.handle.Position = value;
			}
		}

		public bool Detect(Point pos)
		{
			//	Détecte si la souris est sur la poignée.
			return this.handle.Detect(pos);
		}

		public void Draw(Graphics graphics, DrawingContext drawingContext)
		{
			//	Dessine la poignée sur le segment.
			this.handle.Draw(graphics, drawingContext);
		}


		#region OpletGeometry
		protected void InsertOpletGeometry()
		{
			//	Ajoute un oplet pour mémoriser la géométrie du segment.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletGeometry oplet = new OpletGeometry(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise toutes les informations sur la géométrie de l'objet.
		protected class OpletGeometry : AbstractOplet
		{
			public OpletGeometry(SelectedSegment host)
			{
				this.host = host;
				this.pos = host.Position;
			}

			protected void Swap()
			{
				Point temp = this.pos;
				this.pos = this.host.Position;
				this.host.Position = temp;
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected SelectedSegment				host;
			protected Point							pos;
		}
		#endregion

		
		public static int Search(UndoableList list, int rank)
		{
			//	Cherche un segment sélectionné dans une liste.
			for ( int i=0 ; i<list.Count ; i++ )
			{
				SelectedSegment ss = list[i] as SelectedSegment;
				if ( ss.Rank == rank )  return i;
			}
			return -1;
		}

		public static int[] Sort(UndoableList list)
		{
			//	Retourne un index trié de tous les segments sélectionnés pour que ceux ayant
			//	un rang élévé soient traités en premier.
			int[] index = new int[list.Count];
			for ( int i=0 ; i<list.Count ; i++ )
			{
				index[i] = i;
			}

			bool more;
			do
			{
				more = false;
				for ( int i=0 ; i<list.Count-1 ; i++ )
				{
					SelectedSegment ss1 = list[index[i+0]] as SelectedSegment;
					SelectedSegment ss2 = list[index[i+1]] as SelectedSegment;

					if ( SelectedSegment.SortCompare(ss1, ss2) )
					{
						int t      = index[i+0];
						index[i+0] = index[i+1];  // index[i+0] <-> index[i+1]
						index[i+1] = t;
						more = true;
					}
				}
			}
			while ( more );

			return index;
		}

		protected static bool SortCompare(SelectedSegment ss1, SelectedSegment ss2)
		{
			if ( ss1.rank != ss2.rank )
			{
				return (ss1.rank < ss2.rank);
			}
			else
			{
				return (ss1.scale < ss2.scale);
			}
		}

		public static void InsertOpletGeometry(UndoableList list, Objects.Abstract obj)
		{
			//	Mémorise pour le undo.
			for ( int i=0 ; i<list.Count ; i++ )
			{
				SelectedSegment ss = list[i] as SelectedSegment;
				ss.InsertOpletGeometry();
			}
		}

		public static void Update(UndoableList list, Objects.Abstract obj)
		{
			//	Mise à jour après un changement de géométrie.
			Path path = obj.GetShaperPath();

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
