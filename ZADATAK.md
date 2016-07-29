# NuGet Updater
U tvrtci koristimo zajednički projekt "Acceleratio.Common" koji se koristi u ostalim projektima za potrebe dijeljenja zajedničkog koda. Ovaj projekt smo zapakirali i referencirali u ostalim projektima kroz NuGet paket.
NuGet update kroz Visual Studio nam nije radio kako treba (zbog problematičnog NuGet managera u Visual Studio 2013), na projektima se nisu reference postavljale dobro te automatski restore nakon povučenih promjena na drugom računalu nije radio dobro.

Zato smo napravili alat koji:

1. Updatea sve projekte u solutionu koristeći binary NuGet.exe (https://dist.nuget.org/index.html). 
Međutim, pojavila se potreba za updateanjem NuGet paketa na specifične verzije Acceleratio.Common NuGet paketa, što smo implementirali u NuGet source kodu te kompajlirali svoj NuGet.exe.
Naš alat koristi taj modificirani NuGet.exe koji hostamo mi (downloada se prilikom pokretanja aplikacije).

2. Uploada sve datoteke iz packages foldera na source control (jer restore ovih paketa nije radio dobro, tako da bi se datoteke distribuirale kroz source control bez potrebe za njihovim preuzimanjem kroz restore). Postoji opcija da se prilikom svakog updatea kroz alat pobrišu starije verzije Acceleratio.Common paketa iz packages foldera na source controlu.

# Opis alata
Alat izgleda ovako:

![alt tag](https://www.dropbox.com/s/4j0qgjn204f69yd/updater.png?raw=1)

Elementi sučelja (u redosljedu konfiguriranja):

1.	NuGet repository - repozitorij sa NuGet paketima. Acceleratio koristi repozitorij na adresi http://nuget.acceleratio.hr/nuget/
2.	Browse for solution directory - odabir root solution foldera, iz čijih će se podfoldera (rekurzivno) učitati sln datoteke u Solutions listu u sučelju.
3.	Root directory - odabrani root directory.
4.	Package(s) - nakon odabira root foldera ili klikom na "Load" pokraj NuGet Repository kontrole, učitat će se checkbox lista paketa iz repozitorija u ovu kontrolu. Potrebno je odabrati koje pakete želimo updateati.
5.	Package version - verzija paketa koji se updatea. U slučaju da se odabere više paketa, uzima se zajednička verzija odabranih paketa (presjek).
6.	Latest - označiti ako želimo pakete updateati na zadnju verziju, inače odznačiti i odabrati željenu verziju.
7.	Advanced Options: Upload files to packages folder on source control - označiti ako želimo sve datoteke paketa iz packages foldera uplodati na TFS (potrebno jer NuGet restore ne radi dobro u nekim situacijama).
8.	Advanced Options: Delete older files from packages folder on source control (this could take a while) - pobriše stare foldere paketa iz packages foldera na TFS-u (nije potrebno, ali poželjno jer se zna tih foldera nakupiti na TFS-u).

Primjer jednostavnog use-case za update Acceleratio.Common na zadnju verziju:

1.	Odabrati Browse for solution directory i odabrati root folder, npr. C:\Projects
2.	Označiti solutione u listi u kojima želimo updateati Acceleratio.Common, u našem slučaju to su Common.UserControls.sln i TSL.sln
3.	Kliknuti na Update selected solutions. Pričekati da alat završi update.
4.	Ako je sve prošlo OK, commitati promjene kroz Team Explorer u Visual Studiu.

# Zadatak

Potrebno je proučiti source kod sa sljedećeg linka:
https://www.dropbox.com/s/nacr02ycrn1vtes/Acceleratio.Common.Updater.zip?dl=1

Solution bi se trebao pokretati iz Visual Studio Community 2015 bez ikakvih podešavanja.

Potrebno je:

1. Refaktorirati kod koliko je to moguće u raspoloživom vremenu, npr. premjestiti kod iz Form1.cs, srediti razbacane stringove, pathove, napraviti potrebne klase za iste itd. – sve po vlastitom nahođenju što mislite da bi se trebalo ili da se može napraviti. Imajte na umu da je alat originalno pisan da se što prije završi i nije bilo predviđeno da se taj kod nadograđuje, te ga je zato potrebno srediti.

2. Izbaciti hardkodiran URL NuGet repository TextBox kontrole (http://nuget.acceleratio.hr/nuget). Međutim, jednom kad se URL unese, kod ponovnog pokretanja programa URL se mora automatski unijeti u TextBox kontrolu. Perzistenciju (spremanje) rješiti kroz Windows Registry (proučiti koja je najbolja praksa za spremanje podataka u registry za portable aplikacije).

3. Potrebno je NuGet.exe koji se trenutno dohvaća iz našeg Dropbox repozitorija (URL se nalazi u kodu) postaviti tako da se NuGet.exe uploada u folder unutar GitHub repositoriju i dohvaća iz istog na isti način kao preko Dropbox linka (više o GitHubu u točki 4).

4. Kad su prethodne točke napravljene, potrebno je postaviti projekt u naš GitHub repozitorij koji se nalazi na adresi: https://github.com/Acceleratio/NuGet-Updater

5. Potrebno je napisati uredan i gramatički točan README.md (na engleskom jeziku) kako bi vjerno reprezentirao svrhu alata. 

Dana su potrebna prava osobi koja rješava zadatak da se kod može pushati na GitHub.

