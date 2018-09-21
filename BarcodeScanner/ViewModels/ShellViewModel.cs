using Caliburn.Micro;
using System;
using BarcodeScanner.Models;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

namespace BarcodeScanner.ViewModels
{
	public class ShellViewModel : Screen, INotifyPropertyChanged
	{
		#region Private Properties/Fields
		private DispatcherTimer _clockTimer;
		private DispatcherTimer _saveDataTimer;
		private bool isFirstTick = true;
		private IniParse IniParser = new IniParse("Models/Scanner.ini");
		#endregion

		#region Public Properties/Fields
		public string FormName        { get; set; } = "DefaultName";
		public string CurrentTime     { get; set; } = DateTime.Now.ToString("HH:mm");
		public string CurrentDate     { get; set; } = DateTime.Now.ToShortDateString();

		/// <summary>
		/// Model for all scanner data
		/// </summary>
		public Scanner Scanner        { get; set; } = new Scanner();

		#endregion

		/// <summary>
		/// UI Constructor
		/// </summary>
		public ShellViewModel()
		{
			//Clock Timer
			_clockTimer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(3) };
			_clockTimer.Tick += (sender, args) => ClockTimerTick(DateTime.Now);

			//Save Data Timer
			_saveDataTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(30) };
			_saveDataTimer.Tick += (sender, args) => DataTimerTick(DateTime.Now);

			Debug.WriteLine($"ShellVM CTOR on thread [{Thread.CurrentThread.ManagedThreadId}]");

			LoadIniVar();
		}

		/// <summary>
		/// Load initilization variables
		/// </summary>
		private void LoadIniVar()
		{
			
		}


		#region Timer Events

		/// <summary>
		/// Method fired from Clock timer to update UI DateTime
		/// </summary>
		/// <param name="t2"></param>
		private void ClockTimerTick(DateTime t2)
		{
			TimeSpan deltaSeconds;

			//Timer Interval Settings
			if (isFirstTick)
			{
				isFirstTick = false;
				deltaSeconds = t2.Date.AddMinutes(t2.Minute + 1) - t2;
			}
			else
			{
				deltaSeconds = TimeSpan.FromMinutes(1);
			}
			_clockTimer.Interval = deltaSeconds;

			//Update DateTime
			CurrentTime = t2.ToString("HH:mm");
			CurrentDate = t2.ToShortDateString();

		}

		/// <summary>
		/// Needs Completetion
		/// </summary>
		/// <param name="t2"></param>
		private void DataTimerTick(DateTime t2)
		{
			//Used to save data in case of power failure
		}
		#endregion






	}


}
