Fitnesz App README
Üdvözlünk a Fitnesz App projektjében! Ez az alkalmazás segít a felhasználóknak edzőtermeket keresni és kapcsolatba lépni személyi edzőkkel.
Az app emellett képes lesz posztokat megjeleníteni és email-t küldeni.

Projekt áttekintés
A Fitnesz App egy modern webalkalmazás, amely segít a felhasználóknak könnyedén megtalálni a legközelebbi edzőtermeket és kapcsolatba lépni a személyi edzőkkel.
Az alkalmazás funkciói közé tartozik:

-Edzőtermek keresése
-Személyi edzőkkel való kapcsolatfelvétel
-Posztok megjelenítése
-E-mailek küldése

Telepítés és futtatás
-Backend
A backend az alkalmazás adatkezeléséért és a szerveroldali funkciókért felelős.
A backend kódját a backend branch tartalmazza.
Az adatbázis is itt található, amely a következő táblákat tartalmazza:

Felhasználók: Információkat tartalmaz a felhasználókról.
Posztok: A felhasználók számára releváns posztok és hírek.
Edzőtermek: A rendszerbe felvitt edzőtermek adatait tartalmazza.

A backend telepítése és futtatása a következő lépésekben történik:

Klónozd a projektet:

git clone https://github.com/yourusername/fitness-app.git
Navigálj a backend mappába és telepítsd a szükséges függőségeket:

cd backend
npm install
Állítsd be az adatbázist. A projekt a MySQL adatbázist használja. Készíts egy új adatbázist és futtasd az adatbázis létrehozásához szükséges SQL fájlokat.

Indítsd el a backend szervert:

npm start
A backend most elérhető lesz a http://localhost:3000 címen.

Frontend
A frontend React alapú, és képes az adatokat lekérni a backend API-ról. A frontend telepítése és futtatása a következő lépésekben történik:

Navigálj a frontend mappába:

cd frontend
Telepítsd a szükséges függőségeket:

npm install
Indítsd el a frontend szervert:

npm start
A frontend most elérhető lesz a http://localhost:3001 címen.

Funkciók
Edzőtermek keresése
A felhasználók képesek lesznek az alkalmazásban edzőtermeket keresni a környékükön. Az alkalmazás az edzőtermek helyszíni adatai és nyitvatartása alapján segíti a keresést.

Személyi edzőkkel való kapcsolatfelvétel
A felhasználók személyi edzőkkel is kapcsolatba léphetnek az alkalmazáson keresztül. Az edzők profiljai tartalmazzák elérhetőségeiket és specialitásaikat.

Posztok megjelenítése
Az alkalmazás képes lesz posztokat megjeleníteni a felhasználók számára. A posztok tartalmazhatnak híreket, edzéssel kapcsolatos tippeket, promóciókat stb.

E-mailek küldése
A felhasználók képesek lesznek e-maileket küldeni a személyi edzőknek az alkalmazáson keresztül. Az e-mailek SMTP segítségével kerülnek küldésre.

Használat
Edzőtermek keresése: Az edzőtermek listájában kereshetsz a városodban található edzőtermek között.

Személyi edzőkkel való kapcsolatfelvétel: Az edzők profilján található kapcsolati információk segítségével kapcsolatba léphetsz velük,
vagy az oldalon található email küldő funkcióval küldhetsz nekik emailt.

Posztok olvasása: Az aktuális posztokat az alkalmazás főoldalán találod, és a legfrissebb híreket olvashatod.

E-mailek küldése: Az alkalmazás lehetőséget biztosít arra, hogy közvetlenül küldj e-maileket az edzőknek.

Fejlesztői információk
A backend az Entityframework keretrendszert használja, és MySQL adatbázist alkalmaz.
A frontend a React könyvtárra épül, és a REST API-val kommunikál a backenddel.

Kérdés vagy segítség esetén bátran keress meg minket! :)
