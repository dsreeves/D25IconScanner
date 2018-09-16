using System.Windows;
using Caliburn.Micro;
using BarcodeScanner.ViewModels;


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
			DisplayRootViewFor<ShellViewModel>();
		}
	}
}
