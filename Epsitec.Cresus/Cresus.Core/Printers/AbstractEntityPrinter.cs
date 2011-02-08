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
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public abstract class AbstractEntityPrinter
	{
		public AbstractEntityPrinter()
			: base ()
		{
			this.documentPrinters = new List<AbstractDocumentPrinter> ();
			this.documentTypes    = new List<DocumentTypeDefinition> ();

			this.EntityPrintingSettings = new EntityPrintingSettings ();
		}


		public EntityPrintingSettings EntityPrintingSettings
		{
			get;
			set;
		}


		public List<AbstractDocumentPrinter> DocumentPrinters
		{
			get
			{
				return this.documentPrinters;
			}
		}

		public List<DocumentTypeDefinition> DocumentTypes
		{
			get
			{
				return this.documentTypes;
			}
		}

		public DocumentTypeDefinition SelectedDocumentTypeDefinition
		{
			get
			{
				return this.DocumentTypes.Where (x => x.Type == this.EntityPrintingSettings.DocumentTypeSelected).FirstOrDefault ();
			}
		}


		public void ContinuousPrepare(Business.DocumentType type)
		{
			this.PreviewMode = PreviewMode.ContinuousPreview;
			this.EntityPrintingSettings.DocumentTypeSelected = this.AdjustContinuousDocumentType (type);

			var documentType = this.SelectedDocumentTypeDefinition;
			if (documentType != null)
			{
				foreach (var option in documentType.DocumentOptions)
				{
					if (option.DefautState)
					{
						this.EntityPrintingSettings.DocumentOptionsSelected.Add (option.Option);
					}
				}
			}
		}

		protected virtual Business.DocumentType AdjustContinuousDocumentType(Business.DocumentType type)
		{
			return type;
		}

		public PreviewMode PreviewMode
		{
			//	Permet de savoir si on effectue une impression réelle ou un aperçu avant impression.
			get;
			set;
		}


		public int PageCount()
		{
			int count = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				count += documentPrinter.PageCount ();
			}

			return count;
		}

		public int GetPageRelative(int page)
		{
			int firstDocumentPage = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				int pageCount = documentPrinter.PageCount ();

				if (page < firstDocumentPage+pageCount)
				{
					return page-firstDocumentPage;
				}

				firstDocumentPage += pageCount;
			}

			return -1;
		}

		public PageType GetPageType(int page)
		{
			int firstDocumentPage = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				int pageCount = documentPrinter.PageCount ();

				if (page < firstDocumentPage+pageCount)
				{
					return documentPrinter.GetPageType (page-firstDocumentPage);
				}

				firstDocumentPage += pageCount;
			}

			return PageType.Unknown;
		}

		public AbstractDocumentPrinter GetDocumentPrinter(int page)
		{
			int firstDocumentPage = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				firstDocumentPage += documentPrinter.PageCount ();

				if (page < firstDocumentPage)
				{
					return documentPrinter;
				}
			}

			return null;
		}


		/// <summary>
		/// Spécifie l'unité d'impression, afin de déterminer la taille des pages à produire.
		/// Si on ne spécifie aucune unité d'impression, on utilisera la taille de page préférentielle.
		/// </summary>
		public void SetPrinterUnit(PrinterUnit printerUnit=null)
		{
			foreach (var documentPrinter in this.documentPrinters)
			{
				documentPrinter.SetPrinterUnit (printerUnit);
			}
		}

		/// <summary>
		/// Retourne la taille qui permet d'englober la plus grande des pages.
		/// </summary>
		public virtual Size BoundsPageSize
		{
			get
			{
				double width  = 0;
				double height = 0;

				foreach (var documentPrinter in this.documentPrinters)
				{
					width  = System.Math.Max (width,  documentPrinter.RequiredPageSize.Width);
					height = System.Math.Max (height, documentPrinter.RequiredPageSize.Height);
				}

				return new Size (width, height);
			}
		}

		/// <summary>
		/// Construit toutes les sections de tous les documents.
		/// </summary>
		public virtual void BuildSections(List<DocumentOption> forcingOptionsToClear = null, List<DocumentOption> forcingOptionsToSet = null)
		{
			foreach (var documentPrinter in this.documentPrinters)
			{
				documentPrinter.BuildSections (forcingOptionsToClear, forcingOptionsToSet);
			}
		}


		public int DebugParam1
		{
			get
			{
				return this.documentPrinters[0].DebugParam1;
			}
			set
			{
				if (this.documentPrinters[0].DebugParam1 != value)
				{
					this.documentPrinters[0].DebugParam1 = value;
				}
			}
		}

		public int DebugParam2
		{
			get
			{
				return this.documentPrinters[0].DebugParam2;
			}
			set
			{
				if (this.documentPrinters[0].DebugParam2 != value)
				{
					this.documentPrinters[0].DebugParam2 = value;
				}
			}
		}



		public static AbstractEntityPrinter CreateEntityPrinter(CoreData coreData, AbstractEntity entity)
		{
			var type = AbstractEntityPrinter.FindEntityPrinterType (entity);

			if (type == null)
			{
				return null;
			}

			return System.Activator.CreateInstance (type, new object[] { coreData, entity }) as AbstractEntityPrinter;
		}

		internal static System.Type FindEntityPrinterType(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}
			else
			{
				var types = AbstractEntityPrinter.FindEntityPrinterTypes (entity.GetType ());

				foreach (var type in types)
				{
					if (AbstractEntityPrinter.CheckCompatibleEntityPrinterType (entity, type))
					{
						return type;
					}
				}

				return null;
			}
		}

		private static bool CheckCompatibleEntityPrinterType(AbstractEntity entity, System.Type type)
		{
			var method = type.GetMethod ("CheckCompatibleEntity", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

			if (method == null)
			{
				return true;
			}
			else
			{
				return (bool) method.Invoke (null, new object[] { entity });
			}
		}

		private static IEnumerable<System.Type> FindEntityPrinterTypes(System.Type entityType)
		{
			var baseTypeName = "AbstractEntityPrinter`1";

			//	Find all concrete classes which use either the generic AbstractEntityPrinter base classes,
			//	which match the entity type (usually, there should be exactly one such type).

			var types = from type in typeof (AbstractEntityPrinter).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			return types;
		}


		private static readonly Font specimenFont = Font.GetFont ("Arial", "Bold");

		protected readonly List<AbstractDocumentPrinter>	documentPrinters;
		private readonly List<DocumentTypeDefinition>		documentTypes;
	}


	public class AbstractEntityPrinter<T> : AbstractEntityPrinter
		where T : AbstractEntity
	{
		public AbstractEntityPrinter(T entity)
		{
			this.entity = entity;
		}

		protected readonly T entity;
	}
}
