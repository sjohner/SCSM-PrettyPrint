/* 
 * Copyright (C) 2014 - 2016 Stefan Johner <sjohner@outlook.com>
 * This file is part of Service Manager PrettyPrint.
 * 
 * Service Manager PrettyPrint is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Service Manager PrettyPrint is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Requires Microsoft.EnterpriseManagement.Core and Microsoft.EnterpriseManagement.Common.dll
//Contains EnterpriseManagementObject
using Microsoft.EnterpriseManagement.Common;
//Contains EnterpriseManagementGroup
using Microsoft.EnterpriseManagement;

//Requires Microsoft.EnterpriseManagement.UI.SkdDataAccess.dll
//Contains ConsoleCommand
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;

//Requires Microsoft.EnterpriseManagement.UI.Foundation.dll reference
//Contains NavigationModelNodeBase
using Microsoft.EnterpriseManagement.ConsoleFramework;

//Requires Microsoft.EnterpriseManagement.UI.WpfWizardFramework.dll reference
//Contains WizardStory, WizardData
using Microsoft.EnterpriseManagement.UI.WpfWizardFramework;

//Contains forms
using scsmlab.prettyprint.Forms;

//Contains RegistryKey
using Microsoft.Win32;

namespace scsmlab.prettyprint.Classes
{
    class AdminSettingTaskHandler : ConsoleCommand
    {
        private EnterpriseManagementGroup emg = null;
        private EnterpriseManagementObject emoAdminSetting = null;

        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            //Get SDK Server from Registry and create connection to Management Group based on this value
            RegistryKey scsmRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\System Center\2010\Service Manager\Console\User Settings");
            if (scsmRegistryKey != null)
            {
                string scsmSDKServer = scsmRegistryKey.GetValue("SDKServiceMachine").ToString();
                EnterpriseManagementConnectionSettings connectionSettings = new EnterpriseManagementConnectionSettings(scsmSDKServer);
                this.emg = new EnterpriseManagementGroup(connectionSettings);
            }

            //Get the Object using the GUID from above - since this is a singleton object we can get it by GUID 
            this.emoAdminSetting = this.emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(Constants.ADMINSETTING_SINGLETON_CLASS_ID), ObjectQueryOptions.Default);

            //Create a new "wizard" (also used for property dialogs as in this case), set the title bar, create the data, and add the pages 
            WizardStory wizard = new WizardStory();

            wizard.WizardWindowTitle = "PrettyPrint Settings";
            WizardData data = new AdminSettingWizardData(this.emoAdminSetting);
            wizard.WizardData = data;
            wizard.AddLast(new WizardStep("General Configuration", typeof(AdminSetting_General), wizard.WizardData));
            wizard.AddLast(new WizardStep("Incident Configuration", typeof(AdminSetting_Incident), wizard.WizardData));
            wizard.AddLast(new WizardStep("Service Request Configuration", typeof(AdminSetting_ServiceRequest), wizard.WizardData));
            wizard.AddLast(new WizardStep("Change Request Configuration", typeof(AdminSetting_ChangeRequest), wizard.WizardData));

            //Show the property page 
            PropertySheetDialog tpsd = new PropertySheetDialog(wizard);

            tpsd.Width = 800;
            tpsd.Height = 700;
            tpsd.Title = "SCSMLab PrettyPrint Settings";
            tpsd.ShowInTaskbar = true;
            tpsd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            //Update the view when done so the new values are shown 
            bool? dialogResult = tpsd.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                RequestViewRefresh();
            }
        }
    }
}
