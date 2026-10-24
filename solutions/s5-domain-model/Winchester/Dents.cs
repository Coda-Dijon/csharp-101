namespace Winchester;

// Histoire 1 · Le prix en dents (séance 2 : record struct, constructeur privé, opérateurs ; Result<T>)

// >>> MODELE
public sealed record PrixNegatif(int Valeur) : Erreur;

// Un VALUE OBJECT : une valeur, pas une identité. Deux prix de 12 dents sont le même prix.
// record struct : copié par valeur, égalité structurelle, pas d'allocation (séance 2).
public readonly record struct Dents
{
    public int Valeur { get; }

    // ❌ Un constructeur public qui lance une exception cacherait que la création peut échouer :
    //      public Dents(int valeur) { ArgumentOutOfRangeException.ThrowIfNegative(valeur); ... }
    //    rien dans « new Dents(-1) » ne prévient l'appelant, il le découvre à l'exécution.
    // ✅ Le constructeur est privé, et une FACTORY METHOD dit dans sa signature que ça peut échouer.
    private Dents(int valeur) => Valeur = valeur;

    public static Result<Dents> Creer(int valeur) =>
        valeur < 0 ? new PrixNegatif(valeur) : new Dents(valeur);

    public static Dents Zero => new(0);

    // Ces opérateurs ne peuvent pas produire un prix négatif : ils utilisent le constructeur privé sans risque.
    // uint : le type lui-même interdit une quantité négative (Java n'a pas d'entiers non signés).
    public static Dents operator +(Dents gauche, Dents droite) => new(gauche.Valeur + droite.Valeur);

    public static Dents operator *(Dents prixUnitaire, uint quantite) => new(prixUnitaire.Valeur * (int)quantite);

    public override string ToString() => $"{Valeur} dents";
}
// <<< MODELE
