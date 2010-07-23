//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public abstract class AbstractEntityPrinter
	{
		public AbstractEntityPrinter()
			: base ()
		{
			this.documentTypes = new List<DocumentType> ();
			this.documentOptionsNameSelected = new List<string> ();
			this.documentContainer = new DocumentContainer ();
			this.tableColumns = new Dictionary<string, TableColumn> ();
		}


		public List<DocumentType> DocumentTypes
		{
			get
			{
				return this.documentTypes;
			}
		}

		public string DocumentTypeSelected
		{
			get;
			set;
		}

		public List<string> DocumentOptionsSelected
		{
			get
			{
				return this.documentOptionsNameSelected;
			}
		}

		public bool HasDocumentOption(string name)
		{
			return this.documentOptionsNameSelected.Contains (name);
		}


		public virtual string JobName
		{
			get
			{
				return null;
			}
		}

		public virtual Size PageSize
		{
			get
			{
				return Size.Zero;
			}
		}

		public virtual Margins PageMargins
		{
			get
			{
				return new Margins (10);
			}
		}

		public int PageCount
		{
			get
			{
				return this.documentContainer.PageCount;
			}
		}

		public int CurrentPage
		{
			get
			{
				return this.currentPage;
			}
			set
			{
				if (this.currentPage != value)
				{
					this.currentPage = value;
				}
			}
		}

		public int DebugParam1
		{
			get
			{
				return this.debugParam1;
			}
			set
			{
				if (this.debugParam1 != value)
				{
					this.debugParam1 = value;
				}
			}
		}

		public int DebugParam2
		{
			get
			{
				return this.debugParam2;
			}
			set
			{
				if (this.debugParam2 != value)
				{
					this.debugParam2 = value;
				}
			}
		}

		public void Clear()
		{
			this.documentContainer.Clear ();
		}

		public virtual void BuildSections()
		{
			this.documentContainer.PageSize    = this.PageSize;
			this.documentContainer.PageMargins = this.PageMargins;
		}

		public virtual void PrintCurrentPage(IPaintPort port, Rectangle bounds)
		{
			if (this.HasDocumentOption ("Spec"))
			{
				this.PaintSpecimen(port, bounds);
			}
		}

		private void PaintSpecimen(IPaintPort port, Rectangle bounds)
		{
			//	Dessine un très gros "SPECIMEN" au travers de la page.
			var font = Font.GetFont ("Arial", "Bold");

			var initial = port.Transform;

			if (this.PageSize.Height > this.PageSize.Width)  // portrait ?
			{
				port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationDegTransform (60));

				port.Color = Color.FromBrightness (0.95);
				port.PaintText (34, -36, "SPECIMEN", AbstractEntityPrinter.specimenFont, 56);
			}
			else  // paysage ?
			{
				port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationDegTransform (30));

				port.Color = Color.FromBrightness (0.95);
				port.PaintText (30, -4, "SPECIMEN", AbstractEntityPrinter.specimenFont, 56);
			}

			port.Transform = initial;
		}


		protected static void DocumentTypeAddStyles(List<DocumentOption> options)
		{
			//	Ajoute les options d'impression liées aux BV.
			options.Add (new DocumentOption ( /**           **/  "Style du document :"));
			options.Add (new DocumentOption ("Classic", "Style", "Classique", true));
			options.Add (new DocumentOption ("Modern",  "Style", "Moderne"));
		}

		protected static void DocumentTypeAddBV(List<DocumentOption> options)
		{
			//	Ajoute les options d'impression liées aux BV.
			options.Add (new DocumentOption ( /**    **/  "Type de bulletin de versement :"));
			options.Add (new DocumentOption ("BVR", "BV", "BVR orange", true));
			options.Add (new DocumentOption ("BV",  "BV", "BV rose"));

			options.Add (new DocumentOption ( /**         **/  "Mode d'impression du BV :"));
			options.Add (new DocumentOption ("BV.Simul", null, "Fac-similé complet du BV (pour des essais)", true));
			options.Add (new DocumentOption ("BV.Spec",  null, "Ajoute la mention SPECIMEN"));
		}

		protected static void DocumentTypeAddOrientation(List<DocumentOption> options)
		{
			//	Ajoute les options d'impression liées à l'orientation portrait/paysage.
			options.Add (new DocumentOption ( /**                    **/  "Orientation du papier :"));
			options.Add (new DocumentOption ("Vertical",   "Orientation", "Portrait (papier en hauteur)", true));
			options.Add (new DocumentOption ("Horizontal", "Orientation", "Paysage (papier en largeur)"));
		}

		protected static void DocumentTypeAddSpecimen(List<DocumentOption> options)
		{
			//	Ajoute les options d'impression générales.
			options.Add (new DocumentOption ("Spec", null, "Ajoute la mention SPECIMEN"));
		}

		protected static void DocumentTypeAddMargin(List<DocumentOption> options)
		{
			//	Ajoute une marge verticale.
			options.Add (new DocumentOption (20));
		}



		public static AbstractEntityPrinter CreateEntityPrinter(AbstractEntity entity)
		{
			var type = AbstractEntityPrinter.FindType (entity.GetType ());

			if (type == null)
			{
				return null;
			}

			return System.Activator.CreateInstance (type, new object[] { entity }) as AbstractEntityPrinter;
		}
		
		private static System.Type FindType(System.Type entityType)
		{
			var baseTypeName = "AbstractEntityPrinter`1";

			//	Find all concrete classes which use either the generic AbstractEntityPrinter base classes,
			//	which match the entity type (usually, there should be exactly one such type).

			var types = from type in typeof (AbstractEntityPrinter).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			return types.FirstOrDefault ();
		}


		private static readonly Font specimenFont = Font.GetFont ("Arial", "Bold");

		private readonly List<DocumentType>			documentTypes;
		private readonly List<string>				documentOptionsNameSelected;
		protected readonly DocumentContainer		documentContainer;
		protected Dictionary<string, TableColumn>	tableColumns;
		private int									currentPage;
		private int									debugParam1;
		private int									debugParam2;
	}

	public class AbstractEntityPrinter<T> : AbstractEntityPrinter
		where T : AbstractEntity
	{
		public AbstractEntityPrinter(T entity)
		{
			this.entity = entity;
		}

		protected T entity;
	}
}
