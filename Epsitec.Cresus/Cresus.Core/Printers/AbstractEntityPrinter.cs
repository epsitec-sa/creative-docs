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

					if (this.debugPort != null)
					{
						this.Print (this.debugPort, this.debugBounds);
					}
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

					if (this.debugPort != null)
					{
						this.Print (this.debugPort, this.debugBounds);
					}
				}
			}
		}

		public virtual void Print(IPaintPort port, Rectangle bounds)
		{
			this.debugPort = port;
			this.debugBounds = bounds;
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


		private int debugParam1;
		private int debugParam2;
		private IPaintPort debugPort;
		private Rectangle debugBounds;
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
