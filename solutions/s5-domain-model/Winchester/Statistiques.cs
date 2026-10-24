namespace Winchester;

// Histoire 6 · Les comptes du soir (séance 3 : LINQ, fonctions pures)

// >>> MODELE
public static class Statistiques
{
    // Seules les commandes servies rapportent : celles dévorées en cuisine sont des pertes
    private static IEnumerable<Commande> Servies(IEnumerable<Commande> commandes) =>
        commandes.Where(c => c.Etat is Servie);

    public static Dents ChiffreDAffaires(IEnumerable<Commande> commandes) =>
        Servies(commandes).Aggregate(Dents.Zero, (total, c) => total + c.Total);

    public static string? PlatLePlusPopulaire(IEnumerable<Commande> commandes) =>
        Servies(commandes)
            .GroupBy(c => c.Plat.Nom)
            .MaxBy(groupe => groupe.Sum(c => c.Quantite))
            ?.Key;

    public static IReadOnlyDictionary<string, Dents> ChiffreDAffairesParSorteDeClient(IEnumerable<Commande> commandes) =>
        Servies(commandes)
            .GroupBy(c => c.Client.GetType().Name)
            .ToDictionary(groupe => groupe.Key, groupe => ChiffreDAffaires(groupe));

    public static Dents PertesEnCuisine(IEnumerable<Commande> commandes) =>
        commandes.Where(c => c.Etat is Devoree).Aggregate(Dents.Zero, (total, c) => total + c.Total);
}
// <<< MODELE
