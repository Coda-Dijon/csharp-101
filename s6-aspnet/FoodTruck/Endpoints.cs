using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodTruck;

// Les routes de l'API, en Minimal API : une lambda (ou une méthode) par route.
// Spring : @RestController + @GetMapping / @PostMapping
public static class Endpoints
{
    public static IEndpointRouteBuilder MapFoodTruck(this IEndpointRouteBuilder app)
    {
        app.MapGet("/menu", async (Comptoir comptoir, string? categorie) =>
                TypedResults.Ok(await comptoir.MenuAsync(categorie)))
            .WithSummary("Le menu du jour, filtrable par catégorie");

        app.MapGet("/plats/{id:int}", TrouverPlat)
            .WithSummary("Un plat du menu");

        app.MapPost("/commandes", PasserCommande)
            .WithSummary("Passer une commande");

        app.MapGet("/commandes/{id:int}", async Task<Results<Ok<Commande>, NotFound>> (int id, Comptoir comptoir) =>
                await comptoir.CommandeAsync(id) is { } commande ? TypedResults.Ok(commande) : TypedResults.NotFound())
            .WithSummary("Une commande déjà passée");

        app.MapGet("/stats/chiffre-affaires", async (Comptoir comptoir) =>
                TypedResults.Ok(await comptoir.ChiffreDAffairesParTypeAsync()))
            .WithSummary("Le chiffre d'affaires en dents, par type de zombie");

        return app;
    }

    // 🛠 À toi de coder : 200 avec le plat, ou 404 s'il n'existe pas.
    //    Le type de retour Results<Ok<Plat>, NotFound> documente les deux réponses possibles (dans l'OpenAPI aussi).
    public static async Task<Results<Ok<Plat>, NotFound>> TrouverPlat(int id, Comptoir comptoir)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    // 🛠 À toi de coder : passer une commande.
    //    - client ou plat inconnu                 -> 404
    //    - Cuisine.Decider refuse                 -> un ProblemDetails, avec le code donné par Probleme
    //    - sinon, comptoir.EnregistrerAsync       -> 201 Created, avec l'URL /commandes/{id} de la nouvelle commande
    //      (si EnregistrerAsync renvoie null, le stock a été vidé entre-temps : 409)
    public static async Task<Results<Created<Commande>, NotFound, ProblemHttpResult>> PasserCommande(
        NouvelleCommande demande, Comptoir comptoir)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }

    // 🛠 À toi de coder : traduire un refus de la cuisine en réponse HTTP (RFC 9457, « Problem Details »).
    //    ClientEncoreVivant -> 422, titre « Client encore vivant »
    //    RegimeNonRespecte  -> 422, titre « Régime non respecté »
    //    RuptureDeStock     -> 409, titre « Rupture de stock »
    public static ProblemHttpResult Probleme(Decision refus)
    {
        throw new NotImplementedException("🧟 À toi de jouer !");
    }
}
