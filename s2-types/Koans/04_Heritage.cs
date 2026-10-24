namespace Types;

// 🎯 Objectif : interfaces, classes abstraites, virtual/override... et préférer la composition à l'héritage.
public class AProposDeLHeritage
{
    private interface IMordeur
    {
        string Mordre(string victime);
    }

    // Java : class Rodeur implements IMordeur  /  C# : `:` pour implements comme pour extends
    private class Rodeur : IMordeur
    {
        public string Mordre(string victime) => $"Le rôdeur mord {victime}";
    }

    [Fact]
    public void Une_classe_implemente_une_interface_avec_deux_points()
    {
        // Convention .NET : les interfaces commencent par un I
        IMordeur mordeur = new Rodeur();

        Check.That(mordeur.Mordre("Rick")).IsEqualTo(___);
    }

    private interface ICrieur
    {
        string Nom { get; } // une interface peut exiger une propriété

        string Crier() => $"{Nom} : Braaains !"; // méthode par défaut, comme en Java 8
    }

    private class Coureur : ICrieur
    {
        public string Nom => "Coureur";
    }

    [Fact]
    public void Une_methode_par_defaut_n_est_visible_que_via_l_interface()
    {
        var coureur = new Coureur();

        // coureur.Crier();  -> ne compile pas ! (en Java, si)
        Check.That(((ICrieur)coureur).Crier()).IsEqualTo(___);
    }

    private class Creature
    {
        public virtual string Decrire() => "une créature";

        public string Espece() => "inconnue";
    }

    private class Goule : Creature
    {
        public override string Decrire() => base.Decrire() + " affamée"; // base ≈ super

        public new string Espece() => "goule"; // `new` masque la méthode parente, sans la redéfinir
    }

    [Fact]
    public void Les_methodes_ne_sont_pas_virtuelles_par_defaut()
    {
        // Java : toutes les méthodes sont virtuelles  /  C# : il faut virtual + override
        Creature creature = new Goule();

        Check.That(creature.Decrire()).IsEqualTo(___);
        // ⚠️ sans virtual, c'est le type de la VARIABLE qui choisit la méthode
        Check.That(creature.Espece()).IsEqualTo(___);
        Check.That(((Goule)creature).Espece()).IsEqualTo(___);
    }

    private class Humain
    {
        public List<string> Journal { get; } = [];

        public Humain(string nom) => Journal.Add($"humain {nom}");
    }

    private sealed class Infecte : Humain
    {
        // Java : super(nom);  /  C# : `: base(nom)` avant le corps du constructeur
        public Infecte(string nom) : base(nom) => Journal.Add("infecté");
    }

    [Fact]
    public void Le_constructeur_parent_s_appelle_avec_base()
    {
        var infecte = new Infecte("Morty");

        Check.That(string.Join(" > ", infecte.Journal)).IsEqualTo(___);
    }

    [Fact]
    public void Sealed_interdit_l_heritage_comme_final()
    {
        // class Mutant : Infecte { }  -> ne compile pas
        Check.That(typeof(Infecte).IsSealed).IsEqualTo(___);
        // ⚠️ comme en Java, les classes sont ouvertes à l'héritage par défaut
        Check.That(typeof(Humain).IsSealed).IsEqualTo(___);
    }

    private abstract class Arme
    {
        public abstract int Degats { get; }

        public string Frapper() => $"{Degats} points de dégâts";
    }

    private class Machette : Arme
    {
        public override int Degats => 35;
    }

    [Fact]
    public void Une_classe_abstraite_impose_ses_membres_abstract()
    {
        // new Arme()  -> ne compile pas
        Arme arme = new Machette();

        Check.That(arme.Frapper()).IsEqualTo(___);
    }

    [Fact]
    public void Is_teste_le_type_et_as_convertit_sans_exception()
    {
        object chose = new Machette();

        // Java : chose instanceof Arme  /  (Arme) chose
        Check.That(chose is Arme).IsEqualTo(___);
        Check.That(chose as IMordeur).IsEqualTo(___); // `as` renvoie null au lieu de lever une exception
    }

    // 🛠 À toi de coder : plutôt qu'une sous-classe par combinaison (BatteAiguisee, BatteEnflammee, BatteAiguiseeEnflammee...),
    //    on compose des décorateurs qui enveloppent une autre arme.
    //    - une arme Aiguisee fait deux fois plus de dégâts que l'arme qu'elle enveloppe
    //    - une arme Enflammee en fait 5 de plus
    private interface IArme
    {
        int Degats { get; }
    }

    private class Batte : IArme
    {
        public int Degats => 10;
    }

    private class Aiguisee(IArme arme) : IArme
    {
        public int Degats
        {
            get
            {
                throw new NotImplementedException("🧟 À toi de jouer !");
            }
        }
    }

    private class Enflammee(IArme arme) : IArme
    {
        public int Degats
        {
            get
            {
                throw new NotImplementedException("🧟 À toi de jouer !");
            }
        }
    }

    [Fact]
    public void On_prefere_la_composition_a_l_heritage()
    {
        Check.That(new Aiguisee(new Batte()).Degats).IsEqualTo(20);
        Check.That(new Enflammee(new Batte()).Degats).IsEqualTo(15);
        Check.That(new Enflammee(new Aiguisee(new Batte())).Degats).IsEqualTo(25);
        Check.That(new Aiguisee(new Enflammee(new Batte())).Degats).IsEqualTo(30);
    }
}
