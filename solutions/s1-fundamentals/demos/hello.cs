#!/usr/bin/env dotnet
// Une app C# dans un seul fichier, sans .csproj (≈ `java Hello.java` depuis Java 11)
// Lancer :  dotnet run hello.cs
//   ou même : chmod +x hello.cs && ./hello.cs   (grâce au shebang ci-dessus)

var horde = args.Length > 0 ? int.Parse(args[0]) : 3;

for (var i = 1; i <= horde; i++)
    Console.WriteLine($"🧟 Zombie n°{i} : braaains !");

Console.WriteLine($"Runtime : {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
