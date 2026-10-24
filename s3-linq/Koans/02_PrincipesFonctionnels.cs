using System.Collections.Immutable;

namespace Fonctionnel;

// 🎯 Objectif : quelques principes fonctionnels qui rendent LINQ (et le code en général) plus sûr.
//   - des fonctions pures, sans effet de bord
//   - des données immuables
//   - de petites fonctions que l'on compose
//   - des expressions plutôt que des instructions
public class AProposDesPrincipesFonctionnels
{
    [Fact]
    public void Une_fonction_pure_donne_toujours_le_meme_resultat()
    {
        static int Pure(int faim) => faim + 1; // ne dépend que de ses paramètres

        var repas = 0;
        int Impure(int faim) => faim + ++repas; // dépend d'un état extérieur... et le modifie

        Check.That(Pure(5) == Pure(5)).IsEqualTo(___);
        Check.That(Impure(5)).IsEqualTo(___);
        Check.That(Impure(5)).IsEqualTo(___); // même appel, autre résultat : difficile à tester !
    }

    [Fact]
    public void Un_effet_de_bord_dans_une_requete_LINQ_se_rejoue_a_chaque_parcours()
    {
        var verifications = 0;
        var affames = Registre.Zombies.Where(zombie =>
        {
            verifications++; // ⚠️ effet de bord dans un prédicat
            return zombie.Faim > 5;
        });

        _ = affames.Count();
        _ = affames.Count();

        // 9 zombies, parcourus deux fois : la requête n'est pas un résultat, c'est une recette (voir 05_ExecutionDifferee)
        Check.That(verifications).IsEqualTo(___);
    }

    [Fact]
    public void Avec_des_records_on_transforme_sans_modifier()
    {
        var bob = Registre.Zombies[0];

        var bobNourri = bob with { Faim = 0 };

        Check.That(bob.Faim).IsEqualTo(___); // le registre n'a pas bougé
        Check.That(bobNourri.Faim).IsEqualTo(___);
    }

    [Fact]
    public void Une_collection_immuable_renvoie_une_nouvelle_collection()
    {
        // Java : List.of(...).add(x) lève UnsupportedOperationException
        // C#   : ImmutableList.Add renvoie une NOUVELLE liste, l'originale ne change pas
        ImmutableList<string> horde = ["Bob", "Alice"];

        var hordeAgrandie = horde.Add("Charlie");

        Check.That(horde.Count).IsEqualTo(___);
        Check.That(hordeAgrandie.Count).IsEqualTo(___);
    }

    [Fact]
    public void Une_boucle_avec_accumulateur_devient_une_expression()
    {
        // Le style impératif : une variable qu'on modifie à chaque tour
        var total = 0;
        foreach (var zombie in Registre.Zombies)
            total += zombie.Faim;

        // Le style fonctionnel : on décrit le résultat
        var totalLinq = Registre.Zombies.Sum(zombie => zombie.Faim);
        // Aggregate ≈ reduce en Java : une graine, puis une fonction (accumulateur, élément) -> accumulateur
        var totalAggregate = Registre.Zombies.Aggregate(0, (acc, zombie) => acc + zombie.Faim);

        Check.That(total).IsEqualTo(___);
        Check.That(totalLinq == total && totalAggregate == total).IsEqualTo(___);
    }

    [Fact]
    public void Une_fonction_a_plusieurs_parametres_peut_se_donner_un_par_un()
    {
        // Curryfication : une fonction qui prend `a` et renvoie une fonction qui attend `b`
        Func<int, Func<int, int>> ajouter = a => b => a + b;

        var nourrirDe3 = ajouter(3); // application partielle : on fixe le premier paramètre

        Check.That(ajouter(2)(5)).IsEqualTo(___);
        Check.That(nourrirDe3(4)).IsEqualTo(___);
    }

    // 🛠 À toi de coder : compose deux fonctions, d'abord f puis g (Java : f.andThen(g)).
    private static Func<A, C> Composer<A, B, C>(Func<A, B> f, Func<B, C> g)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_composer_deux_fonctions()
    {
        Func<int, int> doubler = x => x * 2;
        Func<int, string> enCri = x => $"{x} cerveaux !";

        Check.That(Composer(doubler, enCri)(21)).IsEqualTo("42 cerveaux !");
        Check.That(Composer(doubler, doubler)(5)).IsEqualTo(20);
    }

    // 🛠 À toi de coder : fais passer un zombie par toutes les étapes, dans l'ordre, sans jamais le modifier.
    //    Indice : Aggregate, avec le zombie comme graine.
    private static Zombie Traiter(Zombie zombie, params Func<Zombie, Zombie>[] etapes)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_enchainer_des_transformations_immuables()
    {
        Func<Zombie, Zombie> nourrir = z => z with { Faim = Math.Max(0, z.Faim - 5) };
        Func<Zombie, Zombie> deplacer = z => z with { Quartier = "Gare" };
        Func<Zombie, Zombie> evoluer = z => z.Type == TypeDeZombie.Rodeur ? z with { Type = TypeDeZombie.Coureur } : z;
        var bob = Registre.Zombies[0];

        var resultat = Traiter(bob, nourrir, deplacer, evoluer);

        Check.That(resultat).IsEqualTo(bob with { Faim = 2, Quartier = "Gare", Type = TypeDeZombie.Coureur });
        Check.That(Traiter(bob)).IsEqualTo(bob);
        Check.That(bob.Faim).IsEqualTo(7);
    }
}
