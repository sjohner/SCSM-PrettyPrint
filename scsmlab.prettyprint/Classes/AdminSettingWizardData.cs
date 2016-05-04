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

//Requires Microsoft.EnterpriseManagement.Core and Microsoft.EnterpriseManagement.Common
//Contains EnterpriseManagementObject
using Microsoft.EnterpriseManagement.Common;
//Contains EnterpriseManagementGroup
using Microsoft.EnterpriseManagement;
//Contains ManagementPackClass
using Microsoft.EnterpriseManagement.Configuration;

//Requires Microsoft.EnterpriseManagement.UI.WpfWizardFramework.dll reference
//Contains WizardStory, WizardData
using Microsoft.EnterpriseManagement.UI.WpfWizardFramework;

//Contains RegistryKey
using Microsoft.Win32;


namespace scsmlab.prettyprint.Classes
{
    class AdminSettingWizardData : WizardData
    {
        #region Variables

        private EnterpriseManagementGroup emg = null;
        private EnterpriseManagementObject emoAdminSetting = null;
        private ManagementPackClass mpcAdminSetting = null;
        private Guid guidEnterpriseManagementObjectID = Guid.Empty;

        private String strIncidentTemplatePath = String.Empty;
        private String strServiceRequestTemplatePath = String.Empty;
        private String strChangeRequestTemplatePath = String.Empty;
        private IList<ManagementPackProperty> listIncidentProperties = null;
        private IList<ManagementPackTypeProjectionComponent> listIncidentComponents = null;
        private IList<ManagementPackProperty> listServiceRequestProperties = null;
        private IList<ManagementPackTypeProjectionComponent> listServiceRequestComponents = null;
        private IList<ManagementPackProperty> listChangeRequestProperties = null;
        private IList<ManagementPackTypeProjectionComponent> listChangeRequestComponents = null;
        private Boolean boolShowPrintPreview = true;

        public String IncidentTemplatePath
        {
            get
            {
                return this.strIncidentTemplatePath;
            }
            set
            {
                this.strIncidentTemplatePath = value;
            }
        }
        
        public String ServiceRequestTemplatePath
        {
            get
            {
                return this.strServiceRequestTemplatePath;
            }
            set
            {
                this.strServiceRequestTemplatePath = value;
            }
        }

        public String ChangeRequestTemplatePath
        {
            get
            {
                return this.strChangeRequestTemplatePath;
            }
            set
            {
                this.strChangeRequestTemplatePath = value;
            }
        }

        public Boolean ShowPrintPreview
        {
            get
            {
                return this.boolShowPrintPreview;
            }
            set
            {
                this.boolShowPrintPreview = value;
            }
        }

        public IList<ManagementPackProperty> IncidentProperties
        {
            get
            {
                return this.listIncidentProperties;
            }
            set
            {
                this.listIncidentProperties = value;
            }
        }

        public IList<ManagementPackTypeProjectionComponent> IncidentComponents
        {
            get
            {
                return this.listIncidentComponents;
            }
            set
            {
                this.listIncidentComponents = value;
            }
        }

        public IList<ManagementPackProperty> ServiceRequestProperties
        {
            get
            {
                return this.listServiceRequestProperties;
            }
            set
            {
                this.listServiceRequestProperties = value;
            }
        }

        public IList<ManagementPackTypeProjectionComponent> ServiceRequestComponents
        {
            get
            {
                return this.listServiceRequestComponents;
            }
            set
            {
                this.listServiceRequestComponents = value;
            }
        }

        public IList<ManagementPackProperty> ChangeRequestProperties
        {
            get
            {
                return this.listChangeRequestProperties;
            }
            set
            {
                this.listChangeRequestProperties = value;
            }
        }

        public IList<ManagementPackTypeProjectionComponent> ChangeRequestComponents
        {
            get
            {
                return this.listChangeRequestComponents;
            }
            set
            {
                this.listChangeRequestComponents = value;
            }
        }

        #endregion

        internal AdminSettingWizardData(EnterpriseManagementObject emoAdminSetting)
        {
            this.emoAdminSetting = emoAdminSetting;

            //Get SDK Server from Registry and create connection to Management Group based on this value
            RegistryKey scsmRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\System Center\2010\Service Manager\Console\User Settings");
            if (scsmRegistryKey != null)
            {
                string scsmSDKServer = scsmRegistryKey.GetValue("SDKServiceMachine").ToString();
                EnterpriseManagementConnectionSettings connectionSettings = new EnterpriseManagementConnectionSettings(scsmSDKServer);
                this.emg = new EnterpriseManagementGroup(connectionSettings);
            }

            //Get the Admin Setting class
            this.mpcAdminSetting = this.emg.EntityTypes.GetClass(new Guid(Constants.ADMINSETTING_SINGLETON_CLASS_ID));

            //Check wheter properties are Null and get values from Admin Setting object.
            if (this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_IR_TEMPLATEPATH].Value != null)
                this.IncidentTemplatePath = (String)this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_IR_TEMPLATEPATH].Value;
            if (this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_SR_TEMPLATEPATH].Value != null)
                this.ServiceRequestTemplatePath = (String)this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_SR_TEMPLATEPATH].Value;
            if (this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_CR_TEMPLATEPATH].Value != null)
                this.ChangeRequestTemplatePath = (String)this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_CR_TEMPLATEPATH].Value;
            if (this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_PRINTPREVIEW].Value != null)
                this.ShowPrintPreview = (Boolean)this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_PRINTPREVIEW].Value;

            //Get Incident and Service Request properties and components
            this.IncidentProperties = GetPropertiesCollection(new Guid(Constants.INCIDENT_CLASS_ID));
            this.IncidentComponents = GetComponentsCollection(new Guid(Constants.INCIDENT_TYPEPROJECTION));
            this.ServiceRequestProperties = GetPropertiesCollection(new Guid(Constants.SERVICEREQUEST_CLASS_ID));
            this.ServiceRequestComponents = GetComponentsCollection(new Guid(Constants.SERVICEREQUEST_TYPEPROJECTION));
            this.ChangeRequestProperties = GetPropertiesCollection(new Guid(Constants.CHANGEREQUEST_CLASS_ID));
            this.ChangeRequestComponents = GetComponentsCollection(new Guid(Constants.CHANGEREQUEST_TYPEPROJECTION));
        }

        internal IList<ManagementPackProperty> GetPropertiesCollection(Guid classId)
        {
            ManagementPackClass mpc = this.emg.EntityTypes.GetClass(classId);

            //Return list of properties for given class
            return (IList<ManagementPackProperty>)mpc.GetProperties(BaseClassTraversalDepth.Recursive, PropertyExtensionMode.All);           
        }

        internal IList<ManagementPackTypeProjectionComponent> GetComponentsCollection(Guid typeprojectionId)
        {
            ManagementPackTypeProjection mptp = emg.EntityTypes.GetTypeProjection(typeprojectionId);

            //Return list of components for given TypeProjection
            return (IList<ManagementPackTypeProjectionComponent>)mptp.ComponentCollection;
        }

        public override void AcceptChanges(WizardMode wizardMode)
        {
            //Set the property values to the new values 
            this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_IR_TEMPLATEPATH].Value = this.IncidentTemplatePath;
            this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_SR_TEMPLATEPATH].Value = this.ServiceRequestTemplatePath;
            this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_CR_TEMPLATEPATH].Value = this.ChangeRequestTemplatePath;
            this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_PRINTPREVIEW].Value = this.ShowPrintPreview;

            //Update Connector instance 
            this.emoAdminSetting.Commit();

            this.WizardResult = WizardResult.Success;
        }
    }
}
