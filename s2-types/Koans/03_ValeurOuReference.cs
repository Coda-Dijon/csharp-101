namespace Types;

// 🎯 Objectif : class (référence) vs struct (valeur), une différence qui n'existe pas en Java, les paramètres ref / out et la surcharge d'opérateurs.
public class AProposDesValeursEtDesReferences
{
    private class PositionClasse
    {
        public int X { get; set; }
    }

    private struct PositionStruct
    {
        public int X { get; set; }
    }

    [Fact]
    public void Une_classe_est_copiee_par_reference()
    {
        // Comme en Java : a et b désignent le même objet
        var a = new PositionClasse { X = 1 };
        var b = a;

        b.X = 10;

        Check.That(a.X).IsEqualTo(___);
    }

    [Fact]
    public void Une_struct_est_copiee_par_valeur()
    {
        // Comme un int en Java : b est une copie indépendante
        var a = new PositionStruct { X = 1 };
        var b = a;

        b.X = 10;

        Check.That(a.X).IsEqualTo(___);
    }

    [Fact]
    public void Une_methode_recoit_une_copie_de_la_struct_sauf_avec_ref()
    {
        static void Deplacer(PositionStruct p) => p.X = 99;
        static void DeplacerVraiment(ref PositionStruct p) => p.X = 99;

        var position = new PositionStruct { X = 1 };

        Deplacer(position);
        Check.That(position.X).IsEqualTo(___);

        // Java : aucun équivalent, tout est passé par valeur (y compris les références)
        DeplacerVraiment(ref position);
        Check.That(position.X).IsEqualTo(___);
    }

    [Fact]
    public void Ref_sur_une_classe_permet_de_remplacer_l_objet_de_l_appelant()
    {
        // Java passe les références PAR VALEUR : réaffecter le paramètre ne change rien pour l'appelant
        static void Remplacer(PositionClasse p) => p = new PositionClasse { X = 99 };
        // avec ref, on reçoit la variable de l'appelant elle-même
        static void RemplacerVraiment(ref PositionClasse p) => p = new PositionClasse { X = 99 };

        var position = new PositionClasse { X = 1 };

        Remplacer(position);
        Check.That(position.X).IsEqualTo(___);

        RemplacerVraiment(ref position);
        Check.That(position.X).IsEqualTo(___);
    }

    [Fact]
    public void Out_oblige_la_methode_a_produire_une_valeur()
    {
        // ref : la variable doit être initialisée AVANT l'appel, la méthode peut la lire et la modifier
        // out : la variable peut être vide avant l'appel, la méthode DOIT l'affecter avant de rendre la main
        static bool TryFouiller(string lieu, out string trouvaille)
        {
            trouvaille = lieu switch
            {
                "pharmacie" => "bandages",
                "armurerie" => "fusil",
                _ => "",
            };
            return trouvaille != "";
        }

        var ok = TryFouiller("armurerie", out var arme); // déclaration de la variable dans l'appel
        var rien = TryFouiller("cave", out _);            // _ : on ignore la valeur (discard)

        Check.That(ok).IsEqualTo(___);
        Check.That(arme).IsEqualTo(___);
        Check.That(rien).IsEqualTo(___);
    }

    [Fact]
    public void Les_types_primitifs_sont_des_structs()
    {
        Check.That(typeof(int).IsValueType).IsEqualTo(___);
        Check.That(typeof(DateTime).IsValueType).IsEqualTo(___);
        Check.That(typeof(string).IsValueType).IsEqualTo(___);
    }

    [Fact]
    public void Une_struct_a_toujours_une_valeur_par_defaut()
    {
        // Une struct ne peut pas être null : default remplit tous ses champs de zéros
        Check.That(default(PositionStruct).X).IsEqualTo(___);
        Check.That(default(string) is null).IsEqualTo(___);
    }

    [Fact]
    public void Les_structs_se_comparent_par_valeur_par_defaut()
    {
        var s1 = new PositionStruct { X = 1 };
        var s2 = new PositionStruct { X = 1 };
        var c1 = new PositionClasse { X = 1 };
        var c2 = new PositionClasse { X = 1 };

        Check.That(s1.Equals(s2)).IsEqualTo(___);
        Check.That(c1.Equals(c2)).IsEqualTo(___);
        // s1 == s2  -> ne compile pas : une struct doit définir l'opérateur == elle-même
    }

    [Fact]
    public void Modifier_une_struct_dans_une_liste_ne_modifie_qu_une_copie()
    {
        List<PositionStruct> positions = [new PositionStruct { X = 1 }];

        // positions[0].X = 5;  -> erreur de compilation CS1612 : on modifierait une copie temporaire
        var position = positions[0];
        position.X = 5;

        Check.That(positions[0].X).IsEqualTo(___);
    }

