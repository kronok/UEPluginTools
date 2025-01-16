This repository contains various small tools and build scripts that can help with creating and packaging binary builds of Unreal Engine Plugins.
I use these tools to create binary versions of the [**Gradientspace UE Toolbox**](https://www.gradientspace.com/uetoolbox) plugin (try it! free!) 

Currently only Win64 is really supported, mainly because Windows .bat files are used to script the binary build process. In addition, packaged
UE binary plugins have various limitations. I describe these limits in detail on the [UEToolbox installation help page](https://www.gradientspace.com/uetoolbox-installation).
Note that binary plugins *can* be used with a source build, but you have to jump through some hoops - again, see the UEToolbox install page for notes.

I strongly recommend you fork this project if you wish to use these build tools, as non-compatible changes are likely to be made in future.



# UEPluginPackager

Unreal Editor supports compiling a "Packaged" binary version of a Plugin, however the output is not actually shippable.
The resulting folder needs various changes to make an installable binary plugin comparable to what you might download from Fab.
UEPluginPackager is a command-line C# application that does this process, producing a zipped folder that can be distributed,
as well as several "version" text files (which I use for auto-update checking functionality). 

*Note: Currently this tool runs in-place, ie it is destructive, so it should be run on a copy of the build*

The tool is run like this:

**UEPluginPackager** *<BuiltPluginFolder>* *[-EnableByDefault]* *[-NoShipping]*

Where -EnableByDefault is optional and will add the EnableByDefault flag to the resulting .uplugin file, and 
-NoShipping if enabled will delete additional files from the built plugin that prevent it from being used to create Shipping builds
(ie to create a "demo" version of your plugin)

By default, the plugin will delete all temporary build files (ie Build and Intermediate folders), PDB Files, and Private and Public Source files.
This behavior can be controlled by modifing the flags at the top of Program.cs and recompiling.

For a binary plugin to function properly, the various ModuleName.Build.cs files need to have the line **bUsePrecompiled=true;** added to them.
UEPluginPackager does this by string-replacing a token in any *.Build.cs files in the project - you must add the line **//#UEPLUGINTOOL** to your *.Build.cs
and that line will be replaced with the bUsePrecompiled line. 

The .uplugin file will also be updated if necessary (eg if the -EnableByDefault flag was passed)

UEPluginPackager outputs version information text files. The current version of your plugin should be in a single-line text file located at 
*<PluginName>/Source/GS_VERSION_NUMBER.txt*. The version number must be in Major.Minor.Patch format, eg this text file would then contain a line like "0.1.8" (without quotes).

The UE version used to build the plugin can also be included, this is read from a file *<PluginName>/Source/GS_UNREAL_VERSION.txt* if it exists. 
Currently UEPluginPackager does not figure this out itself, you must create this text file (the binary-build scripts below do it automatically).
The version number must be in Major.Minor format.

If you would like to read in more detail about what the requirements are for creating a packaged plugin, I found
[this article by AlgoSyntax to be highly informative](https://store.algosyntax.com/tutorials/unreal-engine/how-to-package-and-sell-binary-plugins-for-ue5/)

# CopyrightChecker

This is a simple C# command-line tool that scans through all the .cpp, .h, and .cs files in all subdirectories of a folder and checks
if the first string matches a hardcoded copyright line (eg *// Copyright Gradientspace Corp. All Rights Reserved.*). The copyright
string is hardcoded in the Program.cs file (to the string above). To use this tool, edit Program.cs to change the copyright string,
and then recompile.

By default the program will simply print out the filenames of any files missing the copyright string. Optionally, *-AddIfMissing* can
be passed as a command-line argument to have the program prepend the string if it is missing.


# BinaryPluginBuildWin64

This subdirectory contains the skeleton of a build system for creating a redistributable binary build of a plugin for multiple Engine versions.

You must have the binary (Launcher) version of the Engine installed for each engine version you want to binary-package for.

You must place your plugin's code in the */PluginSources* folder. A sample folder named MyPluginName is placed there in the git repo, but is non-functional.
In addition, you must edit the **PLUGIN_CONFIG.txt** file, currently this contains a string 'PluginName=MyPluginName', replace MyPluginName with the name of
your plugin folder in the PluginSources directory.

The build system assumes that your plugin's Folder name and the containing .uplugin file have the same name - ie my */PluginSources/GradientspaceUEToolbox/* folder
contains **GradientspaceUEToolbox.uplugin**

Once that is done, the top-level script *build_all.bat* can be run to create binary builds. Do not do anything else while this script is running. The script *clean_all.bat* can
be run to delete all temporary build outputs.

If the script is successful, The */BUILDS/5p3* (etc) folders will be populated with (1) a PluginName_Version_Win64.zip file, (2) a PluginName_Version.txt file, and (3) a PluginName_CURRENTVERSION.txt file.
The version number is taken from the file *<PluginName>/Source/GS_VERSION_NUMBER.txt*, via the UEPluginPackager tool (which is run as part of the build).

In addition to the packaged build, a test project will be created at (eg) */build_5p3_test*, which will contain a sample project and the binary plugin, with .sln generated.
You can open/build/run this project to test that the built plugin functions correctly with that UE version.

Currently (Jan 2025) the scripts are set up to build the plugin 5.3, 5.4, and 5.5. Adding a version is a rote process but requires some effort.
Disabling a version is straightforward, simply comment/delete the relevant line from build_all.bat and clean_all.bat.

See below for more details on the build scripts. Errors handling or reporting is minimal - the assumption is that you have already sorted out compile issues/etc. 
If there are errors, you won't find a .zip file in the BUILD folder, or it will be very small, and you will have to investigate backwards
(running the per-version build_5pX.bat files in a command-line window is a good way to do that).

A precompiled version of UEPluginPackager is used for this build, it is included in the repo in the */BinaryPluginBuildWin64/UEPluginPackager/* subfolder. A script *update_from_vsproj_build.bat* in that
subfolder can be used to update this copy from the Debug build folder of the top-level C# project, ie if you rebuild that, run this script (only works w/ Debug version, not performance-sensitive). 

## Build Process

For a given engine version build file (eg build_5p3.bat) the process is:

* run build_clean.bat with "5p3" as the argument, which will delete any previous outputs of build_5p3.bat
* create a copy of the 5p3 top-level folder, into 5p3_build. This folder contains an empty sample UE project for that engine version, which "hosts" the build process
* copy the plugin source into the 5p3_build/Plugins/YourPluginName/ folder  (done via fetch_plugin.bat in the 5p3_build folder)
* run UnrealVersionSelector.exe on this folder to generate the .sln file
* call RunUAT.bat BuildPlugin to create a binary build of the plugin, in the folder 5p3_build_plugin_binarypkg
* run build_package.bat, which runs UEPluginPackager.exe on the 5p3_build_plugin_binarypkg folder to create the redistributable .zip and version files, and moves them to the BUILDS folder
* run build_test.bat to create a test project (based on the intial 5p3 project) containing the binary plugin in 5p3_build_test

The set_UE5_paths.bat script finds UE5 installation locations using Registry Keys. This uses the script get_reg_key_value.bat.

The set_plugin_config.bat script parses the PLUGIN_CONFIG.txt file and sets variables used by the script

The set_build_paths.bat script sets path variables for the various subfolders, for a given engine version string

build_package.bat, build_test.bat, and build_clean.bat all take an engine version string argument (eg "5p3", "5p4", etc). However the top-level scripts build_5p3.bat, etc, currently do not support a version-string argument, that's why there is a copy for each version (*something to improve in future*)


