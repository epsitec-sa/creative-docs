using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	public enum Type
	{
		None,
		OpaqueRegular,
		TransparencyRegular,
		OpaqueGradient,
		TransparencyGradient,
		OpaquePattern,
		TransparencyPattern,
	}

	/// <summary>
	/// La classe ComplexSurface enregistre les informations d'une surface complexe.
	/// </summary>
	public class ComplexSurface
	{
		public ComplexSurface(int page,
							  IPaintPort port,
							  Objects.Layer layer,
							  Objects.Abstract obj,
							  Properties.Abstract fill,
							  Properties.Line stroke,
							  int rank,
							  int id)
		{
			this.page     = page;
			this.layer    = layer;
			this.obj      = obj;
			this.fill     = fill;
			this.stroke   = stroke;
			this.type     = fill.TypeComplexSurfacePDF(port);
			this.isSmooth = fill.IsSmoothSurfacePDF(port);
			this.rank     = rank;
			this.id       = id;
			this.matrix   = new Transform();  // matrice identité
		}

		// Libère la surface.
		public void Dispose()
		{
			this.layer    = null;
			this.obj      = null;
			this.fill     = null;
			this.stroke   = null;
			this.matrix   = null;
		}

		// Numéro de la page (1..n).
		public int Page
		{
			get { return this.page; }
		}

		// Calque contenant cette surface.
		public Objects.Layer Layer
		{
			get { return this.layer; }
		}

		// Objet utilisant cette surface.
		public Objects.Abstract Object
		{
			get { return this.obj; }
		}

		// Propriété ayant généré cette surface (Gradient ou Font).
		public Properties.Abstract Fill
		{
			get { return this.fill; }
		}

		// Propriété ayant généré cette surface.
		public Properties.Line Stroke
		{
			get { return this.stroke; }
		}

		// Type de la surface.
		public Type Type
		{
			get { return this.type; }
		}

		// Surface floue ?
		public bool IsSmooth
		{
			get { return this.isSmooth; }
		}

		// Rang dans l'objet (0..n).
		public int Rank
		{
			get { return this.rank; }
		}

		// Identificateur unique.
		public int Id
		{
			get { return this.id; }
		}

		// Matrice de transformation.
		public Transform Matrix
		{
			get { return this.matrix; }
			set { this.matrix = value; }
		}

		protected int					page;  // 1..n
		protected Objects.Layer			layer;
		protected Objects.Abstract		obj;
		protected Properties.Abstract	fill;
		protected Properties.Line		stroke;
		protected Type					type;
		protected bool					isSmooth;
		protected int					rank;
		protected int					id;
		protected Transform				matrix;
	}
}
