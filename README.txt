Table of Contents

1.  Copyright
2.  Installation
3.  Upgrading
4.  Version History

1.  COPYRIGHT

Copyright (c) 2005-2006, Clay Alberty
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted 
provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions 
      and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
      and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Clay Alberty nor the names of his contributors may be used to endorse or 
      promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR 
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
OF SUCH DAMAGE.


2.  INSTALLATION

SQLEXPRESS:

For a new install, unzip the package into a folder on the web server.  Go into the App_Data folder and grant
read, write, and execute permissions on the files for the IUSR user.  This will allow ASP.NET to connect to the 
database.

SQL2005:

To install on SQL Server 2005, first set up a membership database by running aspnet_regsql from the framework directory.  If you already have a standard membership database for other sites you can skip this step.  You must then specify the default membership database in the machine.config file.  Next, run the ProntoWiki_Full.sql script in the App_Data directory, either against the membership db or against a new database that you must create.  This will create the core tables and stored procedures needed to run the application.  It will not, at the moment, create any pages or default content.  Change the connection string in the web.config file to point to the database containing the tables created by the .sql script.

When you first install on SQL Server 2005, go to the user.aspx page to create an account.  This page will create the default roles needed to run the application on the first visit to the page.  To get a user into the administrator role - you will need at least one to use the user administration features - the easiest way is to set the defaultUserRoles value in web.config to "Administrator;Reader;Editor" then change back after creating an administrative user. 

GENERAL:

If you get an error in the error handling section of the basepage file, you can turn off server-side error logging
or change it to log to a flat file via two tags in the web.config file:

    <add key="loggingEnabled" value="true" />
    <add key="logFile" value="somefile|eventlog" />

You can also enable error logging to the event log by setting registry permissions for the ASPNET identity (varies
depending on OS) in the registry for HKLM/System/Current Control Set/Services/EventLog.


3.  UPGRADING

If you installed the Alpha version, you will need to run the attachments.sql file found in the root of the
prontowiki folder.  If you are not upgrading or have already executed this file, it can be deleted.

BREAKING CHANGES

The formatting rules between the Alpha and Beta versions have changed.  No future changes are planned, but
the bold formatting and underscore formatting have changed.  Several other features have been added but text
created in the Alpha version will need to be updated to display properly in the Beta release.

4.  VERSION HISTORY

1.0.0.5 - A bug fix release.  Removed useradmin.aspx and replaced with usermembership.aspx, which properly uses the membership objects in the framework to address user administration.  Also generated SQL script to facilitate setup on SQL Server 2005 systems.

Made several bug fixes to urlencode page names containing special characters, spaces, etc.  Also corrected diff.aspx to properly display some formatting elements.

Added functionality for custom tagging after several user requests for enhanced markup.  Now administrators can create custom css classes and the users can instantly use the classes without any change to the PW code.

1.0.0.4 - Not a release candidate yet, but several new features have been added and some things have been fixed.  
The parsing code has been changed extensively, particularly for line break handling.  The engine does its best now to 
force text blocks inside of paragraph elements.  Text created in previous versions should look nearly identical
but there may be some minor changes in line breaking.  Lots of smaller defects have been corrected in the way 
that this version handles formatting from the first beta release, so anyone using the system is encouraged
to upgrade.  Also made several performance enhancements to the parsing code.

Added a diff function that still needs some work on polish but will effectively show text changes between
different versions.

1.0.0.3 - Beta release.  This is a full-featured release of the product.  Future releases are planned based on
user feedback and requests.  This release will be promoted to a final release unless any defects are found
in the code or in the release package.

This release features file and image attachment, automated markup using javascript, enhanced wiki formatting,
corrections to the formatting regexes, a user administration page, et al.

1.0.0.2 - Alpha, build 2.  This release fixed some formatting issues with the wiki regular expressions.

1.0.0.1 - Alpha, build 1.  This was the first functioning build of ProntoWiki.

