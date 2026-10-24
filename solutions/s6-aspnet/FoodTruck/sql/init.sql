-- Le Winchester : un food truck pour zombies. Le script repart de zéro à chaque exécution.
DROP SCHEMA IF EXISTS public CASCADE;
CREATE SCHEMA public;

CREATE TABLE plats (
    id            serial PRIMARY KEY,
    nom           text NOT NULL UNIQUE,
    categorie     text NOT NULL,              -- 'Cerveau' ou 'Chou-fleur'
    prix_en_dents int  NOT NULL,
    stock         int  NOT NULL CHECK (stock >= 0)
);

CREATE TABLE clients (
    id         serial PRIMARY KEY,
    nom        text    NOT NULL UNIQUE,
    type       text    NOT NULL,              -- 'Rodeur', 'Coureur' ou 'Vegan'
    est_vivant boolean NOT NULL DEFAULT false
);

CREATE TABLE commandes (
    id        serial PRIMARY KEY,
    client_id int         NOT NULL REFERENCES clients (id),
    plat_id   int         NOT NULL REFERENCES plats (id),
    quantite  int         NOT NULL CHECK (quantite > 0),
    passee_le timestamptz NOT NULL DEFAULT now()
);

INSERT INTO plats (nom, categorie, prix_en_dents, stock) VALUES
    ('Cervelle nature',      'Cerveau',    12, 10),
    ('Cerveau flambé',       'Cerveau',    20,  3),
    ('Tartare de neurones',  'Cerveau',    15,  0),
    ('Chou-fleur vapeur',    'Chou-fleur',  8, 12),
    ('Gratin de chou-fleur', 'Chou-fleur', 10,  5);

INSERT INTO clients (nom, type, est_vivant) VALUES
    ('Bob',   'Rodeur',  false),
    ('Alice', 'Coureur', false),
    ('Gégé',  'Vegan',   false),
    ('Shaun', 'Rodeur',  true),   -- il imite très bien les zombies
    ('Nina',  'Vegan',   false),
    ('Ed',    'Rodeur',  false);

INSERT INTO commandes (client_id, plat_id, quantite, passee_le) VALUES
    (1, 1, 2, '2026-10-20 12:00+00'),
    (1, 2, 1, '2026-10-21 12:30+00'),
    (2, 1, 3, '2026-10-21 19:00+00'),
    (3, 4, 4, '2026-10-22 12:15+00'),
    (2, 2, 2, '2026-10-22 20:00+00'),
    (5, 5, 1, '2026-10-23 12:45+00'),
    (1, 1, 1, '2026-10-23 21:30+00');
