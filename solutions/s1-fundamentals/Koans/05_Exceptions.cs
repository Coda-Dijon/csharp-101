namespace Fundamentals;

// 🎯 Objectif : try/catch/finally, using... et la grande absente : les checked exceptions.
public class AProposDesExceptions
{
    [Fact]
    public void Il_n_y_a_pas_de_checked_exceptions()
    {
        // Java : void lire() throws IOException  ->  l'appelant DOIT gérer ou propager
        // C#   : aucune clause throws, toutes les exceptions sont "unchecked"
        static void Lire() => throw new IOException("disque rongé par les rats");

        Check.ThatCode(Lire)
            .Throws<IOException>()
            .WithMessage(/*__*/"disque rongé par les rats"/*__*/);
    }

    [Fact]
    public void Finally_s_execute_toujours()
    {
        var journal = new List<string>();

        try
        {
            journal.Add("try");
            throw new InvalidOperationException();
        }
        catch (InvalidOperationException)
        {
            journal.Add("catch");
        }
        finally
        {
            journal.Add("finally");
        }

        Check.That(string.Join(" > ", journal)).IsEqualTo(/*__*/"try > catch > finally"/*__*/);
    }

    [Fact]
    public void Les_filtres_d_exception_choisissent_le_catch_avec_when()
    {
        // Pas d'équivalent en Java : on attrape seulement si la condition est vraie
        static string Gerer(int code)
        {
            try
            {
                throw new HttpRequestException("erreur", null, (System.Net.HttpStatusCode)code);
            }
            catch (HttpRequestException e) when ((int?)e.StatusCode == 404)
            {
                return "introuvable";
            }
            catch (HttpRequestException)
            {
                return "autre erreur";
            }
        }

        Check.That(Gerer(404)).IsEqualTo(/*__*/"introuvable"/*__*/);
        Check.That(Gerer(500)).IsEqualTo(/*__*/"autre erreur"/*__*/);
    }

    [Fact]
    public void La_division_entiere_par_zero_leve_une_exception()
    {
        var zero = 0;

        Check.ThatCode(() => 42 / zero).ThrowsType(/*__*/typeof(DivideByZeroException)/*__*/);
    }

    [Fact]
    public void Mais_la_division_flottante_par_zero_ne_leve_rien()
    {
        var zero = 0.0;

        Check.That(42 / zero).IsEqualTo(/*__*/double.PositiveInfinity/*__*/);
    }

    [Fact]
    public void Les_debordements_sont_silencieux_sauf_en_mode_checked()
    {
        // Comme en Java, int.MaxValue + 1 "boucle" vers les négatifs...
        var max = int.MaxValue;
        Check.That(unchecked(max + 1)).IsEqualTo(/*__*/int.MinValue/*__*/);

        // ... mais C# permet de demander une vérification
        Check.ThatCode(() => checked(max + 1)).ThrowsType(/*__*/typeof(OverflowException)/*__*/);
    }

    [Fact]
    public void Throw_est_aussi_une_expression()
    {
        // Pratique pour valider des paramètres en une ligne
        static string Valider(string? nom) => nom ?? throw new ArgumentNullException(nameof(nom));

        // nameof : le nom de la variable, vérifié à la compilation
        Check.ThatCode(() => Valider(null))
            .Throws<ArgumentNullException>()
            .WithProperty(e => e.ParamName, /*__*/"nom"/*__*/);
    }

    private sealed class Barricade(List<string> journal) : IDisposable
    {
        public void Tenir() => journal.Add("on tient");

        public void Dispose() => journal.Add("barricade démontée");
    }

    [Fact]
    public void Using_libere_les_ressources_comme_try_with_resources()
    {
        // Java : try (var b = new Barricade()) { ... }   (AutoCloseable)
        // C#   : using (var b = ...) { ... }  ou  using var b = ...;  (IDisposable)
        var journal = new List<string>();

        using (var barricade = new Barricade(journal))
        {
            barricade.Tenir();
        }
        journal.Add("fuite");

        Check.That(string.Join(", ", journal)).IsEqualTo(/*__*/"on tient, barricade démontée, fuite"/*__*/);
    }

    // 🛠 À toi de coder : convertit un âge saisi au clavier.
    //    - null ou vide        -> ArgumentException
    //    - pas un nombre       -> FormatException  (indice : int.Parse le fait pour vous)
    //    - négatif             -> ArgumentOutOfRangeException
    private static int LireAge(string? saisie)
    {
        // >>> SOLUTION
        ArgumentException.ThrowIfNullOrEmpty(saisie);
        var age = int.Parse(saisie);
        ArgumentOutOfRangeException.ThrowIfNegative(age);
        return age;
        // <<< SOLUTION
    }

    [Fact]
    public void On_sait_valider_une_saisie()
    {
        Check.That(LireAge("42")).IsEqualTo(42);
        Check.ThatCode(() => LireAge(null)).Throws<ArgumentNullException>();
        Check.ThatCode(() => LireAge("")).Throws<ArgumentException>();
        Check.ThatCode(() => LireAge("quarante-deux")).Throws<FormatException>();
        Check.ThatCode(() => LireAge("-3")).Throws<ArgumentOutOfRangeException>();
    }
}
