using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controls.Panels.Fixtures
{
	public partial class BarcodeTile : UserControl
	{
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
			DependencyProperty.Register("FixtureNumber", typeof(int), typeof(BarcodeTile), null);


		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}
		public static readonly DependencyProperty DateProperty =
			DependencyProperty.Register("Date", typeof(DateTime), typeof(BarcodeTile), null);


		public DateTime Time
		{
			get { return (DateTime)GetValue(TimeProperty); }
			set { SetValue(TimeProperty, (DateTime)value); }
			
		}

		public static readonly DependencyProperty TimeProperty =
			DependencyProperty.Register("Time", typeof(DateTime), typeof(BarcodeTile), null);


		public string SerialNumber
		{
			get { return (string)GetValue(SerialNumberProperty); }
			set { SetValue(SerialNumberProperty, value); }
		}

		public static readonly DependencyProperty SerialNumberProperty =
			DependencyProperty.Register("SerialNumber", typeof(string), typeof(BarcodeTile), null);



		public string OperatorName
		{
			get { return (string)GetValue(OperatorNameProperty); }
			set { SetValue(OperatorNameProperty, value); }
		}

		public static readonly DependencyProperty OperatorNameProperty =
			DependencyProperty.Register("OperatorName", typeof(string), typeof(BarcodeTile), null);




		#endregion

	}
}
