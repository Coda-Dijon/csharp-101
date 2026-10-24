namespace Winchester;

// Histoire 7 · Le rush de minuit (séance 4 : async, SemaphoreSlim, IAsyncEnumerable)

// >>> MODELE
// La COQUILLE autour du cœur : elle lit l'horloge, garde le stock, protège les accès concurrents.
// Toutes les décisions restent dans Commande.Passer, une fonction pure.
public sealed class Comptoir(IReadOnlyDictionary<string, int> stockInitial, Func<DateOnly> horloge)
{
    private readonly Dictionary<string, int> _stock = new(stockInitial);
    private readonly List<Commande> _commandes = [];

    // lock interdit await à l'intérieur : SemaphoreSlim(1, 1) est le verrou des méthodes async
    private readonly SemaphoreSlim _verrou = new(1, 1);

    public int Restant(string plat) => _stock.GetValueOrDefault(plat);

    public IReadOnlyList<Commande> Commandes => _commandes.AsReadOnly();

    public async Task<Result<Commande>> PrendreAsync(Client client, Plat plat, int quantite)
    {
        await _verrou.WaitAsync();
        try
        {
            var restant = Restant(plat.Nom);
            await ConsulterLeCarnet();   // une E/S simulée : sans verrou, une autre commande se glisse ici

            var resultat = Commande.Passer(client, plat, quantite, restant, horloge());
            if (resultat is Result<Commande>.Succes { Valeur: var commande })
            {
                _stock[plat.Nom] = restant - quantite;
                _commandes.Add(commande);
            }
            return resultat;
        }
        finally
        {
            _verrou.Release();
        }
    }

    // La horde arrive en flux : on sert au fil de l'eau, sans attendre la fin de la file
    public async IAsyncEnumerable<Result<Commande>> ServirLaFileAsync(
        IAsyncEnumerable<(Client Client, Plat Plat, int Quantite)> file)
    {
        await foreach (var (client, plat, quantite) in file)
            yield return await PrendreAsync(client, plat, quantite);
    }

    private static Task ConsulterLeCarnet() => Task.Delay(1);
}
// <<< MODELE
