using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace Ecosysteme;

// 🎯 Objectif : exposer la prise de commande du food truck.
//
// À compléter dans le projet FoodTruck :
//   1. Cuisine.Decider (Domain.cs)               : la règle métier, une fonction pure
//   2. Endpoints.Probleme (Endpoints.cs)         : un refus de la cuisine -> une réponse HTTP « Problem Details »
//   3. Endpoints.PasserCommande (Endpoints.cs)   : POST /commandes, qui assemble le tout
//
// Functional core, imperative shell : la décision ne connaît ni la base, ni HTTP ; l'endpoint, si.

public class LaCuisineDecide
{
    private static readonly Client Bob = new(1, "Bob", "Rodeur", EstVivant: false);
    private static readonly Client Shaun = new(4, "Shaun", "Rodeur", EstVivant: true);
    private static readonly Client Gege = new(3, "Gégé", "Vegan", EstVivant: false);
    private static readonly Plat Cervelle = new(1, "Cervelle nature", "Cerveau", 12, Stock: 10);
    private static readonly Plat ChouFleur = new(4, "Chou-fleur vapeur", "Chou-fleur", 8, Stock: 12);

    [Fact]
    public void Un_zombie_affame_est_servi()
    {
        Check.That(Cuisine.Decider(Bob, Cervelle, 2)).IsEqualTo(new Acceptee());
        Check.That(Cuisine.Decider(Gege, ChouFleur, 4)).IsEqualTo(new Acceptee());
    }

    [Fact]
    public void Les_refus_de_la_cuisine()
    {
        Check.That(Cuisine.Decider(Shaun, Cervelle, 1)).IsEqualTo(new ClientEncoreVivant("Shaun"));
        Check.That(Cuisine.Decider(Gege, Cervelle, 1)).IsEqualTo(new RegimeNonRespecte("Gégé", "Cerveau"));
        Check.That(Cuisine.Decider(Bob, Cervelle, 11)).IsEqualTo(new RuptureDeStock("Cervelle nature", 10));
    }

    [Fact]
    public void Un_vivant_est_refuse_avant_tout_le_reste()
    {
        var shaunVegan = Shaun with { Type = "Vegan" };

        Check.That(Cuisine.Decider(shaunVegan, Cervelle, 50)).IsEqualTo(new ClientEncoreVivant("Shaun"));
    }

    [Fact]
    public void Chaque_refus_a_son_code_HTTP()
    {
        Check.That(Endpoints.Probleme(new ClientEncoreVivant("Shaun")).StatusCode).IsEqualTo(422);
        Check.That(Endpoints.Probleme(new RegimeNonRespecte("Gégé", "Cerveau")).StatusCode).IsEqualTo(422);
        Check.That(Endpoints.Probleme(new RuptureDeStock("Tartare de neurones", 0)).StatusCode).IsEqualTo(409);
        Check.That(Endpoints.Probleme(new RuptureDeStock("Tartare de neurones", 0)).ProblemDetails.Title)
            .IsEqualTo("Rupture de stock");
    }
}

public class LaPriseDeCommande(BaseDeDonnees db) : AvecLApi(db)
{
    [Fact]
    public async Task Une_commande_acceptee_renvoie_201_et_son_adresse()
    {
        var reponse = await Api.PostAsJsonAsync("/commandes", new NouvelleCommande(ClientId: 1, PlatId: 1, Quantite: 2));

        Check.That(reponse.StatusCode).IsEqualTo(HttpStatusCode.Created);
        Check.That(reponse.Headers.Location!.ToString()).IsEqualTo("/commandes/8");

        var commande = await Api.GetFromJsonAsync<Commande>("/commandes/8");
        var cervelle = await Api.GetFromJsonAsync<Plat>("/plats/1");
        Check.That(commande!.Quantite).IsEqualTo(2);
        Check.That(cervelle!.Stock).IsEqualTo(8); // le stock a diminué
    }

    [Fact]
    public async Task Un_refus_arrive_en_Problem_Details()
    {
        var shaun = await Api.PostAsJsonAsync("/commandes", new NouvelleCommande(ClientId: 4, PlatId: 1, Quantite: 1));
        var gege = await Api.PostAsJsonAsync("/commandes", new NouvelleCommande(ClientId: 3, PlatId: 1, Quantite: 1));
        var tartare = await Api.PostAsJsonAsync("/commandes", new NouvelleCommande(ClientId: 2, PlatId: 3, Quantite: 1));

        Check.That(shaun.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
        Check.That(shaun.Content.Headers.ContentType!.MediaType).IsEqualTo("application/problem+json");
        Check.That((await shaun.Content.ReadFromJsonAsync<ProblemDetails>())!.Title).IsEqualTo("Client encore vivant");
        Check.That(gege.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
        Check.That(tartare.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Un_client_ou_un_plat_inconnu_renvoie_404()
    {
        var clientInconnu = await Api.PostAsJsonAsync("/commandes", new NouvelleCommande(ClientId: 99, PlatId: 1, Quantite: 1));
        var platInconnu = await Api.PostAsJsonAsync("/commandes", new NouvelleCommande(ClientId: 1, PlatId: 99, Quantite: 1));

        Check.That(clientInconnu.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        Check.That(platInconnu.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Cinq_zombies_pour_trois_cerveaux_flambes()
    {
        // Cinq commandes envoyées EN MÊME TEMPS pour le Cerveau flambé, dont il ne reste que 3.
        // La décision lit un stock qui peut déjà être périmé : c'est l'UPDATE ... WHERE stock >= @Quantite
        // du Comptoir qui tranche, de façon atomique, et l'endpoint transforme un null en 409.
        var horde = Enumerable.Range(0, 5)
            .Select(_ => Api.PostAsJsonAsync("/commandes", new NouvelleCommande(ClientId: 2, PlatId: 2, Quantite: 1)));

        var reponses = await Task.WhenAll(horde);

        Check.That(reponses.Count(r => r.StatusCode == HttpStatusCode.Created)).IsEqualTo(3);
        Check.That(reponses.Count(r => r.StatusCode == HttpStatusCode.Conflict)).IsEqualTo(2);
        Check.That((await Api.GetFromJsonAsync<Plat>("/plats/2"))!.Stock).IsEqualTo(0);
    }
}
