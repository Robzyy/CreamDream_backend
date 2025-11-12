# CreamDream - Specificații Aplicație

## Prezentare Generală Proiect

CreamDream este o platformă e-commerce completă pentru o gogoșărie care oferă servicii de livrare la domiciliu. Sistemul permite clienților să vizualizeze produse (gogoși, bageli și cafea), să plaseze comenzi și să organizeze livrări. Personalul poate gestiona inventarul, procesa comenzile și urmări livrările.

---

## Istorii Utilizator

### Istorii Utilizator - Client

#### US-1: Înregistrare și Autentificare Utilizator
**Ca** client  
**Vreau să** creez un cont cu numele meu, email și număr de telefon  
**Pentru ca** să pot plasa comenzi și urmări livrările mele

**Criterii de Acceptare:**
- Clientul se poate înregistra cu email, nume și telefon
- Email-ul trebuie să fie unic
- Utilizatorul primește o confirmare
- Utilizatorul se poate conecta cu acreditări

#### US-2: Explorare Produse
**Ca** client  
**Vreau să** văd toate produsele disponibile categorizate după tip (gogoși, bageli, cafea)  
**Pentru ca** să pot decide ce să comand

**Criterii de Acceptare:**
- Produsele afișează nume, descriere, preț și cantitate disponibilă
- Produsele sunt filtrabile după categorie
- Produsele arată informații despre alergeni
- Articolele cu stoc epuizat sunt clar marcate

#### US-3: Creare Comandă
**Ca** client  
**Vreau să** adaug produse la o comandă și să specifc o adresă de livrare  
**Pentru ca** să pot avea articole livrate acasă

**Criterii de Acceptare:**
- Clientul poate selecta mai multe produse cu cantități
- Sistemul calculează prețul total
- Clientul poate stabili o adresă de livrare personalizată
- Comanda este creată cu statutul În Așteptare
- Data livrării poate fi stabilită

#### US-4: Vizualizare Istoric Comenzi
**Ca** client  
**Vreau să** văd toate comenzile mele anterioare și actuale cu staturile acestora  
**Pentru ca** să pot urmări livrările și să recomand articole favorite

**Criterii de Acceptare:**
- Clientul vede o listă cu toate comenzile sale
- Fiecare comandă arată statut, preț total, data comenzii
- Adresa de livrare și data sunt vizibile
- Clientul poate filtra după statut

#### US-5: Urmărire Status Comandă
**Ca** client  
**Vreau să** primesc actualizări despre statutul comenzii mele  
**Pentru ca** să știu când va ajunge livrarea mea

**Criterii de Acceptare:**
- Statutul comenzii este: În Așteptare → În Procesare → În Livrare → Livrat
- Clientul poate vedea statutul curent al comenzii sale
- Data livrării este confirmată înainte de expediție

---

### Istorii Utilizator - Personal/Administrator

#### US-6: Gestionare Produse
**Ca** membru al personalului  
**Vreau să** adaug, editez și șterg produse din catalog  
**Pentru ca** inventarul să fie întotdeauna precis

**Criterii de Acceptare:**
- Personalul poate crea noi produse cu detalii (nume, preț, categorie, descriere, cantitate)
- Personalul poate actualiza informațiile produselor
- Personalul poate șterge produse
- Modificările se reflectă imediat

#### US-7: Vizualizare Toate Comenzile
**Ca** membru al personalului  
**Vreau să** văd toate comenzile clienților indiferent de statut  
**Pentru ca** să pot gestiona îndeplinirea comenzilor

**Criterii de Acceptare:**
- Personalul vede toate comenzile din sistem
- Comenzile pot fi filtrate după statut, client sau dată
- Fiecare comandă arată detalii complete, inclusiv articole

#### US-8: Actualizare Status Comandă
**Ca** membru al personalului  
**Vreau să** actualizez statutul comenzii de la În Așteptare → În Procesare → În Livrare → Livrat  
**Pentru ca** clienții să cunoască progresul comenzilor lor

**Criterii de Acceptare:**
- Personalul poate muta comenzi numai la statusuri următoare valide
- Actualizările de statut sunt marcate cu timp
- Schimbări de statut declanșează notificări necesare

#### US-9: Gestionare Inventar
**Ca** membru al personalului  
**Vreau să** urmăresc cantitățile de produse și să ajustez stocul  
**Pentru ca** clienții să nu poată comanda mai mult decât stocul disponibil

