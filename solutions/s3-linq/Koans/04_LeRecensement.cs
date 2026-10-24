namespace Fonctionnel;

// 🎯 Objectif : le recensement de la horde. Le QG a besoin de réponses : écris chaque requête avec LINQ.
//
// Les données sont dans Domain.cs (Registre.Zombies, Registre.Survivants, Registre.Camps, Registre.Rencontres).
// Les questions sont de difficulté croissante. Chaque méthode tient en une expression LINQ.
//
//   Java Streams                         LINQ
//   filter / map / flatMap               Where / Select / SelectMany
//   sorted(comparing(...))               OrderBy / ThenBy
//   anyMatch / allMatch                  Any / All
//   collect(groupingBy(...))             GroupBy
//   reduce                               Aggregate
//   max(comparing(...))                  MaxBy
//   limit / skip                         Take / Skip
//   collect(toList())                    ToList
public class LeRecensement
{
    // 1. Les noms des zombies du « Centre-ville », dans l'ordre du registre.
    private static List<string> NomsDuCentreVille()
    {
        // >>> SOLUTION
        return Registre.Zombies
            .Where(z => z.Quartier == "Centre-ville")
            .Select(z => z.Nom)
            .ToList();
        // <<< SOLUTION
    }

    [Fact]
    public void Q01_Les_zombies_du_centre_ville()
    {
        Check.That(NomsDuCentreVille()).ContainsExactly("Bob", "Charlie", "Lulu", "Nina");
    }

    // 2. Les noms de la horde, du plus affamé au moins affamé ; à faim égale, par ordre alphabétique.
    private static List<string> ParFaimDecroissante()
    {
        // >>> SOLUTION
        return Registre.Zombies
            .OrderByDescending(z => z.Faim)
            .ThenBy(z => z.Nom)
            .Select(z => z.Nom)
            .ToList();
        // <<< SOLUTION
    }

    [Fact]
    public void Q02_La_horde_triee_par_faim()
    {
        Check.That(ParFaimDecroissante())
            .ContainsExactly("Big Mama", "Alice", "Nina", "Lulu", "Bob", "Gégé", "Oscar", "Charlie", "Marcel");
    }

    // 3a. Y a-t-il au moins un Tank dans la horde ?
    private static bool IlYAUnTank()
    {
        // >>> SOLUTION
        return Registre.Zombies.Any(z => z.Type == TypeDeZombie.Tank);
        // <<< SOLUTION
    }

    // 3b. Tous les survivants sont-ils majeurs (18 ans ou plus) ?
    private static bool TousMajeurs()
    {
        // >>> SOLUTION
        return Registre.Survivants.All(s => s.Age >= 18);
        // <<< SOLUTION
    }

    [Fact]
    public void Q03_Les_questions_oui_non()
    {
        Check.That(IlYAUnTank()).IsTrue();
        Check.That(TousMajeurs()).IsFalse(); // Carl n'a que 16 ans
    }

    // 4. Le nombre de zombies de chaque type.
    //    Indice : GroupBy puis ToDictionary... ou directement CountBy (.NET 9).
    private static Dictionary<TypeDeZombie, int> NombreParType()
    {
        // >>> SOLUTION
        return Registre.Zombies
            .CountBy(z => z.Type)
            .ToDictionary();
        // <<< SOLUTION
    }

    [Fact]
    public void Q04_Le_nombre_de_zombies_par_type()
    {
        var nombreParType = NombreParType();

        Check.That(nombreParType[TypeDeZombie.Rodeur]).IsEqualTo(4);
        Check.That(nombreParType[TypeDeZombie.Coureur]).IsEqualTo(2);
        Check.That(nombreParType[TypeDeZombie.Goule]).IsEqualTo(2);
        Check.That(nombreParType[TypeDeZombie.Tank]).IsEqualTo(1);
    }

    // 5. Pour chaque quartier : sa faim moyenne et le nom de son zombie le plus affamé.
    private static List<(string Quartier, double FaimMoyenne, string LePlusAffame)> BilanParQuartier()
    {
        // >>> SOLUTION
        return Registre.Zombies
            .GroupBy(z => z.Quartier)
            .Select(g => (g.Key, g.Average(z => z.Faim), g.MaxBy(z => z.Faim)!.Nom))
            .ToList();
        // <<< SOLUTION
    }

    [Fact]
    public void Q05_Le_bilan_par_quartier()
    {
        var bilan = BilanParQuartier().ToDictionary(b => b.Quartier);

        Check.That(bilan.Count).IsEqualTo(3);
        Check.That(bilan["Centre-ville"].FaimMoyenne).IsEqualTo(6.75);
        Check.That(bilan["Centre-ville"].LePlusAffame).IsEqualTo("Nina");
        Check.That(bilan["Gare"].FaimMoyenne).IsEqualTo(6.0);
        Check.That(bilan["Port"].LePlusAffame).IsEqualTo("Alice");
    }

    // 6. Toutes les compétences des survivants, sans doublon, dans l'ordre où on les rencontre.
    private static List<string> ToutesLesCompetences()
    {
        // >>> SOLUTION
        return Registre.Survivants
            .SelectMany(s => s.Competences)
            .Distinct()
            .ToList();
        // <<< SOLUTION
    }

    [Fact]
    public void Q06_Les_competences_disponibles()
    {
        Check.That(ToutesLesCompetences())
            .ContainsExactly("Leadership", "Tir", "Sabre", "Pistage", "Chasse", "Agriculture", "Éclaireur");
    }

    // 7a. Le total des munitions du camp « Alexandria », avec Sum (les survivants sans arme ne comptent pas).
    private static int MunitionsAvecSum()
    {
        // >>> SOLUTION
        return Registre.Survivants
            .Where(s => s.Camp == "Alexandria")
            .Sum(s => s.Arme?.Munitions ?? 0);
        // <<< SOLUTION
    }

