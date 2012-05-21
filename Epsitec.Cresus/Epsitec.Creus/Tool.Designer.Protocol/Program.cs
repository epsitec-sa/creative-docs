using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.InteropServices;

namespace Epsitec.Designer.Protocol
{
	/// <summary>
	/// This program handles the designer:xyz protocol, which is used to link
	/// between Visual Studio and Crésus Designer.
	/// </summary>
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main()
		{
			System.Diagnostics.Process designerProcess = null;

			foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses ())
			{
				if ((process.ProcessName.Contains ("Designer")) &&
					(process.MainWindowTitle.StartsWith ("Crésus Designer")))
				{
					designerProcess = process;
					break;
				}
			}

			if (designerProcess == null)
			{
				System.Diagnostics.Debug.WriteLine ("No Crésus Designer found");
				return;
			}

			System.Windows.Forms.Application.EnableVisualStyles ();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault (false);
			
			string[] args = System.Environment.GetCommandLineArgs ();
			System.Diagnostics.Debug.WriteLine (string.Join (" | ", args));

			EmptyForm form = new EmptyForm ();
			
			form.Show ();
			
			System.Threading.Thread thread = new System.Threading.Thread (
				delegate ()
				{
					bool success = false;

					Action show =
						delegate ()
						{
							success = Program.SetForegroundWindow (form.Handle);
						};

					form.Invoke (show);

					if (success == false)
					{
						System.Text.StringBuilder commandLine = new System.Text.StringBuilder ();
						System.Diagnostics.Debug.WriteLine ("Failed to bring window to the foreground");

						for (int i = 1; i < args.Length; i++)
						{
							if (i > 0)
							{
								commandLine.Append (" ");
							}

							commandLine.Append (@"""");
							commandLine.Append (args[i]);
							commandLine.Append (@"""");
						}

						commandLine.Append (" x");

						if (args.Length < 10)
						{
							System.Diagnostics.Process.Start (args[0], commandLine.ToString ());
						}
						else
						{
							success = true;
						}
					}

					if (success)
					{
						if (args.Length > 2)
						{
							switch (args[1])
							{
								case "-designer":
									Program.ExceptionWrapper (
										delegate
										{
											Program.HandleDesignerProtocol (args[2]);
											Program.SetForegroundWindow (designerProcess.MainWindowHandle);
										},
										"designer protocol");
									break;

								default:
									break;
							}
						}
					}
					
					Action quit =
						delegate ()
						{
							Program.PostQuitMessage (0);
						};


					form.Invoke (quit);
				});

			thread.Name = "Execution Engine";
			thread.Start ();
			System.Windows.Forms.Application.Run (form);
			thread.Join ();
		}

		[DllImport ("user32.dll")]
		[return: MarshalAs (UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(System.IntPtr hWnd);

		[DllImport ("user32.dll", CharSet=CharSet.Auto, SetLastError=false)]
		static extern void PostQuitMessage(int exitCode);



		private static void ExceptionWrapper(Action action, string actionName)
		{
			try
			{
				action ();
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Failed to execute " + actionName);
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
		}

		private static void HandleDesignerProtocol(string argument)
		{
			string[] args = argument.Split ('/');

			if (args.Length < 2)
			{
				throw new System.ApplicationException ("Invalid resource specification");
			}


			NetNamedPipeBinding binding = new NetNamedPipeBinding (NetNamedPipeSecurityMode.None);
			EndpointAddress address = new EndpointAddress (Addresses.DesignerAddress);
			
			using (ChannelFactory<INavigator> factory = new ChannelFactory<INavigator> (binding, address))
			{
				INavigator proxy = factory.CreateChannel ();

				using (IClientChannel channel = proxy as IClientChannel)
				{
					switch (args[0])
					{
						case "designer:str":
							proxy.NavigateToString (string.Concat ("[", args[1], "]"));
							break;

						case "designer:cap":
							proxy.NavigateToCaption (string.Concat ("[", args[1], "]"));
							break;

						case "designer:fld":
							proxy.NavigateToEntityField (string.Concat ("[", args[1], "]", ":", "[", args[2], "]"));
							break;

						default:
							throw new System.ApplicationException ("Invalid resource specification");
					}
				}
			}
		}

		private delegate void Action();
	}
}