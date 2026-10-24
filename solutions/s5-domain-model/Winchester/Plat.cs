namespace Winchester;

// Histoire 2 · Un cerveau frais (séance 2 : record, enum ; séance 1 : expressions)

// >>> MODELE
public enum Categorie { Cerveau, ChouFleur }

// Le plat ne lit pas l'horloge : on lui passe « aujourd'hui ». Une fonction pure se teste sans mentir sur la date.
public sealed record Plat(string Nom, Categorie Categorie, Dents Prix, DateOnly PrepareLe)
{
    public const int JoursDeConservation = 3;

    public int Age(DateOnly aujourdhui) => aujourdhui.DayNumber - PrepareLe.DayNumber;

    public bool EstPerime(DateOnly aujourdhui) => Age(aujourdhui) > JoursDeConservation;
}
// <<< MODELE
