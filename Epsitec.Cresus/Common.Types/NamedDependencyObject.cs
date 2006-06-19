//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public abstract class NamedDependencyObject : DependencyObject, IName, INameCaption
	{
		public NamedDependencyObject()
		{
		}

		public NamedDependencyObject(string name)
		{
			this.DefineName (name);
		}

		public NamedDependencyObject(string name, long captionId)
		{
			this.DefineName (name);
			this.DefineCaptionId (captionId);
		}
		
		public string Name
		{
			get
			{
				return (string) this.GetValue (NamedDependencyObject.NameProperty);
			}
		}

		public long CaptionId
		{
			get
			{
				return (long) this.GetValue (NamedDependencyObject.CaptionIdProperty);
			}
		}


		#region IName Members

		string IName.Name
		{
			get
			{
				return this.Name;
			}
		}

		#endregion

		#region INameCaption Members

		long INameCaption.CaptionId
		{
			get
			{
				return this.CaptionId;
			}
		}

		#endregion
		
		
		protected void DefineName(string name)
		{
			this.SetLocalValue (NamedDependencyObject.NameProperty, name);
		}

		protected void DefineCaptionId(long captionId)
		{
			this.SetLocalValue (NamedDependencyObject.CaptionIdProperty, captionId);
		}

		
		public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterReadOnly ("Name", typeof (string), typeof (NamedDependencyObject), new DependencyPropertyMetadata ());
		public static readonly DependencyProperty CaptionIdProperty = DependencyProperty.RegisterReadOnly ("CaptionId", typeof (long), typeof (NamedDependencyObject), new DependencyPropertyMetadata (-1L));
	}
}
