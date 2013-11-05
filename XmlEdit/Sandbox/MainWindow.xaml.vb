Imports XmlEdit
Imports System.Xml
Imports System

Class MainWindow

    Private document As XmlDocumentAlterer = New XmlDocumentAlterer()

    Private Sub Proc(act As Action)
        Try
            act()
            doc.Text = document.Document.InnerXml
        Catch ex As Exception
            doc.Text = ex.Message
        End Try
    End Sub

    Private Sub fillIn_Click(sender As Object, e As RoutedEventArgs) Handles fillIn.Click
        Proc(Sub() document.FillIn(xpression.Text))
    End Sub

    Private Sub setVal_Click(sender As Object, e As RoutedEventArgs) Handles setVal.Click
        Proc(Sub() document.SetValue(xpression.Text, val.Text))
    End Sub

    Private Sub reset_Click(sender As Object, e As RoutedEventArgs) Handles reset.Click
        Proc(Sub() document = New XmlDocumentAlterer())
    End Sub

    Private Sub getVal_Click(sender As Object, e As RoutedEventArgs) Handles getVal.Click
        Proc(Sub() val.Text = document.GetValue(xpression.Text, FailureReaction.GenerateErrorOnNull))
    End Sub
End Class
