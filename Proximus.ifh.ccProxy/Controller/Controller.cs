using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using Proximus.ifh.DebugCCProxy.View;
using d=System.Diagnostics.Debug;

namespace Proximus.ifh.DebugCCProxy.Modeless
{
    public class Controller : IGTCustomCommandModeless
    {
        private IGTTransactionManager	_transactionManager;
        private ProxyCCViewmodel		_proxyCCViewmodel;
        private ProxyCCView				_proxyCCView;

        // this is called once when the proxy CC starts up
		public IGTTransactionManager TransactionManager 
		{
            set
            {
				d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );
                _transactionManager = value;
            }
        }

		private IGTApplication _GTApp = null;
		private IGTApplication GTApp
		{
			get
			{
				d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

				if (this._GTApp == null)
				{
					this._GTApp = GTClassFactory.Create<IGTApplication>( );
				}

				return (this._GTApp);
			}
		}

		public void Activate( IGTCustomCommandHelper CustomCommandHelper)
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            bool proxyIsTransaction = this.GetType() == typeof(Proximus.ifh.DebugCCProxy.Modeless.TransactionController);
            _proxyCCViewmodel		= new ProxyCCViewmodel( CustomCommandHelper, _transactionManager, this.GTApp, proxyIsTransaction);
            _proxyCCView			= new ProxyCCView(_proxyCCViewmodel);
            _proxyCCView.Title		= proxyIsTransaction ? "Launch a custom command with job" : "Launch a custom command without job";

			new System.Windows.Interop.WindowInteropHelper(_proxyCCView).Owner = this.GTApp.ApplicationWindow.Handle;
            _proxyCCView.Show();
        }

		public bool CanTerminate { get { return true; } }

		public void Pause()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            _proxyCCViewmodel.PauseCC();
        }

		public void Resume()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            _proxyCCViewmodel.ResumeCC();
        }

		public void Terminate()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            _proxyCCViewmodel.TerminateCC();
        }
    }
}
