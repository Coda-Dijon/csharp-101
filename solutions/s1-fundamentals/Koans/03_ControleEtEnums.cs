namespace Fundamentals;

// 🎯 Objectif : if, boucles, switch... et enums, qui sont bien plus simples qu'en Java.
public enum Etat
{
    Vivant,
    Mordu,
    Zombie,
}

[Flags]
public enum Sens
{
    Aucun = 0,
    Odorat = 1,
    Ouie = 2,
    Vue = 4,
}

public class AProposDuControleEtDesEnums
{
    [Fact]
    public void If_est_une_instruction_mais_le_ternaire_est_une_expression()
    {
        var pointsDeVie = 0;

        var etat = pointsDeVie > 0 ? "debout" : "à terre";

        Check.That(etat).IsEqualTo(/*__*/"à terre"/*__*/);
    }

    [Fact]
    public void Foreach_parcourt_tout_ce_qui_est_enumerable()
    {
        // Java : for (String nom : noms)
        string[] noms = ["Bob", "Alice", "Charlie"];
        var initiales = "";

        foreach (var nom in noms)
            initiales += nom[0];

        Check.That(initiales).IsEqualTo(/*__*/"BAC"/*__*/);
    }

    [Fact]
    public void Break_et_continue_comme_en_Java()
    {
        var somme = 0;
        for (var i = 1; i <= 10; i++)
        {
            if (i % 2 == 0) continue;
            if (i > 7) break;
            somme += i;
        }

        Check.That(somme).IsEqualTo(/*__*/16/*__*/); // 1 + 3 + 5 + 7
    }

    [Fact]
    public void Le_switch_classique_interdit_le_fall_through()
    {
        // Java : oublier un break fait "tomber" dans le case suivant
        // C#   : erreur de compilation ! Chaque section non vide doit finir par break/return/throw
        static string Decrire(int jour)
        {
            switch (jour)
            {
                case 6:
                case 7: // plusieurs labels empilés : autorisé car la section est vide
                    return "week-end";
                default:
                    return "semaine";
            }
        }

        Check.That(Decrire(6)).IsEqualTo(/*__*/"week-end"/*__*/);
        Check.That(Decrire(3)).IsEqualTo(/*__*/"semaine"/*__*/);
    }

    [Fact]
    public void Le_switch_expression_renvoie_une_valeur()
    {
        // Java 14+ : switch (x) { case ... -> ... }  /  C# 8+ : x switch { ... => ... }
        static string Danger(int zombies) => zombies switch
        {
            0 => "calme",
            < 5 => "inquiétant",
            >= 5 and < 50 => "horde",
            _ => "fin du monde", // _ = default
        };

        Check.That(Danger(0)).IsEqualTo(/*__*/"calme"/*__*/);
        Check.That(Danger(12)).IsEqualTo(/*__*/"horde"/*__*/);
        Check.That(Danger(1000)).IsEqualTo(/*__*/"fin du monde"/*__*/);
    }

    [Fact]
    public void Les_enums_sont_des_entiers_nommes()
    {
        // Java : un enum est une classe (champs, méthodes...)
        // C#   : un enum est un simple entier avec des noms, c'est plus léger mais plus pauvre
        Check.That((int)Etat.Zombie).IsEqualTo(/*__*/2/*__*/);
        Check.That(Etat.Mordu.ToString()).IsEqualTo(/*__*/"Mordu"/*__*/);
        Check.That(Enum.Parse<Etat>("Zombie")).IsEqualTo(/*__*/Etat.Zombie/*__*/);
    }

    [Fact]
    public void Un_enum_peut_contenir_une_valeur_non_declaree()
    {
        // ⚠️ Rien n'empêche de caster n'importe quel entier : pensez toujours au `_` dans un switch
        var bizarre = (Etat)42;

        Check.That(Enum.IsDefined(bizarre)).IsEqualTo(/*__*/false/*__*/);
    }

    [Fact]
    public void Les_flags_combinent_plusieurs_valeurs()
    {
        // Java : EnumSet.of(ODORAT, OUIE)  /  C# : des bits combinés avec |
        var sensDuZombie = Sens.Odorat | Sens.Ouie;

        Check.That((int)sensDuZombie).IsEqualTo(/*__*/3/*__*/);
        Check.That(sensDuZombie.HasFlag(Sens.Ouie)).IsEqualTo(/*__*/true/*__*/);
        Check.That(sensDuZombie.HasFlag(Sens.Vue)).IsEqualTo(/*__*/false/*__*/);
    }

    // 🛠 À toi de coder : fais évoluer l'état d'une personne mordue (ou pas) avec un switch expression.
    //    Vivant + morsure -> Mordu ; Mordu -> Zombie (morsure ou pas) ; Zombie reste Zombie ; Vivant sans morsure reste Vivant
    private static Etat Evoluer(Etat etat, bool morsure)
    {
        // >>> SOLUTION
        return (etat, morsure) switch
        {
            (Etat.Vivant, true) => Etat.Mordu,
            (Etat.Vivant, false) => Etat.Vivant,
            (Etat.Mordu, _) => Etat.Zombie,
            (Etat.Zombie, _) => Etat.Zombie,
            _ => throw new ArgumentOutOfRangeException(nameof(etat)),
        };
        // <<< SOLUTION
    }

    [Fact]
    public void On_sait_faire_evoluer_un_etat()
    {
        Check.That(Evoluer(Etat.Vivant, morsure: false)).IsEqualTo(Etat.Vivant);
        Check.That(Evoluer(Etat.Vivant, morsure: true)).IsEqualTo(Etat.Mordu);
        Check.That(Evoluer(Etat.Mordu, morsure: false)).IsEqualTo(Etat.Zombie);
        Check.That(Evoluer(Etat.Zombie, morsure: true)).IsEqualTo(Etat.Zombie);
    }

    // 🛠 À toi de coder : FizzBuzz version zombie, avec un switch sur un tuple.
    //    multiple de 3 -> "Brains", de 5 -> "Braaains", des deux -> "BrainsBraaains", sinon le nombre
    private static string FizzBuzzZombie(int n)
    {
        // >>> SOLUTION
        return (n % 3, n % 5) switch
        {
            (0, 0) => "BrainsBraaains",
            (0, _) => "Brains",
            (_, 0) => "Braaains",
            _ => n.ToString(),
        };
        // <<< SOLUTION
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(3, "Brains")]
    [InlineData(10, "Braaains")]
    [InlineData(30, "BrainsBraaains")]
    public void FizzBuzz_version_zombie(int n, string attendu)
    {
        Check.That(FizzBuzzZombie(n)).IsEqualTo(attendu);
    }
}
