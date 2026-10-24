using System.Net;
using System.Net.Http.Json;

namespace Ecosysteme;

// 🎯 Objectif : appeler l'API du food truck (FoodTruck/Program.cs), démarrée en mémoire pour le test.
//
// Spring : @SpringBootTest + MockMvc ;  .NET : WebApplicationFactory<Program> + un HttpClient
// L'API tourne pour de vrai (routing, sérialisation JSON, injection), sur le PostgreSQL du conteneur.
public class AProposDesMinimalApi(BaseDeDonnees db) : AvecLApi(db)
{
    [Fact]
    public async Task L_API_repond_sur_la_racine()
    {
        var reponse = await Api.GetAsync("/");

        Check.That(reponse.StatusCode).IsEqualTo(___);
        Check.That(await reponse.Content.ReadAsStringAsync()).Contains("Winchester");
    }

    [Fact]
    public async Task Le_menu_arrive_en_JSON()
    {
        // GetFromJsonAsync = GetAsync + désérialisation, avec les conventions du web (camelCase)
        var menu = await Api.GetFromJsonAsync<List<Plat>>("/menu");

        Check.That(menu!.Count).IsEqualTo(___);
        Check.That(menu[0].PrixEnDents).IsEqualTo(___);
    }

    [Fact]
    public async Task Un_parametre_de_la_lambda_se_remplit_avec_la_query_string()
    {
        // MapGet("/menu", (Comptoir comptoir, string? categorie) => ...) :
        // le Comptoir vient de l'injection, categorie vient de ?categorie=... (Spring : @RequestParam)
        var menu = await Api.GetFromJsonAsync<List<Plat>>("/menu?categorie=Chou-fleur");

        Check.That(menu!.Count).IsEqualTo(___);
    }

    [Fact]
    public async Task Une_contrainte_de_route_filtre_les_URL()
    {
        // MapGet("/commandes/{id:int}", ...) : « abc » n'est pas un int, la route ne correspond pas
        var commande = await Api.GetFromJsonAsync<Commande>("/commandes/1");
        var reponse = await Api.GetAsync("/commandes/abc");

        Check.That(commande!.Quantite).IsEqualTo(___);
        Check.That(reponse.StatusCode).IsEqualTo(___);
    }

    [Fact]
    public async Task Un_dictionnaire_devient_un_objet_JSON()
    {
        var chiffreDAffaires = await Api.GetFromJsonAsync<Dictionary<string, int>>("/stats/chiffre-affaires");

        Check.That(chiffreDAffaires!["Rodeur"]).IsEqualTo(___);
        Check.That(chiffreDAffaires.Values.Sum()).IsEqualTo(___);
    }

    [Fact]
    public async Task Le_document_OpenAPI_est_genere_par_le_framework()
    {
        // En Development : /openapi/v1.json (le contrat) et /scalar (l'interface pour l'explorer)
        var document = await Api.GetStringAsync("/openapi/v1.json");

        Check.That(document.Contains("/plats/{id}")).IsEqualTo(___);
        Check.That(document.Contains("Le menu du jour")).IsEqualTo(___); // le .WithSummary(...)
    }

    [Fact]
    public async Task On_sait_ecrire_un_endpoint()
    {
        // 🛠 Implémente la route GET /plats/{id} dans FoodTruck/Program.cs
        var flambe = await Api.GetFromJsonAsync<Plat>("/plats/2");
        var inconnu = await Api.GetAsync("/plats/99");

        Check.That(flambe!.Nom).IsEqualTo("Cerveau flambé");
        Check.That(inconnu.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }
}
