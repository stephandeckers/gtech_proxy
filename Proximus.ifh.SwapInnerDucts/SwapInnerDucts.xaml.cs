/**
 * @Name pxs.PostAndIntegrate.xaml.cs
 * @Purpose 
 * @Date 31 March 2023, 07:23:36
 * @Author S.Deckers
 * @Description 
 */

namespace Proximus.ifh.SwapInnerDucts
{
	#region -- Using directives --
	using System;
	using ADODB;
	using System.Windows;
	using Intergraph.GTechnology.API;
	using Intergraph.GTechnology.Interfaces;
	using d = System.Diagnostics.Debug;
	#endregion

    public static class Global
    {
        public static int CallCount = 0;
    }

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SwapInnerDuctsController : IGTCustomCommandModeless
	{
		private SwapInnerDuctsWindow	_SwapInnerDuctsWindow;
		private SwapInnerDuctsModel		_SwapInnerDuctsModel;

		public IGTCustomCommandHelper CustomCommandHelper	{	get; set; }

		public void Activate( IGTCustomCommandHelper CustomCommandHelper)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty));

			this.CustomCommandHelper = CustomCommandHelper;

			this._SwapInnerDuctsModel	= new SwapInnerDuctsModel	( ) { Name = "Pietje Puk", Age = 34 };
			this._SwapInnerDuctsWindow	= new SwapInnerDuctsWindow	( );
			this._SwapInnerDuctsWindow.DataContext = this._SwapInnerDuctsModel;

			new System.Windows.Interop.WindowInteropHelper( _SwapInnerDuctsWindow).Owner = this.GTApp.ApplicationWindow.Handle;
			this._SwapInnerDuctsWindow.Show( );

			this._SwapInnerDuctsWindow.Closed			+= _SwapInnerDuctsWindow_Closed;
			this._SwapInnerDuctsWindow.button1.Click	+= Button1_Click;
        }

		private void Button1_Click( object sender, RoutedEventArgs e )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty));
			//MessageBox.Show( "Y3");
			//MessageBox.Show( "Y4", caption:"It worked", button:MessageBoxButton.YesNoCancel, icon:MessageBoxImage.Information);
			MessageBox.Show( "Y4", caption:"Changed it again", button:MessageBoxButton.YesNoCancel, icon:MessageBoxImage.Information);
		}

		private void _SwapInnerDuctsWindow_Closed( object sender, EventArgs e )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty));

			if( this.CustomCommandHelper == null)
			{
				return;
			}

			try
			{
				d.WriteLine( this.CustomCommandHelper.GetType().ToString());
				this.CustomCommandHelper.Complete();
			}
			catch
			{
			}
		}

		private IGTApplication _GTApp = null;
		private IGTApplication GTApp
		{
			get
			{
				d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

				if( this._GTApp == null)
				{
					this._GTApp = GTClassFactory.Create<IGTApplication>();
				}

				return( this._GTApp);
			}
		}

		private IGTTransactionManager _transactionManager;
		public IGTTransactionManager TransactionManager
		{
			set
			{
				d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
				_transactionManager = value;
			}
		}

		public void Pause( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		public void Resume( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		public void Terminate( )
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty));
        }

        public virtual bool CanTerminate
        {
            get;
            set;
        }
	}

	public partial class SwapInnerDuctsWindow : Window
	{
		public SwapInnerDuctsWindow( )
		{
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
			InitializeComponent( );
		}

		/// <summary>
		/// Not used but needs to be present to define the eventhandler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button1_Click( object sender, RoutedEventArgs e )
		{
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );	
		}

		/// <summary>
		/// Not used but needs to be present to define the eventhandler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button2_Click( object sender, RoutedEventArgs e )
		{
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );	
		}

		private void Window_Closed( object sender, EventArgs e )
		{
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );	
		}
	}

	public partial class SwapInnerDuctsModel
	{
		public string	Name	{ get; set; }
		public int		Age		{ get; set; }
	}
}