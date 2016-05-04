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

namespace scsmlab.prettyprint.Classes
{
    /// <summary>
    /// //Contains constants which are used within other classes
    /// </summary>
    class Constants
    {
        //WorkItem class properties
        internal const string WORKITEM_ID_DOLLAR = "$Id$";
        internal const string WORKITEM_TITLE = "Title";
        
        //Incident class properties
        internal const string INCIDENT_TYPEPROJECTION = "285cb0a2-f276-bccb-563e-bb721df7cdec";
        internal const string INCIDENT_CLASS_ID = "a604b942-4c7b-2fb2-28dc-61dc6f465c68";

        //Service Request class properties
        internal const string SERVICEREQUEST_TYPEPROJECTION = "e44b7c06-590d-64d6-56d2-2219c5e763e0";
        internal const string SERVICEREQUEST_CLASS_ID = "04b69835-6343-4de2-4b19-6be08c612989";

        //Change Request class properties
        internal const string CHANGEREQUEST_TYPEPROJECTION = "674194d8-0246-7b90-d871-e1ea015b2ea7";
        internal const string CHANGEREQUEST_CLASS_ID = "e6c9cf6e-d7fe-1b5d-216c-c3f5d2c7670c";

        //AdminSetting class properties
        internal const string ADMINSETTING_SINGLETON_CLASS_ID = "992544c4-b1c7-e1f6-4ede-08f7ddc51f02";
        internal const string ADMINSETTING_IR_TEMPLATEPATH = "scsmlabirtemplatepath";
        internal const string ADMINSETTING_SR_TEMPLATEPATH = "scsmlabsrtemplatepath";
        internal const string ADMINSETTING_CR_TEMPLATEPATH = "scsmlabcrtemplatepath";
        internal const string ADMINSETTING_PRINTPREVIEW = "scsmlabprintpreview";
    }
}
