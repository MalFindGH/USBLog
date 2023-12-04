Imports System.IO
Imports System.Management
Imports System.Threading

Module Module1
    Sub Main()
        Dim query As New WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 OR EventType = 3")
        Dim watcher As New ManagementEventWatcher(query)
        Console.WriteLine("USB Monitor started. Press any key to exit.")
        AddHandler watcher.EventArrived, AddressOf USBEventArrived
        watcher.Start()
        Console.ReadKey(True)
        Console.WriteLine($"Exited at {My.Computer.Clock.LocalTime.ToShortTimeString()}")
        watcher.Stop()
    End Sub
    Private Sub USBEventArrived(sender As Object, e As EventArrivedEventArgs)
        Dim driveLetter As String = e.NewEvent.Properties("DriveName").Value.ToString() '.Replace(":", "")
        Dim eventType As Integer = CInt(e.NewEvent.Properties("EventType").Value)
        Dim volumeName As String = GetVolumeLabel(driveLetter)
        If eventType = 2 Then
            Console.WriteLine($"[ USB:{driveLetter.Replace(":", "")} ] [ PLUGGED_IN ] [ {DateTime.Now.ToShortTimeString()} ] [ NAME: {volumeName} ]")
        ElseIf eventType = 3 Then
            Console.WriteLine($"[ USB:{driveLetter.Replace(":", "")} ] [ UNPLUGGED ] [ {DateTime.Now.ToShortTimeString()} ]")
        End If
    End Sub
    Private Function GetVolumeLabel(driveLetter As String) As String
        Dim volumeInfo As DriveInfo = New DriveInfo(driveLetter)
        If volumeInfo.IsReady Then
            Return volumeInfo.VolumeLabel
        Else
            Return "N/A"
        End If
    End Function
End Module