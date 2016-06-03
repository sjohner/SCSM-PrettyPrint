# SCSM-PrettyPrint
PrettyPrint app allows console users to print specific Work Item information using a custom Microsoft Word template. The solution creates a new Microsoft Word document from a given template and replaces bookmarks in the document with Work Item property values.

## Requirements
I recommend using Microsoft Visual Studio 2012 for best experience with .Net 3.5 and Office Interop assemblies. In addition you probably want to install the [System Center Visual Studio Authoring Extensions] (https://www.microsoft.com/en-us/download/details.aspx?id=30169) which enables you to create management packs for SCOM and SCSM right from within Visual Studio.

I also recommend installing the [Service Manager Authoring Tool] (https://www.microsoft.com/de-ch/download/details.aspx?id=40896). It contains a lot of DLL files and management packs that are referenced by this (and many other) Service Manager projects.

**IMPORTANT:** Since *Microsoft.EnterpriseManagement.Common.dll* and *Microsoft.EnterpriseManagement.Core.dll* are not included with Service Manager Authoring Tool, you will have to manually copy them from one of your Service Manager management servers (search for the files in Service Manager install directory) to *scsmlab.prettyprint\Resources* folder.

## Authors
* **Stefan Johner** - *Initial work* - [sjohner](https://github.com/sjohner)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License
This project is licensed under the GNU General Public License - see the [LICENSE.md](LICENSE.md) file for details
