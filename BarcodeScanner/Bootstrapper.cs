using System.Windows;
using Caliburn.Micro;
using BarcodeScanner.ViewModels;
using System.Diagnostics;
using System.Threading;

namespace BarcodeScanner
{
	public class Bootstrapper : BootstrapperBase
	{
		public Bootstrapper()
		{
			Initialize();
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			Debug.WriteLine($"Displaying Root view for ShellVM [{Thread.CurrentThread.ManagedThreadId}]");
			DisplayRootViewFor<ShellViewModel>();
			Debug.WriteLine($"Finished Displaying Root view for ShellVM [{Thread.CurrentThread.ManagedThreadId}]");
		}
	}
}
