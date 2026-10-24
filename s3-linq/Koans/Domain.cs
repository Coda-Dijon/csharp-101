namespace Fonctionnel;

// 🗺️ Le registre de la horde : le QG des survivants a tout noté. Des records de la séance 2, rien de plus.

public enum TypeDeZombie
{
    Rodeur,
    Coureur,
    Goule,
    Tank,
}

public enum Issue
{
    Fuite,
    Victoire,
    Morsure,
}

public record Zombie(string Nom, TypeDeZombie Type, int Faim, string Quartier, DateOnly TransformeLe);

public record Arme(string Nom, int Degats, int Munitions);

public record Survivant(string Nom, int Age, string Camp, IReadOnlyList<string> Competences, Arme? Arme);

public record Camp(string Nom, string Ville, int Capacite);

public record Rencontre(string Survivant, string Zombie, DateOnly Date, Issue Issue);

public static class Registre
{
    public static IReadOnlyList<Zombie> Zombies { get; } =
    [
        new("Bob", TypeDeZombie.Rodeur, 7, "Centre-ville", new(2026, 1, 12)),
        new("Alice", TypeDeZombie.Coureur, 9, "Port", new(2026, 2, 3)),
        new("Charlie", TypeDeZombie.Rodeur, 3, "Centre-ville", new(2025, 12, 24)),
        new("Big Mama", TypeDeZombie.Tank, 10, "Gare", new(2026, 3, 15)),
        new("Gégé", TypeDeZombie.Goule, 6, "Port", new(2026, 1, 30)),
        new("Lulu", TypeDeZombie.Coureur, 8, "Centre-ville", new(2026, 4, 1)),
        new("Marcel", TypeDeZombie.Rodeur, 2, "Gare", new(2025, 11, 2)),
        new("Nina", TypeDeZombie.Goule, 9, "Centre-ville", new(2026, 2, 14)),
        new("Oscar", TypeDeZombie.Rodeur, 5, "Port", new(2026, 3, 1)),
    ];

    public static IReadOnlyList<Survivant> Survivants { get; } =
    [
        new("Rick", 45, "Alexandria", ["Leadership", "Tir"], new Arme("Colt Python", 40, 6)),
        new("Michonne", 35, "Alexandria", ["Sabre", "Pistage"], new Arme("Katana", 50, 0)),
        new("Carl", 16, "Alexandria", ["Tir"], new Arme("Pistolet", 25, 12)),
        new("Daryl", 40, "Hilltop", ["Pistage", "Chasse", "Tir"], new Arme("Arbalète", 45, 20)),
        new("Maggie", 30, "Hilltop", ["Agriculture", "Leadership"], null),
        new("Glenn", 28, "Kingdom", ["Éclaireur", "Pistage"], new Arme("Batte", 15, 0)),
    ];

    public static IReadOnlyList<Camp> Camps { get; } =
    [
        new("Alexandria", "Washington", 2),
        new("Hilltop", "Virginie", 5),
        new("Kingdom", "Virginie", 10),
    ];

    public static IReadOnlyList<Rencontre> Rencontres { get; } =
    [
        new("Rick", "Bob", new(2026, 4, 2), Issue.Victoire),
        new("Rick", "Big Mama", new(2026, 4, 3), Issue.Fuite),
        new("Michonne", "Alice", new(2026, 4, 2), Issue.Victoire),
        new("Michonne", "Nina", new(2026, 4, 5), Issue.Victoire),
        new("Carl", "Charlie", new(2026, 4, 4), Issue.Morsure),
        new("Daryl", "Gégé", new(2026, 4, 1), Issue.Victoire),
        new("Daryl", "Oscar", new(2026, 4, 6), Issue.Fuite),
        new("Glenn", "Lulu", new(2026, 4, 7), Issue.Morsure),
    ];
}
