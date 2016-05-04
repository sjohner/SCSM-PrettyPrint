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

//Contains Process and ProcessStartInfo
using System.Diagnostics;

namespace scsmlab.prettyprint.Forms
{
    /// <summary>
    /// Interaction logic for AdminSetting_General.xaml
    /// </summary>
    public partial class AdminSetting_General : WizardRegularPageBase
    {
        //Local reference to passed wizard data
        private AdminSettingWizardData adminSettingWizardData = null;

        public AdminSetting_General(WizardData wizardData)
        {
            //Set local data reference
            this.DataContext = wizardData;
            this.adminSettingWizardData = this.DataContext as AdminSettingWizardData;

            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void itnetxLogo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://itnetx.ch");
            }
            catch
            {
                MessageBox.Show("Error occurred.  Try opening your browser and navigating to http://itnetx.ch");
            }
        }
    }
}
