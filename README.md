# .NetStandard

## Pourquoi passer au format .NetStandard ?

- **Parce que c'est le nouveau format officiel :**

   Le format PCL avec package.config est amené à disparaitre à plus ou moins long terme et à être remplacé par le .NetStandard et le format PackageReference directement dans le csproj 

- **Mise à jour de lib tiers :**

   Beaucoup de libs tiers adoptent le .NetStandard en version 1.3 et suppérieur et ne peuvent plus être mises à jour sur des projets PCL

- **Simplification de la gestion des dépendances :**

   Avec l'ancien format utilisant le fichier 'package.config', nous sommes obligé de déclarer les dépendances transitives. Si A dépend de B et de C alors dans le package.config, A, B et C seront déclarées. Avec les nouveaux formats (project.json ou PackageReference), nous n'avons plus besoin de déclarer les dépendances transitives

- **Facilitation lors des merges :**

   Avec l'ancien format les dépendances sont déclarées dans le fichier 'package.config', et des références vers chaques dll du package sont ajoutées dans le fichier csproj. Dans le cadre de gros projets, cela pose souvant des problèmes lors des merges et si on ne fait pas bien attention on peut se retrouver avec des incohérences. Les nouveaux formats définie les dépendances à un seul endroit (project.json ou csproj) et en version simple. Les merges sont donc simplifiés


## Difficultées rencontrées

- **Visual Studio For Mac / Xamarin Studio :**

   VS for mac sait gérer tous ces formats mais ne propose pas forcément de templates de projets ou d'outil de convertion cohérent. Par exemple si on créer un nouveau projet .NetStandard, ça va utiliser le format PackagesReferences, mais si on change juste le type d'un projet PCL existant vers .NetStandard via les options, ça utilise le format project.json

- **Outils en ligne de commande :**

   Visiblement xbuild ne sait pas bien gérer les projets utilisant le format PackageReference. En revanche il semble bien gérer les projets utilisant le project.json

- **Tests unitaires :**

   Si beaucoup de lib passent en .NetStandard et .NetCore, NUnit n'a pas encore complètement passé le cap. La version 3.7.0 apporte le .NetStandard sur la lib mais seulement la 3.8 va apporter le support de .NetCore dans le runner 

- **Xamarin Forms :**

   La création de projet .NetStandard avec comme dépendance Xamarin Forms ne fonctionne pas d'emblé, il faut rajouter des configurations spécifiques. De même, la convertion en project.json ou PackageReference n'est pas documentée, et dans VS For Mac les templates de nouveaux projets utilisent toujours l'ancien système


## Les différents formats 

- **Nuget v2 :**
   
   Dans les anciennes versions, les dépendances sont définient en double. Une fois dans le fichier packages.config pour la gestion avec Nuget et une fois dans le fichier csproj pour le link à la compilation. L'IDE s'occupe de garder cette cohérence lors des updates de package. On est donc "obligé" de passer par lui pour les mises à jour de packages ce qui peut prendre énormément de temps. Il ne gère pas non plus les dépendances transitives, donc si on dépend de A qui dépend lui même de B et de C, alors B et C apparaissent dans les différents fichiers. L'IDE s'occupe tout de même d'ajouter les dépendances transitives dans ces fichiers.


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

   Avec l'intégration à MSBuil, les packages sont référencés directement dans le fichier csproj sous forme de PackageReference. Là encore ça gère les dépendances transitives et seule une version "light" de la dépendance apparait.

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

## Format project.json

A noter que ce format est maintenant déprécié depuis Visual Studio For Mac / Visual Studio 2017, et est remplacé par le format PackageReferences (voir ci après) 

- **Création d'une lib**

   VS for mac ne propose pas de template pour le faire. Il faut créer une lib PCL puis la convertir en .NetStandard via *Options > Build > General > Target Framework* et sélectionner netstandard1.4 par exemple.
   
- **Création d'une lib utilisant Xamarin Forms**

   Xamarin Forms n'a pas encore été porté en .NetStandard, on ne peut donc pas l'ajouter si facilement dans les dépendances. Il faut rajouter une configuration dans le project.json pour pouvoir ajouter Xamarin Forms dans un projet de ce type

```json
{
  "dependencies": {
    "Microsoft.NETCore.Portable.Compatibility": "1.0.1",
    "NETStandard.Library": "1.6.0",
    "Xamarin.Forms": "2.3.4.247"
  },
  "frameworks": {
    "netstandard1.4": {
      "imports": "portable-net45+win8+wpa81+wp8"
    }
  }
}
```


- **Création d'un projet de test XUnit**

   La création de projet XUnit sous ce format là n'est plus proposée par l'IDE et je n'ai pas réussit à les faire fonctionner avec une configuration qui été sensé marcher. Étant donné que ce format est déprécié je n'ai pas plus insisté

