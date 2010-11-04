Imports System.Xml.Serialization
Imports System.IO

Public Class VideosStoreXML
    Inherits Videos
    Public Shared DS As DataSet = New DataSet


    Public Overrides Sub LoadData()
        Dim SerialXML As XmlSerializer = Nothing
        Dim StreamIO As StreamReader = Nothing


        Dim uneTable As DataTable
        Dim unevue As DataView

        DS.ReadXml(CurrentSettings.XML_File)
        uneTable = DS.Tables("Movie")
        unevue = New DataView(uneTable)
        Me.ListVidéos = uneTable

    End Sub

    Public Overrides Sub SaveData()
        Dim SerialXML As XmlSerializer = Nothing
        Dim StreamIO As StreamWriter = Nothing

        Try
            DS.WriteXml("z:\Divx_Perso1.xml")
            'SerialXML = New XmlSerializer(GetType(VidéosStoreXML))
            'StreamIO = New StreamWriter("C:\Vidéos.xml")
            'SerialXML.Serialize(StreamIO, Me)

        Catch ex As Exception
            Throw ex

        Finally
            If StreamIO IsNot Nothing Then
                StreamIO.Close()
            End If
        End Try

    End Sub
End Class