    // 7b. Le même total, mais avec Aggregate (≈ reduce).
    private static int MunitionsAvecAggregate()
    {
        // >>> SOLUTION
        return Registre.Survivants
            .Where(s => s.Camp == "Alexandria")
            .Aggregate(0, (total, s) => total + (s.Arme?.Munitions ?? 0));
        // <<< SOLUTION
    }

    [Fact]
    public void Q07_Les_munitions_d_Alexandria()
    {
        Check.That(MunitionsAvecSum()).IsEqualTo(18);
        Check.That(MunitionsAvecAggregate()).IsEqualTo(18);
    }

    // 8. Le nom du survivant qui fait le plus de dégâts. Attention : Maggie n'a pas d'arme.
    private static string LeMieuxArme()
    {
        // >>> SOLUTION
        return Registre.Survivants.MaxBy(s => s.Arme?.Degats ?? 0)!.Nom;
        // <<< SOLUTION
    }

    [Fact]
    public void Q08_Le_survivant_le_mieux_arme()
    {
        Check.That(LeMieuxArme()).IsEqualTo("Michonne");
    }

    // 9a. Chaque survivant avec la ville de son camp.
    //     Indice : Join(autreSource, cléGauche, cléDroite, (gauche, droite) => ...)
    private static List<(string Survivant, string Ville)> VilleDeChaqueSurvivant()
    {
        // >>> SOLUTION
        return Registre.Survivants
            .Join(Registre.Camps, s => s.Camp, c => c.Nom, (s, c) => (s.Nom, c.Ville))
            .ToList();
        // <<< SOLUTION
    }

    // 9b. Les noms des camps qui hébergent plus de survivants que leur capacité.
    private static List<string> CampsSurpeuples()
    {
        // >>> SOLUTION
        return Registre.Camps
            .Where(c => Registre.Survivants.Count(s => s.Camp == c.Nom) > c.Capacite)
            .Select(c => c.Nom)
            .ToList();
        // <<< SOLUTION
    }

    [Fact]
    public void Q09_Les_survivants_et_leurs_camps()
    {
        var villes = VilleDeChaqueSurvivant().ToDictionary(v => v.Survivant, v => v.Ville);

        Check.That(villes.Count).IsEqualTo(6);
        Check.That(villes["Daryl"]).IsEqualTo("Virginie");
        Check.That(villes["Rick"]).IsEqualTo("Washington");
        Check.That(CampsSurpeuples()).ContainsExactly("Alexandria");
    }

    // 10. Le bilan des rencontres de chaque survivant : nombre de victoires, de fuites et de morsures.
    //     Les survivants sans rencontre n'apparaissent pas.
    private static Dictionary<string, (int Victoires, int Fuites, int Morsures)> BilanDesRencontres()
    {
        // >>> SOLUTION
        return Registre.Rencontres
            .GroupBy(r => r.Survivant)
            .ToDictionary(
                g => g.Key,
                g => (g.Count(r => r.Issue == Issue.Victoire),
                      g.Count(r => r.Issue == Issue.Fuite),
                      g.Count(r => r.Issue == Issue.Morsure)));
        // <<< SOLUTION
    }

    [Fact]
    public void Q10_Le_bilan_des_rencontres()
    {
        var bilan = BilanDesRencontres();

        Check.That(bilan.Count).IsEqualTo(5);
        Check.That(bilan["Michonne"]).IsEqualTo((2, 0, 0));
        Check.That(bilan["Rick"]).IsEqualTo((1, 1, 0));
        Check.That(bilan["Carl"]).IsEqualTo((0, 0, 1));
        Check.That(bilan.ContainsKey("Maggie")).IsFalse();
    }

    // 11. Les tours de garde : la horde est découpée en escouades de 3 zombies (dans l'ordre du registre),
    //     et chaque garde surveille une escouade. Indice : Chunk, puis Zip.
    private static List<(string Garde, string[] Escouade)> ToursDeGarde(string[] gardes)
    {
        // >>> SOLUTION
        return gardes
            .Zip(Registre.Zombies.Chunk(3), (garde, escouade) => (garde, escouade.Select(z => z.Nom).ToArray()))
            .ToList();
        // <<< SOLUTION
    }

    [Fact]
    public void Q11_Les_tours_de_garde()
    {
        var tours = ToursDeGarde(["Rick", "Daryl", "Michonne"]);

        Check.That(tours.Count).IsEqualTo(3);
        Check.That(tours[1].Garde).IsEqualTo("Daryl");
        Check.That(tours[1].Escouade).ContainsExactly("Big Mama", "Gégé", "Lulu");
        Check.That(tours[2].Escouade).ContainsExactly("Marcel", "Nina", "Oscar");
    }

    // 12a. Le nom du tout premier zombie transformé.
    private static string LePremierTransforme()
    {
        // >>> SOLUTION
        return Registre.Zombies.MinBy(z => z.TransformeLe)!.Nom;
        // <<< SOLUTION
    }

    // 12b. Les noms des 3 zombies transformés le plus récemment, du plus récent au plus ancien.
    private static List<string> LesTroisPlusRecents()
    {
        // >>> SOLUTION
        return Registre.Zombies
            .OrderByDescending(z => z.TransformeLe)
            .Take(3)
            .Select(z => z.Nom)
            .ToList();
        // <<< SOLUTION
    }

    [Fact]
    public void Q12_La_chronologie_de_l_epidemie()
    {
        Check.That(LePremierTransforme()).IsEqualTo("Marcel");
        Check.That(LesTroisPlusRecents()).ContainsExactly("Lulu", "Big Mama", "Oscar");
    }
}
