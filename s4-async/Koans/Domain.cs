namespace Asynchrone;

// Le Winchester, food truck pour zombies : les tables sont décrites dans sql/init.sql.
// Le domaine complet arrive en séance 5 ; ici, juste ce que les koans lisent depuis PostgreSQL.
public sealed record Plat(int Id, string Nom, string Categorie, int PrixEnDents, int Stock);
