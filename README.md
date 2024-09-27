# TaleWorlds Code Analysis Tool
## Introduction
This tool aims to enforce coding conventions that is determined by the TaleWorlds Entertainment. In the main part of the code the Roslyn API is used to analyze and assign a fixer to the diagnostics. The general structure of the code is based on a MVC-like pattern. As a base of the extension we have used XML files and analyzers by using the XML files. On the top of them , we have a controller and a command to control this extension.
## TO-DOs
* The maximum allowed depth may be controlled from controller window.
## Analyzers
The analyzers have been designed to be atomic as it should be. The explanations of them can be found in the following.
### Inheritance Related Code Analyzers
#### TW2100: Abstract Class Checker
This analyzer is responsible to check whether abstract class has only abstract methods or empty virtual methods.
#### TW2101: Depth of Inheritance Checker
This analyzer is responsible to check whether the class has exceeded the maximum allowed depth of inheritence or not. The maximum allowed depth can be adjustewd from the code.
#### TW2102: Sealed Override Checker
This analyzer is responsible from checking whether the overriden method is sealed or not. It should be sealed to prevent overriding them again.

### Naming Related Code Analyzers
In these type of analyzers, the purpose is reporting diagnostics if any violation is present. Alongside with that, we have tried to suggest some naming with these analyzers. 

#### Conventions: 
The naming related analyzers are using same conventions like PascalCase of camelCase. These conventions act slightly different from each other. Therefore, we have created a behaviour inheritance tree to create more managable and readable code.
Each analyzer acts differently but the main goal is checking whether the declared name is sutiable to its related convention. The table to demonstrate the ideal naming is the following: 

![image](https://github.com/user-attachments/assets/635d5e5d-b0c4-4984-9f94-53fe73595519)

#### TW2000: Class Name Checker
The classes should be named according to the _uscoreCase if it is internal or private. However, it should be named according to the PascalCase if it is public.

#### TW2002: Field Name Checker
The fields should be named according to the _uscorCase if it is private. 

#### TW2003: Interface Name Checker
The interfaces should be named according to the IPascalCase. 

#### TW2004: Local Name Checker
The variables that is declared in the local scope should be named according to camelCase.

#### TW2005: Method Name Checker
The methods should be named according to the _uscoreCase if it is internal or private. However, it should be named according to the PascalCase if it is public or protected.

#### TW2006: Parameter Name Checker
The paremeters should be named according to the camelCase.

#### TW2007: Property Name Checker
The properties should be named according to the _uscoreCase if it is internal or private. However, it should be named according to the PascalCase if it is public or protected.

#### TW2008: Template Parameter Name Checker
The template parameters should be named according to the TPascalCase


### Code Analyzers Related to Other Things

#### TW2001: Class Accessibility Checker
The classes should not have protected accessibility.

#### TW2200: Field Accessibility Checker
The fields should not have public, protected or internal accessibility.

#### TW2205: Immutable Struct Checker
This analyzer checks whether the structs with only one field is immutable or not. 

#### TW2202: Mixed Access Modifier Checker
This analyzer checks whether any declared property or method has mixed access modifiers or not. 

#### TW2204: Var Keyword Checker
This analyzer creates diagnostic if any local variable has explicit type. The var keyword should be rather than that.

#### TW2201: Named Parameter Checker
This analyzer reports diagnostic if any method call has more parameter than allowed argument threshold and these arguments does not specificaly named. 


### Test Cases
In the project there are unit tests fors analyzers and behviours. The external controllers hasn't been tested as unit test, yet.

## TaleworldsAnalyzer Attribute
The analyzers should have TaleworldsAnalyzer Attribute. Thanks to this attrbiute, the new analyzers can be added without any changes. The analyzers that have this attribute is found by FindAnalyzers class. This class uses reflection to find all Taleworlds Analyzers.

## Analyzing Pipeline and Helper Classes
The analyzing pipeline has several steps, you can find specific informations regarding to the steps as document goes. The first thing the program do is determining whether the analyzing is needed or not. The PreAnalyzerConditions class is used to create such control. The inheritance and other checkers category directly jumps to the analyzing logic. However, the name checkers need to check the white list that is added by the user. These white listed words will be ignored in the analyzing logic.

## Checking Preanalyze Conditions Before Creating any Diagnostics
The PreAnalyzerConditions class is used to create such checks. The analyzing is not needed if the project is blacklisted, the context has related disabling comment on the code, or the severity of related diagnostic is adjusted as none. 

### Blacklist Feature
The user can add the project to the local blacklist to ignore it. The user needs to press right mouse click on the project then an option appears as Add to the BlackList. User can press this.

### WhiteList Feature
The user can add some words to the white list. Whitelist control can be done via controller window. Also name checkers has codefixes about adding to the white list. White listed words will be ignored in the name checkers. There are 2 white list which are local and shared. Local is stored in appdata of the computer, while the shared one is stored in the project path. 

### Analyzer Disabling by Comments Feature
The user can write disabling and enabling comments. "//TWCodeAnalysis disable {Insert Diagnostic ID Here}" disables the written diagnostic until it is enabled again by writing "//TWCodeAnalysis enable {Insert Diagnostic ID Here}". similarly all diagnostics can be disabled by writing "//TWCodeAnalysis disable all", and can be enabled by writing "//TWCodeAnalysis disable all".  In addition to that, the user can disable the diagnostic for only one line by using  "//TWCodeAnalysis disable next line all" and  "//TWCodeAnalysis disable next line {Insert Diagnostic ID Here}"

### Custom Severity Feature
The user can set custom severity for each diagnostic by using the controller window. These settings can be found in the solution path. 

## Code Fixers
There are two code fixer provider in the project.

### Comment Disable Fix Provider
In this fix provider, the user can add disabling comments without writing them directly. However, The enable command should be written directly. 

### Name Checker Fix Provider
In this fix provider, the user can add forbidden part of the declaration to the local or shared white list. 

## Editor Commands
There are 2 editor commands, one of them already stated in the black list feature. The second one is for openning controller window. The controller can be found in the View -> Other Windows -> Taleworlds Code Analysis.

## Controller Window
The controller has several functionality to create ease of use for the extension. There is a refresh button which refreshes the analyzers and settings. Next, there are presets for all analyzers, but if the user wants, he/she can set different severity for each analyzer. After adjusting the severities, the user should press save button to apply them. Furthermore, there is a white list panel to control white listed items. White listed words can be controlled from here.

## Building the Extension
The extension has several different components. The first two component is controller window and blacklist command. These two component is connected to the main analyzer vsix file. Therefore you need to build them all to create up to date vsix file.

## Installing the Extension 
After building the project, there will be one extension called TaleworldsCodeAnalysis.vsix. This the only vsix file to install it directly to the visual studio. 

## Contributors
Berkay Karakaya - Arınç Demir with supervises of Oğuzhan Şirin
