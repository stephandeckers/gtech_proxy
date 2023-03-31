using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using d=System.Diagnostics.Debug;

namespace Proximus.ifh.DebugCCProxy
{
	public class Global
	{
		public static int CallCount = 0;
	}

    public class ProxyCCViewmodel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private IGTCustomCommandHelper			_customCommandHelper;
        private IGTCustomCommandModeless		CCModeless;
        private IGTCustomCommandModal			CCModal;
        private IGTTransactionManager			_transactionManager;
        private bool							dllJobMustBeActive = false;
        private bool							_proxyIsTransactionVersion;

        private static string cachedCCFullPath;

		internal IGTApplication GTApp
		{
			get; private set;
		}

        public ProxyCCViewmodel
		(
            IGTCustomCommandHelper	customCommandHelper
		,	IGTTransactionManager	transactionManager
		,	IGTApplication			gtApp
		,	bool					proxyIsTransactionVersion
		)
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            _customCommandHelper		= customCommandHelper;
            _transactionManager			= transactionManager;
            _ccFullPath					= cachedCCFullPath;
			this.GTApp					= gtApp;
            _proxyIsTransactionVersion	= proxyIsTransactionVersion;
        }

        private string _ccFullPath;
        public string CCFullPath
        {
            get { return _ccFullPath; }
            set
            {
                _ccFullPath = value;
#pragma warning disable S2696 // Instance members should not write to "static" fields
                cachedCCFullPath = CCFullPath;
#pragma warning restore S2696 // Instance members should not write to "static" fields
                Tuple<string, bool> assemblyInfo = GetAssemblyInfo(_ccFullPath);
                CCInfo = assemblyInfo.Item1;
                dllJobMustBeActive = assemblyInfo.Item2;

                OnPropertyChanged();
            }
        }

        private string _ccInfo;
        public string CCInfo
        {
            get { return _ccInfo; }
            set
            {
                _ccInfo = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CloseForm()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            if (_customCommandHelper != null)
            try
            {
                _customCommandHelper?.Complete();
            }
            catch
            {
                // do nothing, just accept
            }
            if (CCModeless != null)
            {
                CCModeless.TransactionManager = null;
            }
            if (CCModal != null)
            {
                CCModal.TransactionManager = null;
            }

            _customCommandHelper = null;
            _transactionManager = null;
        }

        public void PauseCC()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            CCModeless?.Pause();
        }

        public void TerminateCC()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            CCModeless?.Terminate();
        }

        public void ResumeCC()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            CCModeless?.Resume();
        }

        public Tuple<string,bool> GetAssemblyInfo(string fileName)
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            fileName = fileName.Trim('"');
            if (!File.Exists(fileName))
            {
                return new Tuple<string,bool> ("File Does not exist", false);
            }
            FileInfo fileInfo = new FileInfo(fileName);
            List<string> assemblyInfo = new List<string>();
            assemblyInfo.Add(fileInfo.CreationTime.ToString());

            string CCFilenameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name).ToUpper();
            string sql =
            @"SELECT G3E_INTERFACE, G3E_USERNAME, G3E_COMMANDCLASS, G3E_ENABLINGMASK, G3E_MODALITY, G3E_SELECTSETENABLINGMASK, G3E_ROLE
            FROM G3E_CUSTOMCOMMAND_OPTABLE WHERE UPPER(G3E_INTERFACE) LIKE :CCNAME || ':%'";

			ADODB.Recordset rs = GTApp.DataContext.Execute( sql, out _, (int)ADODB.CommandTypeEnum.adCmdText, CCFilenameWithoutExtension);

            bool _jobMustBeActive = false;
            if (rs != null && rs.RecordCount > 0)
            {
                rs.MoveFirst();
                assemblyInfo.Add($"G3E_INTERFACE: {Convert.ToString(rs.Fields["G3E_INTERFACE"].Value)}");
                assemblyInfo.Add($"G3E_USERNAME: {Convert.ToString(rs.Fields["G3E_USERNAME"].Value)}");
                assemblyInfo.Add($"G3E_COMMANDCLASS: {Convert.ToString(rs.Fields["G3E_COMMANDCLASS"].Value)}");
                long gEnablingmask = Convert.ToInt64(rs.Fields["G3E_ENABLINGMASK"].Value);
                Tuple<string, bool> enablingInfo = GetEnablingInfo(gEnablingmask);
                assemblyInfo.Add($"G3E_ENABLINGMASK: {enablingInfo.Item1}");
                _jobMustBeActive = enablingInfo.Item2;
                string strModality = Convert.ToInt32(rs.Fields["G3E_MODALITY"].Value) == 1 ? "Modal" : "Modeless";
                assemblyInfo.Add($"G3E_MODALITY: {strModality}");
                assemblyInfo.Add($"G3E_SELECTSETENABLINGMASK: {Convert.ToString(rs.Fields["G3E_SELECTSETENABLINGMASK"].Value)}");
                assemblyInfo.Add($"G3E_ROLE: {Convert.ToString(rs.Fields["G3E_ROLE"].Value)}");
            }
            else
            {
                assemblyInfo.Add("Custom command is not known in G3E_CUSTOMCOMMAND list");
            }

            return new Tuple<string,bool> 
                (string.Join(Environment.NewLine, assemblyInfo.ToArray()),
                _jobMustBeActive);
        }

        public static Tuple<string,bool> GetEnablingInfo(long val)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            StringBuilder sb = new StringBuilder();

            bool _jobMustBeActive = false;

            if (IsBitSet(val, 4))
            {
                sb.Append("Active Job; ");
                _jobMustBeActive = true;
            }
            if (IsBitSet(val, 5)) sb.Append("Active Plot window; ");
            if (IsBitSet(val, 6)) sb.Append("Active map/mapview; ");
            if (IsBitSet(val, 10)) sb.Append("Dev connection; ");
            if (IsBitSet(val, 11)) sb.Append("Active workspace; ");
            if (IsBitSet(val, 13)) sb.Append("G/Technology connection; ");
            if (IsBitSet(val, 16)) sb.Append("Active map view; ");
            if (IsBitSet(val, 18)) sb.Append("Select set = 1; ");
            if (IsBitSet(val, 19)) sb.Append("Select set >= 1; ");
            if (IsBitSet(val, 20)) sb.Append("Feature count = 1; ");
            if (IsBitSet(val, 21)) sb.Append("Select set criteria checked; ");
            if (IsBitSet(val, 23)) sb.Append("New transaction; ");
            if (IsBitSet(val, 24)) sb.Append("Active detail window; ");
            if (IsBitSet(val, 26)) sb.Append("Raster image attached; ");

            return new Tuple<string, bool> (sb.ToString(), _jobMustBeActive);
        }

        public static bool IsBitSet(long value, int pos)
        {
            return (value & (1 << pos)) != 0;
        }

        public void LaunchCC()
        {
			d.WriteLine( string.Format( "{0}.{1}[{2}] ({3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            Tuple<string, bool> assemblyInfo = GetAssemblyInfo(_ccFullPath);
            dllJobMustBeActive = assemblyInfo.Item2;
            try
            {
                if (_proxyIsTransactionVersion && !dllJobMustBeActive)
                {
                    MessageBox.Show("Choose the transactionless version of this Proxy CC");
                    return;
                }

                if (!_proxyIsTransactionVersion && dllJobMustBeActive)
                {
                    MessageBox.Show("Choose the transaction version of this Proxy CC");
                    return;
                }

                string		assemblyFileName		= CCFullPath.Trim('"');
                string		assemblyPath			= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string		assemblyFullFilename	= Path.Combine(assemblyPath, assemblyFileName);
                byte[]		assemblyBytes			= File.ReadAllBytes(assemblyFullFilename);
                Assembly	assemblyToLoad			= Assembly.Load(assemblyBytes);
                List<Assembly> allLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                LoadReferencedAssemblies(assemblyToLoad, assemblyPath, ref allLoadedAssemblies);

				Type entryClass = null;

                entryClass = assemblyToLoad.GetTypes().FirstOrDefault(t => typeof(IGTCustomCommandModal).IsAssignableFrom(t));
				if( entryClass != null)
				{
                    CCModal = (IGTCustomCommandModal)assemblyToLoad.CreateInstance(entryClass.FullName);
                    CCModal.TransactionManager = _transactionManager;
                    CCModal.Activate();
					return;
				}

                entryClass = assemblyToLoad.GetTypes().FirstOrDefault(t => typeof(IGTCustomCommandModeless).IsAssignableFrom(t));

				if( entryClass != null)
				{
					d.WriteLine( "IGTCustomCommandModeless");
                    CCModeless = (IGTCustomCommandModeless)assemblyToLoad.CreateInstance(entryClass.FullName);
                    CCModeless.TransactionManager = _transactionManager;
                    CCModeless.Activate(_customCommandHelper);
					return;
				}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                // do nothing
            }
        }

		private void dumpInterfaces (Type theType)
		{
			foreach ( var item in theType.GetInterfaces())
			{
				d.WriteLine( item.ToString());
			}
		}

        private void LoadReferencedAssemblies(Assembly baseAssembly, string assemblyPath, ref List<Assembly> loadedAssemblies)
        {
            AssemblyName[] refAssemblyToLoad = baseAssembly.GetReferencedAssemblies();
            foreach (AssemblyName aRefa in refAssemblyToLoad)
            {
                string refADispname = aRefa.FullName;
                if (!loadedAssemblies.Any(a => a.GetName().FullName.Equals(refADispname)))
                {
                    string refAssemblyFullFilename = Path.Combine(assemblyPath, aRefa.Name + ".dll");
                    Assembly referenced;
                    if (File.Exists(refAssemblyFullFilename))
                    {
                        byte[] refAssemblyBytes = File.ReadAllBytes(refAssemblyFullFilename);
                        referenced = Assembly.Load(refAssemblyBytes);
                    }
                    else
                    {
                        referenced = Assembly.Load(aRefa);
                    }
                    loadedAssemblies.Add(referenced);
                    LoadReferencedAssemblies(referenced, assemblyPath, ref loadedAssemblies);
                }
            }
        }
    }
}
