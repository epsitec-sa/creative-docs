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


		#region OpletGeometry
		// Ajoute un oplet pour mémoriser la géométrie du segment.
		protected void InsertOpletGeometry()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletGeometry oplet = new OpletGeometry(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise toutes les informations sur la géométrie de l'objet.
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

		
		// Cherche un segment sélectionné dans une liste.
		public static int Search(UndoableList list, int rank)
		{
			for ( int i=0 ; i<list.Count ; i++ )
			{
				SelectedSegment ss = list[i] as SelectedSegment;
				if ( ss.Rank == rank )  return i;
			}
			return -1;
		}

		// Mémorise pour le undo.
		public static void InsertOpletGeometry(UndoableList list, Objects.Abstract obj)
		{
			for ( int i=0 ; i<list.Count ; i++ )
			{
				SelectedSegment ss = list[i] as SelectedSegment;
				ss.InsertOpletGeometry();
			}
		}

		// Mise à jour après un changement de géométrie.
		public static void Update(UndoableList list, Objects.Abstract obj)
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
