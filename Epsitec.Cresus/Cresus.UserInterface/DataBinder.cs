//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.UserInterface
{
	/// <summary>
	/// La classe DataBinder permet de faire le lien entre une interface graphique
	/// et ses données.
	/// </summary>
	public class DataBinder : System.IDisposable
	{
		public DataBinder() : this (new ObjectBundler (true))
		{
		}
		
		public DataBinder(ObjectBundler object_bundler)
		{
			System.Diagnostics.Debug.Assert (object_bundler != null);
			
			this.object_list = new System.Collections.ArrayList ();
			this.object_bundler = object_bundler;
			this.object_bundler.ObjectUnbundled += new BundlingEventHandler (HandleObjectUnbundled);
			this.object_bundler.EnableMapping ();
		}
		
		
		public ObjectBundler				ObjectBundler
		{
			get { return this.object_bundler; }
		}
		
		public DataLayer.DataSet			DataSet
		{
			get{ return this.root_data_set; }
			set
			{
				if (this.root_data_set != value)
				{
					this.Detach ();
					this.root_data_set = value;
					this.Attach ();
				}
			}
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				System.Diagnostics.Debug.Assert (this.object_bundler != null);
				
				this.object_bundler.ObjectUnbundled -= new BundlingEventHandler (this.HandleObjectUnbundled);
				this.object_bundler = null;
				
				this.Detach ();
			}
		}
		
		protected virtual void Attach()
		{
			if (this.root_data_set != null)
			{
				//	S'il y a des objets qui n'ont pas été attachés (l'utilisateur
				//	a appelé ObjectBundler.CreateFromBundle avant d'attacher le data
				//	set), il faut les attacher maintenant.
				
				foreach (object obj in this.object_list)
				{
					ResourceBundle bundle = this.object_bundler.FindBundleFromObject (obj);
					
					this.CreateBinding (obj, bundle.GetFieldString ("binding"));
				}
				
				this.object_list.Clear ();
			}
		}
		
		protected virtual void Detach()
		{
			if (this.root_data_set != null)
			{
				this.root_data_set = null;
			}		
		}
		
		
		protected virtual bool CheckBindingName(string name)
		{
			if (name == null)
			{
				return false;
			}
			if (name.Length < 1)
			{
				return false;
			}
			if (name.Length > 100)
			{
				return false;
			}
			
			return true;
		}
		
		protected virtual void CreateBinding(object obj, string full_binding)
		{
			System.Diagnostics.Debug.Assert (this.CheckBindingName (full_binding));
			System.Diagnostics.Debug.Assert (this.root_data_set != null);
			
			//	Etablit un lien entre l'objet qui appartient à l'interface graphique et
			//	les données qui se trouvent dans le data set. La description du lien est
			//	contenue dans 'full_binding'.
			
			string binding  = full_binding;
			string bind_tag = "data";
			int    pos_sep  = binding.IndexOf ('#');
			
			if (pos_sep >= 0)
			{
				binding  = full_binding.Substring (0, pos_sep);
				bind_tag = full_binding.Substring (pos_sep+1);
			}
			
			DataLayer.DataRecord data_record = this.root_data_set.FindRecord (binding, DataLayer.DataVersion.Original);
			
			if (data_record == null)
			{
				throw new BinderException (string.Format ("Cannot bind object {0} to data '{1}'", obj.GetType ().Name, binding));
			}
			
			//	Le binding peut se faire soit sur les données (nom#data), soit sur
			//	d'autres champs, comme sur l'étiquette (nom#label) ou encore sur la
			//	description (nom#desc).
			
			switch (bind_tag)
			{
				case "data":
					this.CreateDataBinding (obj, data_record, binding);
					break;
				
				case "label":
					this.CreateLabelBinding (obj, data_record, binding);
					break;
				
				case "desc":
					this.CreateDescriptionBinding (obj, data_record, binding);
					break;
			}
		}
		protected virtual void CreateDataBinding(object obj, DataLayer.DataRecord data_record, string binding_path)
		{
			DataLayer.DataType data_type = data_record.DataType;
			IBinder binder = BinderFactory.FindBinder (data_type);
			
			if (binder == null)
			{
				throw new BinderException (string.Format ("No binder for data '{1}' on object {0}", obj.GetType ().Name, binding_path));
			}
			
			binder.CreateBinding (obj, this.DataSet, binding_path, data_record);
		}
		
		protected virtual void CreateLabelBinding(object obj, DataLayer.DataRecord data_record, string binding_path)
		{
			string text = data_record.DataLabel;
			
			if (text != null)
			{
				Widget widget = obj as Widget;
				
				if (widget != null)
				{
					widget.Text = text;
				}
			}
		}
		
		protected virtual void CreateDescriptionBinding(object obj, DataLayer.DataRecord data_record, string binding_path)
		{
			string text = data_record.DataDescription;
			
			if (text != null)
			{
				Widget widget = obj as Widget;
				
				if (widget != null)
				{
					widget.Text = text;
				}
			}
		}
		
		
		protected virtual void HandleObjectUnbundled(object sender, object obj, ResourceBundle bundle)
		{
			//	Regarde si l'objet qui vient d'être re-généré contient une information de
			//	binding pour les données. Si c'est le cas, on en prend note pour un traitement
			//	ultérieur (si aucun data set n'est attaché), ou on le traite tout de suite.
			
			if (bundle.Contains ("binding"))
			{
				string binding = bundle.GetFieldString ("binding");
				
				if (this.CheckBindingName (binding))
				{
					if (this.root_data_set != null)
					{
						this.CreateBinding (obj, binding);
					}
					else
					{
						this.object_list.Add (obj);
					}
				}
				else
				{
					throw new System.FormatException (string.Format ("Bad binding information for class '{0}' in bundle '{1}'", obj.GetType ().ToString (), bundle.Name));
				}
			}
		}
		
		
		protected ObjectBundler					object_bundler;
		protected System.Collections.ArrayList	object_list;
		protected DataLayer.DataSet				root_data_set;
	}
}
