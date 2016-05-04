/* 
 * Copyright (C) 2014 - 2016 Stefan Johner <sjohner@posteo.ch>
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

//Requires Microsoft.EnterpriseManagement.Core.dll and Microsoft.EnterpriseManagement.Common.dll references
//Contains EnterpriseManagementObject
using Microsoft.EnterpriseManagement.Common;
//Contains EnterpriseManagementGroup
using Microsoft.EnterpriseManagement;
//Contains ManagementPackClass
using Microsoft.EnterpriseManagement.Configuration;

//Requires Microsoft.EnterpriseManagement.UI.SkdDataAccess
//Contains ConsoleCommand
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
//Contains BindableCachedDataItem
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;

//Requires Microsoft.EnterpriseManagement.ServiceManager.Application.Common.dll reference
//Contains FormUtilities
using Microsoft.EnterpriseManagement.GenericForm;

//Require Microsoft.EnterpriseManagement.UI.Extensions.dll reference
//ConsoleContextHelper
using Microsoft.EnterpriseManagement.UI.Extensions.Shared;

//Requires Microsoft.EnterpriseManagement.UI.Foundation.dll reference
//Contains NavigationModelNodeBase
using Microsoft.EnterpriseManagement.ConsoleFramework;
//Contains IDataItem
using Microsoft.EnterpriseManagement.UI.DataModel;

//Requires Microsoft Office.Core and Microsoft.Office.Interop.Word references (found in COM references)
//Contains WdWindowState, Document and Application
using Microsoft.Office.Interop.Word;

//Contains MessageBox
using System.Windows;

//Contains RegistryKey
using Microsoft.Win32;

//Contains Process
using System.Diagnostics;

namespace scsmlab.prettyprint.Classes
{
    class IncidentTaskHandler : ConsoleCommand
    {
        private EnterpriseManagementGroup emg = null;
        private EnterpriseManagementObject emoAdminSetting = null;
        private ManagementPackClass mpcAdminSetting = null;

        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            //There will only ever be one item because we are going to limit this task to single select
            foreach (NavigationModelNodeBase node in nodes)
            {
                IDataItem dataItemWorkItem = null;

                //Check if task was started from form
                bool startedFromForm = FormUtilities.Instance.IsNodeWithinForm(nodes[0]);

                //If started from form
                if (startedFromForm)
                {
                    //Get Computer object instance
                    dataItemWorkItem = FormUtilities.Instance.GetFormDataContext(node);
                }
                //Else started from view
                else
                {
                    IDataItem dataItemNode = node;
                    if (dataItemNode != null)
                    {
                        //Get WorkItem object instance from selected navigationNode
                        dataItemWorkItem = ConsoleContextHelper.Instance.GetProjectionInstance((Guid)dataItemNode[Constants.WORKITEM_ID_DOLLAR], new Guid(Constants.INCIDENT_TYPEPROJECTION));
                    }
                }

                //Get SDK Server from Registry and create connection to Management Group based on this value
                RegistryKey scsmRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\System Center\2010\Service Manager\Console\User Settings");
                if (scsmRegistryKey != null)
                {
                    string scsmSDKServer = scsmRegistryKey.GetValue("SDKServiceMachine").ToString();
                    EnterpriseManagementConnectionSettings connectionSettings = new EnterpriseManagementConnectionSettings(scsmSDKServer);
                    emg = new EnterpriseManagementGroup(connectionSettings);
                }

                if (emg != null)
                {
                    //Create new EnterpriseManagementObject with AdminSettings class
                    this.emoAdminSetting = this.emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(Constants.ADMINSETTING_SINGLETON_CLASS_ID), ObjectQueryOptions.Default);
                    this.mpcAdminSetting = this.emg.EntityTypes.GetClass(new Guid(Constants.ADMINSETTING_SINGLETON_CLASS_ID));

                    //Get settings from AdminSettings class and show error message when incident template path is missing
                    if ((this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_IR_TEMPLATEPATH].Value == null || String.IsNullOrEmpty((String)this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_IR_TEMPLATEPATH].Value)))
                    {
                        MessageBox.Show("Configuration missing!\nGo to Admin Settings to configure PrettyPrint App", "Configuration Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {

                        IDataItem dataItemClass = dataItemWorkItem["$Class$"] as IDataItem;

                        BindableCachedDataItem<IComposableProjection> bcdiDC = (BindableCachedDataItem<IComposableProjection>)dataItemWorkItem;

                        List<IDataItem> listDataItemComponents = (List<IDataItem>)((EnterpriseManagementObjectProjectionDataType)bcdiDC.DataType).TypeProjectionDataItem["ComponentCollection"];

                        Dictionary<string, IDataItem> dataItemClassProperties = dataItemClass["PropertyCollection"] as Dictionary<string, IDataItem>;
                        //Create new application
                        Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                        //Create new document from template
                        string strPath = this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_IR_TEMPLATEPATH].Value as String;
                        Document doc = word.Documents.Add(@strPath);
                        //Activate document
                        doc.Activate();

                        //Iterate through bookmarks in active document
                        for (int i = 1; i <= word.ActiveDocument.Bookmarks.Count; i++)
                        {
                            object objIndex = i;

                            //Get bookmark name
                            string bookmarkName = word.ActiveDocument.Bookmarks.get_Item(ref objIndex).Name;

                            //First check if property exists for corresponding bookmark name since there may exist properties which contain
                            //underscore in property name
                            if (dataItemClassProperties.ContainsKey(bookmarkName))
                            {
                                //Check if property is null
                                if (dataItemWorkItem[bookmarkName] != null)
                                {
                                    word.ActiveDocument.Bookmarks[bookmarkName].Select();
                                    word.Selection.TypeText(dataItemWorkItem[bookmarkName].ToString());
                                }
                            }
                            //If not, check if component or related porperty exists for corresponding bookmark name
                            else
                            {
                                string componentName = "";
                                string relatedPropertyName = "";

                                //Check if bookmark contains _ to indicate that a property of a related item should be used
                                //For bookmarks, the name must start with a word character (but not a digit), then any Unicode word character may follow 
                                //up to an overall length of 40 characters. Word characters explicitly exclude white space and punctuation of any kind. 
                                //http://stackoverflow.com/questions/852922/what-are-the-limitations-for-bookmark-names-in-microsoft-word
                                if (bookmarkName.Contains('_'))
                                {
                                    string[] token = bookmarkName.Split('_');
                                    componentName = token[0];
                                    relatedPropertyName = token[1];
                                }
                                else
                                {
                                    componentName = bookmarkName;
                                }

                                //Check if component exists for corresponding bookmark namen
                                foreach (IDataItem component in listDataItemComponents)
                                {
                                    if ((component["Name"] as String).Equals(componentName))
                                    {
                                        //Check if object is related
                                        if (dataItemWorkItem[componentName] != null)
                                        {
                                            string strBookmarkText = "";

                                            word.ActiveDocument.Bookmarks[bookmarkName].Select();

                                            if (String.IsNullOrEmpty(relatedPropertyName))
                                            {
                                                //If component returns a collection, get DisplayName of each item in collection and add it to bookmark text
                                                if (dataItemWorkItem[componentName].GetType() == typeof(DataItemCollection))
                                                {
                                                    foreach (IDataItem collectionItem in (DataItemCollection)dataItemWorkItem[componentName])
                                                    {
                                                        strBookmarkText += (String)collectionItem["DisplayName"] + System.Environment.NewLine;
                                                    }
                                                }
                                                else
                                                {
                                                    strBookmarkText = dataItemWorkItem[componentName].ToString();
                                                }
                                            }
                                            else
                                            {
                                                IDataItem relatedObject = (IDataItem)dataItemWorkItem[componentName];
                                                //Check if related object property is null
                                                if (relatedObject[relatedPropertyName] != null)
                                                {
                                                    strBookmarkText = relatedObject[relatedPropertyName].ToString();
                                                }
                                            }

                                            word.Selection.TypeText(strBookmarkText);
                                        }
                                    }
                                }
                            }
                        }

                        if ((Boolean)this.emoAdminSetting[this.mpcAdminSetting, Constants.ADMINSETTING_PRINTPREVIEW].Value)
                        {
                            //Set print preview mode, make Word visible and activate application
                            word.ActiveWindow.ActivePane.View.Zoom.Percentage = 100;
                            word.PrintPreview = true;
                            word.Visible = true;
                            word.Activate();

                            //Minimize and maximize window to bring it to front otherwise it just blinks in taskbar
                            foreach (Microsoft.Office.Interop.Word.Window window in doc.Windows)
                            {
                                window.WindowState = WdWindowState.wdWindowStateMinimize;
                                window.WindowState = WdWindowState.wdWindowStateMaximize;
                            }
                        }
                        else
                        {
                            //Hide Word application
                            word.Visible = false;

                            //Create custom print dialog because System.Windows.Controls.PrintDialog is missing owner property and cannot be invoked from form
                            PrintDialog pDialog = new PrintDialog();
                            //System.Windows.Controls.PrintDialog pDialog = new System.Windows.Controls.PrintDialog();

                            //Check if task was started from form and use either form window or application main window as owner of print dialog
                            Nullable<Boolean> print = null;
                            if (startedFromForm)
                            {
                                /* Workaround needed because of missing Owner property in WPF PrintDialog
                                 * 
                                 * Application.MainWindow is used for a HWND parent of the PrintDialog. MainWindow property can only be accessed 
                                 * from the main UI thread making PrintDialog uninvokable from an UI thread different than the main UI thread.
                                 * This problem occurs when trying to invoke PrintDialog from a WorkItem form.
                                 * See https://connect.microsoft.com/VisualStudio/feedback/details/462054/wpf-printdialog-is-missing-owner-property-making-it-unusable-from-ui-threads-other-than-main-ui-thread
                                 * for a more detailed description and comments from Microsoft */

                                //When started from form, search for running process where title contians incident id and get its main window handle.
                                IntPtr hWnd = IntPtr.Zero;
                                foreach (Process pList in Process.GetProcesses())
                                {
                                    if (pList.MainWindowTitle.Contains(dataItemWorkItem["Id"] as String))
                                    {
                                        hWnd = pList.MainWindowHandle;
                                    }
                                }

                                //Convert IntPtr to System.Windows.Window object and make it available for later use with custom print dialog
                                System.Windows.Interop.HwndSource hwndSource = System.Windows.Interop.HwndSource.FromHwnd(hWnd);
                                System.Windows.Window formWindow = hwndSource.RootVisual as System.Windows.Window;
                                print = pDialog.ShowDialog(formWindow);
                            }
                            else
                            {
                                //Use console main window as owner
                                print = pDialog.ShowDialog(System.Windows.Application.Current.MainWindow);
                            }

                            //Print if user clicks ok
                            if (print == true)
                            {
                                //Set selected printer as active printer and print active document
                                word.ActivePrinter = pDialog.PrintQueue.FullName;
                                word.ActiveDocument.PrintOut();
                            }

                            //Close document
                            ((Microsoft.Office.Interop.Word._Document)doc).Close(SaveChanges: false);
                            doc = null;

                            //Close Word application
                            ((Microsoft.Office.Interop.Word._Application)word).Quit(SaveChanges: false);
                            word = null;
                        }
                    }
                }
            }
        }
    }
}
