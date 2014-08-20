/*
HSPI_EnableRemotePlugins - HomeSeer 3 Plugin to enable remote plug-in API interface (for HomeSeer HomeTroller Zee or HS3-Pi)
Copyright (C) 2014 iHomeAutomate - http://www.iHomeAutomate.eu

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

namespace HSPI_EnableRemotePlugins
{
	public class HSPI : HomeSeerAPI.IPlugInAPI
	{
		private clsHSPI appCallbackAPI;
		private hsapplication hsApplication;
		private IScsServiceApplication scsServiceApplication;	

		private EventHandler<ServiceClientEventArgs> clientConnectedEventhandler;

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
			hsApplication.WriteLog (PLUGIN_NAME, "Incoming remote connection " + e.Client.ipaddress);
		}

		private enum PluginAccessLevel
		{
			FREE = 1,
			LICENSED = 2
		}
	}
}

