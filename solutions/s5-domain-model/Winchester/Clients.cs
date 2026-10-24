using System.Diagnostics;

namespace Winchester;

// Histoire 3 · Chacun son régime (séance 2 : hiérarchie de records, pattern matching ; séance 3 : extension)

// >>> MODELE
// Une hiérarchie : chaque sorte de client est un type, pas une chaîne ou un booléen
public abstract record Client(string Nom);
public sealed record Rodeur(string Nom) : Client(Nom);
public sealed record ZombieVegan(string Nom) : Client(Nom);
public sealed record Survivant(string Nom) : Client(Nom);   // il imite très bien les zombies, mais il respire

public static class Regime
{
    // Une méthode d'extension (séance 3) : client.PeutManger(plat) se lit comme du français
    public static bool PeutManger(this Client client, Plat plat) => (client, plat.Categorie) switch
    {
        (Survivant, _) => false,
        (ZombieVegan, Categorie.ChouFleur) => true,
        (ZombieVegan, _) => false,
        (Rodeur, _) => true,
        // La hiérarchie n'est pas fermée pour le compilateur : il exige ce dernier cas (les union types le rendront inutile)
        _ => throw new UnreachableException($"Client inconnu : {client}")
    };
}
// <<< MODELE
