# RepairerSerializeReferences
Editor Window to fix Serialize Reference after changing/renaming a class, assembly, or namespace.

[Документация на русском](https://github.com/Paulsams/RepairerSerializeReferences/blob/master/Documentation~/RU.md)

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

## Known issues
1) I had to make it so that the user had to press the "Update" button himself, because if you have a problem that there will be a prefab and there will be a prefab instance on any stage, and you changed SerializeReference to some other reference in contrast to the prefab, then with my algorithm - Unity will try to open the scene, but will encounter throwing its internal exception at which it still makes an AssetDatabase.Refresh(), which causes my window to update and if "Update" was automatic, there would be an infinite recursion.

## Opportunities
1) You can view each lost link individually (both in prefabs and on stage);
2) Allows you to fix all links at once (toggle "update all");
3) Allows you to change only to the type that exists in this assembly and for this namespace.

![image](https://github.com/Paulsams/RepairerSerializeReferences/blob/master/Documentation~/RepairerWindow.gif)

## How to use
This window can be opened in the top menu along the path `Tools/RepairerSerializeReference`, and then you must click the "Update" button yourself, otherwise there will be no update (described in more detail in point 1 in "Known problems").
