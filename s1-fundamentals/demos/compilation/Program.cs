// 🔬 Démo : que devient notre C# une fois compilé ?
//
// 1. dotnet build                                  -> bin/Debug/net10.0/Compilation.dll (de l'IL, pas du code machine)
// 2. dotnet tool install -g ilspycmd               (une fois)
//    ilspycmd -il bin/Debug/net10.0/Compilation.dll   -> l'IL brut (≈ javap -c)
//    ilspycmd bin/Debug/net10.0/Compilation.dll       -> le C# "décompilé"
// 3. Ou sans rien installer : coller ce fichier dans https://sharplab.io
//    (menu "Results" : C# lowered, IL, JIT Asm)
// 4. Dans Rider : menu Tools > IL Viewer
//    - placez le curseur sur `Zombie` ou sur le `foreach` : la fenêtre IL Viewer se synchronise
//    - menu déroulant en haut de la fenêtre : "IL Code", "Low-Level C#" (le code "lowered"), "High-Level C#"
//    - le projet doit être compilé (Build) pour que l'IL soit à jour
//
// Questions à se poser :
//   - Combien de méthodes le compilateur a-t-il générées pour le record `Zombie` ? (Equals, GetHashCode, ToString, Deconstruct...)
//   - En quoi le `foreach` sur un tableau a-t-il été transformé ?
//   - Que devient l'interpolation $"..." ? (indice : DefaultInterpolatedStringHandler)

var horde = new[] { new Zombie("Bob", 3), new Zombie("Alice", 7) };

var total = 0;
foreach (var zombie in horde)
    total += zombie.Faim;

Console.WriteLine($"Faim totale de la horde : {total}");
Console.WriteLine(horde[0]); // ToString() généré par le record
Console.WriteLine(horde[0] == new Zombie("Bob", 3)); // égalité structurelle générée

record Zombie(string Nom, int Faim);
