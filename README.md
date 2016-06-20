# SCSM-PrettyPrint
PrettyPrint app allows console users to print specific Work Item information using a custom Microsoft Word template. The solution creates a new Microsoft Word document from a given template and replaces bookmarks in the document with Work Item property values.

## Requirements
* Visual Studio 2012
* [System Center Visual Studio Authoring Extensions] (https://www.microsoft.com/en-us/download/details.aspx?id=30169)
* [Service Manager Authoring Tool] (https://www.microsoft.com/de-ch/download/details.aspx?id=40896)

I recommend using Microsoft Visual Studio 2012 for best experience with .Net 3.5 and Office Interop assemblies (also see the following [thread on StackOverflow](http://stackoverflow.com/questions/32394541/visual-studio-2015-create-vsto-project-for-office-2010-and-above-that-targets)). In addition you need to install the [System Center Visual Studio Authoring Extensions] (https://www.microsoft.com/en-us/download/details.aspx?id=30169) which enables you to create management packs for SCOM and SCSM right from within Visual Studio.

I also recommend installing the [Service Manager Authoring Tool] (https://www.microsoft.com/de-ch/download/details.aspx?id=40896). It contains a lot of DLL files and management packs that are referenced by this (and many other) Service Manager projects.

**IMPORTANT:** Since *Microsoft.EnterpriseManagement.Common.dll* and *Microsoft.EnterpriseManagement.Core.dll* are not included with Service Manager Authoring Tool, you will have to manually copy them from one of your Service Manager management servers (search for the files in Service Manager install directory) to *scsmlab.prettyprint\Resources* folder.

## Questions and issues

The [github issue tracker](https://github.com/sjohner/SCSM-PrettyPrint/issues) is **_only_** for bug reports and feature requests. Anything else, such as questions for help in using the app, please post on  [Technet Gallery project page](https://gallery.technet.microsoft.com/Service-Manager-PrettyPrint-0e1808b1) or get in contact directly.

## Authors
* **Stefan Johner** - *Initial work* - [sjohner](https://github.com/sjohner)

See also the list of [contributors](https://github.com/sjohner/SCSM-PrettyPrint/contributors) who participated in this project.

## License
This project is licensed under the GNU General Public License - see the [LICENSE.md](LICENSE.md) file for details
