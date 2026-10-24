namespace Fundamentals;

/// <summary>
/// Le fameux blanc à compléter : remplace chaque <c>___</c> par la bonne valeur.
/// Tant qu'il reste un blanc, le test est rouge... et ne te souffle pas la réponse.
/// </summary>
public static class Blanks
{
    public static Blank ___ => throw new BlankNotFilledException();
}

/// <summary>Jamais instancié : les conversions permettent d'écrire <c>___</c> là où NFluent attend un string ou un Type.</summary>
public sealed class Blank
{
    private Blank() { }

    public static implicit operator string(Blank _) => throw new BlankNotFilledException();
    public static implicit operator Type(Blank _) => throw new BlankNotFilledException();
}

public sealed class BlankNotFilledException()
    : Exception("🧟 Il reste un ___ à compléter : quelle valeur attends-tu ici ? (la ligne est indiquée juste en dessous)");
