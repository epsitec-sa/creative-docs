//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/11/2003

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
		
		public DataLayer.DataStore			DataStore
		{
			get{ return this.data_store; }
			set
			{
				if (this.data_store != value)
				{
					this.Detach ();
					this.data_store = value;
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
			if (this.data_store != null)
			{
				//	S'il y a des objets qui n'ont pas été attachés (l'utilisateur
				//	a appelé ObjectBundler.CreateFromBundle avant d'attacher le data
				//	set), il faut les attacher maintenant.
				
				foreach (object obj in this.object_list)
				{
					ResourceBundle bundle = this.object_bundler.FindBundleFromObject (obj);
					
					this.CreateBinding (obj, bundle, bundle[Tags.Binding].AsString);
				}
				
				this.object_list.Clear ();
			}
		}
		
		protected virtual void Detach()
		{
			if (this.data_store != null)
			{
				this.data_store = null;
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
		
		protected virtual void CreateBinding(object obj, ResourceBundle bundle, string full_binding)
		{
			//	Des arguments pour le contrôleur peuvent être spécifiés à la fin
			//	de l'information de binding, par exemple :
			//
			//	nom#data<args><stuff foo="34" bar="x"/><args/>
			//			^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
			//		    arguments pour le contrôleur
			
			string args     = null;
			int    pos_args = full_binding.IndexOf ('<');
			
			if (pos_args > 0)
			{
				args         = full_binding.Substring (pos_args);
				full_binding = full_binding.Substring (0, pos_args);
			}
			
			if (this.CheckBindingName (full_binding) == false)
			{
				throw new System.FormatException (string.Format ("Bad binding information for class '{0}' in bundle '{1}': '{2}'.", obj.GetType ().ToString (), bundle.Name, full_binding));
			}
			
			System.Diagnostics.Debug.Assert (this.data_store != null);
			
			//	Etablit un lien entre l'objet qui appartient à l'interface graphique et
			//	les données qui se trouvent dans le data set. La description du lien est
			//	contenue dans 'full_binding'.
			
			string binding  = full_binding;
			string bind_tag = Tags.Data;
			int    pos_sep  = binding.IndexOf ('#');
			
			if (pos_sep >= 0)
			{
				binding  = full_binding.Substring (0, pos_sep);
				bind_tag = full_binding.Substring (pos_sep+1);
			}
			
			Database.DbColumn db_column = this.data_store.FindDbColumn (binding);
			
			
			//	Le binding peut se faire soit sur les données (nom#data), soit sur
			//	d'autres champs, comme sur l'étiquette (nom#label) ou encore sur la
			//	description (nom#desc).
			
			switch (bind_tag)
			{
				case Tags.Data:			this.CreateDataBinding (obj, db_column, binding, args);		break;
				case Tags.Caption:		this.CreateCaptionBinding (obj, db_column, binding);		break;
				case Tags.Description:	this.CreateDescriptionBinding (obj, db_column, binding);	break;
				
				default:
					throw new BinderException (string.Format ("Unknown bind tag '{0}' used with data '{1}'.", bind_tag, binding));
			}
		}
		
		protected virtual void CreateDataBinding(object obj, Database.DbColumn db_column, string path, string args)
		{
			IBinder binder = BinderFactory.FindBinderForType (db_column.Type);
			
			if (binder == null)
			{
				throw new BinderException (string.Format ("No binder for data '{0}' on object {1}.", path, obj.GetType ().Name));
			}
			
			binder.CreateBinding (obj, this.DataStore, path, args, db_column);
		}
		
		protected virtual void CreateCaptionBinding(object obj, Database.DbColumn db_column, string binding_path)
		{
			string text = db_column.Caption;
			
			if (text != null)
			{
				Widget widget = obj as Widget;
				
				if (widget != null)
				{
					widget.Text = text;
				}
			}
		}
		
		protected virtual void CreateDescriptionBinding(object obj, Database.DbColumn db_column, string binding_path)
		{
			string text = db_column.Description;
			
			if (text != null)
			{
				Widget widget = obj as Widget;
				
				if (widget != null)
				{
					widget.Text = text;
				}
			}
		}
		
		
		protected virtual void HandleObjectUnbundled(object sender, BundlingEventArgs e)
		{
			//	Regarde si l'objet qui vient d'être re-généré contient une information de
			//	binding pour les données. Si c'est le cas, on en prend note pour un traitement
			//	ultérieur (si aucun data set n'est attaché), ou on le traite tout de suite.
			
			if (e.Bundle.Contains (Tags.Binding))
			{
				string binding = e.Bundle[Tags.Binding].AsString;
				
				if (binding != null)
				{
					string[] bind_args = System.Utilities.Split (binding, ';');
					
					for (int i = 0; i < bind_args.Length; i++)
					{
						string bind_arg = bind_args[i].Trim ();
						
						if (this.data_store != null)
						{
							this.CreateBinding (e.Object, e.Bundle, bind_arg);
						}
						else
						{
							this.object_list.Add (e.Object);
						}
					}
				}
				else
				{
					throw new System.FormatException (string.Format ("Bad binding information for class '{0}' in bundle '{1}': null.", e.Object.GetType ().ToString (), e.Bundle.Name));
				}
			}
		}
		
		
		
		protected ObjectBundler					object_bundler;
		protected System.Collections.ArrayList	object_list;
		protected DataLayer.DataStore			data_store;
	}
}
