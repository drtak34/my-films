Public Class AntSettings
    Public UserSettingsFile As String = My.Application.Info.DirectoryPath & "\AMCUpdater_Settings.xml"
    Public ReadOnly UserDefaultSettingsFile As String = My.Application.Info.DirectoryPath & "\AMCUpdater_DefaultSettings.xml"
    Public Shared dsSettings As DataSet = New DataSet
    Private Shared dsDefaultSettings As DataSet = New DataSet

    Public Property Movie_Scan_Path() As String
        Get
            Return ReadAttribute("Movie_Scan_Path")
        End Get
        Set(ByVal value As String)
            SetAttribute("Movie_Scan_Path", value)
        End Set
    End Property
    Public Property Movie_Fanart_Path() As String
        Get
            Return ReadAttribute("Movie_Fanart_Path")
        End Get
        Set(ByVal value As String)
            SetAttribute("Movie_Fanart_Path", value)
        End Set
    End Property
    Public Property XML_File() As String
        Get
            Return ReadAttribute("XML_File")
        End Get
        Set(ByVal value As String)
            SetAttribute("XML_File", value)
        End Set
    End Property
    Public Property Ant_Media_Type() As String
        Get
            Return ReadAttribute("Ant_Media_Type")
        End Get
        Set(ByVal value As String)
            SetAttribute("Ant_Media_Type", value)
        End Set
    End Property
    Public Property Ant_Media_Label() As String
        Get
            Return ReadAttribute("Ant_Media_Label")
        End Get
        Set(ByVal value As String)
            SetAttribute("Ant_Media_Label", value)
        End Set
    End Property
    Public Property Override_Path() As String
        Get
            Return ReadAttribute("Override_Path")
        End Get
        Set(ByVal value As String)
            SetAttribute("Override_Path", value)
        End Set
    End Property
    Public Property File_Types_Media() As String
        Get
            Return ReadAttribute("File_Types_Media")
        End Get
        Set(ByVal value As String)
            SetAttribute("File_Types_Media", value)
        End Set
    End Property
    Public Property File_Types_Non_Media() As String
        Get
            Return ReadAttribute("File_Types_Non_Media")
        End Get
        Set(ByVal value As String)
            SetAttribute("File_Types_Non_Media", value)
        End Set
    End Property
    Public Property Master_Title() As String
        Get
            Return ReadAttribute("Master_Title")
        End Get
        Set(ByVal value As String)
            SetAttribute("Master_Title", value)
        End Set
    End Property
    Public Property Backup_XML_First() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Backup_XML_First").ToLower
            If tempvalue = "true" Then
                Return tempvalue
            Else
                Return False
            End If
            Return ReadAttribute("Backup_XML_First")
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Backup_XML_First", value)
        End Set
    End Property
    Public Property Overwrite_XML_File() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Overwrite_XML_File").ToLower
            If tempvalue = "true" Then
                Return tempvalue
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Overwrite_XML_File", value)
        End Set
    End Property
    Public Property Ant_Database_Source_Field() As String
        Get
            Return ReadAttribute("Ant_Database_Source_Field")
        End Get
        Set(ByVal value As String)
            SetAttribute("Ant_Database_Source_Field", value)
        End Set
    End Property
    Public Property Purge_Missing_Files() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Purge_Missing_Files").ToLower
            If tempvalue = "true" Or tempvalue = "false" Then
                Return tempvalue
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Purge_Missing_Files", value)
        End Set
    End Property
    Public Property RegEx_Check_For_MultiPart_Files() As String
        Get
            Return ReadAttribute("RegEx_Check_For_MultiPart_Files")
        End Get
        Set(ByVal value As String)
            SetAttribute("RegEx_Check_For_MultiPart_Files", value)
        End Set
    End Property
    Public Property Scan_For_DVD_Folders() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Scan_For_DVD_Folders").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
            Return ReadAttribute("Scan_For_DVD_Folders")
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Scan_For_DVD_Folders", value)
        End Set
    End Property
    Public Property Execute_Program() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Execute_Program").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Execute_Program", value)
        End Set
    End Property
    Public Property Execute_Program_Path() As String
        Get
            Return ReadAttribute("Execute_Program_Path")
        End Get
        Set(ByVal value As String)
            SetAttribute("Execute_Program_Path", value)
        End Set
    End Property
    Public Property Execute_Only_For_Orphans() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Execute_Only_For_Orphans").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Execute_Only_For_Orphans", value)
        End Set
    End Property
    Public Property Log_Level() As String
        Get
            Return ReadAttribute("Log_Level")
        End Get
        Set(ByVal value As String)
            SetAttribute("Log_Level", value)
        End Set
    End Property
    Public Property Date_Handling() As String
        Get
            Return ReadAttribute("Date_Handling")
        End Get
        Set(ByVal value As String)
            SetAttribute("Date_Handling", value)
        End Set
    End Property
    Public Property Movie_Title_Handling() As String
        Get
            Return ReadAttribute("Movie_Title_Handling")
        End Get
        Set(ByVal value As String)
            SetAttribute("Movie_Title_Handling", value)
        End Set
    End Property
    Public Property Internet_Parser_Path() As String
        Get
            Return ReadAttribute("Internet_Parser_Path")
        End Get
        Set(ByVal value As String)
            SetAttribute("Internet_Parser_Path", value)
        End Set
    End Property
    Public Property Store_Short_Names_Only() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Store_Short_Names_Only").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Store_Short_Names_Only", value)
        End Set
    End Property
    Public Property Excluded_Movies_File() As String
        Get
            Return ReadAttribute("Excluded_Movies_File")
        End Get
        Set(ByVal value As String)
            SetAttribute("Excluded_Movies_File", value)
        End Set
    End Property
    Public Property Database_Fields_To_Import() As String
        Get
            Return ReadAttribute("Database_Fields_To_Import")
        End Get
        Set(ByVal value As String)
            SetAttribute("Database_Fields_To_Import", value)
        End Set
    End Property
    Public Property Import_File_On_Internet_Lookup_Failure() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Import_File_On_Internet_Lookup_Failure").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Import_File_On_Internet_Lookup_Failure", value)
        End Set
    End Property
    Public Property Internet_Lookup_Always_Prompt() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Internet_Lookup_Always_Prompt").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Internet_Lookup_Always_Prompt", value)
        End Set
    End Property
    Public Property Read_DVD_Label() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Read_DVD_Label").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Read_DVD_Label", value)
        End Set
    End Property
    Public Property DVD_Drive_Letter() As String
        Get
            Return ReadAttribute("DVD_Drive_Letter")
        End Get
        Set(ByVal value As String)
            SetAttribute("DVD_Drive_Letter", value)
        End Set
    End Property
    Public Property Manual_XML_File() As String
        Get
            Return ReadAttribute("Manual_XML_File")
        End Get
        Set(ByVal value As String)
            SetAttribute("Manual_XML_File", value)
        End Set
    End Property
    Public Property Manual_Internet_Parser_Path() As String
        Get
            Return ReadAttribute("Manual_Internet_Parser_Path")
        End Get
        Set(ByVal value As String)
            SetAttribute("Manual_Internet_Parser_Path", value)
        End Set
    End Property
    Public Property Manual_Excluded_Movies_File() As String
        Get
            Return ReadAttribute("Manual_Excluded_Movies_File")
        End Get
        Set(ByVal value As String)
            SetAttribute("Manual_Excluded_Movies_File", value)
        End Set
    End Property
    Public Property Manual_Internet_Lookup_Always_Prompt() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Manual_Internet_Lookup_Always_Prompt").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Manual_Internet_Lookup_Always_Prompt", value)
        End Set
    End Property
    Public Property Excluded_Folder_Strings() As String
        Get
            Return ReadAttribute("Excluded_Folder_Strings")
        End Get
        Set(ByVal value As String)
            SetAttribute("Excluded_Folder_Strings", value)
        End Set
    End Property
    Public Property Excluded_File_Strings() As String
        Get
            Return ReadAttribute("Excluded_File_Strings")
        End Get
        Set(ByVal value As String)
            SetAttribute("Excluded_File_Strings", value)
        End Set
    End Property
    Public Property Filter_Strings() As String
        Get
            Return ReadAttribute("Filter_Strings")
        End Get
        Set(ByVal value As String)
            SetAttribute("Filter_Strings", value)
        End Set
    End Property
    Public Property Check_Field_Handling() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Check_Field_Handling").ToLower
            If tempvalue = "true" Then
                Return tempvalue
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Check_Field_Handling", value)
        End Set
    End Property
    Public Property Folder_Name_Is_Group_Name() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Folder_Name_Is_Group_Name").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Folder_Name_Is_Group_Name", value)
        End Set
    End Property
    Public Property Group_Name_Applies_To() As String
        Get
            Return ReadAttribute("Group_Name_Applies_To")
        End Get
        Set(ByVal value As String)
            SetAttribute("Group_Name_Applies_To", value)
        End Set
    End Property
    Public Property Parse_Playlist_Files() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Parse_Playlist_Files").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Parse_Playlist_Files", value)
        End Set
    End Property
    Public Property Store_Image_With_Relative_Path() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Store_Image_With_Relative_Path").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Store_Image_With_Relative_Path", value)
        End Set
    End Property
    Public Property Image_Download_Filename_Prefix() As String
        Get
            Return ReadAttribute("Image_Download_Filename_Prefix")
        End Get
        Set(ByVal value As String)
            SetAttribute("Image_Download_Filename_Prefix", value)
        End Set
    End Property
    Public Property Use_Folder_Dot_Jpg() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Use_Folder_Dot_Jpg").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Use_Folder_Dot_Jpg", value)
        End Set
    End Property
    Public Property Prohibit_Internet_Lookup() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Prohibit_Internet_Lookup").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Prohibit_Internet_Lookup", value)
        End Set
    End Property
    Public Property Use_XBMC_nfo() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Use_XBMC_nfo").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Use_XBMC_nfo", value)
        End Set
    End Property
    Public Property Use_Page_Grabber() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Use_Page_Grabber").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Use_Page_Grabber", value)
        End Set
    End Property
    Public Property Parse_Subtitle_Files() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Parse_Subtitle_Files").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Parse_Subtitle_Files", value)
        End Set
    End Property
    Public Property Rescan_Moved_Files() As Boolean
        Get
            Dim tempvalue As String = ReadAttribute("Rescan_Moved_Files").ToLower
            If tempvalue = "true" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            SetAttribute("Rescan_Moved_Files", value)
        End Set
    End Property

    Public Sub New()
        Dim AppPath As String = My.Application.Info.DirectoryPath
        Dim dt As New DataTable("Values")
        dt.Columns.Add("Option", System.Type.GetType("System.String"))
        dt.Columns.Add("Value", System.Type.GetType("System.String"))
        dt.Columns("Option").ColumnMapping = MappingType.Attribute
        dt.Columns("Value").ColumnMapping = MappingType.Element
        dt.Rows.Add("Movie_Scan_Path", "") 'DefaultMoviePath
        dt.Rows.Add("XML_File", "") 'DefaultXMLPath
        dt.Rows.Add("Movie_Fanart_Path", "") 'DefaultFanartPath
        dt.Rows.Add("Ant_Media_Type", "HDD") 'DefaultMediaType
        dt.Rows.Add("Ant_Media_Label", "Server") 'DefaultMediaLabel
        dt.Rows.Add("Override_Path", "") 'DefaultOverridePath
        dt.Rows.Add("File_Types_Media", "avi;mpg;divx;mpeg;wmv;mkv") 'FileTypesToCheck
        dt.Rows.Add("File_Types_Non_Media", "iso;img") 'FileTypesNonMedia
        dt.Rows.Add("Backup_XML_First", "True") 'DefaultBackupXML
        dt.Rows.Add("Overwrite_XML_File", "True") 'DefaultOverwriteXML
        dt.Rows.Add("Ant_Database_Source_Field", "Source") 'AMCDatabaseSource
        dt.Rows.Add("Purge_Missing_Files", "False") 'DefaultPurgeMissing
        dt.Rows.Add("RegEx_Check_For_MultiPart_Files", "[-|_]cd[0-9]|[-|_]disk[0-9]|[0-9]of[0-9]") 'RegExCheckMultiPart
        dt.Rows.Add("Scan_For_DVD_Folders", "True") 'DefaultScanDVDFolders
        dt.Rows.Add("Execute_Program", "False") 'ExecuteProgram
        dt.Rows.Add("Execute_Program_Path", "C:\Program Files\Ant Movie Catalog\MovieCatalog.exe") 'ExecuteProgramPath
        dt.Rows.Add("Execute_Only_For_Orphans", "True") 'ExecuteOnlyForOrphans
        dt.Rows.Add("Log_Level", "All Events") 'LogLevel
        dt.Rows.Add("Date_Handling", "File Created Date") 'DateHandling
        dt.Rows.Add("Movie_Title_Handling", "File Name + Internet Lookup")
        dt.Rows.Add("Internet_Parser_Path", AppPath & "\Scripts\MyFilmsIMDB.xml") 'DefaultParserPath
        dt.Rows.Add("Store_Short_Names_Only", "False") 'ShortNames
        dt.Rows.Add("Excluded_Movies_File", AppPath & "\AMCUpdater_Excluded_Files.txt") 'DefaultExcludeFile
        dt.Rows.Add("Database_Fields_To_Import", "Date|True;Rating|True;Year|True;Length|True;VideoBitrate|True;AudioBitrate|True;Disks|True;Checked|True;MediaLabel|True;MediaType|True;OriginalTitle|True;TranslatedTitle|True;FormattedTitle|True;Director|True;Producer|True;Country|True;Category|True;Actors|True;URL|True;Description|True;Comments|True;VideoFormat|True;AudioFormat|True;Resolution|True;Framerate|True;Languages|True;Subtitles|True;Size|True;Picture|True") 'DatabaseFields
        dt.Rows.Add("Import_File_On_Internet_Lookup_Failure", "True") 'ImportFileOnInternetLookupFailure
        dt.Rows.Add("Internet_Lookup_Always_Prompt", "True") 'Set to True to always get choice of Movie from Internet lookup; false to attempt auto-match as before.
        dt.Rows.Add("Read_DVD_Label", "False")
        dt.Rows.Add("DVD_Drive_Letter", "")
        dt.Rows.Add("Manual_XML_File", "")
        dt.Rows.Add("Manual_Internet_Parser_Path", AppPath & "\Scripts\MyFilmsIMDB.xml")
        dt.Rows.Add("Manual_Excluded_Movies_File", AppPath & "\AMCUpdater_Excluded_Files.txt")
        dt.Rows.Add("Manual_Internet_Lookup_Always_Prompt", "True")
        dt.Rows.Add("Excluded_Folder_Strings", "")
        dt.Rows.Add("Excluded_File_Strings", "")
        dt.Rows.Add("Filter_Strings", "\([0-9][0-9][0-9][0-9]\)")
        dt.Rows.Add("Check_Field_Handling", "True")
        dt.Rows.Add("Folder_Name_Is_Group_Name", "False")
        dt.Rows.Add("Group_Name_Applies_To", "Original Title")
        dt.Rows.Add("Parse_Playlist_Files", "False")
        dt.Rows.Add("Store_Image_With_Relative_Path", "True")
        dt.Rows.Add("Image_Download_Filename_Prefix", "")
        dt.Rows.Add("Use_Folder_Dot_Jpg", "False")
        dt.Rows.Add("Prohibit_Internet_Lookup", "False")
        dt.Rows.Add("Parse_Subtitle_Files", "False")
        dt.Rows.Add("Rescan_Moved_Files", "False")
        dt.Rows.Add("Master_Title", "TranslatedTitle")
        dt.Rows.Add("Use_XBMC_nfo", "False")
        dt.Rows.Add("Use_Page_Grabber", "False")

        dsDefaultSettings.Tables.Add(dt)
        dsDefaultSettings.CaseSensitive = False
        dsDefaultSettings.Tables(0).PrimaryKey = New DataColumn() {dsDefaultSettings.Tables(0).Columns("Option")}

    End Sub

    Private Function ReadAttribute(ByVal OptionName As String) As String
        Dim value As String = ""
        If dsSettings.Tables.Count > 0 Then
            Dim row As DataRow = dsSettings.Tables(0).Rows.Find(OptionName)
            If Not row Is Nothing Then
                If Not row.Item("Value") Is DBNull.Value Then
                    value = dsSettings.Tables(0).Rows.Find(OptionName).Item("Value")
                Else
                    value = ""
                End If
            Else
                row = dsDefaultSettings.Tables(0).Rows.Find(OptionName)
                If Not row Is Nothing Then
                    If Not row.Item("Value") Is DBNull.Value Then
                        value = dsDefaultSettings.Tables(0).Rows.Find(OptionName).Item("Value")
                        LogEvent("Error Reading Config Attribute '" & OptionName & "' - Using Default Value of '" & value.ToString & "'", EventLogLevel.ErrorOrSimilar)
                    Else
                        LogEvent("Error Reading Config Attribute '" & OptionName & "' - No Default Setting Found.", EventLogLevel.ErrorOrSimilar)
                    End If
                Else
                    LogEvent("Error Reading Config Attribute '" & OptionName & "' - No Default Setting Found.", EventLogLevel.ErrorOrSimilar)
                End If

                'MsgBox("Failed reading Attribute - " & OptionName)
            End If
        End If
        Return value
    End Function
    Private Sub SetAttribute(ByVal OptionName As String, ByVal OptionValue As String)

        If dsSettings.Tables.Count > 0 Then
            Dim row As DataRow = dsSettings.Tables(0).Rows.Find(OptionName)
            If Not row Is Nothing Then
                row.Item("Value") = OptionValue
            Else
                'dsSettings.Tables(0).Rows.Add()

                dsSettings.Tables(0).Rows.Add(OptionValue, OptionName)
                'LogEvent("Failed Setting Config Attribute '" & OptionName & "'", EventLogLevel.ErrorOrSimilar)
                'MsgBox("Failed Setting Attribute - " & OptionName)
            End If
        End If

    End Sub

    Public Sub LoadUserSettings(Optional ByVal SettingsFile As String = "")

        Dim FileToLoad As String
        dsSettings = New DataSet
        If SettingsFile.Length = 0 Then
            'No file specified - use defaults:
            FileToLoad = UserDefaultSettingsFile
            If File.Exists(FileToLoad) Then
                dsSettings.ReadXml(FileToLoad, XmlReadMode.InferSchema)
                dsSettings.CaseSensitive = False
                If dsSettings.Tables.Count = 1 Then
                    dsSettings.Tables(0).PrimaryKey = New DataColumn() {dsSettings.Tables(0).Columns("Option")}
                Else
                    MsgBox("Error reading default settings file - recreating file.")
                    CreateDefaultFiles()
                    LoadUserSettings()
                End If
            Else
                'Default file not found - create new:
                MsgBox("Default settings not found - creating default config file in application directory")
                CreateDefaultFiles()
                LoadUserSettings()
            End If
        Else
            'User has specified a config file:
            FileToLoad = SettingsFile
            If File.Exists(FileToLoad) Then
                Try
                    dsSettings.ReadXml(FileToLoad, XmlReadMode.InferSchema)
                    dsSettings.CaseSensitive = False
                    dsSettings.Tables(0).PrimaryKey = New DataColumn() {dsSettings.Tables(0).Columns("Option")}
                    'If dsSettings.Tables(0).Rows.Count < 24 Then
                    'MsgBox("Error parsing config file - loading defaults instead")
                    'LoadUserSettings()
                    'End If
                Catch ex As Exception
                    MsgBox("Error reading config file - loading defaults instead")
                    dsSettings.Clear()
                    LoadUserSettings()
                End Try
            Else
                'Error - cannot load file
                Err.Raise(10000, , "Error loading custom config file - file not found")
            End If
        End If

    End Sub

    Public Sub SaveUserSettings()

        If File.Exists(UserSettingsFile) Then
            File.Delete(UserSettingsFile)
        End If
        Try
            dsSettings.WriteXml(UserSettingsFile)
            MsgBox("Settings saved to file")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Public Sub SaveDefaultSettings()
        'Called on GUI mode exit - saves all current settings for load on next launch.
        Try
            dsSettings.WriteXml(UserDefaultSettingsFile)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Public Sub CreateDefaultFiles()
        dsDefaultSettings.WriteXml(UserDefaultSettingsFile)
    End Sub




End Class
