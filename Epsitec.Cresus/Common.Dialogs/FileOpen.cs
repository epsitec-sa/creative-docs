//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe FileOpen présente le dialogue pour ouvrir un fichier.
	/// </summary>
	public class FileOpen : Helpers.IFilterCollectionHost
	{
		public FileOpen()
		{
			this.dialog = new System.Windows.Forms.OpenFileDialog ();
			this.filters = new Helpers.FilterCollection (this);
			
			this.dialog.AddExtension = true;
			this.dialog.CheckFileExists = true;
			this.dialog.CheckPathExists = true;
			this.dialog.DereferenceLinks = true;
			this.dialog.RestoreDirectory = true;
			this.dialog.ShowHelp = false;
			this.dialog.ShowReadOnly = false;
			this.dialog.ValidateNames = true;
		}
		
		#region IDialog Members
		public void OpenDialog()
		{
			this.dialog.Filter = this.Filters.FileDialogFilter;
			this.dialog.FilterIndex = this.filter_index + 1;
			
			System.Windows.Forms.DialogResult result = this.dialog.ShowDialog (this.owner == null ? null : this.owner.PlatformWindowObject as System.Windows.Forms.IWin32Window);
			
			switch (result)
			{
				case System.Windows.Forms.DialogResult.OK:
				case System.Windows.Forms.DialogResult.Yes:
					this.result       = DialogResult.Accept;
					this.filter_index = this.dialog.FilterIndex - 1;
					break;
				
				default:
					this.result = DialogResult.Cancel;
					break;
			}
		}
		
		public Common.Widgets.Window	Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}
		#endregion
		
		public string							DefaultExt
		{
			get { return this.dialog.DefaultExt; }
			set { this.dialog.DefaultExt = value; }
		}
		
		public string							FileName
		{
			get { return this.dialog.FileName; }
			set { this.dialog.FileName = value; }
		}
		
		public string[]							FileNames
		{
			get { return this.dialog.FileNames; }
		}
		
		public Helpers.FilterCollection			Filters
		{
			get { return this.filters; }
		}
		
		public int								FilterIndex
		{
			get { return this.filter_index; }
			set { this.filter_index = value; }
		}
		
		public string							InitialDirectory
		{
			get { return this.dialog.InitialDirectory; }
			set { this.dialog.InitialDirectory = value; }
		}
		
		public bool								AcceptMultipleSelection
		{
			get { return this.dialog.Multiselect; }
			set { this.dialog.Multiselect = value; }
		}
		
		public string							Title
		{
			get { return this.dialog.Title; }
			set { this.dialog.Title = value; }
		}
		
		public DialogResult						Result
		{
			get
			{
				return this.result;
			}
		}
		
		
		#region IFilterCollectionHost Members
		public void FilterCollectionChanged()
		{
			//	Rien de spécial à faire...
		}
		#endregion
		
		
		Common.Widgets.Window					owner;
		System.Windows.Forms.OpenFileDialog		dialog;
		Helpers.FilterCollection				filters;
		int										filter_index;
		private DialogResult					result = DialogResult.None;
	}
}
