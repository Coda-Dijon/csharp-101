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

        Check.That(etat).IsEqualTo(___);
    }

    [Fact]
    public void Foreach_parcourt_tout_ce_qui_est_enumerable()
    {
        // Java : for (String nom : noms)
        string[] noms = ["Bob", "Alice", "Charlie"];
        var initiales = "";

        foreach (var nom in noms)
            initiales += nom[0];

        Check.That(initiales).IsEqualTo(___);
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

        Check.That(somme).IsEqualTo(___); // 1 + 3 + 5 + 7
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

        Check.That(Decrire(6)).IsEqualTo(___);
        Check.That(Decrire(3)).IsEqualTo(___);
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

        Check.That(Danger(0)).IsEqualTo(___);
        Check.That(Danger(12)).IsEqualTo(___);
        Check.That(Danger(1000)).IsEqualTo(___);
    }

    [Fact]
    public void Les_enums_sont_des_entiers_nommes()
    {
        // Java : un enum est une classe (champs, méthodes...)
        // C#   : un enum est un simple entier avec des noms, c'est plus léger mais plus pauvre
        Check.That((int)Etat.Zombie).IsEqualTo(___);
        Check.That(Etat.Mordu.ToString()).IsEqualTo(___);
        Check.That(Enum.Parse<Etat>("Zombie")).IsEqualTo(___);
    }

    [Fact]
    public void Un_enum_peut_contenir_une_valeur_non_declaree()
    {
        // ⚠️ Rien n'empêche de caster n'importe quel entier : pensez toujours au `_` dans un switch
        var bizarre = (Etat)42;

        Check.That(Enum.IsDefined(bizarre)).IsEqualTo(___);
    }

    [Fact]
    public void Les_flags_combinent_plusieurs_valeurs()
    {
        // Java : EnumSet.of(ODORAT, OUIE)  /  C# : des bits combinés avec |
        var sensDuZombie = Sens.Odorat | Sens.Ouie;

        Check.That((int)sensDuZombie).IsEqualTo(___);
        Check.That(sensDuZombie.HasFlag(Sens.Ouie)).IsEqualTo(___);
        Check.That(sensDuZombie.HasFlag(Sens.Vue)).IsEqualTo(___);
    }

    // 🛠 À toi de coder : fais évoluer l'état d'une personne mordue (ou pas) avec un switch expression.
    //    Vivant + morsure -> Mordu ; Mordu -> Zombie (morsure ou pas) ; Zombie reste Zombie ; Vivant sans morsure reste Vivant
    private static Etat Evoluer(Etat etat, bool morsure)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
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
        throw new NotImplementedException("🧟 À toi de jouer !");
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
