# .NetStandard

## Pourquoi passer au format .NetStandard ?

- **Parce que c'est le nouveau format officiel :**

   Le format PCL est amené à disparaitre à plus ou moins long terme et à être remplacé par le .NetStandard     

- **Mise à jour de lib tiers :**

   Beaucoup de libs tiers adoptent le .NetStandard en version 1.3 et suppérieur et ne peuvent plus être mises à jour sur des projets PCL même 111

- **Simplification de la gestion des dépendances :**

   Avec l'ancien format utilisant le fichier 'package.config', nous sommes obligé de déclarer les dépendances transitive. Si A dépend de B et de C alors dans le package.config, A, B et C seront déclarées. Avec les nouveaux formats (project.json ou PackageReference), nous n'avons plus besoin de déclarer les dépendances transitives

- **Facilitation lors des merges :**

   Avec l'ancien format les dépendances sont déclarées dans le fichier 'package.config', et des références vers chaques dll du package sont ajoutées dans le fichier csproj. Dans le cadre de gros projet, cela pose souvant des problème lors des merges et si on ne fait pas bien attention on peut se retrouver avec des incohérences. Le nouveau format définie les dépendance à un seul endroit (project.json ou csproj) et en version simple. Les merges sont donc simplifiés


## Difficultées rencontrées

- **Visual Studio For Mac / Xamarin Studio :**

   VS for mac sait gérer tous ces formats mais ne propose pas forcément de templates de projets ou d'outil de convertion cohérent. Par exemple si on créer un nouveau projet .NetStandard, ça va utiliser les PackagesReferences, mais si on change juste le type de projet de PCL existant vers .NetStandard via les options, ça utilise le fichier project.json

- **Outils en ligne de commande :**

   Visiblement xbuild ne sait pas bien gérer les projets utilisant les PackageReference. En revanche il semble bien gérer les projets utilisant le project.json

- **Tests unitaires :**

   Si beaucoup de lib passent en .NetStandard et .NetCore, NUnit n'a pas encore complètement passé le cap. La version 3.7.0 apporte le .NetStandard sur la lib mais seulement la 3.8 va apporter le support de .NetCore dans le runner 

- **Xamarin Forms :**

   La création de projet .NetStandard avec comme dépendance Xamarin Forms ne fonctionne pas d'emblé, il faut rajouter des configurations spécifiques. De même, la convertion en project.json ou PackageReference n'est pas documentée, et dans VS For Mac les templates de nouveaux projets utilisent toujours l'ancien système


## Les différents formats 

- **Nuget v2 :**
   
   Dans les anciennes versions, les dépendances sont définient en double. Une fois dans le fichier packages.config pour la gestion avec Nuget et une fois dans le fichier csproj pour le link à la compilation. L'IDE s'occupe de garder cette cohérence lors des updates de package. On est donc "obligé" de passer par lui pour les mises à jour de packages ce qui peut prendre énormément de temps. Il ne gère pas non plus les dépendances transitive, donc si on dépend de A qui dépend lui même de B et de C, alors B et C apparait dans les différents fichier. L'IDE s'occupe tout de même d'ajouter les dépendances dans les différents fichier lui même


*package.config :*
```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="Microsoft.CSharp" version="4.0.1" targetFramework="portable45-net45+win8+wpa81" />
  <package id="Serilog" version="2.4.0" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Collections" version="4.0.11" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Dynamic.Runtime" version="4.0.11" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Globalization" version="4.0.11" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Linq" version="4.1.0" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Reflection" version="4.1.0" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Reflection.Extensions" version="4.0.1" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Runtime" version="4.1.0" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Runtime.Extensions" version="4.1.0" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Text.RegularExpressions" version="4.1.0" targetFramework="portable45-net45+win8+wpa81" />
  <package id="System.Threading" version="4.0.11" targetFramework="portable45-net45+win8+wpa81" />
</packages>
```

*project.csproj :*
```xml
  <!-- Extrait de la partie référence -->
  <ItemGroup>
    <Reference Include="Serilog">
      <HintPath>..\packages\Serilog.2.4.0\lib\netstandard1.0\Serilog.dll</HintPath>
    </Reference>
  </ItemGroup>
```


- **Nuget v3 :**

   Dans cette version, les libs sont référencés dans un fichier project.json uniquement, et nuget gère les dépendances transitive, donc dans le cas précédent, seul A apparait.

*project.json :*
```json
{
  "dependencies": {
    "Microsoft.NETCore.Portable.Compatibility": "1.0.1",
    "NETStandard.Library": "1.6.0",
    "Serilog": "2.4.0"
  },
  "frameworks": {
    "netstandard1.4": {}
  }
}
```

- **Nuget v4 :**

   Avec l'intégration à MSBuil, les packages sont référencés directement dans le fichier csproj sous forme de PackageReference. Là encore ça gère les dépendances transitive et seul une version "light" de la dépendance apparait

*project.csproj :*
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard1.4</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Serilog" Version="2.4.0" />
  </ItemGroup>
</Project>
```


## Format PackageReferences

- **Création d'une lib**

   Pour ce type de projet, VS for mac propose un template fonctionnel *Add New Project > .Net Core > Library > .Net Standard Library*

- **Création d'un projet de test XUnit**

   Là encore VS for mac propose un template fonctionnel *Add New Project > .Net Core > Tests > xUnit Test Project*

- ** Création d'un projet de test NUnit**

   Cette fois ci rien n'est proposé. Il faut créer un projet .NetStandard (voir ci-dessus) et modifier le fichier csproj de la façon suivante : 

   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
        <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>netcoreapp1.1</TargetFramework>
        </PropertyGroup>

        <ItemGroup>
            <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.0.0" />
            <PackageReference Include="NUnit" Version="3.6.0" />
            <PackageReference Include="NUnit3TestAdapter" Version="3.8.0-alpha1" />
        </ItemGroup>
    </Project>
   ```

   Le runner est en version alpha et ne fonctionne pas super bien dans VS for mac. J'ai régulièrement des problèmes de process de build qui ne se terminent pas et je n'ai pas réussi à lancer les tests NUnit en mode debug.

- ** Lignes de commande

   **Build** : Pour builder un projet de ce type il faut utiliser msbuild à la place de xbuild.

        xbuild project.sln

   **Lancement des tests** : Il faut utiliser l'outil dotnet

        dotnet restore
        dotnet test project.Tests.csproj
