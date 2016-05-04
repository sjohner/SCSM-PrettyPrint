using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//Requires Microsoft.EnterpriseManagement.UI.WpfWizardFramework.dll reference
//Contains WizardStory, WizardData
using Microsoft.EnterpriseManagement.UI.WpfWizardFramework;

//Contains classes
using scsmlab.prettyprint.Classes;

namespace scsmlab.prettyprint.Forms
{
    /// <summary>
    /// Interaction logic for AdminSetting_Incident.xaml
    /// </summary>
    public partial class AdminSetting_Incident : WizardRegularPageBase
    {
        //Local reference to passed wizard data
        private AdminSettingWizardData adminSettingWizardData = null;

        public AdminSetting_Incident(WizardData wizardData)
        {
            //Set local data reference
            this.DataContext = wizardData;
            this.adminSettingWizardData = this.DataContext as AdminSettingWizardData;

            InitializeComponent();
        }

        private void buttonShowIncidentProperties_Click(object sender, RoutedEventArgs e)
        {
            PropertyWindow propertyWindow = new PropertyWindow();

            //Set property and component list item sources
            propertyWindow.listViewProperties.ItemsSource = this.adminSettingWizardData.IncidentProperties;
            propertyWindow.listViewComponents.ItemsSource = this.adminSettingWizardData.IncidentComponents;

            //Get current application (SCSM Console)
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;

            //Create new property window
            Window window = new Window();
            window.Height = 450;
            window.Width = 550;

            //Set window position
            window.Left = mainWindow.Left + (mainWindow.Width - window.Width) / 2;
            window.Top = mainWindow.Top + (mainWindow.Height - window.Height) / 2;

            //Set content and show new window
            window.Content = propertyWindow;
            window.Show();
        }
    }
}
