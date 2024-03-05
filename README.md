# RepairerSerializeReferences
Editor Window to fix Serialize Reference after changing/renaming a class, assembly, or namespace.

[Документация на русском](Documentation~/RU.md)

## Add to project
To add this package to the project, follow these steps:
1) Open PackageManager;
2) Select "Add package from get URL";
3) Insert links to packages that are dependencies of this package:
    + `https://github.com/Paulsams/MiscUtilities.git`
4) Insert this link `https://github.com/Paulsams/RepairerSerializeReferences.git`

## Dependencies
- Is using:
    + MicsUtilities: https://github.com/Paulsams/MiscUtilities.git

## Opportunities
1) You can view each lost link individually (both in prefabs and on the stage);
2) Allows you to fix all links at once ("Update All" switch);
3) Allows you to change only to the Type that exists in a given assembly and in a given namespace.

![image](Documentation~/RepairerWindow.gif)

## How to use
This window can be opened in the top menu along the path `Window/RepairerSerializeReference`, and then you must click the "Update" button yourself, otherwise there will be no update.