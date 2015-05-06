/*
HSPI_EnableRemotePlugins - HomeSeer 3 Plugin to enable remote plug-in API interface (for HomeSeer HomeTroller Zee or HS3-Pi)
Copyright (C) 2014-2015 iHomeAutomate - http://www.iHomeAutomate.com

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using Scheduler;
using HSCF.Communication.ScsServices.Service;
using System.Collections.Generic;
using HSCF.Communication.Scs.Communication.EndPoints.Tcp;
using System.Collections.Specialized;
using System.Net.Sockets;
using HomeSeerAPI;
using System.Timers;
using System.Threading;
using System.IO;
using System.Reflection;

namespace HSPI_EnableRemotePlugins
{
	public class HSPI : HomeSeerAPI.IPlugInAPI
	{
		private clsHSPI appCallbackAPI;
		private hsapplication hsApplication;
		private IScsServiceApplication scsServiceApplication;	

		private System.Timers.Timer checkPluginInitTimer;
		private System.Timers.Timer disableHSCheckTimer;

		private EventHandler<ServiceClientEventArgs> clientConnectedEventhandler;
		private ElapsedEventHandler disableHSCheckTimerEventHandler;
		private ElapsedEventHandler checkPluginInitTimerEventHandler;

		private static string PLUGIN_NAME = "EnableRemotePlugins";
		private static int PLUGIN_API_PORT = 10400;	

		public int Capabilities ()
		{
			return (int)Enums.eCapabilities.CA_IO;
		}

		public int AccessLevel ()
		{
			return (int)PluginAccessLevel.FREE;
		}

		public bool SupportsMultipleInstances ()
		{
			return false;
		}

		public bool SupportsMultipleInstancesSingleEXE ()
		{
			return false;
		}

		public bool SupportsAddDevice ()
		{
			return false;
		}

		public string InstanceFriendlyName ()
		{
			return "";
		}

		public IPlugInAPI.strInterfaceStatus InterfaceStatus ()
		{
			IPlugInAPI.strInterfaceStatus status = new IPlugInAPI.strInterfaceStatus();
			status.intStatus = IPlugInAPI.enumInterfaceStatus.OK;
			return status;
		}

		public void HSEvent (Enums.HSEvent eventType, object[] parameters)
		{
		}

		public string GenPage (string link)
		{
			return "";
		}

		public string PagePut (string data)
		{
			return "";
		}

		public void ShutdownIO ()
		{
			if (this.scsServiceApplication != null) {
				hsApplication.WriteLog (PLUGIN_NAME, "Shutting down");

				if (scsServiceApplication != null) {
					if (clientConnectedEventhandler != null) {
						scsServiceApplication.ClientConnected -= clientConnectedEventhandler;
					}

					scsServiceApplication.Stop ();
				}

				this.scsServiceApplication = null;
				this.hsApplication = null;
				this.appCallbackAPI = null;
				this.clientConnectedEventhandler = null;
			}

			if (checkPluginInitTimer != null) {
				checkPluginInitTimer.Stop ();
				if (checkPluginInitTimerEventHandler != null) {
					checkPluginInitTimer.Elapsed -= checkPluginInitTimerEventHandler;
				}
			}

			if (disableHSCheckTimer != null) {
				disableHSCheckTimer.Stop ();
				if (disableHSCheckTimerEventHandler != null) {
					disableHSCheckTimer.Elapsed -= disableHSCheckTimerEventHandler;
				}

			}
		}

		public bool RaisesGenericCallbacks ()
		{
			return false;
		}

		public void SetIOMulti (List<CAPI.CAPIControl> colSend)
		{
		}

		public string InitIO (string port)
		{
			if (hsApplication != null) {
				hsApplication.WriteLog (PLUGIN_NAME, "Init plugin");

				startTimers();
				initTimerEventHandlers();
				addExePluginsToTifList (); // Booh, this will only work when the HSPI_<plugin>.exe is compiled against the correct Zee references.

				try {
					clientConnectedEventhandler = new EventHandler<ServiceClientEventArgs> (clientConnected);

					scsServiceApplication = ScsServiceBuilder.CreateService (new ScsTcpEndPoint ("127.0.0.1", PLUGIN_API_PORT));
					scsServiceApplication.AddService<IHSApplication, hsapplication> (hsApplication);
					scsServiceApplication.AddService<IAppCallbackAPI, clsHSPI> (appCallbackAPI);
					scsServiceApplication.ConnectionTimeout = 120000;
					scsServiceApplication.ClientConnected += clientConnectedEventhandler;
					scsServiceApplication.Start ();

					hsApplication.WriteLog (PLUGIN_NAME, "Remote plug-in API interface started on port " + PLUGIN_API_PORT);

					return "";
				}
				catch(SocketException exception) {
					hsApplication.WriteLog (PLUGIN_NAME, exception.Message);
					return "Could not init plugin. Is port " + PLUGIN_API_PORT + " already in use?";
				}
			}

			return "Could not init plugin, no hsapplication available.";

		}

		public IPlugInAPI.PollResultInfo PollDevice (int dvref)
		{
			return new IPlugInAPI.PollResultInfo();
		}

		public bool SupportsConfigDevice ()
		{
			return false;
		}

		public bool SupportsConfigDeviceAll ()
		{
			return false;
		}

		public Enums.ConfigDevicePostReturn ConfigDevicePost (int @ref, string data, string user, int userRights)
		{
			return Enums.ConfigDevicePostReturn.DoneAndCancel;
		}

		public string ConfigDevice (int @ref, string user, int userRights, bool newDevice)
		{
			return "";
		}

		public SearchReturn[] Search (string SearchString, bool RegEx)
		{
			throw new NotImplementedException();
		}

		public object PluginFunction (string procName, object[] parms)
		{
			return null;
		}

		public object PluginPropertyGet(string procName, object[] parms)
		{
			return null;
		}

		public void PluginPropertySet(string procName, object value)
		{			
		}

		public void SpeakIn (int device, string txt, bool w, string host)
		{			
		}

		public int ActionCount ()
		{		
			return 0;
		}

		public bool ActionConfigured (IPlugInAPI.strTrigActInfo ActInfo)
		{			
			return false;
		}

		public string ActionBuildUI (string sUnique, IPlugInAPI.strTrigActInfo ActInfo)
		{
			return "";
		}

		public IPlugInAPI.strMultiReturn ActionProcessPostUI (NameValueCollection PostData, IPlugInAPI.strTrigActInfo TrigInfoIN)
		{		
			return new IPlugInAPI.strMultiReturn();
		}

		public string ActionFormatUI (IPlugInAPI.strTrigActInfo ActInfo)
		{
			return "";
		}

		public bool ActionReferencesDevice (IPlugInAPI.strTrigActInfo ActInfo, int dvRef)
		{
			return false;
		}

		public bool HandleAction (IPlugInAPI.strTrigActInfo ActInfo)
		{
			return false;
		}

		public string TriggerBuildUI (string sUnique, IPlugInAPI.strTrigActInfo TrigInfo)
		{
			return "";
		}

		public IPlugInAPI.strMultiReturn TriggerProcessPostUI (NameValueCollection PostData, IPlugInAPI.strTrigActInfo TrigInfoIN)
		{
			return new IPlugInAPI.strMultiReturn();
		}

		public string TriggerFormatUI (IPlugInAPI.strTrigActInfo TrigInfo)
		{
			return "";
		}

		public bool TriggerTrue (IPlugInAPI.strTrigActInfo TrigInfo)
		{
			return false;
		}

		public bool TriggerReferencesDevice (IPlugInAPI.strTrigActInfo TrigInfo, int dvRef)
		{
			return false;
		}

		public string GetPagePlugin (string page, string user, int userRights, string queryString)
		{
			return "";
		}

		public string PostBackProc (string page, string data, string user, int userRights)
		{
			return "";
		}

		public IHSApplication HSObj {
			get {
				return hsApplication;
			}
			set {
				this.hsApplication = (hsapplication)value;
			}
		}

		public IAppCallbackAPI CallBackObj {
			get {
				return appCallbackAPI;
			}
			set {
				this.appCallbackAPI = (clsHSPI)value;
			}
		}

		public string Name {
			get {
				return PLUGIN_NAME;
			}
		}

		public bool HSCOMPort {
			get {
				return false;
			}
		}

		public bool ActionAdvancedMode {
			get {
				return false;
			}
			set {

			}
		}

		public string ActionName {
			get {
				return PLUGIN_NAME + " Action";
			}
		}

		public bool HasConditions {
			get {
				return false;
			}
		}

		public bool HasTriggers {
			get {
				return false;
			}
		}

		public int TriggerCount {
			get {
				return 0;
			}
		}

		public string TriggerName {
			get {
				return "";
			}
		}

		public int SubTriggerCount {
			get {
				return 0;
			}
		}

		public string SubTriggerName {
			get {
				return "";
			}
		}

		public bool TriggerConfigured {
			get {
				return false;
			}
		}

		public bool Condition {
			get {
				return false;
			}
			set {

			}
		}

		public string get_ActionName(int ActionNumber)
		{
			return "";
		}

		public bool get_Condition(IPlugInAPI.strTrigActInfo TrigInfo)
		{
			return false;
		}

		public bool get_HasConditions(int TriggerNumber)
		{
			return false;
		}

		public int get_SubTriggerCount(int TriggerNumber)
		{
			return 0;
		}

		public string get_SubTriggerName(int TriggerNumber, int SubTriggerNumber)
		{
			return "";
		}

		public bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo TrigInfo)
		{
			return false;
		}

		public string get_TriggerName(int TriggerNumber)
		{
			return "";
		}

		public void set_Condition(IPlugInAPI.strTrigActInfo TrigInfo, bool Value)
		{			
		}

		private void clientConnected (object sender, ServiceClientEventArgs e)
		{
			disableHSCheckTimer.Start();
			checkPluginInitTimer.Interval = 5;

			hsApplication.WriteLog (PLUGIN_NAME, "Incoming remote connection " + e.Client.ipaddress + " (ClientId " + e.Client.ClientId + ")");

			Console.WriteLine ("You should be able to remotely connect the plugin if it's free or one of the below entries:");
			for (int i=0; i<appCallbackAPI.TifList.Count; i++) {
				Scheduler.Classes.clsPlugInfo pluginfo = (Scheduler.Classes.clsPlugInfo)appCallbackAPI.TifList.GetByIndex(i);
				Console.WriteLine (pluginfo.FileName + " :: " + pluginfo.PluginName + " :: " + pluginfo.RegistrationStatus + " :: " + pluginfo.AccessLevel);

				// Regarding remote plugins and its licensing mechanism:
				// "Yes, you need to have a copy on the HS machine. The reason is that when the remote plug-in tries to connect, the copy on the HS machine is what is used to determine the licensing that it uses. If it is a licensed plug-in, then HS has to check to see if there is a license. "
				// source: http://forums.homeseer.com/showthread.php?t=169287

				// The workaround addExePluginsToTifList () unfortunately does not work due to conflicting HomeSeerAPI.IPlugInAPI (Zee vs HS3) interface :-(
			}
		}

		private void checkPluginTimer_Elapsed (object sender, ElapsedEventArgs e)
		{
			checkPluginsToInitialize();
		}

		private void disableHSCheckTimer_Elapsed (object sender, ElapsedEventArgs e)
		{
			appCallbackAPI.StopCheckTimer(); // It'll make sure the out-of-box plugin check is disabled for our custom initialization to be triggered.
		}

		private void startTimers()
		{
			hsApplication.WriteLog (PLUGIN_NAME, "Starting timer(s) for remote plugins to be initialized.");

			disableHSCheckTimer = new System.Timers.Timer ();
			disableHSCheckTimer.Interval = 5;
			disableHSCheckTimer.AutoReset = false;
			disableHSCheckTimer.Stop(); // By default, do not enable. Only enable when there is a new incoming connection.

			checkPluginInitTimer = new System.Timers.Timer ();
			checkPluginInitTimer.Interval = 30000;
			checkPluginInitTimer.AutoReset = false;
			checkPluginInitTimer.Start();
		}

		private void initTimerEventHandlers()
		{
			hsApplication.WriteLog (PLUGIN_NAME, "Initializing eventhandler(s) for remote plugins to be initialized.");

			if (disableHSCheckTimerEventHandler == null) {
				disableHSCheckTimerEventHandler = new ElapsedEventHandler (disableHSCheckTimer_Elapsed);
				if (disableHSCheckTimer != null) {
					disableHSCheckTimer.Elapsed += disableHSCheckTimerEventHandler;
				}
			}

			if (checkPluginInitTimerEventHandler == null) {
				checkPluginInitTimerEventHandler = new ElapsedEventHandler (checkPluginTimer_Elapsed);
				if (checkPluginInitTimer != null) {
					checkPluginInitTimer.Elapsed += checkPluginInitTimerEventHandler;
				}
			}
		}

		private void checkPluginsToInitialize ()
		{
			checkPluginInitTimer.Stop();

			object syncRoot = appCallbackAPI.IOobjsPending.SyncRoot;
			lock (syncRoot) {
				if (appCallbackAPI.IOobjsPending.Count > 0) {
					checkPluginInitTimer.Interval = 30000;
					hsApplication.WriteLog (PLUGIN_NAME, "Preparing to send .InitIO to " + appCallbackAPI.IOobjsPending.Count + " plugin(s).");

					for (int i = 0; i < appCallbackAPI.IOobjsPending.Count; i++) {
						clsHSPI.PluginHolder pluginHolder = (clsHSPI.PluginHolder)appCallbackAPI.IOobjsPending.GetByIndex (i);
						if (pluginHolder == null) {
							i++;
							continue;
						}

						if (pluginHolder.waitingToConnect | pluginHolder.shuttingDown) {
							i++;
							continue;
						}

						HomeSeerAPI.IPlugInAPI clientProxy = pluginHolder.client.GetClientProxy<HomeSeerAPI.IPlugInAPI> ();
						pluginHolder.obj_ref = clientProxy; 

						hsApplication.WriteLog (PLUGIN_NAME, "'" + pluginHolder.obj_name + ":" + pluginHolder.InstanceName + "'.InitIO(" + pluginHolder.sPortNumber + ")");
						string text = clientProxy.InitIO (pluginHolder.sPortNumber);

						if (text == "") {
							if (!pluginHolder.obj_ref.SupportsMultipleInstances () & pluginHolder.InstanceName != "") { // An instancename given when not supporting multiple instances? :-)
								pluginHolder.InstanceName = "";
							}
							if (appCallbackAPI.IOobjs.ContainsKey (pluginHolder.obj_name + ":" + pluginHolder.InstanceName)) {
								appCallbackAPI.IOobjs.Remove (pluginHolder.obj_name + ":" + pluginHolder.InstanceName);
							}
							appCallbackAPI.IOobjs.Add (pluginHolder.obj_name + ":" + pluginHolder.InstanceName, pluginHolder);
							PageBuilderAndMenu.RebuildPlugMenu ();

							hsApplication.WriteLog (PLUGIN_NAME, "'" + pluginHolder.obj_name + ":" + pluginHolder.InstanceName + "' initialized.");
						}
						else {
							hsApplication.WriteLog (PLUGIN_NAME, "InitIO failed for '" + pluginHolder.obj_name + ":" + pluginHolder.InstanceName + "' > " + text);
							pluginHolder.obj_ref.ShutdownIO ();
						}	
					}
					appCallbackAPI.IOobjsPending.Clear ();
					disableHSCheckTimer.Stop(); // No pending plugins until the next connection, let's disable.
				}
			}

			checkPluginInitTimer.Start();
		}

		/**
		 * Find all HSPI_*.exe in hsApplication.GetAppPath() and try to instantiate their HSPI class
		 * !! Note, there are API conflicts between HS3 and Zee interfaces. It's currently not possible to add out-of-box HS3 plugins.
		 * It should work however when plugins are recompiled against the correct Zee references.
		 * */
		private void addExePluginsToTifList() {
			string[] exePluginPaths = Directory.GetFiles(hsApplication.GetAppPath(), "HSPI_*.exe");
			foreach (string exePluginPath in exePluginPaths) {
				if (!exePluginPath.Contains ("vshost")) { /* Skip that */
					String pluginFilename = Path.GetFileName (exePluginPath);
					String className = pluginFilename.Replace(".exe", ".HSPI");
					procesPluginInfo (exePluginPath, pluginFilename, className);
				}
			}				
		}

		private void procesPluginInfo (string exePluginPath, string pluginFilename, string className)
		{
			AppDomain appDomain = null;
			AppDomainSetup appDomainSetup = new AppDomainSetup ();
			appDomainSetup.ApplicationBase = hsApplication.GetAppPath ();
			if (File.Exists (exePluginPath + ".config")) {
				appDomainSetup.ConfigurationFile = exePluginPath + ".config";
			}
			appDomainSetup.ApplicationName = PLUGIN_NAME + "AppDomain";
			appDomain = AppDomain.CreateDomain (PLUGIN_NAME + "AppDomain", null, appDomainSetup);
			clsHSPI.Worker worker = (clsHSPI.Worker)appDomain.CreateInstanceAndUnwrap (typeof(clsHSPI.Worker).Assembly.FullName, typeof(clsHSPI.Worker).FullName);
			worker.gpath = hsApplication.GetAppPath ();
			worker.filename = pluginFilename;
			worker.classname = className;
			try {
				worker.ProcessAssembly (); // TypeLoadException :-?. Mismatching HomeSeerAPI.IPlugInAPI interface :-(
				/*
				FileStream fileStream = new FileStream (hsApplication.GetAppPath() + "/" + pluginFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				BinaryReader binaryReader = new BinaryReader (fileStream);
				byte[] rawAssembly = binaryReader.ReadBytes (Convert.ToInt32 (fileStream.Length));
				fileStream.Close ();
				binaryReader.Close ();
				Assembly assembly = AppDomain.CurrentDomain.Load (rawAssembly);
				HomeSeerAPI.IPlugInAPI hspi = (HomeSeerAPI.IPlugInAPI)assembly.CreateInstance (className, false, BindingFlags.CreateInstance, null, null, null, null);
				Console.WriteLine(hspi.Name + " :: " + hspi.AccessLevel());
				*/

				Scheduler.Classes.clsPlugInfo plugInfo = new Scheduler.Classes.clsPlugInfo ();
				plugInfo.ProgID = className;
				plugInfo.HSCOMPort = worker.comport;
				plugInfo.PluginName = worker.plugname;
				plugInfo.Capabilities = worker.capabilites;
				plugInfo.AccessLevel = worker.accessLevel;
				plugInfo.FileName = pluginFilename;
				plugInfo.SupportsMultipleInstances = worker.supportsMultipleInstances;
				plugInfo.SupportsMultipleInstancesSingleEXE = worker.supportsMultipleInstancesSingleEXE;
				plugInfo.RegistrationStatus = "NotChecked";
				appCallbackAPI.TifList.Add (plugInfo.PluginName + ":", plugInfo);
				hsApplication.WriteLog (PLUGIN_NAME, "Class " + className + " (" + pluginFilename + ") inspected for plug-info list");
			}
			catch (System.TypeLoadException typeLoadException) {
				hsApplication.WriteLog (PLUGIN_NAME, "Could not inspect " + className + " (" + pluginFilename + "). Failed with " + worker.errorMessage);
				Console.WriteLine (typeLoadException);
			}
			catch (Exception ex) {
				hsApplication.WriteLog (PLUGIN_NAME, "Could not inspect " + className + " (" + pluginFilename + "). Failed with " + worker.errorMessage);
				Console.WriteLine (ex);
			}
		}

		private enum PluginAccessLevel
		{
			FREE = 1,
			LICENSED = 2
		}
	}
}											