**Criterii de Acceptare:**
- Sistemul previne comandarea mai multor articole decât sunt disponibile
- Personalul poate ajusta manual cantitățile
- Articolele cu stoc scăzut sunt marcate
- Sistemul arată cantitate pentru fiecare produs

#### US-10: Gestionare Utilizatori Autorizați
**Ca** manager  
**Vreau să** atribui roluri și permisiuni membrilor personalului  
**Pentru ca** doar utilizatorii autorizați să poată modifica datele

**Criterii de Acceptare:**
- Managerul poate crea conturi admin/personal
- Diferite roluri au permisiuni diferite
- Permisiunile includ: vizualizare, creare, editare, ștergere pe diverse resurse
- Schimbări de rol au efect imediat

---

## Diagrama Bază de Date

```
┌─────────────────────────────────────────────────────────────────────┐
│                      BAZA DE DATE CREAMDREAM                        │
└─────────────────────────────────────────────────────────────────────┘

                              ┌──────────────┐
                              │   UTILIZATORI│
                              ├──────────────┤
                              │ id (PK)      │
                              │ nume         │
                              │ email (UQ)   │
                              │ telefon      │
                              │ creatLa      │
                              └──────┬───────┘
                                     │
                     ┌───────────────┴───────────────┐
                     │                               │
        ┌────────────▼──────────────┐   ┌───────────▼──────────────┐
        │      CLIENTI             │   │   UTILIZATORI_AUTORIZATI │
        ├──────────────────────────┤   ├──────────────────────────┤
        │ id (PK, FK→Utilizatori)  │   │ id (PK, FK→Utilizatori)  │
        │ adresa                   │   │ mascaraRoluri (roluri)   │
        │                          │   │                          │
        └────────────┬─────────────┘   └──────────────────────────┘
                     │
                     │ 1:N
        ┌────────────▼──────────────┐
        │      COMENZI             │
        ├──────────────────────────┤
        │ id (PK)                  │
        │ idClient (FK)            │
        │ dataComanda              │
        │ pretTotal                │
        │ statut (enum)            │
        │ adresaLivrare            │
        │ dataLivrare              │
        └────────────┬──────────────┘
                     │
                     │ 1:N
        ┌────────────▼──────────────┐
        │    ARTICOLE_COMENZI      │
        ├──────────────────────────┤
        │ id (PK)                  │
        │ idComanda (FK)           │
        │ idProdu (FK)             │
        │ cantitate                │
        │ pretUnitar               │
        └────────────┬──────────────┘
                     │
                     │ N:1
        ┌────────────▼──────────────┐
        │      PRODUSE             │
        ├──────────────────────────┤
        │ id (PK)                  │
        │ nume                     │
        │ descriere                │
        │ pret                     │
        │ categorie (enum)         │
        │ cantitate                │
        │ creatLa                  │
        └──────────────────────────┘

ENUMERĂRI:
- CategorieProdu: Gogoasă (0), Bagel (1), Cafea (2)
- StatutComanda: În Așteptare (0), În Procesare (1), În Livrare (2), Livrat (3), Anulat (4)
- Roluri (mascara): Administrator, Personal, Utilizator
```

---

## Diagrame de Activitate

### Diagrama de Activitate: Fluxul Comenzii Clientului

```
┌─────────────────────────────────────────────────────────────────────┐
│                  FLUXUL COMENZII CLIENTULUI                         │
└─────────────────────────────────────────────────────────────────────┘

ÎNCEPUT
  │
  ▼
┌─────────────────────┐
│ Explorează Produse  │
│ după Categorie      │
└────────┬────────────┘
         │
         ▼
    ┌─────────┐
    │ Produs  │ ─── Nu ──┐
    │ Găsit?  │          │
    └────┬────┘          │
         │ Da            │
         ▼               │
    ┌──────────────┐     │
    │ Vizualizează │     │
    │ Detalii &    │     │
    │ Verifică     │     │
    │ Disponibilitate│   │
    └────┬─────────┘     │
         │               │
         ▼               │
    ┌──────────────┐     │
    │ Adaugă la    │     │
    │ Comandă?     │     │
    └────┬─────────┘     │
         │ Nu            │
         │◄──────────────┘
         │ Da
         ▼
    ┌──────────────┐
    │ Selectează   │
    │ Cantitate    │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Adauga        │
    │ Articol la    │
    │ Coș           │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Mai Multe     │
    │ Articole?     │
    └────┬─────────┘
         │
    Da──┤──Nu
        │
        ▼
    ┌──────────────┐
    │ Revizuiește  │
    │ Comandă      │
    │ Preț Total   │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Introdu/     │
    │ Confirmă     │
    │ Adresa       │
    │ Livrare      │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Setează Data │
    │ & Ora        │
    │ Livrare      │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Confirmă     │
    │ Comandă      │
    └────┬─────────┘
         │
         ▼
    ┌──────────────────┐
    │ Comandă Creată   │
    │ Statut: În       │
    │ Așteptare        │
    └────┬─────────────┘
         │
         ▼
    ┌──────────────────┐
    │ Trimite Email/   │
    │ SMS de           │
    │ Confirmare       │
    └────┬─────────────┘
         │
         ▼
      SFÂRȘIT
```

