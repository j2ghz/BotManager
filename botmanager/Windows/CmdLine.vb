﻿Imports System.Threading
Imports BotManager.Manager.Properties
Imports System.Management

Namespace Windows
    Public Class CmdLine
        Public Shared Function Run(ByRef pInfo As ProcessStartInfo, waitForExit As Boolean) As Process
            'Dim pInfo As New ProcessStartInfo
            'pInfo.WorkingDirectory = Path.GetDirectoryName(botProperties.TempExecutablePath)
            'pInfo.FileName = Path.GetFileName(botProperties.TempExecutablePath)

            Dim p As Process = Process.Start(pInfo)
            If waitForExit Then
                While Not p.HasExited
                    Thread.Sleep(100)
                End While
            End If
            Thread.Sleep(100)

            Return p
        End Function

        Public Shared Sub Kill(ByRef botProperties As Bot)
            Try
                Dim p As Process = Process.GetProcessById(botProperties.ProcessId)
                p.Kill()
                botProperties.IsRunning = False
            Catch
            End Try
        End Sub

        Public Shared Function IsRunning(ByRef botProperties As Bot) As Boolean
            Try
                Dim p As Process = Process.GetProcessById(botProperties.ProcessId)
                If Not p.Responding Then
                    Kill(botProperties)
                    Return False
                End If
                Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE Name='WerFault.exe'")

                For Each process As ManagementObject in searcher.Get()
                    If process("CommandLine").ToString().Contains(botProperties.ProcessId.ToString())
                        process.InvokeMethod("Terminate", Nothing)
                        Return False
                    End If
                Next

                Return True
            Catch
                Return False
            End Try
        End Function
    End Class
End NameSpace