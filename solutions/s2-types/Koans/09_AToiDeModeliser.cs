#define MODELE_ECRIT // ⬅️ décommente cette ligne quand tes types compilent : les tests s'activent

namespace Types;

// 🛠 Mini-énoncé : la carte de patrouille du bunker
//
// Les éclaireurs notent sur une carte tout ce qui rôde autour du bunker.
// Chaque repère a un nom, une position et un niveau de menace. Écris les types suivants, dans ce fichier :
//
//   1. un enum Menace : Faible, Moyenne, Elevee (dans cet ordre : il sert à les comparer)
//   2. une struct Position : deux entiers X et Y, comparée par valeur
//      -> new Position(3, 4)
//   3. une interface IRepere : Nom (string), Position (Position) et Menace (Menace), en lecture seule
//   4. une classe abstraite MortVivant qui implémente IRepere, avec un constructeur (string nom, Position position)
//      et une propriété Menace abstraite
//   5. deux morts-vivants qu'on ne peut plus dériver : Rodeur (menace Faible) et Tank (menace Elevee)
//      -> new Rodeur("Bob", new Position(1, 2))
//   6. un Pillard : un humain hostile, PAS un mort-vivant, mais un IRepere quand même (menace Moyenne)
//      -> new Pillard("Negan", new Position(0, 9))

// >>> MODELE
public enum Menace
{
    Faible,
    Moyenne,
    Elevee,
}

public readonly record struct Position(int X, int Y);

public interface IRepere
{
    string Nom { get; }

    Position Position { get; }

    Menace Menace { get; }
}

public abstract class MortVivant(string nom, Position position) : IRepere
{
    public string Nom => nom;

    public Position Position => position;

    public abstract Menace Menace { get; }
}

public sealed class Rodeur(string nom, Position position) : MortVivant(nom, position)
{
    public override Menace Menace => Menace.Faible;
}

public sealed class Tank(string nom, Position position) : MortVivant(nom, position)
{
    public override Menace Menace => Menace.Elevee;
}

public sealed class Pillard(string nom, Position position) : IRepere
{
    public string Nom => nom;

    public Position Position => position;

    public Menace Menace => Menace.Moyenne;
}
// <<< MODELE

public class AToiDeModeliser
{
#if !MODELE_ECRIT
    [Fact]
    public void Ecris_ton_modele_puis_decommente_le_define_en_haut_du_fichier()
    {
        throw new NotImplementedException("🧟 À toi de jouer ! Lis l'énoncé en haut de 09_AToiDeModeliser.cs");
    }
#else
    [Fact]
    public void Une_position_est_une_valeur()
    {
        var bunker = new Position(3, 4);

        Check.That(typeof(Position).IsValueType).IsTrue();
        Check.That(bunker).IsEqualTo(new Position(3, 4));
        Check.That(bunker.X + bunker.Y).IsEqualTo(7);
    }

    [Fact]
    public void Chaque_repere_a_son_niveau_de_menace()
    {
        Check.That(new Rodeur("Bob", new Position(1, 2)).Menace).IsEqualTo(Menace.Faible);
        Check.That(new Pillard("Negan", new Position(0, 9)).Menace).IsEqualTo(Menace.Moyenne);
        Check.That(new Tank("Big Mama", new Position(5, 5)).Menace).IsEqualTo(Menace.Elevee);
        Check.That(Menace.Elevee > Menace.Faible).IsTrue();
    }

    [Fact]
    public void Les_morts_vivants_partagent_une_classe_abstraite()
    {
        Check.That(typeof(MortVivant).IsAbstract).IsTrue();
        Check.That(typeof(MortVivant).IsAssignableFrom(typeof(Rodeur))).IsTrue();
        Check.That(typeof(MortVivant).IsAssignableFrom(typeof(Tank))).IsTrue();
        Check.That(typeof(MortVivant).IsAssignableFrom(typeof(Pillard))).IsFalse();
        Check.That(typeof(Rodeur).IsSealed && typeof(Tank).IsSealed).IsTrue();
    }

    [Fact]
    public void Le_nom_et_la_position_sont_en_lecture_seule()
    {
        Check.That(typeof(IRepere).GetProperty("Nom")!.CanWrite).IsFalse();
        Check.That(typeof(Rodeur).GetProperty("Nom")!.SetMethod?.IsPublic ?? false).IsFalse();
        Check.That(typeof(Pillard).GetProperty("Position")!.SetMethod?.IsPublic ?? false).IsFalse();
    }

    [Fact]
    public void Tous_les_reperes_se_manipulent_via_l_interface()
    {
        IRepere[] carte =
        [
            new Rodeur("Bob", new Position(1, 2)),
            new Tank("Big Mama", new Position(5, 5)),
            new Pillard("Negan", new Position(0, 9)),
        ];

        var lePlusDangereux = carte.MaxBy(repere => repere.Menace)!;

        Check.That(lePlusDangereux.Nom).IsEqualTo("Big Mama");
        Check.That(lePlusDangereux.Position).IsEqualTo(new Position(5, 5));
    }
#endif
}
