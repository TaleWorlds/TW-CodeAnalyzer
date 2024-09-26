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
##TaleworldsAnalyzer Attribute
## Analyzing Pipeline and Helper Classes
## Checking Preanalyze Conditions Before Creating any Diagnostics
### Blacklist Feature
### WhiteList Feature
### Analyzer Disabling by Comments Feature
### Custom Severity Feature
## Code Fixers
## Editor Commands
## Controller Window
## Building the Extension
## Installing the Extension 
## Contributors