- **Création d'un projet de test NUnit**

   Il y avait un projet de runner ([NUnit 3 Test Runner for .NET Core](https://github.com/nunit/dotnet-test-nunit)) pour ce format mais il n'a pas été finalisé et a été abandonné pour le nouveau format PackageReference. [See Rob Prouse Post](http://www.alteridem.net/2016/06/18/nunit-3-testing-net-core-rc2/). Je n'ai pas non plus réussit à faire fonctionner un sample.

- **Création d'un projet Application Mobile + Xamarin Forms**

  1. Shared : 
      
      Il faut commencer par créer un projet *Blank Forms App*. Si on convertie en projet .NetStandard (voir ci-dessus) on va avoir une erreur car *Xamarin.Forms* actuellement présent dans le projet n'est pas compatible. Le plus simple est de supprimer la dépendance *Xamarin.Forms*, de convertir le projet comme ci-dessus puis de rajouter *Xamarin.Forms*
  
  2. Android : 

      VS for mac ne propose rien pour le faire depuis l'IDE. Il faut le faire à la main. Pour cela il faut supprimer toutes les références à *Xamarin.Forms* dans le csproj, et ajouter le fichier project.json

Supprimer les lignes référencant une dll provenant du répertoire *../packages/* :
```xml
<Reference Include="Xamarin.Android.Support.v4">
  <HintPath>..\packages\Xamarin.Android.Support.v4.23.3.0\lib\MonoAndroid403\Xamarin.Android.Support.v4.dll</HintPath>
</Reference>
[...]
  ```

Supprimer les imports de targets provenant du même dossier :
```xml
  <Import Project="..\packages\Xamarin.Forms.2.3.4.247\build\portable-win+net45+wp80+win81+wpa81+MonoAndroid10+Xamarin.iOS10+xamarinmac20\Xamarin.Forms.targets" Condition="Exists('..\packages\Xamarin.Forms.2.3.4.247\build\portable-win+net45+wp80+win81+wpa81+MonoAndroid10+Xamarin.iOS10+xamarinmac20\Xamarin.Forms.targets')" />
```

Ajouter le fichier projet.json suivant :
```json
{
  "dependencies": {
    "Xamarin.Forms": "2.3.4.247"
  },
  "frameworks": {
    "MonoAndroid,Version=v7.1": {}
  },
  "runtimes": {
    "win": {}
  }
}
```

   3. iOS

      Comme pour android il faut supprimer les références à *Xamarin.Forms* dans le fichier *csproj* puis ajouter le fichier *project.json* suivant :

```json
{
  "dependencies": {
    "Xamarin.Forms": "2.3.4.247"
  },
  "frameworks": {
    "Xamarin.iOS,Version=v1.0": {}
  },
  "runtimes": {
    "win": {},
    "win-x86": {}
  }
}
```

      

- **Outils en ligne de commande**

   *Build* : Pour builder un projet de ce type on peut utiliser xbuild ou msbuild.

        xbuild project.sln

   *Lancement des tests* : Il faut utiliser l'outil dotnet

        dotnet restore
        dotnet test project.Tests.csproj


## Format PackageReferences

- **Création d'une lib**

   Pour ce type de projet, VS for mac propose un template fonctionnel *Add New Project > .Net Core > Library > .Net Standard Library*

- **Création d'une lib utilisant Xamarin Forms**

   Xamarin Forms n'a pas encore été porté en .NetStandard, on ne peut donc pas l'ajouter si facilement dans les dépendances. Il faut ajouter une configuration dans le csproj pour que ça fonctionne. Il faut ajouter le 'PackageTargetFallback'

   ```xml
   <Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
      <TargetFramework>netstandard1.4</TargetFramework>
      <PackageTargetFallback>portable-net45+win8+wpa81+wp8</PackageTargetFallback>
      <DebugType>full</DebugType>
    </PropertyGroup>

    <ItemGroup>
      <PackageReference Include="Xamarin.Forms" Version="2.3.4.247" />
    </ItemGroup>
  </Project>

   ```


- **Création d'un projet de test XUnit**

   Là encore VS for mac propose un template fonctionnel *Add New Project > .Net Core > Tests > xUnit Test Project*

- **Création d'un projet de test NUnit**

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

   Le runner est en version alpha et ne fonctionne pas super bien dans VS for mac. J'ai régulièrement des problèmes de process de build qui ne se terminent pas et je n'ai pas réussi à lancer les tests NUnit en mode debug. De plus je n'ai pas vu de fichier de résultat des tests dans TestResult.xml non plus

- **Création d'un projet Application Mobile + Xamarin Forms**

On va commencer par créer une nouvelle application en utilisant le template Blank Forms App
On va ensuite convertir la partie PCL en procédant comme pour le format project.json
Ensuite on va convertir les projets natifs
Suppression des packages config
Ajout des     :
<CopyNuGetImplementations>true</CopyNuGetImplementations>
<RestoreProjectStyle>PackageReference</RestoreProjectStyle>

Lancer msbuild /t:restore
Builder avec l'ide

- **Outils en ligne de commande**

   *Build* : Pour builder un projet de ce type il faut utiliser msbuild à la place de xbuild.

        msbuild project.sln

   *Lancement des tests* : Il faut utiliser l'outil dotnet

        dotnet restore
        dotnet test project.Tests.csproj


## Références 

- https://oren.codes/2016/02/08/project-json-all-the-things/
- https://oren.codes/2017/04/23/using-xamarin-forms-with-net-standard-vs-2017-edition/
- https://docs.microsoft.com/en-us/dotnet/core/tutorials/using-on-mac-vs-full-solution
- http://blog.nuget.org/20170316/NuGet-now-fully-integrated-into-MSBuild.html