
SELECT id_box, MAX(mois) AS dernier_mois, MAX(annee) AS derniere_annee FROM paiement_detail WHERE id_box = ? GROUP BY id_box;


SELECT 
    *
FROM locataire;

SELECT 
    p.id_paiement, 
    p.montant AS montant, 
    p.mois, 
    p.annee, 
    p.id_box, 
    IIf(IsNull(tp.montant),0,tp.montant) AS payees, 
    IIf(tp.montant Is Null,p.montant,p.montant-tp.montant) AS reste
FROM paiement AS p 
LEFT JOIN ttl_payee AS tp ON p.id_paiement = tp.id_paiement
WHERE 


SELECT 
    m.id_marchee, 
    box.id_box, 
    ([box].[longueur]*[box].[largeur]) AS volume, 
    m.loyer AS loyer
FROM marchee AS m
INNER JOIN box ON marchee.id_marchee = box.id_marchee;

SELECT 
    *
FROM loyer AS l
WHERE l.id_marchee = ?


SELECT TOP 1 
    l.* 
FROM loyer AS l
WHERE (Year(l.daty) < 2025 OR (Year(l.daty) <= 2025 AND Month(l.daty) <= 6))
AND l.id_marchee= 1
ORDER BY Year(l.daty) DESC, Month(l.daty) DESC;


-- DETAIL PAIEMENT (PAIEMENT NON PAYEE)
-- VIEW paiement_detail
SELECT 
    p.id_paiement AS id_paiement,  
    p.mois AS mois, 
    p.annee AS annee, 
    p.id_contrat AS id_contrat, 
    IIf(IsNull(tp.montant),0,tp.montant) AS payee,
    IIf(IsNull(tp.montant),p.montant_du,p.montant_du-tp.montant) AS reste, 
    c.id_locataire AS id_locataire, 
    c.id_box AS id_box, 
    p.date_echeance AS date_echeance,
    c.debut AS debut
FROM (paiement AS p LEFT JOIN ttl_payee AS tp ON p.id_paiement = tp.id_paiement) 
LEFT JOIN contrats AS c ON p.id_contrat = c.id_contrat
-- WHERE p.payee= False




-- TOTAL LOYER PAYER
-- VIEW ttl_payee

SELECT 
    hp.id_paiement, 
    sum(hp.montant) AS montant,
    sum(hp.penalite) AS penalite
FROM histo_paiement AS hp
GROUP BY hp.id_paiement;



-- valide contrat (sans tenir en compte le jour)
SELECT 
    c.*
FROM contrats AS c
WHERE c.id_locataire = ?
AND c.id_box = ?
AND DateSerial(?, ?, 1) BETWEEN DateSerial(Year(c.debut), Month(c.debut), 1)  AND DateSerial(Year(c.fin), Month(c.fin) + 1, 0);



-- REQUETE contrat aloa indrindra
SELECT 
    c.*
FROM contrats AS c
WHERE c.id_locataire = ?
AND c.debut <= DateSerial(?, ? + 1, 0);


-- REQUETE : Contrats farany pour locataire
SELECT 
    c.*,
    pd.id_paiement
FROM  contrats AS c
LEFT JOIN paiement_detail AS pd ON c.id_contrat = pd.id_contrat
WHERE c.id_locataire = 9
AND (pd.annee = (SELECT MIN(annee) FROM paiement_detail WHERE reste <> 0)
OR (pd.annee = (SELECT MIN(annee) FROM paiement_detail WHERE reste <> 0) 
AND pd.mois = (SELECT MIN(mois) FROM paiement_detail WHERE reste <> 0)));


SELECT 
    pd.*
FROM paiement_detail AS pd 
WHERE pd.reste > 0
HAVING MIN (pd.annee) AND MIN (pd.mois)
GROUP BY pd.id_contrat;


SELECT
    c.*, 
    YEAR(pd.periode) AS annee, 
    MONTH(pd.periode) AS mois
FROM contrats AS c
LEFT JOIN (
    SELECT 
        id_contrat, 
        MAX(DateSerial(annee, mois, 1)) AS periode
    FROM paiement_detail
    GROUP BY id_contrat
) AS pd ON c.id_contrat = pd.id_contrat;



-- REQUETE PERIODE PAIEMENT CONTRAT FARANY
SELECT 
    id_contrat, 
    MAX (DateSerial(annee, mois, 1)) AS periode
FROM paiement_detail
GROUP BY id_contrat



-- REQUETE AFFICHAGE BOX 
SELECT 
    b.*,
    pd.id_locataire AS id_locataire,
    IIf(IsNull(pd.reste), 0, pd.reste) AS reste,
    IIf(IsNull(pd.payee), 0, pd.payee) AS payee
FROM box AS b
LEFT JOIN (
    SELECT 
        -- co.*
        co.id_contrat,
        co.id_locataire,
        co.id_box,
        IIf(IsNull(co.mois), 10, co.mois) AS mois,
        IIf(IsNull(co.annee), 2024, co.annee) AS annee,
        co.reste AS reste,
        co.payee AS payee
    FROM (
        SELECT
            c.id_contrat,
            c.id_locataire,
            c.id_box,
            pd.mois AS mois, 
            pd.annee AS annee, 
            IIf(IsNull(pd.reste), 0, pd.reste) AS reste,
            IIf(IsNull(pd.payee), 0, pd.payee) AS payee
        FROM contrats AS c
        LEFT JOIN paiement_detail AS pd ON c.id_contrat = pd.id_contrat
        WHERE DateSerial(2024, 10, 1) BETWEEN DateSerial(Year(c.debut), Month(c.debut), 1)  AND DateSerial(Year(c.fin), Month(c.fin) + 1, 0)
    ) co
) AS pd 
ON b.id_box = pd.id_box

    WHERE mois = 10 AND annee = 2024


