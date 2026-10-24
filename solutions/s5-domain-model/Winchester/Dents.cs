namespace Winchester;

// Histoire 1 · Le prix en dents (séance 2 : record struct, opérateurs ; séance 1 : exceptions)

// >>> MODELE
// Un VALUE OBJECT : une valeur, pas une identité. Deux prix de 12 dents sont le même prix.
// record struct : copié par valeur, égalité structurelle, pas d'allocation (séance 2).
public readonly record struct Dents
{
    public int Valeur { get; }

    public Dents(int valeur)
    {
        // Un prix négatif est un bug de l'appelant, pas un refus métier : une exception, tout de suite
        ArgumentOutOfRangeException.ThrowIfNegative(valeur);
        Valeur = valeur;
    }

    public static Dents Zero => new(0);

    public static Dents operator +(Dents gauche, Dents droite) => new(gauche.Valeur + droite.Valeur);

    public static Dents operator *(Dents prixUnitaire, int quantite) => new(prixUnitaire.Valeur * quantite);

    public override string ToString() => $"{Valeur} dents";
}
// <<< MODELE