    [Fact]
    public void Le_boxing_copie_la_struct_dans_un_objet()
    {
        // Java : int -> Integer  /  C# : n'importe quelle struct -> object
        var position = new PositionStruct { X = 1 };
        object boite = position;

        position.X = 42;

        Check.That(((PositionStruct)boite).X).IsEqualTo(___);
    }

    // La bonne pratique : des structs immuables. Le compilateur garantit qu'aucun champ ne change.
    // (record struct : la version « valeur » des records, que l'on détaille dans 05_Records.cs)
    private readonly record struct Coordonnees(double Latitude, double Longitude);

    [Fact]
    public void Une_readonly_record_struct_est_une_petite_valeur_immuable()
    {
        var bunker = new Coordonnees(48.85, 2.35);

        // bunker.Latitude = 0;  -> ne compile pas
        var refuge = bunker with { Longitude = 2.29 };

        Check.That(bunker.Longitude).IsEqualTo(___);
        Check.That(refuge == new Coordonnees(48.85, 2.29)).IsEqualTo(___);
    }

    // Une vraie valeur peut avoir ses propres opérateurs : une méthode public static nommée operator +
    // Java : pas de surcharge d'opérateurs, on écrit rations.plus(autres), comme BigDecimal.add(...)
    private readonly record struct Rations(int Jours)
    {
        public static Rations operator +(Rations gauche, Rations droite) => new(gauche.Jours + droite.Jours);
        public static Rations operator *(Rations rations, int survivants) => new(rations.Jours * survivants);

        // < et > vont par paires : définir l'un sans l'autre ne compile pas (CS0216), tout comme == sans !=
        public static bool operator <(Rations gauche, Rations droite) => gauche.Jours < droite.Jours;
        public static bool operator >(Rations gauche, Rations droite) => gauche.Jours > droite.Jours;

        // 🛠 À toi de coder : consommer des rations, sans jamais descendre sous 0 jour
        public static Rations operator -(Rations stock, Rations consommees)
        {
            throw new NotImplementedException("🧟 À toi de jouer !");
        }
    }

    [Fact]
    public void Une_struct_peut_surcharger_les_operateurs()
    {
        var conserves = new Rations(3);
        var eau = new Rations(2);

        var stock = conserves + eau;
        var pourLeGroupe = conserves * 4;
        stock += eau; // += se déduit de + : rien à écrire

        Check.That(stock.Jours).IsEqualTo(___);
        Check.That(pourLeGroupe.Jours).IsEqualTo(___);
    }

    [Fact]
    public void Les_operateurs_de_comparaison_vont_par_paires()
    {
        var conserves = new Rations(3);
        var eau = new Rations(2);

        Check.That(eau < conserves).IsEqualTo(___);
        Check.That(eau > conserves).IsEqualTo(___);
        // == et != : générés par le record struct, comparés champ par champ (à écrire soi-même sur une struct)
        Check.That(eau + eau == new Rations(4)).IsEqualTo(___);
        // conserves <= eau  -> ne compile pas : <= et >= sont une autre paire, à définir aussi
    }

    [Fact]
    public void On_sait_ecrire_un_operateur()
    {
        var stock = new Rations(5);

        Check.That((stock - new Rations(2)).Jours).IsEqualTo(3);
        Check.That((stock - new Rations(8)).Jours).IsEqualTo(0);
    }

    // 🛠 À toi de coder : soigne un survivant en modifiant directement sa struct, sans dépasser 100 points de vie.
    private struct Sante
    {
        public int PointsDeVie { get; set; }
    }

    private static void Soigner(ref Sante sante, int points)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_modifier_une_struct_par_reference()
    {
        var sante = new Sante { PointsDeVie = 50 };

        Soigner(ref sante, 30);
        Check.That(sante.PointsDeVie).IsEqualTo(80);

        Soigner(ref sante, 50);
        Check.That(sante.PointsDeVie).IsEqualTo(100);
    }

    // 🛠 À toi de coder : lis une ligne du registre des zombies, au format "nom:faim", dans le style TryParse.
    //    - ligne valide   -> true, avec le nom et la faim en sortie
    //    - ligne invalide -> false, avec "" et 0 en sortie (un paramètre out doit TOUJOURS être affecté)
    //    Indices : ligne.Split(':'), int.TryParse(texte, out var n)
    private static bool TryLireZombie(string ligne, out string nom, out int faim)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    [Fact]
    public void On_sait_ecrire_une_methode_TryXxx()
    {
        Check.That(TryLireZombie("Bob:7", out var nom, out var faim)).IsTrue();
        Check.That(nom).IsEqualTo("Bob");
        Check.That(faim).IsEqualTo(7);

        Check.That(TryLireZombie("Bob:affamé", out nom, out faim)).IsFalse();
        Check.That(nom).IsEqualTo("");
        Check.That(faim).IsEqualTo(0);

        Check.That(TryLireZombie("n'importe quoi", out _, out _)).IsFalse();
    }
}
