Imports MediaPortal.Configuration

Namespace My

    ' The following events are availble for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        <STAThread()> _
        Sub Main()

        End Sub

        Private Sub MyApplication_ShutDown(ByVal Sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown

        End Sub

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            'Dim Settings As New AntSettings

            'Read Dir for Logging, if 2nd Commandlineparameter is present
            Dim LogDirectoryParam As String = ""
            Dim CommandlineString As String = ""
            CommandlineString = Application.CommandLineArgs.ToList().Aggregate(CommandlineString, Function(current, commandLineArg) current + (commandLineArg.ToString() & " "))
            If My.Application.CommandLineArgs.Count > 1 Then
                If My.Application.CommandLineArgs.Item(1) = "LogDirectory" And Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config).ToString) Then
                    LogDirectoryParam = Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\log"
                Else
                    LogDirectoryParam = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\" + My.Application.CommandLineArgs.Item(1)
                End If
            Else
                If (Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\log")) Then
                    CurrentLogDirectory = Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\log\AMCUpdater.log"
                Else
                    CurrentLogDirectory = My.Application.Info.DirectoryPath & "\AMCUpdater.log"
                End If
            End If

            If My.Application.CommandLineArgs.Count > 0 Then
                CurrentSettings.LoadUserSettings(My.Application.CommandLineArgs.Item(0))

                If CurrentSettings.LogDirectory.Length > 0 Then
                    CurrentLogDirectory = CurrentSettings.LogDirectory + "\MyFilmsAMCupdater.log"
                ElseIf LogDirectoryParam.Length > 0 Then
                    If Directory.Exists(LogDirectoryParam) Then
                        CurrentLogDirectory = LogDirectoryParam + "\MyFilmsAMCupdater.log"
                    End If
                Else
                    CurrentLogDirectory = My.Application.Info.DirectoryPath & "\AMCUpdater.log"
                End If

                Dim ConfigFile As String = My.Application.CommandLineArgs.Item(0)
                If Not IO.File.Exists(ConfigFile) Then
                    LogEvent("ErrorEvent - Config File '" & ConfigFile.ToString & "' not found.", EventLogLevel.ErrorEvent)
                    Exit Sub
                End If

                ' Add config file name to window title
                If (ConfigFile.ToString.Length > 0) Then
                    Form1.Text = "AMC Updater - " & ConfigFile.ToString.Substring(ConfigFile.ToString.LastIndexOf("\") + 1)
                Else
                    Form1.Text = "Ant Movie Catalog Auto-Updater"
                End If

                LogEvent("", EventLogLevel.ErrorEvent)
                LogEvent("AMCUpdater " & My.Application.Info.Version.ToString & " Starting", EventLogLevel.ImportantEvent)
                LogEvent("'" & My.Application.CommandLineArgs.Count.ToString & "' Arguments Found : " & CommandlineString, EventLogLevel.ImportantEvent)

                If (My.Application.CommandLineArgs.Count < 3 And My.Application.CommandLineArgs.Count > 0) Then
                    Dim c As New BatchApplication
                    LogEvent("", EventLogLevel.ErrorEvent)
                    LogEvent("Running in Console Mode", EventLogLevel.ImportantEvent)
                    e.Cancel = True
                    c.Main()
                ElseIf My.Application.CommandLineArgs.Item(2) = "GUI" Then
                    LogEvent("Running in forced GUI Mode with Configfile", EventLogLevel.ImportantEvent)
                Else
                    Dim c As New BatchApplication
                    LogEvent("Running in forced Console Mode", EventLogLevel.ImportantEvent)
                    e.Cancel = True
                    c.Main()
                End If
            Else
                LogEvent("", EventLogLevel.ErrorEvent)
                LogEvent("AMCUpdater " & My.Application.Info.Version.ToString & " Starting", EventLogLevel.ImportantEvent)
                LogEvent("'" & My.Application.CommandLineArgs.Count.ToString & "' Arguments Found : " & CommandlineString, EventLogLevel.ImportantEvent)
                LogEvent("Running in GUI Mode", EventLogLevel.ImportantEvent)
                CurrentSettings.LoadUserSettings()
            End If
        End Sub


        Class BatchApplication
            Sub Main()
                Console.WriteLine("AMCupdater: Starting in Batch Mode")
                LogEvent(" : Starting Batch Mode", EventLogLevel.ImportantEvent)
                LogEvent(" - Movie Path : " & CurrentSettings.Movie_Scan_Path, EventLogLevel.ImportantEvent)
                LogEvent(" - XML File Path : " & CurrentSettings.XML_File, EventLogLevel.ImportantEvent)
                LogEvent(" - Parser File Path : " & CurrentSettings.Internet_Parser_Path, EventLogLevel.ImportantEvent)
                LogEvent(" - Grabber_Override_Language    : " + CurrentSettings.Grabber_Override_Language.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Grabber_Override_PersonLimit : " + CurrentSettings.Grabber_Override_PersonLimit.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Grabber_Override_TitleLimit  : " + CurrentSettings.Grabber_Override_TitleLimit.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Grabber_Override_GetRoles    : " + CurrentSettings.Grabber_Override_GetRoles.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Exclude File Path : " & CurrentSettings.Excluded_Movies_File, EventLogLevel.ImportantEvent)
                LogEvent(" - Fanart Path : " & CurrentSettings.Movie_Fanart_Path, EventLogLevel.ImportantEvent)
                LogEvent(" - Fanart Resolution Limits (Min/Max): " & CurrentSettings.Movie_Fanart_Resolution_Min & ", " & CurrentSettings.Movie_Fanart_Resolution_Max, EventLogLevel.ImportantEvent)
                LogEvent(" - Fanart Download Limit: " & CurrentSettings.Movie_Fanart_Number_Limit, EventLogLevel.ImportantEvent)
                LogEvent(" - PersonArtwork Path : " & CurrentSettings.Movie_PersonArtwork_Path, EventLogLevel.ImportantEvent)
                LogEvent(" - Media Label : " & CurrentSettings.Ant_Media_Label, EventLogLevel.ImportantEvent)
                LogEvent(" - Media Type : " & CurrentSettings.Ant_Media_Type, EventLogLevel.ImportantEvent)
                LogEvent(" - Override Path : " & CurrentSettings.Override_Path, EventLogLevel.ImportantEvent)
                LogEvent(" - Store Short Names : " & CurrentSettings.Store_Short_Names_Only, EventLogLevel.ImportantEvent)
                LogEvent(" - Backup Option : " & CurrentSettings.Backup_XML_First, EventLogLevel.ImportantEvent)
                LogEvent(" - Overwrite Original File Option : " & CurrentSettings.Overwrite_XML_File.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Source Field : " & CurrentSettings.Ant_Database_Source_Field.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Purge Missing Movies From Database : " & CurrentSettings.Purge_Missing_Files.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - PurgeMissing Always: " + CurrentSettings.Purge_Missing_Files_When_Source_Unavailable.ToString, EventLogLevel.ImportantEvent)
                LogEvent(" - Check for folders containing DVD copies : " + CurrentSettings.Scan_For_DVD_Folders.ToString, EventLogLevel.ImportantEvent)

                For Each s As String In My.Application.CommandLineArgs
                    Console.WriteLine(s)
                    If s.ToLower.Contains("Movie_Scan_Path=".ToLower) Then
                        CurrentSettings.Movie_Scan_Path = s.Substring(s.IndexOf("=") + 1)
                        LogEvent(" - OVERRIDE - Movie Path : " & CurrentSettings.Movie_Scan_Path, EventLogLevel.ImportantEvent)
                    ElseIf s.ToLower.Contains("XML_File=".ToLower) Then
                        CurrentSettings.XML_File = s.Substring(s.IndexOf("=") + 1)
                        LogEvent(" - OVERRIDE - XML File Path : " & CurrentSettings.XML_File, EventLogLevel.ImportantEvent)
                    ElseIf s.ToLower.Contains("Override_Path=".ToLower) Then
                        CurrentSettings.Override_Path = s.Substring(s.IndexOf("=") + 1)
                        LogEvent(" - OVERRIDE - Override Path : " & CurrentSettings.Override_Path, EventLogLevel.ImportantEvent)
                    ElseIf s.ToLower.Contains("Internet_Parser_Path=".ToLower) Then
                        CurrentSettings.Internet_Parser_Path = s.Substring(s.IndexOf("=") + 1)
                        LogEvent(" - OVERRIDE - Parser Path : " & CurrentSettings.Internet_Parser_Path, EventLogLevel.ImportantEvent)
                    End If
                Next

                Try
                    Dim f As New IO.FileInfo(CurrentSettings.XML_File.ToString)
                    If Not f.Exists Then
                        LogEvent("XML File '" + CurrentSettings.XML_File.ToString + "' Not Found.", EventLogLevel.ImportantEvent)
                        Dim destXml As New Xml.XmlTextWriter(CurrentSettings.XML_File.ToString, System.Text.Encoding.Default)
                        destXml.Formatting = System.Xml.Formatting.Indented
                        destXml.WriteStartDocument(False)
                        destXml.WriteStartElement("AntMovieCatalog")
                        destXml.WriteStartElement("Catalog")
                        destXml.WriteElementString("Properties", "")
                        destXml.WriteStartElement("Contents")
                        destXml.WriteEndElement()
                        destXml.WriteEndElement()
                        destXml.WriteEndElement()
                        destXml.Close()
                        LogEvent("Creating XML File", EventLogLevel.ImportantEvent)
                        LogEvent(" - FilePath : " + CurrentSettings.XML_File.ToString, EventLogLevel.ImportantEvent)
                    End If


                    Dim Ant As New AntProcessor
                    Ant.InteractiveMode = False
                    Ant.ProcessXML()
                    Ant.ProcessMovieFolder()
                    Ant.ProcessOrphanFiles()
                    If Ant.CountOrphanFiles > 0 Or Ant.CountOrphanRecords > 0 Then
                        Ant.UpdateXMLFile()
                    End If
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ErrorEvent : " + ex.Message.ToString, EventLogLevel.ErrorEvent)
                Finally
                    LogEvent(" - Processing Complete.", EventLogLevel.ImportantEvent)
                End Try
            End Sub
        End Class

        Private Sub MyApplication_StartupNextInstance(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance

        End Sub
    End Class

End Namespace

