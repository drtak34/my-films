Public Class AntRecord
    'Inherits Xml.XmlElement

    Private _MovieNumber As Integer
    Private _FilePath As String = String.Empty
    Private _AllFilesPath As String = String.Empty
    Private _FileName As String = String.Empty
    Private _MediaLabel As String = String.Empty
    Private _MediaType As String = String.Empty
    Private _SourceField As String = "Source"
    Private _OverridePath As String = String.Empty
    Private _ParserPath As String = String.Empty
    'Private _XMLPath As String = String.Empty
    Private _InternetLookupOK As Boolean
    Private _InteractiveMode As Boolean
    Private _ExcludeFile As String = String.Empty
    Private _ImagePath As String = String.Empty
    Private _IsBatchMode As Boolean
    Private _InternetLookupAlwaysPrompt As Boolean
    Private _DateHandling As String = String.Empty
    Private _MovieTitleHandling As String = String.Empty
    Private _LastOutputMessage As String = String.Empty
    Private _Read_DVD_Label As Boolean = False
    Private _Dont_Ask_Interactive As Boolean = False
    Private _XMLFilePath As String = String.Empty
    Private _XMLTempFilePath As String = String.Empty
    Private _InternetSearchHint As String = String.Empty
    Private _InternetSearchHintYear As String = String.Empty
    Private _InternetSearchHintIMDB_Id As String = String.Empty
    Private _GroupName As String = String.Empty
    Private _DownloadImage As Boolean = False
    Private _MasterTitle As String = String.Empty
    '    Private _Process_Mode As Process_Mode_Names
    Private _DatabaseFields As New Hashtable
    Private Shared _InternetData() As String
    Public ProhibitInternetLookup As Boolean
    Public UseXBMCnfo As Boolean
    Public UsePageGrabber As Boolean

    Private Shared _XMLDoc As New Xml.XmlDocument
    Private Shared _XMLElement As Xml.XmlElement

    Private Enum Grabber_Output
        'OriginalTitle = 0
        'TranslatedTitle = 1
        'PicturePathLong = 2
        'Description = 3
        'Rating = 4
        'Actors = 5
        'Director = 6
        'Producer = 7
        'Year = 8
        'Country = 9
        'Category = 10
        'URL = 11
        'PicturePathShort = 12
        'SubUrlPersons = 13
        'Comments = 14
        'Language = 15
        'Tagline = 16
        'Certification = 17
        'SubUrlTitles = 18
        'SubUrlCertification = 19
        'Writer = 20
        'SubUrlComment = 21
        ''IMDBrank = 21
        'Studio = 22
        'Edition = 23
        'Fanart = 24
        'AspectRatio = 25
        'TranslatedTitleAllNames = 26
        'TranslatedTitleAllValues = 27
        'CertificationAllNames = 28
        'CertificationAllValues = 29

        OriginalTitle = 30
        TranslatedTitle = 31
        PicturePathLong = 32
        Description = 33
        Rating = 34
        Actors = 35
        Director = 36
        Producer = 37
        Year = 38
        Country = 39
        Category = 40
        URL = 41
        PicturePathShort = 42
        SubUrlPersons = 43
        Comments = 44
        Language = 45
        Tagline = 46
        Certification = 47
        SubUrlTitles = 48
        SubUrlCertification = 49
        Writer = 50
        SubUrlComment = 51
        'IMDBrank = 51
        Studio = 52
        Edition = 53
        Fanart = 54
        AspectRatio = 55
        TranslatedTitleAllNames = 56
        TranslatedTitleAllValues = 57
        CertificationAllNames = 58
        CertificationAllValues = 59
    End Enum
    Public Enum Process_Mode_Names
        Import
        Update
    End Enum

    Public Property InternetLookupAlwaysPrompt() As Boolean
        Get
            Return _InternetLookupAlwaysPrompt
        End Get
        Set(ByVal value As Boolean)
            _InternetLookupAlwaysPrompt = value
        End Set
    End Property

    Public Property SourceField() As String
        Get
            Return _SourceField
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _SourceField = Value
        End Set
    End Property
    Public Property MediaType() As String
        Get
            Return _MediaType
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _MediaType = Value
        End Set
    End Property
    Public Property MediaLabel() As String
        Get
            Return _MediaLabel
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _MediaLabel = Value
        End Set
    End Property
    Public Property FileName() As String
        Get
            Return _FileName
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _FileName = Value
        End Set
    End Property
    Public Property FilePath() As String
        Get
            Return _FilePath
        End Get
        Set(ByVal Value As String)
            'If there's more than one file in FilePath just take the first one:
            If Value.Contains(";") Then
                'First reset _AllFilesPath...
                _AllFilesPath = Value
                _FilePath = Value.Substring(0, Value.IndexOf(";"))
            Else
                _FilePath = Value
            End If

        End Set
    End Property
    Public Property AllFilesPath() As String
        Get
            Return _AllFilesPath
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _AllFilesPath = Value
        End Set
    End Property
    Public Property OverridePath() As String
        Get
            Return _OverridePath
        End Get
        Set(ByVal Value As String)
            ' Sets the property value.
            _OverridePath = Value
        End Set
    End Property
    Public Property MovieNumber() As Integer
        Get
            Return _MovieNumber
        End Get
        Set(ByVal Value As Integer)
            ' Sets the property value.
            _MovieNumber = Value
        End Set
    End Property
    Public Property XMLElement() As Xml.XmlElement
        Get
            Return _XMLElement
        End Get
        Set(ByVal value As Xml.XmlElement)
            _XMLElement = value
        End Set
    End Property
    Public Property XMLDoc() As Xml.XmlDocument
        Get
            Return _XMLDoc
        End Get
        Set(ByVal value As Xml.XmlDocument)
            _XMLDoc = value
        End Set
    End Property
    Public Property ParserPath()
        Get
            Return _ParserPath
        End Get
        Set(ByVal value)
            _ParserPath = value
        End Set
    End Property
    Public ReadOnly Property InternetLookupOK() As Boolean
        Get
            Return _InternetLookupOK
        End Get
    End Property
    Public Property InteractiveMode() As Boolean
        Get
            Return _InteractiveMode
        End Get
        Set(ByVal value As Boolean)
            _InteractiveMode = value
        End Set
    End Property
    Public Property ExcludeFile() As String
        Get
            Return _ExcludeFile
        End Get
        Set(ByVal value As String)
            _ExcludeFile = value
        End Set
    End Property
    Public Property ImagePath() As String
        Get
            Return _ImagePath
        End Get
        Set(ByVal value As String)
            _ImagePath = value
        End Set
    End Property
    Public Property DateHandling() As String
        Get
            Return _DateHandling
        End Get
        Set(ByVal value As String)
            _DateHandling = value
        End Set
    End Property
    Public Property MovieTitleHandling() As String
        Get
            Return _MovieTitleHandling
        End Get
        Set(ByVal value As String)
            _MovieTitleHandling = value
        End Set
    End Property
    Public Property Read_DVD_Label() As Boolean
        Get
            Return _Read_DVD_Label
        End Get
        Set(ByVal value As Boolean)
            _Read_DVD_Label = value
        End Set
    End Property
    Public Property Dont_Ask_Interactive() As Boolean
        Get
            Return _Dont_Ask_Interactive
        End Get
        Set(ByVal value As Boolean)
            _Dont_Ask_Interactive = value
        End Set
    End Property
    Public Property XMLFilePath() As String
        Get
            Return _XMLFilePath
        End Get
        Set(ByVal value As String)
            _XMLFilePath = value
        End Set
    End Property
    Public ReadOnly Property LastOutputMessage() As String
        Get
            Return _LastOutputMessage
        End Get
    End Property
    Public Property InternetSearchHint() As String
        Get
            Return _InternetSearchHint
        End Get
        Set(ByVal value As String)
            _InternetSearchHint = value
        End Set
    End Property
    Public Property InternetSearchHintYear() As String
        Get
            Return _InternetSearchHintYear
        End Get
        Set(ByVal value As String)
            _InternetSearchHintYear = value
        End Set
    End Property
    Public Property InternetSearchHintIMDB_Id() As String
        Get
            Return _InternetSearchHintIMDB_Id
        End Get
        Set(ByVal value As String)
            _InternetSearchHintIMDB_Id = value
        End Set
    End Property
    Public Property GroupName() As String
        Get
            Return _GroupName
        End Get
        Set(ByVal value As String)
            _GroupName = value
        End Set
    End Property
    Public Property MasterTitle() As String
        Get
            Return _MasterTitle
        End Get
        Set(ByVal value As String)
            _MasterTitle = value
        End Set
    End Property

    Public Sub New()

        Dim DBFields() = CurrentSettings.Database_Fields_To_Import.Split(";")
        Dim FieldName As String
        Dim FieldChecked As Boolean

        For Each blah As String In DBFields
            FieldName = blah.Substring(0, blah.IndexOf("|")).ToLower
            FieldChecked = blah.Substring(blah.IndexOf("|") + 1)
            If Not _DatabaseFields.ContainsKey(FieldName) Then
                _DatabaseFields.Add(FieldName, FieldChecked)
            End If
            If FieldName = "picture" Then
                If FieldChecked = True Then
                    _DownloadImage = True
                End If
            End If
        Next


    End Sub
    Public Sub CreateElement()
        _XMLElement = XMLDoc.CreateElement("Movie")
    End Sub
    Public Function VerifyElement(ByVal otitle As String, ByVal currentNode As Xml.XmlNode) As Xml.XmlNode

        Dim CurrentNode2 As Xml.XmlNode
        Dim CurrentAttribute2 As String
        'CurrentNode2 = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@OriginalTitle='" & otitle & "']")
        CurrentNode2 = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@OriginalTitle=""" & otitle & """]")
        If (Not CurrentNode2 Is Nothing) Then
            If (CurrentNode2.Attributes("Number").Value) <> (currentNode.Attributes("Number").Value) Then 'check, if two movies with same otitle but different recordnumber exist
                CurrentAttribute2 = _SourceField

                ' This also doesn't work, as there is not yet a source for new movie available in XML Element - might be called later?
                'If (CurrentNode2.Attributes(_SourceField).Value = currentNode.Attributes(_SourceField).Value) Then
                '    currentNode.Attributes.RemoveAll()
                '    Return CurrentNode2
                'End If

                ' Guzzi: Removed the following stuff, as it replaced existing movies with same title, but different content (e.g. director's cut or extended versions)
                'If _XMLElement.Attributes(CurrentAttribute2) Is Nothing Then
                '    currentNode.Attributes.RemoveAll()
                '    Return CurrentNode2
                'End If
                'If _XMLElement.Attributes(CurrentAttribute2).Value.ToString = String.Empty Then
                '    currentNode.Attributes.RemoveAll()
                '    Return CurrentNode2
                'End If

            End If
        End If
        Return currentNode
    End Function
    Private Sub DoInternetLookup(ByVal SearchString As String, Optional ByVal Year As String = "", Optional ByVal IMDB_Id As String = "") ' Guzzi: Added year and imdb id as optional (search) parameters
        'This is now reset on ProcessFile, since all processing will begin with that Sub.
        '_LastOutputMessage = ""
        If CurrentSettings.Prohibit_Internet_Lookup = True Or ProhibitInternetLookup = True Then
            _InternetLookupOK = False
            Exit Sub
        End If
        Try
            If IsInternetLookupNeeded() = True Then
                wurl.Clear()
                Dim wpage As Int16 = -1
                Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
                While (True)
                    wurl = Gb.ReturnURL(SearchString, _ParserPath, wpage, _InternetLookupAlwaysPrompt)
                    If (wurl.Count = 1) And _InternetLookupAlwaysPrompt = False Then
                        _InternetData = Gb.GetDetail(wurl.Item(0).URL, _ImagePath, _ParserPath, _DownloadImage)
                        _InternetLookupOK = True

                        If bgwFolderScanUpdate.CancellationPending = True Then
                            Exit Sub
                        End If
                        Exit While
                    Else
                        Dim wtitle As String
                        Dim wyear As String
                        Dim wlimityear As Boolean = False

                        'If _InteractiveMode = True Then
                        If (_InteractiveMode = True And _Dont_Ask_Interactive = False) Then
                            frmList.txtSearchString.Text = SearchString
                            frmList.chkDontAskAgain.Checked = False
                            frmList.txtTmpParserFilePath.Text = _ParserPath
                            frmList.lstOptions.Items.Clear()
                            If _FileName.ToString <> "" Then
                                frmList.Text = _FileName
                            Else
                                frmList.Text = _FilePath
                            End If
                            If (wurl.Count = 0) Then
                                frmList.lstOptions.Items.Add("Movie not found...")
                            Else
                                For i As Integer = 0 To wurl.Count - 1
                                    If wurl.Item(i).Year.ToString = _InternetSearchHintYear Then
                                        wlimityear = True
                                    End If
                                Next
                                For i As Integer = 0 To wurl.Count - 1
                                    wtitle = wurl.Item(i).Title.ToString
                                    wyear = wurl.Item(i).Year.ToString
                                    If (_InternetSearchHint.Length > 0 And wtitle.Contains(_InternetSearchHint) And _InternetLookupAlwaysPrompt = False And (wlimityear = False Or wyear = _InternetSearchHintYear)) Then
                                        _InternetData = Gb.GetDetail(wurl.Item(i).URL, _ImagePath, _ParserPath, _DownloadImage)
                                        _InternetLookupOK = True
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                    If wyear = _InternetSearchHintYear Then
                                        frmList.lstOptions.Items.Add(wtitle & " - (+++ recommended by grabber autoselection +++)" & " (hints: year=" & _InternetSearchHintYear & ", imdb=" & _InternetSearchHintIMDB_Id & ")")
                                    Else
                                        frmList.lstOptions.Items.Add(wtitle & " (hints: year=" & _InternetSearchHintYear & ", imdb=" & _InternetSearchHintIMDB_Id & ")")
                                    End If
                                Next
                            End If
                            If frmList.lstOptions.Items.Count > 0 Then
                                frmList.lstOptions.SelectedIndex = 0
                                frmList.btnOK.Enabled = True
                            Else
                                frmList.btnOK.Enabled = False
                            End If

                            Dim returnValue As System.Windows.Forms.DialogResult

                            'frmList.Parent = Form1
                            returnValue = frmList.ShowDialog()
                            SearchString = frmList.txtSearchString.Text
                            Dim blah4 As Boolean = frmList.chkDontAskAgain.Checked
                            Dim wentry As Integer = frmList.lstOptions.SelectedIndex
                            If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptions.SelectedItem.ToString = "Movie not found...") Then
                                _InternetLookupOK = False
                                _LastOutputMessage = "Failed to load Internet Data for " & FilePath
                                Exit While
                            End If
                            If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptions.SelectedItem.ToString = "+++") Then
                                wpage = Convert.ToInt16(wurl.Item(wentry).IMDBURL)
                            Else
                                If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And frmList.lstOptions.SelectedItem.ToString = "---") Then
                                    wpage = Convert.ToInt16(wurl.Item(wentry).IMDBURL)
                                Else
                                    If ((returnValue = Windows.Forms.DialogResult.OK) And (wentry > -1) And (frmList.lstOptions.SelectedItem.ToString.Length > 0)) Then
                                        _InternetData = Gb.GetDetail(wurl.Item(wentry).url, _ImagePath, frmList.txtTmpParserFilePath.Text, _DownloadImage)
                                        _InternetLookupOK = True
                                        Exit While
                                    Else 'If returnValue = Windows.Forms.DialogResult.Cancel Then
                                        'The ElseIf didn't always fire if you close the window or something.
                                        'User cancelled - check for 'Ignore File' flag.
                                        If frmList.chkDontAskAgain.Checked And Not CurrentSettings.Excluded_File_Strings.Contains(FilePath.ToLower) Then
                                            'Add to excluded file list to ignore in future scans:
                                            'My.Computer.FileSystem.WriteAllText(_ExcludeFile, FilePath.ToLower + vbCrLf, True)
                                            CurrentSettings.Excluded_File_Strings += ("|" + FilePath.ToLower)
                                        End If
                                        _InternetLookupOK = False
                                        'LogEvent("Failed to load Internet Data for " & FilePath, EventLogLevel.ImportantEvent)
                                        _LastOutputMessage = "Failed to load Internet Data for " & FilePath
                                        Exit While
                                    End If
                                End If
                            End If
                        Else
                            ' In batch mode, try to identify the right movie with  optseachstring
                            'LogEvent(SearchString & " - " & wurl.Count.ToString & " Movies found", EventLogLevel.Informational)
                            If _InternetSearchHint.Length > 0 Then
                                For i As Integer = 0 To wurl.Count - 1
                                    If wurl.Item(i).Year.ToString = _InternetSearchHintYear Then
                                        wlimityear = True
                                    End If
                                Next
                                For i As Integer = 0 To wurl.Count - 1
                                    wtitle = wurl.Item(i).Title.ToString
                                    wyear = wurl.Item(i).Year.ToString
                                    If (_InternetSearchHint.Length > 0 And wtitle.Contains(_InternetSearchHint) And (wlimityear = False Or wyear = _InternetSearchHintYear)) Then
                                        'Dim datas As String()
                                        _InternetData = Gb.GetDetail(wurl.Item(i).URL, _ImagePath, _ParserPath, _DownloadImage)
                                        'CreateXmlnetInfos(datas)
                                        _InternetLookupOK = True
                                        If bgwFolderScanUpdate.CancellationPending = True Then
                                            Exit Sub
                                        End If
                                        Exit While
                                    End If
                                Next
                            End If

                            _LastOutputMessage = SearchString & " - " & wurl.Count.ToString & " Movies found from Internet lookup."
                            _InternetLookupOK = False
                            Exit While
                            'dtmulti.Rows.Add(FilePath)
                        End If
                    End If
                End While
            End If
        Catch ex As Exception
            _LastOutputMessage = "ERROR : Error on Internet Lookup for " & _FileName.ToString & " : " & ex.Message.ToString & " - Stacktrace: " & ex.StackTrace.ToString
        End Try
    End Sub

    Public Sub ProcessFile(ByVal ProcessMode As Process_Mode_Names)

        _LastOutputMessage = ""
        'LogEvent("ProcessFile() - Start !", EventLogLevel.InformationalWithGrabbing)
        Try

            Dim attr As Xml.XmlAttribute
            Dim element As Xml.XmlElement
            Dim TempValue As String
            Dim CurrentAttribute As String

            'First ensure we have a valid movie number so the record can be saved:
            TempValue = _MovieNumber
            CurrentAttribute = "Number"
            If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                attr.Value = TempValue
                _XMLElement.Attributes.Append(attr)
                'Removed by OH August 9th 2008 - why would we ever need to update this value?  If it is there, then good.  If not then it must be added.  But not updated!
                'Else
                '_XMLElement.Attributes(CurrentAttribute).Value = TempValue
            End If
            'LogEvent("ProcessFile() - get valid record number: '" & TempValue & "'", EventLogLevel.InformationalWithGrabbing)
            TempValue = ""
            Dim title As String = ""
            Dim ttitle As String = ""
            Dim director As String = ""
            Dim year As Int16 = 0
            Dim imdb_id As String = "" ' Guzzi Added for exact IMDB matching
            If (ProcessMode = Process_Mode_Names.Import) Then
                'Second get a decent Movie Title which we can then use for Internet Lookups as well as the Original Title field.
                'LogEvent("ProcessFile() - Import: Get search & matching hints...", EventLogLevel.InformationalWithGrabbing)
                If _DatabaseFields("originaltitle") = True Then
                    'add a test for manual update when no file specified => Internet lookup with OriginalTitle     
                    If (_FilePath.Length > 0) Then
                        TempValue = GetTitleFromFilePath(_FilePath)
                        CurrentAttribute = "OriginalTitle"
                        If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                            attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                            attr.Value = TempValue
                            _XMLElement.Attributes.Append(attr)
                        Else
                            _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                        End If
                        title = TempValue
                        'LogEvent("ProcessFile() - Import - hints - title: '" & title & "'", EventLogLevel.InformationalWithGrabbing)
                        TempValue = ""
                    End If
                End If
                If _DatabaseFields("year") = True Then
                    'try to get year from filepath/name 
                    If (_FilePath.Length > 0) Then
                        TempValue = GetYearFromFilePath(_FilePath)
                        CurrentAttribute = "Year"
                        If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                            attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                            attr.Value = TempValue
                            _XMLElement.Attributes.Append(attr)
                        Else
                            _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                        End If
                        _InternetSearchHintYear = TempValue
                        'LogEvent("ProcessFile() - Import - hints - year: '" & _InternetSearchHintYear & "'", EventLogLevel.InformationalWithGrabbing)
                        TempValue = ""
                    End If
                End If
                'try to get IMDB Id from filepath/name
                If (_FilePath.Length > 0) Then
                    TempValue = GetIMDBidFromFilePath(_FilePath)
                    CurrentAttribute = "IMDB_Id"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                    _InternetSearchHintIMDB_Id = TempValue
                    'LogEvent("ProcessFile() - Import - hints - imdb_id: '" & _InternetSearchHintIMDB_Id & "'", EventLogLevel.InformationalWithGrabbing)
                    TempValue = ""
                End If
            End If

            If IsInternetLookupNeeded() = True Then
                If ProcessMode = Process_Mode_Names.Update Then
                    Dim wdirector As String
                    If _XMLElement.Attributes("Director") IsNot Nothing Then
                        If _XMLElement.Attributes("Director").Value.Length > 0 Then
                            wdirector = System.Text.RegularExpressions.Regex.Replace(_XMLElement.Attributes("Director").Value, "\b(and|und|en|et|y|&)\b", ",")

                            If wdirector.IndexOf(",") > 0 Then
                                _InternetSearchHint = wdirector.Substring(0, wdirector.IndexOf(",") - 1).Trim()
                            Else
                                _InternetSearchHint = wdirector.Trim()
                            End If
                        Else
                            If _XMLElement.Attributes("Year") IsNot Nothing Then
                                _InternetSearchHint = _XMLElement.Attributes("Year").Value.ToString
                                _InternetSearchHintYear = _XMLElement.Attributes("Year").Value.ToString
                            End If
                        End If
                    End If
                    'LogEvent("ProcessFile() - Update - _InternetSearchHint: '" & _InternetSearchHint & "'", EventLogLevel.InformationalWithGrabbing)
                End If
                If _XMLElement.Attributes("OriginalTitle") IsNot Nothing Then
                    'Looks like we have a value here - if so, use it for the lookup.  Assuming here that it has already been 'cleaned'.
                    If _XMLElement.Attributes("OriginalTitle").Value.ToString = String.Empty Then
                        'No original title available, so use the clean filename instead:
                        DoInternetLookup(GetTitleFromFilePath(_FilePath))
                    Else
                        If _XMLElement.Attributes("OriginalTitle").Value.ToString.Contains("\") = True Then
                            'I don't think this is needed now, but leave it for the time being to be safe!
                            DoInternetLookup(_XMLElement.Attributes("OriginalTitle").Value.ToString.Substring(_XMLElement.Attributes("OriginalTitle").Value.ToString.LastIndexOf("\") + 1))
                        Else
                            'Do Internet lookup with the existing Original Title value.
                            DoInternetLookup(_XMLElement.Attributes("OriginalTitle").Value.ToString)
                        End If
                    End If
                Else
                    'No original title available, so use the clean filename instead:
                    DoInternetLookup(GetTitleFromFilePath(_FilePath))
                End If
            End If


            If CurrentSettings.Use_XBMC_nfo = True Or UseXBMCnfo = True Then
                'Now update the Original Title with the XBMCnfo-file, if set to do so:
                TempValue = GetXBMCnfoData(_FilePath, "OriginalTitle")
            End If

            If _InternetLookupOK = True Then
                'Now update the Original Title with the Internet value, if set to do so:
                If _MovieTitleHandling.Contains("Internet Lookup") = True Then
                    If _DatabaseFields("originaltitle") = True Then
                        CurrentAttribute = "OriginalTitle"
                        TempValue = _InternetData(Grabber_Output.OriginalTitle)
                        If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                            attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                            attr.Value = TempValue
                            If attr.Value <> "" Then
                                _XMLElement.Attributes.Append(attr)
                            End If
                        Else
                            _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                        End If
                        title = TempValue
                        TempValue = ""
                    End If
                End If
            Else
                'If the Internet Lookup has failed, and the user has requested a Translated Title, use the Original Title instead:
                If _DatabaseFields("translatedtitle") = True Then
                    CurrentAttribute = "TranslatedTitle"
                    TempValue = GetTitleFromFilePath(_FilePath)
                    If _XMLElement.Attributes("OriginalTitle") IsNot Nothing Then
                        If _XMLElement.Attributes("OriginalTitle").Value.ToString <> String.Empty Then
                            TempValue = _XMLElement.Attributes("OriginalTitle").Value.ToString
                        End If
                    End If

                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
            End If

            'Finally, check to see if there's a group name attached to this, and apply it.
            If _GroupName <> "" Then
                If CurrentSettings.Folder_Name_Is_Group_Name = True Then
                    If CurrentSettings.Group_Name_Applies_To = "Original Title" Or CurrentSettings.Group_Name_Applies_To = "Both Titles" Then
                        If _XMLElement.Attributes("OriginalTitle").Value <> _GroupName.ToString Then
                            _XMLElement.Attributes("OriginalTitle").Value = _GroupName.ToString & "\" & _XMLElement.Attributes("OriginalTitle").Value
                        End If
                    End If
                End If
            End If

            ' Guzzi: Original Code does remove old entries with same otitle name
            'Try to see if entry already exist with empty movie filename => delete the new entry and update existing one
            If _DatabaseFields("originaltitle") = True Or _DatabaseFields("translatedtitle") = True Then
                _XMLElement = VerifyElement(_XMLElement.Attributes("OriginalTitle").Value.ToString, _XMLElement)
            Else
                _LastOutputMessage = "ERROR : Error importing " & _FileName.ToString & " : No originaltitle or translatedtitle activated"

            End If

            If _DatabaseFields("formattedtitle") = True Then
                CurrentAttribute = "FormattedTitle"
                If _XMLElement.Attributes("TranslatedTitle") IsNot Nothing Then
                    'TempValue = _XMLElement.Attributes("TranslatedTitle").Value
                    'Guzzi: Reverted Change to use InetNames for dupe checks to avoid overwriting old DB entries
                    TempValue = Grabber.GrabUtil.TitleToArchiveName(_XMLElement.Attributes("TranslatedTitle").Value)
                ElseIf _XMLElement.Attributes("OriginalTitle") IsNot Nothing Then
                    'TempValue = _XMLElement.Attributes("OriginalTitle").Value
                    'Guzzi: Reverted Change to use InetNames for dupe checks to avoid overwriting old DB entries
                    TempValue = Grabber.GrabUtil.TitleToArchiveName(_XMLElement.Attributes("OriginalTitle").Value)
                End If
                If TempValue.Trim <> String.Empty Then
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If

            End If

            If _DatabaseFields("date") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "Date")
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    CurrentAttribute = "Date"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
            End If
            TempValue = ""


            If _DatabaseFields("checked") = True Then
                TempValue = CurrentSettings.Check_Field_Handling.ToString
                CurrentAttribute = "Checked"
                If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                    attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                    attr.Value = TempValue
                    _XMLElement.Attributes.Append(attr)
                Else
                    _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                End If
                TempValue = ""
            End If

            If _DatabaseFields("medialabel") = True And (_FilePath.Length > 0) Then
                CurrentAttribute = "MediaLabel"
                If _Read_DVD_Label = True Then
                    Dim DrivePath As String = String.Empty
                    Dim DriveLetter As String = String.Empty
                    Dim DriveLabel As String = String.Empty
                    If _FilePath.IndexOf(":") > 0 Then
                        'We're scanning a drive not a UNC, check what drive:
                        DriveLetter = _FilePath.Substring(0, _FilePath.IndexOf(":") + 1)
                        Dim myObjectSearcher As System.Management.ManagementObjectSearcher
                        Dim myObject As System.Management.ManagementObject
                        myObjectSearcher = New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_CDROMDrive")
                        For Each myObject In myObjectSearcher.Get
                            If myObject("MediaLoaded") = "True" Then
                                If myObject("Drive").ToString.ToLower = DriveLetter.ToLower Then
                                    DriveLabel = myObject("VolumeName").ToString
                                End If
                            End If
                        Next
                    End If
                    If DriveLabel = String.Empty Then
                        TempValue = _MediaLabel
                    Else
                        TempValue = DriveLabel
                    End If
                Else
                    TempValue = _MediaLabel
                End If

                If Not String.IsNullOrEmpty(TempValue) And Not TempValue.Contains("ERROR") Then
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""

            End If

            If _DatabaseFields("mediatype") = True Then
                If Not String.IsNullOrEmpty(_MediaType) Then
                    CurrentAttribute = "MediaType"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _MediaType
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _MediaType
                    End If
                    TempValue = ""
                End If
            End If

            If (_FilePath.Length > 0) Then
                If Not String.IsNullOrEmpty(_OverridePath) Then
                    TempValue = _OverridePath
                Else
                    TempValue = _FilePath
                End If
                If CurrentSettings.Store_Short_Names_Only Then
                    If TempValue.Contains("\") Then
                        TempValue = TempValue.Substring(TempValue.LastIndexOf("\") + 1)
                    End If
                End If
                CurrentAttribute = _SourceField
                If Not (CurrentAttribute = "(none)" Or String.IsNullOrEmpty(CurrentAttribute)) Then
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
            End If

            TempValue = ""

            If _DatabaseFields("subtitles") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "textstreamlanguagelist")
                CurrentAttribute = "Subtitles"
                If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                    attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                    attr.Value = TempValue
                    If attr.Value <> "" And Not attr.Value.Contains("ERROR") Then
                        _XMLElement.Attributes.Append(attr)
                    End If
                Else
                    _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                End If
                TempValue = ""
            End If

            If _DatabaseFields("languages") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "audiostreamlanguagelist")
                CurrentAttribute = "Languages"
                If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                    attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                    attr.Value = TempValue
                    If attr.Value <> "" And Not attr.Value.Contains("ERROR") Then
                        _XMLElement.Attributes.Append(attr)
                    End If
                Else
                    _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                End If
                TempValue = ""
            End If

            If _DatabaseFields("resolution") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "Resolution")
                CurrentAttribute = "Resolution"
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("length") = True Then
                'TempValue = fnGetFileData(_FilePath, "Runtime")
                CurrentAttribute = "Length"
                If _AllFilesPath <> "" Then
                    For Each wfile As String In _AllFilesPath.Split(";")
                        If GetFileData(wfile, "runtime") <> "" And Not GetFileData(wfile, "runtime").Contains("ERROR") Then
                            If TempValue = "" Then
                                TempValue = GetFileData(wfile, "runtime")
                            Else
                                TempValue = CLng(TempValue) + GetFileData(wfile, "runtime")
                            End If
                            'Diskcount += 1
                        End If
                    Next

                Else
                    TempValue = GetFileData(_FilePath, "runtime")
                End If
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("videoformat") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "VideoFormat")
                CurrentAttribute = "VideoFormat"
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("videobitrate") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "VideoBitrate")
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    CurrentAttribute = "VideoBitrate"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("audioformat") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "AudioFormat")
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    CurrentAttribute = "AudioFormat"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("audiobitrate") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "AudioBitrate")
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    CurrentAttribute = "AudioBitrate"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("framerate") = True And (_FilePath.Length > 0) Then
                TempValue = GetFileData(_FilePath, "Framerate")
                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    CurrentAttribute = "Framerate"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("size") = True Then
                CurrentAttribute = "Size"
                If _AllFilesPath <> "" Then
                    For Each wfile As String In _AllFilesPath.Split(";")
                        If GetFileData(wfile, "FileSize") <> "" And Not GetFileData(wfile, "FileSize").Contains("ERROR") Then
                            If TempValue = "" Then
                                TempValue = GetFileData(wfile, "FileSize")
                            Else
                                TempValue = CLng(TempValue) + GetFileData(wfile, "FileSize")
                            End If
                        End If
                    Next
                Else
                    TempValue = GetFileData(_FilePath, "FileSize")
                End If

                If TempValue <> "" And Not TempValue.Contains("ERROR") Then
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = TempValue
                        _XMLElement.Attributes.Append(attr)
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = TempValue
                    End If
                End If
                TempValue = ""
            End If

            If _DatabaseFields("disks") = True Then
                CurrentAttribute = "Disks"
                Dim Diskcount As Integer = 0
                If _AllFilesPath <> "" Then
                    For Each wfile As String In _AllFilesPath.Split(";")
                        Diskcount += 1
                    Next
                Else
                    Diskcount = 1
                End If

                If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                    attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                    attr.Value = Diskcount
                    _XMLElement.Attributes.Append(attr)
                Else
                    _XMLElement.Attributes(CurrentAttribute).Value = Diskcount
                End If
            End If


            If _DatabaseFields("description") = True And CurrentSettings.Use_Page_Grabber = True Or UsePageGrabber = True Then
                'Now update the Description with the description from HTML-File, if set to do so:
                CurrentAttribute = "Description"
                TempValue = GetHTMLFileData(_FilePath, "description")
                If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                    attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                    attr.Value = GetHTMLFileData(_FilePath, "description")
                    If attr.Value <> "" Then
                        _XMLElement.Attributes.Append(attr)
                    End If
                Else
                    _XMLElement.Attributes(CurrentAttribute).Value = GetHTMLFileData(_FilePath, "description")
                End If
            End If


            If _InternetLookupOK = False Then
                'Additional attempt to load picture with folder.jpg settings, in case Internet lookup fails
                If _DatabaseFields("picture") = True And CurrentSettings.Use_Folder_Dot_Jpg = True Then
                    CurrentAttribute = "Picture"


                    'GUZZI: Added to try to get Picture with Moviename.JPG !!!
                    'Dim ReturnValue As String
                    'Dim Filename As String = FilePath
                    'Get the file name itself off the end: .avi
                    'Dim FileNameEnd As String = Filename.Substring(InStrRev(Filename, "."))
                    'Filename = Filename.Replace(FileNameEnd, "")
                    'ReturnValue = Filename + "jpg"

                    Dim FileExists As Boolean = False
                    Dim NewFileName As String = _FilePath
                    If NewFileName.Contains("\") = True Then
                        NewFileName = NewFileName.Substring(0, NewFileName.LastIndexOf("\"))
                    End If
                    NewFileName += "\folder.jpg"
                    Try
                        If File.Exists(NewFileName) Then
                            FileExists = True
                        End If
                    Catch ex As Exception

                    End Try

                    If FileExists = True Then
                        If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                            attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                            attr.Value = NewFileName
                            _XMLElement.Attributes.Append(attr)
                        Else
                            _XMLElement.Attributes(CurrentAttribute).Value = NewFileName
                        End If
                    End If

                End If

            Else
                'Load all the Internet data...
                title = _InternetData(Grabber_Output.OriginalTitle)
                If _DatabaseFields("translatedtitle") = True Then
                    CurrentAttribute = "TranslatedTitle"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.TranslatedTitle)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.TranslatedTitle)
                    End If
                End If

                'Finally, check to see if there's a group name attached to this, and apply it.
                If _GroupName <> "" Then
                    If CurrentSettings.Folder_Name_Is_Group_Name = True Then
                        If CurrentSettings.Group_Name_Applies_To = "Translated Title" Or CurrentSettings.Group_Name_Applies_To = "Both Titles" Then
                            If _XMLElement.Attributes("TranslatedTitle").Value <> _GroupName.ToString Then
                                _XMLElement.Attributes("TranslatedTitle").Value = _GroupName.ToString & "\" & _XMLElement.Attributes("TranslatedTitle").Value
                            End If
                        End If
                    End If
                End If

                If _DatabaseFields("year") = True Then
                    CurrentAttribute = "Year"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Year)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Year)
                    End If
                End If

                If _DatabaseFields("country") = True Then
                    CurrentAttribute = "Country"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Country)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Country)
                    End If
                End If

                If _DatabaseFields("category") = True Then
                    CurrentAttribute = "Category"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Category)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Category)
                    End If
                End If

                If _DatabaseFields("url") = True Then
                    CurrentAttribute = "URL"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.URL)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.URL)
                    End If
                End If


                If _DatabaseFields("rating") = True Then
                    CurrentAttribute = "Rating"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Rating)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Rating)
                    End If
                End If

                If _DatabaseFields("director") = True Then
                    CurrentAttribute = "Director"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Director)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Director)
                    End If
                End If

                If _DatabaseFields("producer") = True Then
                    CurrentAttribute = "Producer"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Producer)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Producer)
                    End If
                End If

                If _DatabaseFields("actors") = True Then
                    CurrentAttribute = "Actors"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Actors)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Actors)
                    End If
                End If

                If _DatabaseFields("description") = True Then
                    CurrentAttribute = "Description"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Description)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Description)
                    End If
                End If

                If _DatabaseFields("comments") = True Then
                    CurrentAttribute = "Comments"
                    If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                        attr.Value = _InternetData(Grabber_Output.Comments)
                        If attr.Value <> "" Then
                            _XMLElement.Attributes.Append(attr)
                        End If
                    Else
                        ' _XMLElement.Attributes(CurrentAttribute).Value = ""
                        _XMLElement.Attributes(CurrentAttribute).Value = _InternetData(Grabber_Output.Comments) ' Guzzi addded grabbed commment
                    End If
                End If

                If _DatabaseFields("picture") = True Then
                    CurrentAttribute = "Picture"
                    If CurrentSettings.Use_Folder_Dot_Jpg = True Then
                        'First check to see if the file exists in a nice safe way:
                        Dim FileExists As Boolean = False
                        Dim FileIsLinked As Boolean = False
                        Dim NewFileName As String = _FilePath
                        If NewFileName.Contains("\") = True Then
                            NewFileName = NewFileName.Substring(0, NewFileName.LastIndexOf("\"))
                        End If
                        NewFileName += "\folder.jpg"
                        Try
                            If File.Exists(NewFileName) Then
                                FileExists = True
                            End If
                        Catch ex As Exception

                        End Try

                        'Then check if the current record already has a reference to that file:
                        If _XMLElement.Attributes(CurrentAttribute) IsNot Nothing Then
                            If _XMLElement.Attributes(CurrentAttribute).Value.ToLower = NewFileName.ToLower Then
                                FileIsLinked = True
                            End If
                        End If


                        If ProcessMode = Process_Mode_Names.Import Then
                            'Import run - use existing folder.jpg if present; add it if not.
                            If FileExists = True Then
                                'It's there; use it.  Delete the one we just downloaded, if needed:
                                If _InternetData(Grabber_Output.PicturePathLong) <> String.Empty Then
                                    File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                End If
                            Else
                                'If we have anything from the grabber, copy it and use that.
                                If _InternetData(Grabber_Output.PicturePathLong) <> String.Empty Then
                                    File.Copy(_InternetData(Grabber_Output.PicturePathLong), NewFileName)
                                    File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                    FileExists = True
                                End If
                            End If
                        Else
                            'Manual Update - have to assume we'll overwrite the existing folder.jpg with the new one, if possible.
                            If FileExists = True Then
                                'check it's used; if not just use it and we're done.
                                If FileIsLinked = True Then
                                    'It's already in use, so update it:
                                    If _InternetData(Grabber_Output.PicturePathShort) <> String.Empty Then
                                        If FileExists = True Then
                                            System.IO.File.Delete(NewFileName)
                                        End If
                                        File.Copy(_InternetData(Grabber_Output.PicturePathLong), NewFileName)
                                        File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                        FileExists = True
                                    End If
                                End If
                            Else
                                'Try and get a new one:
                                If _InternetData(Grabber_Output.PicturePathShort) <> String.Empty Then
                                    If FileExists = True Then
                                        System.IO.File.Delete(NewFileName)
                                    End If
                                    File.Copy(_InternetData(Grabber_Output.PicturePathLong), NewFileName)
                                    File.Delete(_InternetData(Grabber_Output.PicturePathLong))
                                    FileExists = True
                                End If
                            End If
                        End If

                        'Now check if we have a valid link; and use it.
                        If FileExists = True Then
                            If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                                attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                                attr.Value = NewFileName
                                _XMLElement.Attributes.Append(attr)
                            Else
                                _XMLElement.Attributes(CurrentAttribute).Value = NewFileName
                            End If
                        End If

                    Else
                        'Not using folder.jpg - use default location instead (with the xml file, maybe using override path)
                        If _InternetData(Grabber_Output.PicturePathShort) <> String.Empty Then
                            Dim PicturePathPrefix As String = CurrentSettings.Image_Download_Filename_Prefix.ToString 'Covers\'
                            Dim PicturePathFull As String = _InternetData(Grabber_Output.PicturePathLong) 'C:\Ant Movie Catalog\2001_ A Space Odyssey.jpg
                            Dim PictureFileName As String = _InternetData(Grabber_Output.PicturePathShort) '2001_ A Space Odyssey.jpg
                            Dim PicturePathToSave As String = String.Empty

                            'Separate the folder from the prefix string (if needed)
                            Dim PrefixString As String = String.Empty
                            Dim PrefixPath As String = String.Empty
                            If PicturePathPrefix <> String.Empty Then
                                If PicturePathPrefix.Contains("\") = True Then
                                    PrefixPath = PicturePathPrefix.Substring(0, PicturePathPrefix.LastIndexOf("\") + 1)
                                    PrefixString = PicturePathPrefix.Substring(PicturePathPrefix.LastIndexOf("\") + 1)
                                Else
                                    PrefixString = PicturePathPrefix
                                End If
                            End If

                            Dim NewFileName As String = String.Empty
                            If PrefixString <> String.Empty Then
                                'Need to rename the file.
                                NewFileName = PicturePathFull.Replace(PictureFileName, PrefixString & PictureFileName)
                                If Not File.Exists(NewFileName) Then
                                    File.Copy(PicturePathFull, NewFileName)
                                End If
                                File.Delete(PicturePathFull)
                                PicturePathFull = NewFileName
                                PictureFileName = PrefixString & PictureFileName
                            End If

                            If PrefixPath <> String.Empty Then
                                'Need to add the new folder to the short (relative) path:
                                PictureFileName = PrefixPath & PictureFileName
                            End If

                            If CurrentSettings.Store_Image_With_Relative_Path = True Then
                                PicturePathToSave = PictureFileName
                            Else
                                PicturePathToSave = PicturePathFull
                            End If

                            If PicturePathToSave <> String.Empty Then
                                If _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                                    attr = _XMLDoc.CreateAttribute(CurrentAttribute)
                                    attr.Value = PicturePathToSave
                                    _XMLElement.Attributes.Append(attr)
                                Else
                                    _XMLElement.Attributes(CurrentAttribute).Value = PicturePathToSave
                                End If
                            End If
                        End If
                    End If
                End If


                ' Guzzi: Added Languages, Writer, Certification, Tagline
                If _DatabaseFields("languages") = True Then
                    CurrentAttribute = "Languages"
                    If _XMLElement.Item(CurrentAttribute) Is Nothing Then
                        element = _XMLDoc.CreateElement(CurrentAttribute)
                        element.InnerText = _InternetData(Grabber_Output.Language)
                        If element.InnerText <> "" Then
                            _XMLElement.AppendChild(element)
                        End If
                    Else
                        _XMLElement.Item(CurrentAttribute).InnerText = _InternetData(Grabber_Output.Language)
                    End If
                End If

                If _DatabaseFields("certification") = True Then
                    CurrentAttribute = "Certification"
                    If _XMLElement.Item(CurrentAttribute) Is Nothing Then
                        element = _XMLDoc.CreateElement(CurrentAttribute)
                        element.InnerText = _InternetData(Grabber_Output.Certification)
                        If element.InnerText <> "" Then
                            _XMLElement.AppendChild(element)
                        End If
                    Else
                        _XMLElement.Item(CurrentAttribute).InnerText = _InternetData(Grabber_Output.Certification)
                    End If
                End If

                If _DatabaseFields("writer") = True Then
                    CurrentAttribute = "Writer"
                    If _XMLElement.Item(CurrentAttribute) Is Nothing Then
                        element = _XMLDoc.CreateElement(CurrentAttribute)
                        element.InnerText = _InternetData(Grabber_Output.Writer)
                        If element.InnerText <> "" Then
                            _XMLElement.AppendChild(element)
                        End If
                    Else
                        _XMLElement.Item(CurrentAttribute).InnerText = _InternetData(Grabber_Output.Writer)
                    End If
                End If

                If _DatabaseFields("tagline") = True Then
                    CurrentAttribute = "Tagline"
                    If _XMLElement.Item(CurrentAttribute) Is Nothing Then
                        element = _XMLDoc.CreateElement(CurrentAttribute)
                        element.InnerText = _InternetData(Grabber_Output.Tagline)
                        If element.InnerText <> "" Then
                            _XMLElement.AppendChild(element)
                        End If
                    Else
                        _XMLElement.Item(CurrentAttribute).InnerText = _InternetData(Grabber_Output.Tagline)
                    End If
                End If

                'If _DatabaseFields("imdbrank") = True Then
                '    CurrentAttribute = "ImdbRank"
                '    If _XMLElement.Item(CurrentAttribute) Is Nothing Then
                '        element = _XMLDoc.CreateElement(CurrentAttribute)
                '        element.InnerText = _InternetData(Grabber_Output.IMDBrank)
                '        If element.InnerText <> "" Then
                '            _XMLElement.AppendChild(element)
                '        End If
                '    Else
                '        _XMLElement.Item(CurrentAttribute).InnerText = _InternetData(Grabber_Output.IMDBrank)
                '    End If
                'End If

                If _DatabaseFields("studio") = True Then
                    CurrentAttribute = "Studio"
                    If _XMLElement.Item(CurrentAttribute) Is Nothing Then
                        element = _XMLDoc.CreateElement(CurrentAttribute)
                        element.InnerText = _InternetData(Grabber_Output.Studio)
                        If element.InnerText <> "" Then
                            _XMLElement.AppendChild(element)
                        End If
                    Else
                        _XMLElement.Item(CurrentAttribute).InnerText = _InternetData(Grabber_Output.Studio)
                    End If
                End If
            End If

            'get fanart
            If _InternetLookupOK = True Then
                If _DatabaseFields("fanart") = True And CurrentSettings.Prohibit_Internet_Lookup = False Then
                    Dim fanart As List(Of Grabber.DBMovieInfo)
                    Dim ttitleCleaned As String = ""
                    CurrentAttribute = "Year"
                    If Not _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        Try
                            year = CInt(_XMLElement.Attributes(CurrentAttribute).Value)
                        Catch
                            year = 0
                        End Try
                    End If
                    CurrentAttribute = "Director"
                    If Not _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        director = _XMLElement.Attributes(CurrentAttribute).Value
                    End If
                    CurrentAttribute = "OriginalTitle"
                    If Not _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        title = _XMLElement.Attributes(CurrentAttribute).Value
                    End If
                    CurrentAttribute = "TranslatedTitle"
                    If Not _XMLElement.Attributes(CurrentAttribute) Is Nothing Then
                        ttitle = _XMLElement.Attributes(CurrentAttribute).Value
                        If ttitle.Contains("(") Then
                            ttitleCleaned = ttitle.Substring(0, ttitle.IndexOf("("))
                        Else
                            ttitleCleaned = ttitle
                        End If
                    End If
                    Dim Gb As Grabber.Grabber_URLClass = New Grabber.Grabber_URLClass
                    If title.Length > 0 Then
                        If title.Contains("\") = True Then
                            title = title.Substring(0, title.IndexOf("\") - 1)
                            'Console.WriteLine("-" & .GroupName.ToString & "-")
                            'fanart = Gb.GetFanart(title, ttitle, year, director, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title)
                            fanart = Gb.GetFanart(title, ttitleCleaned, year, director, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title)
                        Else
                            'fanart = Gb.GetFanart(title, ttitle, year, director, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title)
                            fanart = Gb.GetFanart(title, ttitleCleaned, year, director, CurrentSettings.Movie_Fanart_Path, True, False, CurrentSettings.Master_Title)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            _LastOutputMessage = "ERROR : Error importing " & _FileName.ToString & " : " & ex.Message.ToString
        End Try
    End Sub

    Public Sub UpdateElement()

        Dim CurrentNode As Xml.XmlNode

        CurrentNode = XMLDoc.SelectSingleNode("//AntMovieCatalog/Catalog/Contents/Movie[@Number='" & _MovieNumber & "']")
        CurrentNode.Attributes(_SourceField).Value = _OverridePath
        _XMLElement = CurrentNode

    End Sub

    Public Sub SaveProgress()
        XMLDoc.Save(_XMLFilePath)
    End Sub




End Class





