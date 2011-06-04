using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using MediaPortal.Common.Utils;

//using System.Runtime.CompilerServices;

// Les informations générales relatives à un assembly dépendent de 
// l'ensemble d'attributs suivant. Changez les valeurs de ces attributs pour modifier les informations
// associées à un assembly.
[assembly: AssemblyTitle("MyFilms")]
[assembly: AssemblyDescription("MyFilms Movie Plugin")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("MediaPortal")]
[assembly: AssemblyCopyright("Copyright © 2011")]
[assembly: AssemblyTrademark("Zebons, Guzzi")]
[assembly: AssemblyCulture("")]

// L'affectation de la valeur false à ComVisible rend les types invisibles dans cet assembly 
// aux composants COM. Si vous devez accéder à un type dans cet assembly à partir de 
// COM, affectez la valeur true à l'attribut ComVisible sur ce type.
[assembly: ComVisible(true)]

// Le GUID suivant est pour l'ID de la typelib si ce projet est exposé à COM
[assembly: Guid("c7d55fd9-ab19-4dcf-86f9-71c28273898b")]

// Les informations de version pour un assembly se composent des quatre valeurs suivantes :
//
//      Version principale
//      Version secondaire 
//      Numéro de build
//      Révision
//
[assembly: AssemblyVersion("5.0.1.939")]
[assembly: AssemblyFileVersion("5.0.1.939")]
[assembly: NeutralResourcesLanguageAttribute("")]
#if MP11
#else
//[assembly: CompatibleVersion("1.1.7.0", "1.1.6.0")]
[assembly: CompatibleVersion("1.1.7.27830", "1.1.6.27644")]
[assembly: UsesSubsystem("MP.SkinEngine")]
[assembly: UsesSubsystem("MP.Config")]
[assembly: UsesSubsystem("MP.Players")]
[assembly: UsesSubsystem("MP.DB")]
#endif
