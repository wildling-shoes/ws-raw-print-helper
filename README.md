# Introduction 
This windows program reads files in printer language (from web apps) and sends them directly to the printer. 

# Getting Started
1. Install ws-raw-print-helper_setup.msi 
2. Open the program (WS Print Helper) once to select a printer for raw printing.
3. Tell your browser to open .zpl directly when downloaded. This way .zpl files from the web go directly to the printer.

# Usage
You can use this program to print files in printer language stored in your file system. 
The more interesting use case however is, to use this tool to print labels directly from web applications: 
If the web application in the browser supplies a ressource with the content type "application/zpl", 
the browser will automatically start this little tool and the label is printed without further ado.

You can change the content type used in the installer project to "application/prn" or whatever you like.

# Build and Test
The program is build as a ready2run single file application including the .net framework.
The installer is created with the WiX Toolset using an installer project from the Wix Toolset Visual Studio 2022 Extension.
The installerproject runs dotnet publish of the main project before the build.
You can find the msi file in the bin folder of the installer project.

# Credits
This program uses [RawPrint](https://github.com/frogmorecs/RawPrint) to send the files to the printer.