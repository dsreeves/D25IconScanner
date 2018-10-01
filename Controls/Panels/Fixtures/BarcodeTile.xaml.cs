using System;
using System.Windows;
using System.Windows.Controls;

namespace Controls.Panels.Fixtures
{
	public partial class BarcodeTile : UserControl
	{
		private static DateTime dt = DateTime.Now;
		public BarcodeTile()
		{
			InitializeComponent();
		}

		#region Dependency Properties


		public int FixtureNumber
		{
			get { return (int)GetValue(FixtureNumberProperty); }
			set { SetValue(FixtureNumberProperty, value); }
		}
		public static readonly DependencyProperty FixtureNumberProperty =
			DependencyProperty.Register("FixtureNumber", typeof(int), typeof(BarcodeTile), new PropertyMetadata(0));



		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		public static readonly DependencyProperty DateProperty =
			DependencyProperty.Register("Date", typeof(DateTime), typeof(BarcodeTile), new PropertyMetadata(dt));



		public DateTime Time
		{
			get { return (DateTime)GetValue(TimeProperty); }
			set { SetValue(TimeProperty, (DateTime)value); }
			
		}

		public static readonly DependencyProperty TimeProperty =
			DependencyProperty.Register("Time", typeof(DateTime), typeof(BarcodeTile), new PropertyMetadata(dt));


		public string SerialNumber
		{
			get { return (string)GetValue(SerialNumberProperty); }
			set { SetValue(SerialNumberProperty, value); }
		}

		public static readonly DependencyProperty SerialNumberProperty =
			DependencyProperty.Register("SerialNumber", typeof(string), typeof(BarcodeTile), new PropertyMetadata(""));



		public string OperatorName
		{
			get { return (string)GetValue(OperatorNameProperty); }
			set { SetValue(OperatorNameProperty, value); }
		}

		public static readonly DependencyProperty OperatorNameProperty =
			DependencyProperty.Register("OperatorName", typeof(string), typeof(BarcodeTile), new PropertyMetadata(""));


		#endregion

	}
}
