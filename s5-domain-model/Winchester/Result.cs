namespace Winchester;

// 🎁 Fourni : la boîte à outils pour dire « ça a marché » ou « non, et voici pourquoi » sans exception.
//
// Une exception, c'est pour un bug (un prix négatif). Un refus métier (Shaun est encore vivant)
// est un résultat normal : il apparaît dans le type de retour, et l'appelant ne peut pas l'ignorer.
// Java : pas d'équivalent standard ; on écrirait une sealed interface Result<T> permits Succes, Echec.

// Les erreurs du domaine dérivent de ce record : à toi de les écrire, une par refus.
public abstract record Erreur;

public abstract record Result<T>
{
    // Constructeur privé : seuls Succes et Echec, déclarés à l'intérieur, peuvent en hériter.
    // C'est une hiérarchie fermée « à la main » (les union types de C# 15 le feront pour nous).
    private Result() { }

    public sealed record Succes(T Valeur) : Result<T>;
    public sealed record Echec(Erreur Erreur) : Result<T>;

    // Les conversions implicites : on écrit « return commande; » ou « return new RuptureDeStock(...); »
    public static implicit operator Result<T>(T valeur) => new Succes(valeur);
    public static implicit operator Result<T>(Erreur erreur) => new Echec(erreur);

    // Ouvrir la boîte : un cas pour le succès, un cas pour l'échec
    public TSortie Match<TSortie>(Func<T, TSortie> succes, Func<Erreur, TSortie> echec) => this switch
    {
        Succes s => succes(s.Valeur),
        Echec e => echec(e.Erreur),
        _ => throw new InvalidOperationException("Ni succès, ni échec ?")
    };

    // Transformer la valeur, si elle est là (≈ Optional.map en Java)
    public Result<TSortie> Map<TSortie>(Func<T, TSortie> transformer) =>
        Match<Result<TSortie>>(valeur => transformer(valeur), erreur => erreur);

    // Enchaîner une étape qui peut elle-même échouer (≈ Optional.flatMap) : le premier échec court-circuite la suite
    public Result<TSortie> Bind<TSortie>(Func<T, Result<TSortie>> etapeSuivante) =>
        Match(etapeSuivante, erreur => erreur);

    // Select et SelectMany branchent Result<T> sur la syntaxe requête de LINQ (séance 3) :
    //   from commande in Commande.Passer(...) from enCuisine in commande.Preparer("Gégé") select enCuisine
    public Result<TSortie> Select<TSortie>(Func<T, TSortie> transformer) => Map(transformer);

    public Result<TSortie> SelectMany<TIntermediaire, TSortie>(
        Func<T, Result<TIntermediaire>> etapeSuivante, Func<T, TIntermediaire, TSortie> projeter) =>
        Bind(valeur => etapeSuivante(valeur).Map(intermediaire => projeter(valeur, intermediaire)));
}
