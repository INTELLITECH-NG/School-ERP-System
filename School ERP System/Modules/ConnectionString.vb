﻿Imports System.IO
Module ConnectionString
    Dim st As String
    Public Function ReadCS() As String
        Using sr As StreamReader = New StreamReader(Application.StartupPath & "\SQLSettings1.dat")
            st = sr.ReadLine()
        End Using
        Return st
    End Function
    Public ReadOnly cs As String = ReadCS()
End Module
