using System;
using System.ComponentModel;
using System.Windows.Media;

namespace BarcodeScanner.Models
{
	public class KPIModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		#region KPI Properties

		// Current KPI Properties
		public int cVacKPI                     { get; set; } = 0;
		public int cSpacerKPI                  { get; set; } = 0;
		public int cCupKPI                     { get; set; } = 0;
		public int cUpTimeKPI                  { get; set; } = 0;
		public int cOEEKPI                     { get; set; } = 0;

		//Used to calc Percentage Values
		public double _cUpTimeKPI              { get; set; } = 0;
		public double _cOEEKPI                 { get; set; } = 0;

		// Previous KPI Properties
		public int pVacKPI                     { get; set; } = 0;
		public int pSpacerKPI                  { get; set; } = 0;
		public int pCupKPI                     { get; set; } = 0;
		public int pUpTimeKPI                  { get; set; } = 0;
		public int pOEEKPI                     { get; set; } = 0;

		public bool Fix1                       { get; set; } = false;
		public bool Fix2                       { get; set; } = false;
		public bool Fix3                       { get; set; } = false;
		public bool Fix4                       { get; set; } = false;
		public bool Fix5                       { get; set; } = false;
		public bool Fix6                       { get; set; } = false;

		#endregion

		#region Backcolor's
		//Fixture Backcolor Properties (red by default)
		//public SolidColorBrush F1Color         { get; set; } = Brushes.Red;
		//public SolidColorBrush F2Color         { get; set; } = Brushes.Red;
		//public SolidColorBrush F3Color         { get; set; } = Brushes.Red;
		//public SolidColorBrush F4Color         { get; set; } = Brushes.Red;
		//public SolidColorBrush F5Color         { get; set; } = Brushes.Red;
		//public SolidColorBrush F6Color         { get; set; } = Brushes.Red;

		////Current KPI BackColors 
		//public SolidColorBrush cVacKPIColor    { get; set; } = Brushes.Red;
		//public SolidColorBrush cSpacerKPIColor { get; set; } = Brushes.Red;
		//public SolidColorBrush cCupKPIColor    { get; set; } = Brushes.Red;
		//public SolidColorBrush cUpTimePIColor  { get; set; } = Brushes.Red;
		//public SolidColorBrush cOEEKPIColor    { get; set; } = Brushes.Red;

		////Previous KPI BackColors 
		//public SolidColorBrush pVacKPIColor    { get; set; } = Brushes.Red;
		//public SolidColorBrush pSpacerKPIColor { get; set; } = Brushes.Red;
		//public SolidColorBrush pCupKPIColor    { get; set; } = Brushes.Red;
		//public SolidColorBrush pUpTimePIColor  { get; set; } = Brushes.Red;
		//public SolidColorBrush pOEEKPIColor    { get; set; } = Brushes.Red;

		////Backcolors
		//public SolidColorBrush BackgroudColor_Red         = Brushes.Red;
		//public SolidColorBrush BackgroudColor_Green       = Brushes.Green;
		//public SolidColorBrush BackgroudColor_ForestGreen = Brushes.ForestGreen;
		#endregion

		public KPIModel()
		{
			Console.WriteLine("KPI model created");
		}


	}
}
