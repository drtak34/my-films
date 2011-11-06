Imports System.Runtime.InteropServices
'Imports System.Xml
Imports System.Collections
Imports System.Linq
Imports System.Globalization
Imports System.ComponentModel
Imports System.Threading
Imports MediaPortal.Configuration
Imports System.Xml


Module Module1

    Public WithEvents bgwFolderScanUpdate As New System.ComponentModel.BackgroundWorker
    Public CurrentSettings As New AntSettings
    Public wurl As New ArrayList

    Public Enum EventLogLevel As Integer
        None
        InformationalWithGrabbing
        Informational
        ImportantEvent
        ErrorOrSimilar
    End Enum


    Public Function GetDVDFolderName(ByVal FileName As String) As String
        'Function to try and guess the correct movie name for a DVD image stored in a folder.
        'DVD files may be stored directly in a folder with the name of the movie.
        'DVD files may also retain their structure so .vob files will be in \VIDEO_TS\ - assume parent of that is movie name.

        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS.IFO
        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS\VIDEO_TS.IFO

        Dim TempString As String
        Dim ReturnValue As String

        'Get the file name itself off the end: \VIDEO_TS.IFO
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)

        'This trims the filename and leaves DVDs\ShawshankRedemption, the"
        FileName = FileName.Replace(FileNameEnd, "")

        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "video_ts" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If
            'Check to see if we've still got a nested path.  Take the next level up if so.
            If TempString.Contains("\") = True Then
                ReturnValue = TempString.Substring(InStrRev(TempString, "\"))
            Else
                ReturnValue = TempString
            End If

            'do more processing?
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue

    End Function

    Public Function GetBRFolderName(ByVal FileName As String) As String
        'Function to try and guess the correct movie name for a BR image stored in a folder.
        'filename = BRs\IRON MAN\BDMV\index.bdmv
        'filename = BRs\IRON MAN\index.bdmv

        Dim TempString As String
        Dim ReturnValue As String

        'Get the file name itself off the end: \index.bdmv
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)

        'This trims the filename and leaves BRs\IRON MAN"
        FileName = FileName.Replace(FileNameEnd, "")

        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "bdmv" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If
            'Check to see if we've still got a nested path.  Take the next level up if so.
            If TempString.Contains("\") = True Then
                ReturnValue = TempString.Substring(InStrRev(TempString, "\"))
            Else
                ReturnValue = TempString
            End If

            'do more processing?
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue

    End Function

    Public Function GetDVDMovieFile(ByVal FileName As String) As String
        'Function to try and guess the correct movie file by getting the biggest from directory.
        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS.IFO
        'filename = DVDs\Shawshank Redemption, the\VIDEO_TS\VIDEO_TS.IFO

        Dim ReturnValue As String
        Dim directory As String

        If FileName.Contains("\") Then
            directory = FileName.Substring(0, FileName.LastIndexOf("\"))
        Else
            Return FileName
        End If

        Dim dir As New System.IO.DirectoryInfo(directory)
        Dim fileList As IEnumerable(Of System.IO.FileInfo) = dir.GetFiles("*.vob", System.IO.SearchOption.AllDirectories)

        ' Return the size of the largest file
        Dim maxSize = Aggregate aFile In fileList Into Max(GetFileLength(aFile))

        Dim filesByLengDesc = From file In fileList Let filelength = GetFileLength(file) Where filelength > 0 Order By filelength Descending Select file

        Dim longestFile As FileSystemInfo = filesByLengDesc.First

        ReturnValue = longestFile.FullName.ToString
        Return ReturnValue

    End Function

    Public Function GetBRMovieFile(ByVal FileName As String) As String
        'Function to try and guess the correct movie file by getting the biggest from directory.
        'filename = BRs\IRON MAN\BDMV\index.bdmv
        'filename = BRs\IRON MAN\index.bdmv

        Dim ReturnValue As String
        Dim directory As String

        If FileName.Contains("\") Then
            directory = FileName.Substring(0, FileName.LastIndexOf("\"))
        Else
            Return FileName
        End If

        'Take a snapshot of the folder contents
        Dim dir As New System.IO.DirectoryInfo(directory)
        Dim fileList = dir.GetFiles("*.m2ts", System.IO.SearchOption.AllDirectories)

        ' Return the size of the largest file
        Dim maxSize = Aggregate aFile In fileList Into Max(GetFileLength(aFile))

        'Dim maxSize = fileLengths.Max
        'Console.WriteLine("The length of the largest file under {0} is {1}", directory, maxSize)

        ' Return the FileInfo object of the largest file
        ' by sorting and selecting from the beginning of the list
        Dim filesByLengDesc = From file In fileList Let filelength = GetFileLength(file) Where filelength > 0 Order By filelength Descending Select file

        Dim longestFile = filesByLengDesc.First

        'Console.WriteLine("The largest file under {0} is {1} with a length of {2} bytes", directory, longestFile.FullName, longestFile.Length)


        'Try - didn't work ...
        'Dim FileInfo As FileSystemInfo = New DirectoryInfo(directory).GetFileSystemInfos().OrderByDescending(fi >= fi.Length).First()

        ReturnValue = longestFile.FullName
        Return ReturnValue

    End Function

    ' This method is used to catch the possible exception
    ' that can be raised when accessing the FileInfo.Length property.
    ' In this particular case, it is safe to ignore the exception.
    Function GetFileLength(ByVal fi As System.IO.FileInfo) As Long
        Dim retval As Long
        Try
            retval = fi.Length
        Catch ex As FileNotFoundException
            ' If a file is no longer present,
            ' just return zero bytes. 
            retval = 0
        End Try

        Return retval
    End Function

    Public Function StripPathFromFile(ByVal FilePath As String, ByVal ScanPath As String, Optional ByVal OverridePath As String = "") As String
        'Function to take a full file path and convert it to be relative to the scanned movie folder.
        'This should always receive an override path - if none specified it should be set to the scanned movie folder path.

        'e.g. If local move path to scan is c:\files\movies and the files should be stored in the Ant Catalog as \\server\movies\
        'then it uses the OverridePath ('\\server\movies\') in place of the physical root.  If the file path doesn't contain
        'the given override path then it just crops from the last \ character to the end.
        'Also optionally trims off the file extension (not yet implemented).

        If OverridePath <> String.Empty Then
            ScanPath = OverridePath
        End If

        'If FilePath.IndexOf(OverridePath) > -1 Then
        'FilePath = Replace(FilePath, OverridePath, "")
        'Else
        'FilePath = FilePath.Substring(InStrRev(FilePath, "\"))
        'End If

        'This is case sensitive!
        'FilePath = Replace(FilePath.ToLower, ScanPath.ToLower, "")
        FilePath = Replace(FilePath, ScanPath, "", 1, -1, CompareMethod.Text)

        'FilePath = "c:\Movies\Learners 1\"
        'ScanPath = "C:\Movies\"
        'OverridePath = ""

        Return FilePath
    End Function

    Public Function GetNextMovieID(ByVal FilePath As String)
        'Function to access the given XML database file and parse it to find the next largest available MovieNumber (unique key) value.

        Dim CurrentMax As Integer = 0
        If FilePath <> "" Then
            If File.Exists(FilePath) Then
                Dim textReader As Xml.XmlTextReader = New Xml.XmlTextReader(FilePath)

                While textReader.Read()
                    If textReader.Name = "Movie" Then
                        If textReader.GetAttribute("Number") > CurrentMax Then
                            CurrentMax = textReader.GetAttribute("Number")
                        End If
                    End If
                End While

                If Not (textReader Is Nothing) Then
                    textReader.Close()
                End If
            End If
            CurrentMax = CurrentMax + 1
            Return CurrentMax
        Else
            LogEvent("Error - Cannot access file " & FilePath & " to get next available Movie ID.", EventLogLevel.ErrorOrSimilar)
            Return 0
        End If

    End Function

    Public Function GetTitleFromFilePath(ByVal FilePath As String)
        'File Name
        'Folder Name
        'File Name + Internet Lookup 
        'Folder Name + Internet Lookup

        Dim CleanString As String = ""

        If CurrentSettings.Movie_Title_Handling.Contains("File Name") Then
            'Strip Path
            CleanString = FilePath.Substring(FilePath.LastIndexOf("\") + 1)
            'Strip Extension
            CleanString = CleanString.Substring(0, CleanString.LastIndexOf("."))
        ElseIf CurrentSettings.Movie_Title_Handling.Contains("Folder Name") Then
            'Strip filename:
            CleanString = FilePath.Substring(0, FilePath.LastIndexOf("\"))
            'Strip Path:
            CleanString = CleanString.Substring(CleanString.LastIndexOf("\") + 1)
        Else
            CleanString = FilePath
        End If


        Dim CutText As New Regex("\(" & CurrentSettings.RegEx_Check_For_MultiPart_Files & "\)")
        Dim m As Match
        m = CutText.Match(CleanString)
        If m.Success = True Then
            'Finally remove anything which may be a multi-part indicator (e.g. 1of2)
            CleanString = CutText.Replace(CleanString, "")
        End If
        CutText = New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
        m = CutText.Match(CleanString)
        If m.Success = True Then
            CleanString = CutText.Replace(CleanString, "")
        End If

        'If CurrentSettings.Scan_For_DVD_Folders = True Then
        If FilePath.ToLower.Contains("video_ts") = True Then
            CleanString = GetDVDFolderName(FilePath)
        End If
        If FilePath.ToLower.Contains("index.bdmv") = True Then
            CleanString = GetBRFolderName(FilePath)
        End If
        'End If


        'Tidy up any trailing spaces:
        CleanString = CleanString.Trim

        CleanString = RemoveNastyCharacters(CleanString)

        Return CleanString

    End Function
    Public Function GetYearFromFilePath(ByVal FilePath As String)
        'File Name
        'Folder Name
        'File Name + Internet Lookup 
        'Folder Name + Internet Lookup

        Dim CleanString As String = FilePath

        ' remove IMDB ID from text
        Dim CutIMDB As New Regex("tt\d{7}")
        Dim imdb As Match
        imdb = CutIMDB.Match(CleanString)
        If imdb.Success = True Then
            CleanString = CleanString.Replace(imdb.Value, "")
        End If

        ' Check different methods to get year from CleanString
        Dim MatchList As MatchCollection
        Dim matcher As Match

        Dim p As New Regex("\(" & "[0-9]{4}" & "\)") ' year in brackets
        MatchList = p.Matches(CleanString)
        If MatchList.Count > 0 Then
            matcher = MatchList(MatchList.Count - 1)
            Return matcher.Value.Substring(1, 4)
        End If

        Dim p1 As New Regex("[^0-9][0-9]{4}[^0-9]") ' year in non numbers
        MatchList = p1.Matches(CleanString)
        If MatchList.Count > 0 Then
            matcher = MatchList(MatchList.Count - 1)
            Return matcher.Value.Substring(1, 4)
        End If

        Dim p2 As New Regex("[0-9]{4}") ' year just as 4-digit
        MatchList = p2.Matches(CleanString)
        If MatchList.Count > 0 Then
            matcher = MatchList(MatchList.Count - 1)
            Return matcher.Value
        End If
        Return "" ' if nothing could be found ...


        ''Dim CutText As New Regex("\(" & "([^)]*)" & "\)")
        'Dim CutText As New Regex("[^0-9][0-9]{4}[^0-9]")
        'Dim m As Match
        'm = CutText.Match(CleanString)
        'If m.Success = True Then
        '    Return m.Value.Substring(1, 4) 'remove ()
        'Else
        '    ' remove IMDB ID from text
        '    Dim CutIMDB As New Regex("tt\d{7}")
        '    Dim imdb As Match
        '    imdb = CutIMDB.Match(CleanString)
        '    If imdb.Success = True Then
        '        CleanString = CleanString.Replace(imdb.Value, "")
        '    End If

        '    Dim CutText2 As New Regex("[0-9]{4}")
        '    Dim m2 As Match
        '    m2 = CutText2.Match(CleanString)
        '    If m2.Success = True Then
        '        Return m2.Value
        '    Else
        '        Return ""
        '    End If
        'End If

        'Tidy up any trailing spaces:
        CleanString = CleanString.Trim
        CleanString = RemoveNastyCharacters(CleanString)
        Return CleanString
    End Function
    Public Function GetIMDBidFromFilePath(ByVal FilePath As String)
        Dim CleanString As String = ""

        CleanString = FilePath

        Dim CutText As New Regex("" & "tt\d{7}" & "")
        Dim m As Match
        m = CutText.Match(CleanString)
        If m.Success = True Then
            Return m.Value
        Else
            Return ""
        End If
    End Function

    Public Function RemoveNastyCharacters(ByVal strText As String)

        Dim blah As String
        Dim RegCheck As Regex
        Dim NewText As String

        For Each blah In CurrentSettings.Filter_Strings.Split("|")
            If blah.Length > 0 Then
                'Check for RegEx expression special characters:
                'If blah = "." Or blah = "'" Or blah = "^" Or blah = "$" Or blah = "?" Or blah = "*" Or blah = "+" Or blah = "|" Then
                'blah = "\" & blah
                'End If
                If blah.Length = 1 Then
                    'Probably not a regex, due to complexity of any single character, just do a replace.
                    NewText = strText.Replace(blah, " ")
                Else
                    'This should work for any multi-character string:
                    RegCheck = New Regex(blah)
                    NewText = RegCheck.Replace(strText, " ")
                End If
                If NewText.Trim.Length > 0 Then
                    'Check to ensure last operation didn't wipe the string out!
                    strText = NewText
                Else
                    'If NewText is blank, then exit here with the previous value of strText
                    Exit For
                End If
            End If
        Next

        'Tidy up the beginning and end of the string:
        strText = strText.Trim

        'Loop through to remove any groups of spaces left by character replacement:
        If strText.Contains("  ") = True Then
            While strText.Contains("  ") = True
                strText = strText.Replace("  ", " ")
            End While
        End If

        Return strText
    End Function


    Public Function GetFileData(ByVal FilePath As String, ByVal DataItem As String)
        'Function to retreive information from the given file.

        If FilePath.ToLower.EndsWith("video_ts.ifo") = True Then
            FilePath = GetDVDMovieFile(FilePath)
        End If
        If FilePath.ToLower.EndsWith("index.bdmv") = True Then
            FilePath = GetBRMovieFile(FilePath)
        End If

        Dim ReturnValue As String = ""
        Dim TempInteger As Long = 0
        Dim TempString As String = ""
        Dim MI As MediaInfo = New MediaInfo
        Dim i As Integer = 0
        If Not System.IO.File.Exists(FilePath) Then
            Return "ERROR : File cannot be found"
            LogEvent("Error - Cannot open file for analysis - " & FilePath, EventLogLevel.ErrorOrSimilar)
            Exit Function
        End If

        Dim f As New IO.FileInfo(FilePath)

        Select Case DataItem.ToLower

            'Guzzi Test'
            Case "beschreibung"
                Try
                    TempString = "Test-Guzzi-Beschreibung"
                    ReturnValue = TempString

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, 3)
                    ReturnValue = ""
                End Try

            Case "filename"
                Try
                    'ReturnValue = MI.Get_(StreamKind.General, 0, "FileName")
                    TempString = f.Name
                    'Console.WriteLine(f.Extension)
                    TempString = TempString.Replace(f.Extension, "")
                    'Put this bit in here to remove the '1of2' type bits using the system variable regex expression.
                    Dim SplitText As New Regex("\(" & CurrentSettings.RegEx_Check_For_MultiPart_Files & "\)")
                    TempString = SplitText.Replace(TempString, "")
                    SplitText = New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
                    TempString = SplitText.Replace(TempString, "")
                    ReturnValue = TempString

                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try

            Case "runtime"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    'PlayTime value is in miliseconds!
                    TempString = MI.Get_(StreamKind.General, 0, "PlayTime")
                    MI.Close()

                    Integer.TryParse(TempString, TempInteger)
                    If TempInteger <> 0 Then
                        ReturnValue = CLng(TempInteger / 60000).ToString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "videoformat"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "Codec/String")
                    MI.Close()
                    If TempString <> "" Then
                        ReturnValue = TempString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "videobitrate" 'divide by 1000 as returned in bps.
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "BitRate")
                    MI.Close()
                    Integer.TryParse(TempString, TempInteger)
                    If TempInteger <> 0 Then
                        ReturnValue = CInt(TempInteger / 1000).ToString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "audioformat"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.Audio, 0, "Codec/String")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "audiostreamcount"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.General, 0, "AudioCount")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "audiostreamcodeclist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.General, 0, "Audio_Codec_List")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "audiostreamlanguagelist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    'First get the count if possible
                    TempString = ""
                    Integer.TryParse(MI.Get_(StreamKind.General, 0, "AudioCount"), TempInteger)
                    If TempInteger > 0 Then
                        For i = 0 To TempInteger - 1
                            TempString = ""
                            'Try to get the 'proper' language for this stream:
                            TempString = MI.Get_(StreamKind.Audio, i, "Language/String")
                            If TempString = "" Then
                                'If not, check the IAS value - maybe has a language string there:
                                TempString = MI.Get_(StreamKind.General, 0, "IAS" & (i + 1).ToString)
                            End If
                            If TempString <> "" Then
                                'Build the list:
                                If ReturnValue = "" Then
                                    ReturnValue = TempString
                                Else
                                    ReturnValue += " / " & TempString
                                End If
                            End If
                            If ReturnValue = "" Then
                                'Still no value, maybe just put in the number of audio streams?
                                If MI.Get_(StreamKind.General, 0, "AudioCount") <> "1" Then
                                    ReturnValue = MI.Get_(StreamKind.General, 0, "AudioCount").ToString
                                End If
                            End If
                        Next
                    Else
                        'Cannot even get the count of streams - return empty:
                        ReturnValue = ""
                    End If
                    MI.Close()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "audiobitrate" 'divide
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Audio, 0, "BitRate")
                    MI.Close()
                    Integer.TryParse(TempString, TempInteger)
                    If TempInteger <> 0 Then
                        ReturnValue = CInt((TempInteger / 1000)).ToString
                    Else
                        ReturnValue = ""
                    End If
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "textstreamcodeclist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    ReturnValue = MI.Get_(StreamKind.General, 0, "Text_Codec_List")
                    MI.Close()
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "textstreamlanguagelist"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    Integer.TryParse(MI.Get_(StreamKind.General, 0, "TextCount"), TempInteger)
                    If TempInteger <> 0 Then
                        For i = 0 To TempInteger - 1
                            TempString = ""
                            TempString = MI.Get_(StreamKind.Text, i, "Language/String")
                            If TempString <> "" Then
                                'Build the string
                                If ReturnValue = "" Then
                                    ReturnValue = TempString
                                Else
                                    ReturnValue += " / " & TempString
                                End If
                                'Check for a subtitle description:
                                TempString = MI.Get_(StreamKind.Text, i, "Title")
                                If TempString <> "" Then
                                    'Clean up the title a bit:
                                    TempString = TempString.Replace("<", "")
                                    TempString = TempString.Replace(">", "")
                                    TempString = TempString.Replace("(", "")
                                    TempString = TempString.Replace(")", "")
                                    ReturnValue += " (" & TempString & ")"
                                End If
                            End If
                        Next
                    End If
                    MI.Close()
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try

            Case "resolution"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "Width") & "x" & MI.Get_(StreamKind.Visual, 0, "Height")
                    MI.Close()
                    If TempString = "x" Then
                        TempString = ""
                    End If
                    ReturnValue = TempString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "framerate"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "FrameRate")
                    MI.Close()
                    ReturnValue = TempString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "filesize" 'get in MB = divide by 1024 twice
                Try
                    TempString = CStr(f.Length)
                    TempInteger = CLng(TempString)
                    ReturnValue = CLng((TempInteger / 1048576)).ToString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "date"
                Try
                    Select Case CurrentSettings.Date_Handling
                        'File Created Date
                        'Current System Date
                        'No Date
                        Case "File Created Date"
                            ReturnValue = f.CreationTime.Date
                        Case "File Modified Date"
                            ReturnValue = f.LastWriteTime.Date
                        Case "Current System Date"
                            ReturnValue = My.Computer.Clock.LocalTime.Date
                        Case "No Date"
                            ReturnValue = String.Empty
                        Case Else
                            ReturnValue = String.Empty
                    End Select
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case "aspectratio"
                Try
                    MI = New MediaInfo
                    MI.Open(FilePath)
                    TempString = MI.Get_(StreamKind.Visual, 0, "AspectRatio")
                    MI.Close()
                    ReturnValue = TempString
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try
            Case Else
                ReturnValue = "Unknown Variable Requested"
        End Select
        'Console.WriteLine(DataItem.ToString + " - " + ReturnValue.ToString)
        '        MI.Close()
        If MI IsNot Nothing Then
            MI = Nothing
        End If

        Return ReturnValue

    End Function


    Public Function GetHTMLFileData(ByVal FilePath As String, ByVal DataItem As String)
        'Guzzi: Function to retreive information from the HTML-Files in movie directory...
        'Search for goldesel 2004.htm, index.php.htm, Detail.aspx.htm, goldesel.to - quality source for more than 10 years.htm 
        Dim ReturnValue As String = ""
        Dim TempInteger As Long = 0
        Dim TempString As String = ""
        Dim Directoryname As String = ""
        Dim HTMLfilename As String = ""
        'Dim MI As MediaInfo = New MediaInfo
        Dim i As Integer = 0

        Directoryname = fnGetFilePath(FilePath)

        If System.IO.File.Exists(Directoryname + "\" + "goldesel 2004.htm") Then
            HTMLfilename = Directoryname + "\" + "goldesel 2004.htm"
        End If
        If System.IO.File.Exists(Directoryname + "\" + "index.php.htm") Then
            HTMLfilename = Directoryname + "\" + "index.php.htm"
        End If
        If System.IO.File.Exists(Directoryname + "\" + "Detail.aspx.htm") Then
            HTMLfilename = Directoryname + "\" + "Detail.aspx.htm"
        End If
        If System.IO.File.Exists(Directoryname + "\" + "goldesel.to - quality source for more than 10 years.htm") Then
            HTMLfilename = Directoryname + "\" + "goldesel.to - quality source for more than 10 years.htm"
        End If

        If Not System.IO.File.Exists(HTMLfilename) Then
            LogEvent("Error - Cannot open file for analysis - " & HTMLfilename, EventLogLevel.ErrorOrSimilar)
            Return "ERROR : File " + HTMLfilename + " cannot be found"
            Exit Function
        End If

        Dim f As New IO.FileInfo(HTMLfilename)

        Select Case DataItem.ToLower

            'Guzzi Test'
            Case "beschreibung"
                Try
                    TempString = "Test-Guzzi-Beschreibung"
                    ReturnValue = TempString

                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, 3)
                    ReturnValue = ""
                End Try

            Case "description"
                'GUZZI: Added to get the Content of the HTML xxx.htm File !!!
                Dim textReader As String
                'LogEvent("fnGetDescription: Try to read Description from index.php.htm for File: " + FilePath, 2)
                Try
                    textReader = My.Computer.FileSystem.ReadAllText(FilePath, System.Text.Encoding.UTF7)
                    LogEvent("fnGetDescription: Parsing complete for  " + FilePath, 2)
                    Dim SearchString As String = textReader
                    Dim SearchChar As String = "<b>Beschreibung:"
                    Dim StartPos As Integer
                    StartPos = InStr(1, SearchString, SearchChar, CompareMethod.Text)
                    Dim MidWords As String = Mid(textReader, StartPos + 20)
                    Dim EndPos As Integer
                    SearchChar = "</td>"
                    EndPos = InStr(1, MidWords, SearchChar, CompareMethod.Text)
                    Dim MidDescription As String = Microsoft.VisualBasic.Left(MidWords, EndPos - 1)
                    Dim Guzzidescription As String = Regex.Replace(MidDescription, "\r\n", " ")
                    Dim Guzzidescription2 As String = Regex.Replace(Guzzidescription, "\<[^\>]+\>", "")
                    'MsgBox(Guzzidescription2)
                    ReturnValue = Guzzidescription2
                Catch ex As Exception
                    LogEvent("fnGetDescription: ERROR - Cannot parse " + FilePath, 3)
                End Try

            Case "filename"
                Try
                    'ReturnValue = MI.Get_(StreamKind.General, 0, "FileName")
                    TempString = f.Name
                    'Console.WriteLine(f.Extension)
                    TempString = TempString.Replace(f.Extension, "")
                    'Put this bit in here to remove the '1of2' type bits using the system variable regex expression.
                    Dim SplitText As New Regex("\(" & CurrentSettings.RegEx_Check_For_MultiPart_Files & "\)")
                    TempString = SplitText.Replace(TempString, "")
                    SplitText = New Regex(CurrentSettings.RegEx_Check_For_MultiPart_Files)
                    TempString = SplitText.Replace(TempString, "")
                    ReturnValue = TempString

                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try

            Case "date"
                'GUZZI: Added to get Content (Date Added) of the index.php.htm File !!!
                'Dim FilePath As String = ""
                'Dim ReturnValue As String = ""
                Dim textReader As String = ""
                Dim FileName As String = ""
                Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)
                FileName = FileName.Replace(FileNameEnd, "")
                FilePath = FileName + "\index.php.htm"
                Dim fi As New IO.FileInfo(FilePath)
                If Not fi.Exists Then
                    LogEvent("fnGetDateAdded: Error - Cannot open file for analysis: " & FilePath, 3)
                    Return "ERROR : File " + FilePath + " cannot be found"
                    Exit Function
                End If
                Try
                    textReader = My.Computer.FileSystem.ReadAllText(FilePath, System.Text.Encoding.UTF7)
                    'MsgBox(textReader)
                    LogEvent("fnGetDateAdded: Parsing complete for  " + FilePath, 2)
                    Dim SearchString As String = textReader
                    Dim SearchChar As String = "<b>Eintragsdatum:</b>"
                    Dim StartPos As Integer
                    StartPos = InStr(1, SearchString, SearchChar, CompareMethod.Text)
                    Dim MidWords As String = Mid(textReader, StartPos + 3)
                    ' MsgBox(MidWords)
                    Dim EndPos As Integer
                    SearchChar = "</td>"
                    EndPos = InStr(20, MidWords, SearchChar, CompareMethod.Text)
                    Dim MidDescription As String = Microsoft.VisualBasic.Left(MidWords, EndPos - 1)
                    Dim Guzzidescription As String = Regex.Replace(MidDescription, "\r\n", " ")
                    Dim Guzzidescription2 As String = Regex.Replace(Guzzidescription, "\<[^\>]+\>", "")
                    Dim Guzzidescription3 As String = Guzzidescription2.Substring(InStrRev(Guzzidescription2, " "))
                    ReturnValue = Guzzidescription3
                Catch ex As Exception
                    LogEvent("fnGetDateAdded: ERROR - Cannot parse " + FilePath, 3)
                    'Console.WriteLine(ex.Message)
                    LogEvent("ERROR : " + ex.Message.ToString, EventLogLevel.ErrorOrSimilar)
                    ReturnValue = ""
                End Try

            Case Else
                ReturnValue = "Unknown Variable Requested"
        End Select
        'Console.WriteLine(DataItem.ToString + " - " + ReturnValue.ToString)
        '        MI.Close()
        Return ReturnValue

    End Function

    Public Function fnGetFolderName(ByVal FileName As String) As String
        'GUZZI: Added to get the Root Folder Name of a Moviefile !!!
        Dim TempString As String
        Dim ReturnValue As String
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)
        FileName = FileName.Replace(FileNameEnd, "")
        If FileName.Contains("\") Then
            TempString = FileName.Substring(InStrRev(FileName, "\"))
        Else
            TempString = FileName
        End If

        If TempString.ToLower = "video_ts" Then
            TempString = FileName.Replace(TempString, "")
            'Check that there isn't a trailing backslash (probably is)
            If TempString.EndsWith("\") = True Then
                TempString = TempString.Substring(0, Len(TempString) - 1)
            End If
            'Check to see if we've still got a nested path.  Take the next level up if so.
            If TempString.Contains("\") = True Then
                ReturnValue = TempString.Substring(InStrRev(TempString, "\"))
            Else
                ReturnValue = TempString
            End If

            'do more processing
        Else
            ReturnValue = TempString
        End If

        Return ReturnValue
    End Function

    Public Function fnGetFilePath(ByVal FileName As String) As String
        'GUZZI: Added to get the Root Folder Name of a Moviefile !!!
        Dim ReturnValue As String
        Dim FileNameEnd As String = FileName.Substring(InStrRev(FileName, "\") - 1)
        ReturnValue = FileName.Replace(FileNameEnd, "")
        Return ReturnValue
    End Function


    Public Function GetGroupName(ByVal FilePath As String, ByVal Movie_Title_Handling As String, ByVal Group_Name_Identifier As String)
        Dim ReturnValue As String = String.Empty

        '2001\2001 - A Space Odyssey.avi
        '2010\2010.avi
        'Cloverfield\Cloverfield 1of2.avi
        'Cloverfield\Cloverfield 2of2.avi
        'Elizabeth - The Golden Age\Elizabeth - The Golden Age.avi
        'Mist, the\Mist, the.avi
        'Oxford Murders\The Oxford Murders.avi
        'Peliculas\Resident Evil\Resident Evil 2 Apocalypse.avi
        'Peliculas\Resident Evil\Resident Evil Extinction.avi
        'Peliculas\Resident Evil\Resident Evil.avi
        'Romeo + Juliet\Romeo + Juliet.avi
        'Run Lola Run\Run Lola Run (1998).avi
        'Schindler's List\Schindler's List.avi
        'The Matrix Trilogy\The Matrix Reloaded.mkv
        'The Matrix Trilogy\The Matrix Revisited (1of2).mkv
        'The Matrix Trilogy\The Matrix Revisited (2of2).mkv
        'The Matrix Trilogy\The Matrix.mkv

        If FilePath.Contains("\") = False Then
            Return ReturnValue
        End If
        ' If parts can be found...
        If Group_Name_Identifier.Length = 0 Then ' no search expression defined - use standard rules
            If FilePath.Split("\").Length = 2 Then
                'Just a folder and a filename it seems - use the parent.
                ReturnValue = FilePath.Substring(0, FilePath.IndexOf("\"))
            Else
                Dim Blah As String() = FilePath.Split("\")
                'We should now have at least 3 strings, the last of which will be the filename.  Let's use the one before that:
                ' Todo depends on Title handling ...
                'File(Name)
                'Folder(Name)
                'Relative(Name)

                If Movie_Title_Handling.Contains("Folder") And Blah.Length > 2 Then ' if it's "folder handling", use parent folder of the movie folder
                    ReturnValue = Blah(Blah.Length - 3)
                Else
                    ReturnValue = Blah(Blah.Length - 2) ' use movie folder itself
                End If
            End If

        Else ' search expression found - search for last match
            Dim Blah As String() = FilePath.Split("\")
            'We should now have at least 3 strings, the last of which will be the filename.  Let's use the one before that:
            For Each Part As String In Blah
                If Part.Contains(Group_Name_Identifier) Then
                    ReturnValue = Part.Replace(Group_Name_Identifier, "").Trim
                End If
            Next
            ' finally we have the last found match, use it as ReturnValue
        End If

        Return ReturnValue

    End Function

    Public Function GetEdition(ByVal FilePath As String, ByVal Movie_Title_Handling As String)
        Dim ReturnValue As String = ""

        Dim RegCheck As Regex
        Dim Matches As MatchCollection


        For Each Rules In CurrentSettings.Edition_Strings.Split(";")
            Dim Rule() As String = Rules.Split("|")
            Dim EditionSearchExpression As String = Rule(0).ToString
            Dim EditionReplaceExpression As String = Rule(1).ToString
            Dim EditionFound As Boolean = False

            If EditionSearchExpression.Length > 0 Then
                If EditionSearchExpression.Length = 1 Then 'Probably not a regex, due to complexity of any single character, just do a replace.
                    If FilePath.Contains(EditionSearchExpression) Then
                        EditionFound = True
                    End If
                Else
                    RegCheck = New Regex(EditionSearchExpression) 'This should work for any multi-character string:
                    Matches = RegCheck.Matches(FilePath)
                    If (Matches.Count > 0) Then
                        EditionFound = True
                    End If
                End If
            End If
            If (EditionFound = True) Then
                ReturnValue = AddToCommaList(ReturnValue, EditionReplaceExpression)
            End If
        Next


        '[sS]tandard|Standard Edition;[eE]xtended|Extended Edition;[cC]ollector|Collectors Edition;[dD]irector|Directors Cut;[uU]nrated|Unrated
        'If FilePath.ToLower.Contains("standard") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Standard Edition")
        'End If

        'If FilePath.ToLower.Contains("extended") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Extended Edition")
        'End If

        'If FilePath.ToLower.Contains("collector") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Collectors Edition")
        'End If

        'If FilePath.ToLower.Contains("director") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Directors Cut")
        'End If

        'If FilePath.ToLower.Contains("unrated") = True Then
        '    ReturnValue = AddToCommaList(ReturnValue, "Unrated")
        'End If

        Return ReturnValue
    End Function

    Public Function AddToCommaList(ByVal CommaList As String, ByVal Value As String) As String

        If CommaList.Length = 0 Then
            CommaList = Value
        Else
            CommaList = CommaList + ", " + Value
        End If
        Return CommaList
    End Function

    <STAThread()> _
    Public Sub LogEvent(ByVal EventString As String, ByVal LogLevel As EventLogLevel)
        'LogLevels...
        ' 0 = None (no logging)
        ' 1 = InformationalWithGrabbing
        ' 2 = Informational
        ' 3 = Verbose (most events)
        ' 4 = Debug (all events)
        ' Errors = 3
        ' Major operations = 2
        ' Individual Files = 1

        'CurrentSettings values:
        'All Events with Grabbing
        'All Events
        'Major Events
        'ErrorsOnly

        Dim CurrentLogName As String = CurrentSettings.Log_Level
        Dim LogItem As Boolean = False
        If CurrentLogName = "All Events with Grabbing" Then
            LogItem = True
        ElseIf CurrentLogName = "All Events" Then
            If LogLevel >= 2 Then
                LogItem = True
            End If
        ElseIf CurrentLogName = "Major Events" Then
            If LogLevel >= 3 Then
                LogItem = True
            End If
        ElseIf CurrentLogName = "Errors Only" Then
            If LogLevel >= 4 Then
                LogItem = True
            End If
        End If


        'Sub to write log information.  If the GUI is running then it also appends to the txtProcess box.
        Dim path As String
        Dim LogText As String = My.Computer.Clock.LocalTime.ToString + " - " + EventString
        Dim LogDirectoryParam As String
        If My.Application.CommandLineArgs.Count > 1 Then
            If My.Application.CommandLineArgs.Item(1) = "LogDirectory" Then
                LogDirectoryParam = Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\log"
            Else
                LogDirectoryParam = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\" + My.Application.CommandLineArgs.Item(1)
            End If

            If CurrentSettings.LogDirectory.Length > 0 Then
                path = CurrentSettings.LogDirectory + "\MyFilmsAMCupdater.log"
            ElseIf Directory.Exists(LogDirectoryParam) Then
                path = LogDirectoryParam + "\MyFilmsAMCupdater.log"
            Else
                path = My.Application.Info.DirectoryPath & "\AMCUpdater.log"
            End If
        Else
            If (System.IO.Directory.Exists(Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\log")) Then
                path = Config.GetDirectoryInfo(Config.Dir.Config).ToString & "\log\AMCUpdater.log"
            Else
                path = My.Application.Info.DirectoryPath & "\AMCUpdater.log"
            End If
        End If


        If Form1.Visible = True Then
            dgLogWindow.txtLogWindow.AppendText(LogText & vbCrLf)
            If LogLevel = EventLogLevel.ImportantEvent Then
                Form1.ToolStripStatusLabel.Text = EventString
            End If
        End If

        If LogItem = True Then
            Try
                My.Computer.FileSystem.WriteAllText(path, vbCrLf + LogText, True)
            Catch ex As Exception

            End Try
        End If

    End Sub

    Public Enum ButtonStatus
        ReadyToParseXML = 0
        ReadyToSearchFolders = 1
        ReadyToFindOrphans = 2
        ReadyToDoImport = 3
        DisableAll = 4
    End Enum

    Public Sub SetCheckButtonStatus(ByVal ProcessStep As ButtonStatus)
        If ProcessStep = ButtonStatus.ReadyToParseXML Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = False
            Form1.btnFindOrphans.Enabled = False
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.ReadyToSearchFolders Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = True
            Form1.btnFindOrphans.Enabled = False
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.ReadyToFindOrphans Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = True
            Form1.btnFindOrphans.Enabled = True
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.ReadyToDoImport Then
            Form1.btnParseXML.Enabled = True
            Form1.btnProcessMovieList.Enabled = True
            Form1.btnFindOrphans.Enabled = True
            Form1.btnUpdateXML.Enabled = True
            Form1.btnJustDoIt.Enabled = True
        ElseIf ProcessStep = ButtonStatus.DisableAll Then
            Form1.btnParseXML.Enabled = False
            Form1.btnProcessMovieList.Enabled = False
            Form1.btnFindOrphans.Enabled = False
            Form1.btnUpdateXML.Enabled = False
            Form1.btnJustDoIt.Enabled = False
        End If
    End Sub


    Public Function GetFileChecksum(ByVal FilePath As String)

        Dim fs As FileStream = New FileStream(FilePath, FileMode.Open, FileAccess.Read)
        Dim length As Long = fs.Length
        'Falls over here because length is 734,208,000 bytes.
        length = 1024000
        Dim arr(length) As Byte
        'fs.Read(arr, 0, fs.Length)
        fs.Read(arr, 0, length)
        fs.Close()
        Dim tmpHash() As Byte
        tmpHash = New System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(arr)
        Return ByteArrayToString(tmpHash)
    End Function

    Public Function ByteArrayToString(ByVal arrInput() As Byte) As String
        Dim i As Integer
        Dim sOutput As New System.Text.StringBuilder(arrInput.Length)
        For i = 0 To arrInput.Length - 1
            sOutput.Append(arrInput(i).ToString("X2"))
        Next
        Return sOutput.ToString()
    End Function

    Public Function IsFileNeeded() As Boolean

        Dim ReturnValue As Boolean = False
        If Not CurrentSettings Is Nothing Then
            Dim MediaData As New ArrayList
            MediaData.Add("date")
            MediaData.Add("length")
            MediaData.Add("videobitrate")
            MediaData.Add("audiobitrate")
            MediaData.Add("disks")
            MediaData.Add("videoformat")
            MediaData.Add("audioformat")
            MediaData.Add("resolution")
            MediaData.Add("framerate")
            MediaData.Add("languages")
            MediaData.Add("subtitles")
            MediaData.Add("size")

            Dim DBFields() = CurrentSettings.Database_Fields_To_Import.Split(";")
            Dim FieldName As String
            Dim FieldChecked As Boolean
            Dim DatabaseFields As New Hashtable

            For Each blah As String In DBFields
                FieldName = blah.Substring(0, blah.IndexOf("|")).ToLower
                FieldChecked = blah.Substring(blah.IndexOf("|") + 1)
                If Not DatabaseFields.ContainsKey(FieldName) Then
                    DatabaseFields.Add(FieldName, FieldChecked)
                End If
            Next


            'If CurrentSettings.Movie_Title_Handling.Contains("Internet Lookup") Then
            '   MediaData.Add("originaltitle")
            'End If

            For Each blah As String In MediaData
                If DatabaseFields.ContainsKey(blah) Then
                    If DatabaseFields(blah) = True Then
                        ReturnValue = True
                    End If
                End If
            Next
        End If

        Return ReturnValue

    End Function

    Public Function IsInternetLookupNeeded() As Boolean

        Dim ReturnValue As Boolean = False
        If Not CurrentSettings Is Nothing Then
            Dim InternetData As New ArrayList
            InternetData.Add("rating")
            InternetData.Add("year")
            InternetData.Add("translatedtitle")
            InternetData.Add("director")
            InternetData.Add("producer")
            InternetData.Add("country")
            InternetData.Add("category")
            InternetData.Add("actors")
            InternetData.Add("url")
            InternetData.Add("description")
            InternetData.Add("comments")
            InternetData.Add("picture")
            'InternetData.Add("language")
            InternetData.Add("tagline")
            InternetData.Add("certification")
            InternetData.Add("writer")
            'InternetData.Add("languages")
            InternetData.Add("imdb_id")
            InternetData.Add("tmdb_id")
            InternetData.Add("imdbrank")
            InternetData.Add("studio")
            InternetData.Add("fanart") ' Guzzi: added on request Dadeo


            If CurrentSettings.Movie_Title_Handling.Contains("Internet Lookup") Then
                InternetData.Add("originaltitle")
            End If

            Dim DBFields() = CurrentSettings.Database_Fields_To_Import.Split(";")
            Dim FieldName As String
            Dim FieldChecked As Boolean
            Dim DatabaseFields As New Hashtable

            For Each blah As String In DBFields
                FieldName = blah.Substring(0, blah.IndexOf("|")).ToLower
                FieldChecked = blah.Substring(blah.IndexOf("|") + 1)
                If Not DatabaseFields.ContainsKey(FieldName) Then
                    DatabaseFields.Add(FieldName, FieldChecked)
                End If
            Next

            For Each blah As String In InternetData
                If DatabaseFields.ContainsKey(blah) Then
                    If DatabaseFields(blah) = True Then
                        ReturnValue = True
                    End If
                End If
            Next

        End If

        Return ReturnValue

    End Function

    Public Function GetFanartTitle(ByVal MovieRecord As XmlElement, ByRef title As String, ByRef ttitle As String, ByRef ftitle As String, ByRef year As Integer, ByRef director As String) As String
        Dim fanartTitle As String = ""
        Dim mastertitle As String = String.Empty
        title = String.Empty
        ttitle = String.Empty
        ftitle = String.Empty
        director = String.Empty
        year = 0

        If Not MovieRecord.Attributes("OriginalTitle") Is Nothing Then
            title = RemoveGroupName(MovieRecord.Attributes("OriginalTitle").Value)
            If CurrentSettings.Master_Title = "OriginalTitle" Then
                mastertitle = title
            End If
        End If

        If Not MovieRecord.Attributes("TranslatedTitle") Is Nothing Then
            ttitle = RemoveGroupName(MovieRecord.Attributes("TranslatedTitle").Value)
            'If ttitle.Contains("(") Then
            '    ttitle = ttitle.Substring(0, ttitle.IndexOf("("))
            'End If
            If CurrentSettings.Master_Title = "TranslatedTitle" Then
                mastertitle = ttitle
            End If
        End If

        If Not MovieRecord.Attributes("FormattedTitle") Is Nothing Then
            ftitle = RemoveGroupName(MovieRecord.Attributes("FormattedTitle").Value)
            If CurrentSettings.Master_Title = "FormattedTitle" Then
                mastertitle = ftitle
            End If
        End If

        If title.ToString().Length > 0 Then
            If Not MovieRecord.Attributes("Year") Is Nothing Then
                Try
                    year = CInt(MovieRecord.Attributes("Year").Value)
                Catch
                    year = 0
                End Try
            End If
            If Not MovieRecord.Attributes("Director") Is Nothing Then
                director = MovieRecord.Attributes("Director").Value
            Else
                director = ""
            End If
        End If


        If CurrentSettings.Master_Title = "FormattedTitle" Then ' // special setting for formatted title - we don't want to use it, as it is usually too long and causes problems with path length
            If Not String.IsNullOrEmpty(ttitle) Then
                fanartTitle = ttitle
            ElseIf Not String.IsNullOrEmpty(title) Then
                fanartTitle = title
            Else
                fanartTitle = ftitle
            End If
        Else
            If Not String.IsNullOrEmpty(mastertitle) Then
                fanartTitle = mastertitle
            ElseIf Not String.IsNullOrEmpty(title) Then
                fanartTitle = title
            ElseIf Not String.IsNullOrEmpty(ttitle) Then
                fanartTitle = ttitle
            ElseIf Not String.IsNullOrEmpty(ftitle) Then
                fanartTitle = ftitle
            Else
                fanartTitle = ""
            End If
        End If
        Return fanartTitle
    End Function
    Public Function RemoveGroupName(ByVal FullName As String) As String
        Dim Name As String
        If FullName.Contains("\") = True Then
            'FullName = FullName.Substring(0, FullName.IndexOf("\") - 1) ' remove rest of group name part (group fanart)
            Name = FullName.Substring(FullName.IndexOf("\") + 1) ' remove group name part
        Else
            Name = FullName
        End If
        Return Name
    End Function

    '<FlagsAttribute()> Public Enum EAccessType As Integer
    '    change = 1
    '    insert = 2
    '    delete = 4
    '    move = 8
    '    copy = 16
    'End Enum

    '' ermittelt die EmumMember Namen
    '' Bsp: ist Value 7 (change AND insert AND delete) ist das Ergebnis {"change", "insert", "delete"}
    'Public Function GetEnumItemNames(ByVal Value As EAccessType) As String()
    '    Dim a As String
    '    Dim b As String
    '    Dim c As String()

    '    a = Value.ToString
    '    a = a.Replace(" ", "")
    '    c = Split(a, ",")

    '    a = ""
    '    For Each b In c
    '        If a <> "" Then a &= ","
    '        a &= GetEnumValue(b)
    '    Next
    '    c = Split(a, ",")
    '    Return c
    'End Function

    '' Ermittel den EnumMember Wert anhand dessen Namen 
    'Private Function GetEnumValue(ByVal strName As String) As EAccessType
    '    Enum.Parse(EAccessType.GetType(), strName) ' @_ntr_: danke ;-)
    'End Function

End Module
