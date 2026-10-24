namespace Types;

// 🎯 Objectif : dire adieu aux getters/setters. En C#, l'encapsulation se déclare en une ligne.
public class AProposDesProprietes
{
    private class Survivant
    {
        public string Nom { get; set; } = "Inconnu";

        public int PointsDeVie { get; private set; } = 100;

        public bool EstVivant => PointsDeVie > 0; // propriété calculée : pas de champ derrière

        public void Encaisser(int degats) => PointsDeVie = Math.Max(0, PointsDeVie - degats);
    }

    [Fact]
    public void Une_propriete_remplace_le_couple_getter_setter()
    {
        // Java : private String nom; public String getNom() {...} public void setNom(String nom) {...}
        // C#   : public string Nom { get; set; }  -> le champ est généré par le compilateur
        var survivant = new Survivant();

        Check.That(survivant.Nom).IsEqualTo(___);

        survivant.Nom = "Rick"; // ça ressemble à un champ, mais c'est un appel au setter
        Check.That(survivant.Nom).IsEqualTo(___);
    }

    [Fact]
    public void Un_setter_prive_protege_l_etat()
    {
        // survivant.PointsDeVie = 1000;  -> ne compile pas : le setter est private
        var survivant = new Survivant();

        survivant.Encaisser(30);
        Check.That(survivant.PointsDeVie).IsEqualTo(___);
        Check.That(survivant.EstVivant).IsEqualTo(___);

        survivant.Encaisser(500);
        Check.That(survivant.PointsDeVie).IsEqualTo(___);
        Check.That(survivant.EstVivant).IsEqualTo(___);
    }

    private class Arme
    {
        public required string Nom { get; init; }

        public int Degats { get; init; } = 10;
    }

    [Fact]
    public void Les_initialiseurs_d_objet_remplacent_les_builders()
    {
        // Java : Arme.builder().nom("Tronçonneuse").degats(50).build()  (Lombok)
        // C#   : on affecte les propriétés entre accolades, juste après le constructeur
        var tronconneuse = new Arme { Nom = "Tronçonneuse", Degats = 50 };
        var batte = new Arme { Nom = "Batte" };

        Check.That(tronconneuse.Degats).IsEqualTo(___);
        Check.That(batte.Degats).IsEqualTo(___);

        // init     : affectable seulement à la création -> batte.Degats = 99; ne compile pas
        // required : oublier Nom ne compile pas         -> new Arme { Degats = 3 }
    }

    private class Barricade
    {
        // C# 14 : `field` désigne le champ généré, plus besoin de le déclarer à la main
        public int Solidite
        {
            get;
            set => field = Math.Clamp(value, 0, 100);
        }
    }

    [Fact]
    public void Un_setter_peut_valider_la_valeur()
    {
        var barricade = new Barricade();

        barricade.Solidite = 150;
        Check.That(barricade.Solidite).IsEqualTo(___);

        barricade.Solidite = -5;
        Check.That(barricade.Solidite).IsEqualTo(___);
    }

    // C# 12 : constructeur primaire, les paramètres sont visibles dans toute la classe
    private class Rodeur(string nom, int faim)
    {
        public string Nom => nom;

        public string Grogner() => $"{nom} a faim ({faim}/10)";
    }

    [Fact]
    public void Un_constructeur_primaire_evite_de_recopier_les_parametres()
    {
        // Java : private final String nom; public Rodeur(String nom) { this.nom = nom; }
        var rodeur = new Rodeur("Bob", 7);

        Check.That(rodeur.Grogner()).IsEqualTo(___);

        // ⚠️ contrairement à un record, les paramètres ne deviennent PAS des propriétés publiques
        Check.That(typeof(Rodeur).GetProperties().Length).IsEqualTo(___);
    }

    private class Horde(string[] membres)
    {
        public int Taille => membres.Length;

        public string this[int index] => membres[index]; // indexeur : la syntaxe [] sur vos objets
    }

    [Fact]
    public void Un_indexeur_donne_acces_aux_elements_avec_des_crochets()
    {
        // Java : horde.get(0)  /  C# : horde[0], comme pour une List ou un Dictionary
        var horde = new Horde(["Bob", "Alice", "Charlie"]);

        Check.That(horde[1]).IsEqualTo(___);
        Check.That(horde.Taille).IsEqualTo(___);
    }

    // 🛠 À toi de coder : une réserve de conserves.
    //    - EstVide est vrai quand il ne reste plus de conserve
    //    - Manger() retire une conserve, ou lève une InvalidOperationException si la réserve est vide
    private class Reserve(int conserves)
    {
        public int Conserves { get; private set; } = conserves;

        public bool EstVide
        {
            get
            {
                throw new NotImplementedException("🧟 À toi de jouer !");
            }
        }

        public void Manger()
        {
            throw new NotImplementedException("🧟 À toi de jouer !");
        }
    }

    [Fact]
    public void On_sait_encapsuler_un_etat()
    {
        var reserve = new Reserve(2);

        reserve.Manger();
        Check.That(reserve.Conserves).IsEqualTo(1);
        Check.That(reserve.EstVide).IsFalse();

        reserve.Manger();
        Check.That(reserve.EstVide).IsTrue();
        Check.ThatCode(reserve.Manger).Throws<InvalidOperationException>();
    }
}
