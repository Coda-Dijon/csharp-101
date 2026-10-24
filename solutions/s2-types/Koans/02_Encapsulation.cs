using System.Reflection;

namespace Types;

// 🎯 Objectif : choisir qui a le droit de voir quoi. Les mots-clés ressemblent à Java, les valeurs par défaut non.
//
//   Java                        C#
//   public                      public
//   protected (+ package)       protected            : les classes dérivées SEULEMENT
//   (package-private)           internal             : tout l'assembly (le .dll), pas le dossier
//   private                     private              : la valeur par défaut des membres
//   -                           protected internal   : dérivées OU même assembly
//   -                           private protected    : dérivées ET même assembly
//   -                           file                 : ce fichier .cs uniquement (C# 11)

// Sans modificateur, une classe de premier niveau est internal : invisible hors de ce projet
class Cachette
{
}

file class Secret // `file` : même un autre fichier du projet ne la voit pas
{
}

public class AProposDeLEncapsulation
{
    private class Survivant
    {
        string _surnom = "Bob l'éponge"; // sans modificateur, un membre est private

        protected int Faim = 3;

        protected internal string Camp = "Alexandria";

        private protected string CodeRadio = "1234";

        public string Presenter() => $"On m'appelle {_surnom}";
    }

    [Fact]
    public void Un_membre_sans_modificateur_est_private()
    {
        // Java : sans modificateur, le membre est visible dans tout le package
        var champ = typeof(Survivant).GetField("_surnom", BindingFlags.NonPublic | BindingFlags.Instance)!;

        Check.That(champ.IsPrivate).IsEqualTo(/*__*/true/*__*/);
        Check.That(new Survivant().Presenter()).IsEqualTo(/*__*/"On m'appelle Bob l'éponge"/*__*/);
    }

    [Fact]
    public void Une_classe_sans_modificateur_est_internal()
    {
        // internal : visible partout dans l'assembly S2.Types.Koans.dll, invisible pour les autres projets
        Check.That(typeof(Cachette).IsPublic).IsEqualTo(/*__*/false/*__*/);
        Check.That(typeof(Secret).IsPublic).IsEqualTo(/*__*/false/*__*/);
        // une classe imbriquée sans modificateur est private, comme un membre
        Check.That(typeof(Survivant).IsNestedPrivate).IsEqualTo(/*__*/true/*__*/);
    }

    private class Chef : Survivant
    {
        public string Rapport() => $"faim {Faim}, camp {Camp}, radio {CodeRadio}";
    }

    [Fact]
    public void Protected_est_reserve_aux_classes_derivees()
    {
        // new Survivant().Faim  -> ne compile pas ici : on n'hérite pas de Survivant
        // (en Java, si : protected ouvre aussi l'accès à tout le package)
        Check.That(new Chef().Rapport()).IsEqualTo(/*__*/"faim 3, camp Alexandria, radio 1234"/*__*/);
    }

    [Fact]
    public void Protected_internal_et_private_protected_combinent_les_deux()
    {
        // protected internal : dérivées OU même assembly -> on peut le lire d'ici, sans hériter
        Check.That(new Survivant().Camp).IsEqualTo(/*__*/"Alexandria"/*__*/);

        // private protected : dérivées ET même assembly -> new Survivant().CodeRadio ne compile pas ici
        var codeRadio = typeof(Survivant).GetField("CodeRadio", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Check.That(codeRadio.IsFamilyAndAssembly).IsEqualTo(/*__*/true/*__*/);
    }

    private class Horde
    {
        private readonly List<string> _membres = ["Bob"];

        public IReadOnlyList<string> Membres => _membres;              // ⚠️ fuite : c'est toujours la List

        public IReadOnlyList<string> MembresProteges => _membres.AsReadOnly(); // une vue en lecture seule

        public void Recruter(string nom) => _membres.Add(nom);
    }

    [Fact]
    public void Exposer_une_collection_en_lecture_seule_demande_une_vraie_protection()
    {
        // readonly empêche de réaffecter _membres, pas de modifier son contenu
        var horde = new Horde();
        horde.Recruter("Alice");

        // un appelant malin peut caster l'interface vers la List et la modifier...
        if (horde.Membres is List<string> liste) liste.Add("Intrus");
        Check.That(horde.Membres.Count).IsEqualTo(/*__*/3/*__*/);

        // ... mais pas la vue renvoyée par AsReadOnly()
        Check.That(horde.MembresProteges is List<string>).IsEqualTo(/*__*/false/*__*/);
    }

    private class Creature
    {
        public virtual string Cri() => "...";
    }

    private class Zombie : Creature
    {
        // sealed override : les classes dérivées de Zombie ne pourront plus redéfinir Cri()
        public sealed override string Cri() => "Braaains";
    }

    private sealed class Goule : Zombie
    {
        // public override string Cri() => "Grrr";  -> ne compile pas : Cri est scellée dans Zombie
    }

    [Fact]
    public void Sealed_ferme_une_classe_ou_une_seule_methode()
    {
        // Java : final class / final sur une méthode  /  C# : sealed class / sealed override
        Check.That(new Goule().Cri()).IsEqualTo(/*__*/"Braaains"/*__*/);
        Check.That(typeof(Goule).IsSealed).IsEqualTo(/*__*/true/*__*/);
        Check.That(typeof(Zombie).GetMethod("Cri")!.IsFinal).IsEqualTo(/*__*/true/*__*/);
    }

    // 🛠 À toi de coder : un bunker qui protège ses occupants.
    //    - Entrer(nom) ajoute l'occupant et renvoie true, ou renvoie false si le bunker est plein
    //    - Occupants expose la liste SANS permettre de la modifier depuis l'extérieur (même avec un cast)
    private sealed class Bunker(int capacite)
    {
        private readonly List<string> _occupants = [];

        public IReadOnlyList<string> Occupants
        {
            get
            {
                // >>> SOLUTION
                return _occupants.AsReadOnly();
                // <<< SOLUTION
            }
        }

        public bool Entrer(string nom)
        {
            // >>> SOLUTION
            if (_occupants.Count >= capacite) return false;
            _occupants.Add(nom);
            return true;
            // <<< SOLUTION
        }
    }

    [Fact]
    public void On_sait_proteger_l_etat_interne()
    {
        var bunker = new Bunker(capacite: 2);

        Check.That(bunker.Entrer("Rick")).IsTrue();
        Check.That(bunker.Entrer("Morty")).IsTrue();
        Check.That(bunker.Entrer("Negan")).IsFalse();

        Check.That(bunker.Occupants).ContainsExactly("Rick", "Morty");
        Check.That(bunker.Occupants is List<string>).IsFalse();
    }
}
