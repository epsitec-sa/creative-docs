//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.Editors
{
	public abstract class AbstractEditor
	{
		public AbstractEditor(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
		{
			this.accessor    = accessor;
			this.baseType    = baseType;
			this.subBaseType = subBaseType;
			this.isTimeless  = isTimeless;
		}


		public bool								EditionDirty
		{
			get
			{
				return this.editionDirty;
			}
		}

		public virtual bool						HasError
		{
			get
			{
				return false;
			}
		}

		public abstract PageType PageType
		{
			get;
			set;
		}

		public abstract PageType				MainPageType
		{
			get;
		}

		public virtual void CreateUI(Widget parent)
		{
		}


		protected void StartEdition(Guid objectGuid, Timestamp? timestamp)
		{
			if (!this.HasError && this.accessor.EditionAccessor.SaveObjectEdition ())
			{
				this.OnDataChanged ();
			}

			this.accessor.EditionAccessor.StartObjectEdition (this.baseType, objectGuid, timestamp);

			if (this.editionDirty)
			{
				this.editionDirty = false;
				this.OnValueChanged (ObjectField.Unknown);
			}
		}

		protected void ValueEdited(ObjectField field)
		{
			this.editionDirty = true;
			this.OnValueChanged (field);
		}


		#region Events handler
		private void OnValueChanged(ObjectField field)
		{
			this.ValueChanged.Raise (this, field);
		}

		public event EventHandler<ObjectField> ValueChanged;


		private void OnDataChanged()
		{
			this.DataChanged.Raise (this);
		}

		public event EventHandler DataChanged;
		#endregion


		protected readonly DataAccessor			accessor;
		protected readonly BaseType				baseType;
		protected readonly BaseType				subBaseType;
		protected readonly bool					isTimeless;

		private bool							editionDirty;
	}
}
