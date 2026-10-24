using System.Diagnostics.CodeAnalysis;

namespace Types;

// 🎯 Objectif : modéliser des valeurs immuables en une ligne, comme les records de Java 16... avec quelques bonus.
public class AProposDesRecords
{
    // Java 16+ : record Zombie(String nom, int faim) {}
    private record Zombie(string Nom, int Faim);

    [Fact]
    public void Un_record_genere_constructeur_proprietes_et_ToString()
    {
        var bob = new Zombie("Bob", 3);

        // Java : bob.nom()  /  C# : une vraie propriété, en PascalCase
        Check.That(bob.Nom).IsEqualTo(/*__*/"Bob"/*__*/);
        // Java : Zombie[nom=Bob, faim=3]
        Check.That(bob.ToString()).IsEqualTo(/*__*/"Zombie { Nom = Bob, Faim = 3 }"/*__*/);
    }

    [Fact]
    public void Les_records_ont_une_egalite_structurelle()
    {
        var a = new Zombie("Bob", 3);
        var b = new Zombie("Bob", 3);

        // == est redéfini : il compare les valeurs, pas les références
        Check.That(a == b).IsEqualTo(/*__*/true/*__*/);
        Check.That(ReferenceEquals(a, b)).IsEqualTo(/*__*/false/*__*/);
        Check.That(a.GetHashCode() == b.GetHashCode()).IsEqualTo(/*__*/true/*__*/);
    }

    [Fact]
    public void With_cree_une_copie_modifiee()
    {
        // Java : new Zombie(bob.nom(), 10)  (les "withers" sont encore en preview en Java)
        var bob = new Zombie("Bob", 3);

        var bobAffame = bob with { Faim = 10 };

        Check.That(bob.Faim).IsEqualTo(/*__*/3/*__*/); // l'original n'a pas bougé
        Check.That(bobAffame.Faim).IsEqualTo(/*__*/10/*__*/);
        Check.That(bobAffame.Nom).IsEqualTo(/*__*/"Bob"/*__*/);
    }

    [Fact]
    public void Un_record_se_deconstruit()
    {
        // Java 21 : if (z instanceof Zombie(var nom, var faim))
        var (nom, faim) = new Zombie("Alice", 8);

        Check.That($"{nom}:{faim}").IsEqualTo(/*__*/"Alice:8"/*__*/);
    }

    private record Groupe(string Nom, List<string> Membres);

    [Fact]
    public void L_immuabilite_d_un_record_n_est_pas_profonde()
    {
        var groupe = new Groupe("Les Survivants", ["Rick"]);

        // groupe.Nom = "...";  -> ne compile pas... mais la liste, elle, reste modifiable
        groupe.Membres.Add("Morty");
        Check.That(groupe.Membres.Count).IsEqualTo(/*__*/2/*__*/);

        // ⚠️ et l'égalité compare la RÉFÉRENCE de la liste, pas son contenu
        var copie = new Groupe("Les Survivants", ["Rick", "Morty"]);
        Check.That(groupe == copie).IsEqualTo(/*__*/false/*__*/);
    }

    // ❌ La validation dans le constructeur : la création peut échouer, mais rien ne le dit.
    //    « new TemperatureQuiExplose(-300) » ressemble à n'importe quel new... jusqu'à l'exception.
    private record TemperatureQuiExplose(double Celsius)
    {
        public double Celsius { get; } = Celsius >= -273.15
            ? Celsius
            : throw new ArgumentOutOfRangeException(nameof(Celsius));
    }

    // ✅ Constructeur privé + FACTORY METHOD : la signature avoue que la création peut échouer.
    //    Temperature? : « peut-être pas de température ». L'appelant est obligé de traiter le null (séance 1).
    private sealed record Temperature
    {
        private const double ZeroAbsolu = -273.15;

        public double Celsius { get; }

        private Temperature(double celsius) => Celsius = celsius;

        public static Temperature? Creer(double celsius) =>
            celsius >= ZeroAbsolu ? new Temperature(celsius) : null;

        // La variante TryXxx, comme int.TryParse (voir les paramètres out dans 03_ValeurOuReference)
        public static bool TryCreer(double celsius, [NotNullWhen(true)] out Temperature? temperature)
        {
            temperature = Creer(celsius);
            return temperature is not null;
        }

        public double Fahrenheit => Celsius * 9 / 5 + 32;
    }

    [Fact]
    public void Un_constructeur_qui_lance_cache_que_la_construction_peut_echouer()
    {
        // Ça compile sans un avertissement... et ça explose à l'exécution
        Check.ThatCode(() => new TemperatureQuiExplose(-300)).ThrowsType(/*__*/typeof(ArgumentOutOfRangeException)/*__*/);
    }

    [Fact]
    public void Une_factory_method_rend_l_echec_visible()
    {
        // new Temperature(100) ne compile pas : le constructeur est privé, Creer est la seule porte d'entrée
        var eau = Temperature.Creer(100);
        var impossible = Temperature.Creer(-300);

        Check.That(eau?.Fahrenheit).IsEqualTo(/*__*/212.0/*__*/);
        Check.That(impossible is null).IsEqualTo(/*__*/true/*__*/);

        // TryCreer : un booléen pour le if, la valeur dans le out
        var reussi = Temperature.TryCreer(-300, out var temperature);
        Check.That(reussi).IsEqualTo(/*__*/false/*__*/);
    }

    private abstract record Creature(string Nom);

    private record Humain(string Nom) : Creature(Nom);

    private record MortVivant(string Nom) : Creature(Nom);

    [Fact]
    public void Un_record_peut_heriter_d_un_autre_record()
    {
        // Java : un record est implicitement final, pas d'héritage possible
        Creature rick = new Humain("Rick");
        Creature rickApres = new MortVivant("Rick");

        Check.That(rick.Nom == rickApres.Nom).IsEqualTo(/*__*/true/*__*/);
        // l'égalité tient aussi compte du type exact
        Check.That(rick == rickApres).IsEqualTo(/*__*/false/*__*/);
    }

    // 🛠 À toi de coder : un cerveau vieillit d'un jour à chaque appel de Vieillir(), sans jamais être modifié.
    //    - Vieillir() renvoie une copie avec un jour de plus (indice : with)
    //    - EstPerime est vrai au-delà de 3 jours
    private record Cerveau(string Proprietaire, int Jours)
    {
        public Cerveau Vieillir()
        {
            // >>> SOLUTION
            return this with { Jours = Jours + 1 };
            // <<< SOLUTION
        }

        public bool EstPerime
        {
            get
            {
                // >>> SOLUTION
                return Jours > 3;
                // <<< SOLUTION
            }
        }
    }

    [Fact]
    public void On_sait_faire_evoluer_une_valeur_immuable()
    {
        var frais = new Cerveau("Bob", 3);

        var vieux = frais.Vieillir();

        Check.That(frais.Jours).IsEqualTo(3);
        Check.That(frais.EstPerime).IsFalse();
        Check.That(vieux).IsEqualTo(new Cerveau("Bob", 4));
        Check.That(vieux.EstPerime).IsTrue();
    }
}