### Diagrama de Activitate: Fluxul Procesării Comenzii de Personal

```
┌─────────────────────────────────────────────────────────────────────┐
│              FLUXUL PROCESĂRII COMENZII DE PERSONAL                 │
└─────────────────────────────────────────────────────────────────────┘

ÎNCEPUT
  │
  ▼
┌────────────────────┐
│ Vizualizează       │
│ Coada de Comenzi   │
│ (Statut: În        │
│ Așteptare)         │
└────────┬───────────┘
         │
         ▼
    ┌──────────────┐
    │ Selectează   │
    │ Comandă de   │
    │ Procesat     │
    └────┬─────────┘
         │
         ▼
    ┌──────────────────┐
    │ Verifică Inventar│
    │ pentru Toate     │
    │ Articolele       │
    └────┬─────────────┘
         │
         ▼
    ┌──────────────┐
    │ Stoc         │
    │ Suficient?   │
    └────┬─────────┘
         │
    Nu──┤
        ▼
    ┌──────────────┐
    │ Marchează    │
    │ Comandă ca   │
    │ Anulată      │
    │ (Stoc Scazut)│
    └────┬─────────┘
         │
    Da──┼──────┐
         │      │
         ▼      │
    ┌──────────────┐
    │ Actualizează │
    │ Statut       │
    │ → În Procesare│
    └────┬─────────┘
         │
         │◄─────┘
         ▼
    ┌──────────────┐
    │ Reduce       │
    │ Cantități    │
    │ Stoc         │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Pregătește   │
    │ Pachet       │
    │ Articole     │
    └────┬─────────┘
         │
         ▼
    ┌──────────────────┐
    │ Actualizează     │
    │ Statut           │
    │ → În Livrare     │
    └────┬─────────────┘
         │
         ▼
    ┌──────────────┐
    │ Expediază la │
    │ Livrare      │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Confirmare   │
    │ Livrare      │
    │ Primită      │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Actualizează │
    │ Statut       │
    │ → Livrat     │
    └────┬─────────┘
         │
         ▼
    ┌──────────────┐
    │ Trimite      │
    │ Notificare   │
    │ Client       │
    └────┬─────────┘
         │
         ▼
      SFÂRȘIT
```

### Diagrama de Activitate: Fluxul Gestionării Produselor

