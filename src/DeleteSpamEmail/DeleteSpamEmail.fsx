#r @"../packages/MailKit/lib/net45/MailKit.dll"
#r @"../packages/MimeKit/lib/net45/MimeKit.dll"

open System
open MailKit.Net.Pop3
open MimeKit

type ClientInfo = { Server: string; Port: int; Username: string; Password: string }

module RetrieveMessage = 
    let getClient server port = 
        let client = new Pop3Client()
        client.Connect(server, port)
        client

    let authenticate (username: string, password: string) (client: Pop3Client)  =
        client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
        client.Authenticate(username, password)
        client

    let getMessages (predicator) (client: Pop3Client) = [ 
        for i in [0..client.Count] do
            let message = client.GetMessage(i)
            if predicator message.Subject 
            then yield message
    ]