# System Rejestracji Czasu Pracy — Timesheet

Instrukcja uruchomienia lokalnego projektu (backend ASP.NET Core Web API + frontend HTML/CSS/JS).

## 1\. Wymagane oprogramowanie

|Narzędzie|Wersja|Uwagi|
|-|-|-|
|.NET SDK|9.0 lub nowszy|wymagane przez `AddOpenApi()` w `Program.cs`; sprawdź instalację: `dotnet --version`|
|Edytor / IDE|Visual Studio 2022 (17.10+) lub VS Code|w VS Code zalecane rozszerzenie **C# Dev Kit<br />Live Server**|
|Rozszerzenie REST Client (opcjonalnie)|—|do testowania endpointów z pliku `TimesheetAPI.http` bez Postmana|
|Przeglądarka internetowa|aktualna wersja Chrome / Edge / Firefox|do uruchomienia `index.html`|

Pobierz .NET SDK ze strony: https://dotnet.microsoft.com/download

## 2\. Struktura projektu

```
TimesheetAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── EmployeesController.cs
│   ├── ProjectTasksController.cs
│   ├── TimesheetEntriesController.cs
│   └── ReportsController.cs
├── Models/
│   ├── Employee.cs
│   ├── ProjectTask.cs
│   ├── TimesheetEntry.cs
│   └── EmployeeProjectTask.cs
├── Data/
│   └── AppDbContext.cs
├── Program.cs
├── appsettings.json
└── TimesheetAPI.http

Frontend/
├── index.html
└── Styles/
    └── style.css
```

> \*\*Ważne:\*\* `index.html` ładuje arkusz stylów ze ścieżki względnej `Styles/style.css` (`<link rel="stylesheet" href="Styles/style.css">`). Plik `style.css` musi więc znajdować się w podfolderze `Styles` względem `index.html`.

## 3\. Konfiguracja połączenia z bazą danych

Projekt korzysta z bazy SQLite skonfigurowanej w `Program.cs` przez connection string `DefaultConnection`. Jeśli plik `appsettings.json` nie zawiera jeszcze tego wpisu, dodaj go w katalogu głównym projektu backendowego:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=timesheet.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Plik bazy danych (`timesheet.db`) zostanie utworzony automatycznie w katalogu projektu po wykonaniu migracji (krok 5).

## 4\. Instalacja zależności NuGet

W katalogu projektu backendowego (tam, gdzie znajduje się plik `.csproj`) wykonaj:

```bash
dotnet restore
```

Jeśli pakiety nie zostały jeszcze dodane do projektu, doinstaluj je ręcznie:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.OpenApi
```

## 5\. Utworzenie bazy danych (migracje EF Core)

Zainstaluj globalne narzędzie EF Core CLI (jednorazowo, jeśli nie jest jeszcze zainstalowane):

```bash
dotnet tool install --global dotnet-ef
```

Następnie wygeneruj migrację i utwórz bazę danych:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Po tym kroku w katalogu projektu powinien pojawić się plik `timesheet.db` oraz folder `Migrations/`.

## 6\. Zaufanie certyfikatowi HTTPS (środowisko deweloperskie)

Backend domyślnie nasłuchuje na `https://localhost:7050` (zgodnie z adresem zaszytym w `index.html` i `TimesheetAPI.http`). Aby przeglądarka oraz `fetch()` mogły bez problemu komunikować się z lokalnym serwerem HTTPS, zaufaj deweloperskiemu certyfikatowi .NET:

```bash
dotnet dev-certs https --trust
```

> Sprawdź również plik `Properties/launchSettings.json` — port w profilu `https` powinien być ustawiony na `7050`. Jeśli .NET wygenerował inny port, zmień go w `launchSettings.json` na `7050` albo zaktualizuj zmienną `BASE\_URL` w `index.html` i adres `@TimesheetAPI\_HostAddress` w `TimesheetAPI.http`, tak aby wszystkie trzy miejsca wskazywały ten sam adres.

## 7\. Uruchomienie backendu

W katalogu projektu backendowego:

```bash
dotnet run
```

lub w trybie z automatycznym przeładowaniem po zmianach w kodzie:

```bash
dotnet watch run
```

W Visual Studio Code:

W terminalu wejdź używając komendy cd do folderu w którym jest backend (podstawowo TimesheetAPI)

Następnie wpisz komendę:

dotnet run --launch-profile "https"



Po starcie serwera specyfikacja OpenAPI dostępna jest (w środowisku deweloperskim) pod adresem `https://localhost:7050/openapi/v1.json`.

## 8\. Uruchomienie frontendu

Frontend jest plikiem statycznym i nie wymaga osobnego serwera — wystarczy otworzyć `index.html` bezpośrednio w przeglądarce (dwuklik na pliku lub „Otwórz z” → przeglądarka). Backend ma skonfigurowane CORS (`AllowAnyOrigin`), dzięki czemu żądania `fetch()` z pliku otwartego lokalnie (`file://`) zostaną zaakceptowane.

Alternatywnie można serwować frontend przez prosty serwer statyczny, np. rozszerzenie **Live Server** w VS Code.

## 9\. Dane startowe i logowanie testowe

Baza danych po migracji jest pusta — pracowników należy dodać ręcznie, np. korzystając z pliku `TimesheetAPI.http` (sekcja „1. ZARZĄDZANIE PRACOWNIKAMI I KONTAMI”) lub wysyłając żądania `POST /api/employees`. Plik `.http` zawiera gotowe przykłady, m.in.:

|Login|Hasło|Rola|
|-|-|-|
|`pracownik1`|`asdf`|Employee|
|`admin1`|`asdf`|Admin|

Po dodaniu pracowników zaloguj się w aplikacji frontendowej odpowiednimi danymi, aby zobaczyć panel Pracownika lub panel Administratora.

## 10\. Testowanie API

Wszystkie endpointy można przetestować bez frontendu, korzystając z pliku `TimesheetAPI.http`:

* w VS Code: zainstaluj rozszerzenie **REST Client**, otwórz plik i kliknij „Send Request” nad wybranym żądaniem;
* alternatywnie zaimportuj żądania do Postmana / Insomnia.

## 11\. Reset danych testowych

Aby wyczyścić wpisy czasu pracy, przydziały zadań i zadania projektowe (np. przed kolejną prezentacją projektu), wykonaj żądanie:

```
DELETE https://localhost:7050/api/timesheetentries/clear-dev
```

> Endpoint nie usuwa kont pracowników (tabela `Employees`) — wyłącznie dane „robocze”.

## 12\. Najczęstsze problemy

|Problem|Możliwa przyczyna / rozwiązanie|
|-|-|
|Frontend nie łączy się z API (błąd sieci / CORS w konsoli)|Sprawdź, czy backend działa i czy port w `BASE\_URL` (`index.html`) zgadza się z portem w `launchSettings.json`.|
|Przeglądarka blokuje żądanie HTTPS jako niezaufane|Wykonaj `dotnet dev-certs https --trust` i zrestartuj przeglądarkę.|
|Błąd przy `dotnet ef database update`|Sprawdź, czy zainstalowano `dotnet-ef` globalnie oraz pakiet `Microsoft.EntityFrameworkCore.Design` w projekcie.|
|Logowanie zwraca 401 mimo poprawnych danych|Sprawdź wielkość liter w loginie/haśle — porównanie w `AuthController` jest dokładne (case-sensitive).|