SELECT
    id_contrat,
    id_locataire,
    id_box
FROM contrats
WHERE DateSerial(2025, 1, 1) BETWEEN 
DateSerial(Year(debut), Month(debut), 1)  
AND DateSerial(Year(fin), Month(fin) + 1, 0)


SELECT 
    b.*,
    d.id_locataire,
    d.reste,
    d.payee
FROM box AS b LEFT JOIN (
    SELECT 
        c.id_box,
        c.id_locataire,
        IIf(IsNull(pd.mois), 10, pd.mois) AS mois,
        IIf(IsNull(pd.annee), 2024, pd.annee) AS annee,
        IIf(IsNull(pd.reste), 0, pd.reste) AS reste,
        IIf(IsNull(pd.payee), 0, pd.payee) AS payee
    FROM (
        SELECT
            id_contrat,
            id_locataire,
            id_box
        FROM contrats
        WHERE DateSerial(2024, 10, 1) BETWEEN 
        DateSerial(Year(debut), Month(debut), 1)  
        AND DateSerial(Year(fin), Month(fin) + 1, 0)
    ) AS c LEFT JOIN (
        SELECT 
            *
        FROM paiement_detail
        WHERE mois = 10 AND annee = 2024
    ) AS pd 
    ON c.id_contrat = pd.id_contrat    
) AS d ON b.id_box = d.id_box

SELECT 
    b.*,
    d.id_locataire,
    d.reste,
    d.payee
FROM box AS b LEFT JOIN (
    SELECT 
        c.id_box,
        c.id_locataire,
        Nz(pd.mois, 10) AS mois,
        Nz(pd.annee, 2024) AS annee,
        Nz(pd.reste, 0) AS reste,
        Nz(pd.payee, 0) AS payee
    FROM (
        SELECT
            id_contrat,
            id_locataire,
            id_box
        FROM contrats
        WHERE DateSerial(2024, 10, 1) BETWEEN 
        DateSerial(Year(debut), Month(debut), 1)  
         AND DateSerial(Year(fin), Month(fin) + 1, 0)
    ) AS c LEFT JOIN paiement_detail AS pd  ON c.id_contrat = pd.id_contrat  
    AND pd.mois = 10  
    AND pd.annee = 2024
) AS d ON b.id_box = d.id_box



SELECT 
    c.id_contrat,
    c.id_box,
    c.id_locataire,
    IIf(IsNull(pd.mois), 10, pd.mois) AS mois,
    IIf(IsNull(pd.annee), 2024, pd.annee) AS annee,
    IIf(IsNull(pd.reste), 0, pd.reste) AS reste,
    IIf(IsNull(pd.payee), 0, pd.payee) AS payee
FROM (
    SELECT
        id_contrat,
        id_locataire,
        id_box
    FROM contrats
    WHERE DateSerial(2024, 10, 1) BETWEEN 
    DateSerial(Year(debut), Month(debut), 1)  
    AND DateSerial(Year(fin), Month(fin) + 1, 0)
) AS c LEFT JOIN (
    SELECT 
        *
    FROM paiement_detail
    WHERE mois = 10 AND annee = 2024
) AS pd 
ON c.id_contrat = pd.id_contrat

-- WHERE 


-- TROSA 

SELECT 
    pd.id_contrat AS id_contrat,
    max (DateSerial(annee, mois, 1) ) AS periode,
    sum (pd.reste) AS reste,
    sum (pd.payee) AS payee
FROM paiement_detail AS pd
GROUP BY pd.id_contrat;

SELECT 
    c.*,
    cp.periode AS periode,
    cp.reste AS reste,
    cp.payee AS payee
FROM contrats AS c 
LEFT JOIN (
    SELECT 
        pd.id_contrat AS id_contrat,
        sum (pd.reste) AS reste,
        sum (pd.payee) AS payee
    FROM paiement_detail AS pd
    GROUP BY pd.id_contrat
) AS cp ON c.id_contrat = cp.id_contrat


SELECT
    l.*,
    NZ(lp.reste, 0) AS reste
FROM locataire AS l
LEFT JOIN (
    SELECT
        pd.id_locataire AS id_locataire,
        sum (pd.reste) AS reste,
        sum (pd.payee) AS payee
    FROM paiement_detail AS pd
    GROUP BY pd.id_locataire
) AS lp ON l.id_locataire = lp.id_locataire

SELECT 
    c.id_locataire,
    SUM(cp.reste) AS reste
FROM contrats AS c
LEFT JOIN (
    SELECT 
        pd.id_contrat AS id_contrat,
        sum (pd.reste) AS reste,
        sum (pd.payee) AS payee
    FROM paiement_detail AS pd
    GROUP BY pd.id_contrat
) AS cp ON c.id_contrat = cp.id_contrat
GROUP BY c.id_locataire


SELECT
    pd.id_locataire AS id_locataire,
    sum (pd.reste) AS reste,
    sum (pd.payee) AS payee
FROM paiement_detail AS pd
GROUP BY pd.id_locataire



SELECT 
    cf.id_contrat AS id_contrat,
    cf.debut AS debut,
    cf.fin AS fin,
    cf.id_locataire AS id_locataire,
    cf.id_box AS id_box,
    cf.reglee AS reglee,
    IIf(IsNull(cf.annee), YEAR(cf.debut), cf.annee) AS annee,
    IIf(IsNull(cf.mois), MONTH(cf.debut), cf.mois) AS mois
FROM paiement_contrat_farany AS cf
WHERE id_locataire = 10