// 🔬 Démo : que devient notre C# une fois compilé ?
//
// 1. dotnet build                                  -> bin/Debug/net10.0/Compilation.dll (de l'IL, pas du code machine)
// 2. dotnet tool install -g ilspycmd               (une fois)
//    ilspycmd -il bin/Debug/net10.0/Compilation.dll   -> l'IL brut (≈ javap -c)
//    ilspycmd bin/Debug/net10.0/Compilation.dll       -> le C# "décompilé"
// 3. Ou sans rien installer : coller ce fichier dans https://sharplab.io
//    (menu "Results" : C# lowered, IL, JIT Asm)
// 4. Dans Rider : menu Tools > IL Code (ou Search Everywhere ⇧⇧ puis "IL Viewer")
//    - placez le curseur sur un `foreach` : la fenêtre IL Viewer se synchronise
//    - menu déroulant en haut de la fenêtre : "IL Code", "Low-Level C#" (le code "lowered"), "High-Level C#"
//    - le projet doit être compilé (Build) pour que l'IL soit à jour
//
// Questions à se poser :
//   - Que devient `var` dans l'IL ? (indice : le type est écrit en toutes lettres)
//   - En quoi le `foreach` sur un tableau a-t-il été transformé ?
//   - Que devient l'interpolation $"..." ? (indice : DefaultInterpolatedStringHandler)
//   - Que reste-t-il de la constante `Cri` et de sa concaténation ?

string[] horde = ["Bob", "Alice", "Charlie"];
int[] faims = [3, 7, 2];

var total = 0;
foreach (var faim in faims)
    total += faim;

foreach (var nom in horde)
    Console.WriteLine($"{nom} a faim");

const string Cri = "Braaa" + "ains"; // évaluée à la compilation
Console.WriteLine($"Faim totale de la horde : {total}. {Cri} !");