```
┌─────────────────────────────────────────────────────────────────────┐
│                 FLUXUL GESTIONĂRII PRODUSELOR                       │
└─────────────────────────────────────────────────────────────────────┘

ÎNCEPUT
  │
  ▼
┌────────────────────┐
│ Administrator      │
│ Vizualizează Lista │
│ de Produse         │
└────────┬───────────┘
         │
         ▼
    ┌──────────────┐
    │ Alege Acțiune│
    └────┬─────────┘
         │
    ┌────┼────┬─────┐
    │    │    │     │
  Arata Adaug Edita Sterge
    │    │    │     │
    └─┬──┼────┼──┬──┘
      │  │    │  │
      │  │    │  └─────────────┐
      │  │    │                │
      ▼  ▼    ▼                ▼
   ┌──────┐ ┌──────┐       ┌──────────┐
   │Arata │ │Introdu│       │Confirmă  │
   │Detalii│ │Detalii│       │Ștergere  │
   │Produs │ │Nume   │       │(Verifică │
   └──────┘ │Preț   │       │Comenzi   │
      │     │Categor│       │Asociate) │
      │     │Descrip│       └──────┬───┘
      │     │Cant   │              │
      │     │       │              ▼
      │     └───┬───┘         ┌──────────┐
      │         │             │Șterge    │
      │         ▼             │Produs    │
      │     ┌──────────┐      └──────┬───┘
      │     │Validează │             │
      │     │Intrare   │             │
      │     └───┬──────┘             │
      │         │                    │
      │         ▼                    │
      │     ┌──────────┐             │
      │     │Salvează  │             │
      │     │în Baza   │             │
      │     │Date      │             │
      │     └───┬──────┘             │
      │         │                    │
      └─────┬───┴────────────────────┘
            │
            ▼
    ┌──────────────────┐
    │ Actualizează     │
    │ Catalogul de     │
    │ Produse          │
    └────┬─────────────┘
         │
         ▼
    ┌──────────────┐
    │ Mai Multe    │
    │ Acțiuni?     │
    └────┬─────────┘
         │
    Da──┤──Nu
         │  │
         └──┼────┐
            │    │
            ▼    ▼
          Buclă SFÂRȘIT
```

---

## Prezentare Generală Puncte Finale API

### Puncte Finale Client

| Metod | Punct Terminal | Descriere |
|--------|----------|-------------|
| GET | `/api/produse` | Obține toate produsele |
| GET | `/api/produse/{id}` | Obține produs după ID |
| GET | `/api/clienti/{id}` | Obține profil client |
| POST | `/api/comenzi` | Creează comandă nouă |
| GET | `/api/comenzi/client/{idClient}` | Obține comenzile clientului |
| GET | `/api/comenzi/{id}` | Obține detalii comandă |
| GET | `/api/comenzi/statut/{statut}` | Obține comenzi după statut |
| POST | `/api/articolecomezi` | Adauga articol la comandă |
| GET | `/api/articolecomezi/{id}` | Obține articole comandă |

### Puncte Finale Personal/Administrator

| Metod | Punct Terminal | Descriere |
|--------|----------|-------------|
| POST | `/api/produse` | Crează produs |
| PUT | `/api/produse/{id}` | Actualizează produs |
| DELETE | `/api/produse/{id}` | Șterge produs |
| PUT | `/api/comenzi/{id}` | Actualizează statut comandă |
| DELETE | `/api/comenzi/{id}` | Anulează comandă |
| POST | `/api/utilizatoriautorizati` | Creează utilizator autorizat |
| GET | `/api/utilizatoriautorizati` | Listează utilizatori autorizați |
| PUT | `/api/utilizatoriautorizati/{id}` | Actualizează roluri utilizator |

---

## Stiva Tehnologică

- **Backend**: C# .NET 8.0
- **Baza de Date**: PostgreSQL 16
- **ORM**: Entity Framework Core 8.0
- **Documentație API**: Swagger/OpenAPI
- **Implementare**: Docker & Docker Compose
- **Arhitectură**: Pe straturi (Controllers → Services → Repository → Database)

---

## Rezumat Modele Date

### Entități Core

1. **Utilizator** - Entitate utilizator de bază cu email, nume, telefon
2. **Client** - Extinde Utilizator, adaugă adresă de livrare
3. **UtilizatorAutorizat** - Extinde Utilizator, adaugă permisiuni rol
4. **Produs** - Articole magazin (gogoși, bageli, cafea) cu inventar
5. **Comandă** - Comenzi client cu informații livrare
6. **ArticolComandă** - Articole de linie în comenzi

### Relații Cheie

- **Client** 1:N **Comandă** (client are mai multe comenzi)
- **Comandă** 1:N **ArticolComandă** (comandă are mai multe articole)
- **Produs** 1:N **ArticolComandă** (produs poate fi în mai multe comenzi)
- **Utilizator** 1:1 **Client** sau **UtilizatorAutorizat**

---

## Îmbunătățiri Viitoare

- [ ] Integrare procesare plăți
- [ ] Notificări prin email pentru statut comandă
- [ ] Notificări prin SMS pentru livrări
- [ ] Program loialitate/recompense
- [ ] Tablă de bord analizie avansată
- [ ] Limitare rate API
- [ ] Autentificare/Autorizare (JWT tokens)
- [ ] Recenzii și evaluări utilizatori
- [ ] Gestionare meniu sezonier
- [ ] Optimizare program livrare
