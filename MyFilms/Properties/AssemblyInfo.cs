using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MediaPortal.Common.Utils;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
[assembly: AssemblyTitle("MyFilms")]
[assembly: AssemblyDescription("MyFilms Movie Plugin")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("MediaPortal")]
[assembly: AssemblyCopyright("Copyright © 2011")]
[assembly: AssemblyTrademark("Zebons, Guzzi")]
[assembly: AssemblyCulture("")]


// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten. Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("c7d55fd9-ab19-4dcf-86f9-71c28273898b")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
//[assembly: AssemblyVersion("5.0.2.1372")]
//[assembly: AssemblyFileVersion("5.0.2.1372")]
//[assembly: NeutralResourcesLanguageAttribute("")]

[assembly: CompatibleVersion("1.3.100.0", "1.1.6.27644")]
[assembly: UsesSubsystem("MP.SkinEngine")]
[assembly: UsesSubsystem("MP.Config")]
[assembly: UsesSubsystem("MP.Players.Video")]
[assembly: UsesSubsystem("MP.DB")]
