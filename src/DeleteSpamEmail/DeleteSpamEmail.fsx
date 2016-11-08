#r @"../packages/MailKit/lib/net45/MailKit.dll"
#r @"../packages/MimeKit/lib/net45/MimeKit.dll"

open System
open System.Linq
open System.Threading
open System.IO
open MailKit.Net.Pop3
open MimeKit

type ClientInfo = { Server: string; Port: int; Username: string; Password: string }
type Result = 
    | Success of string
    | IOError of IOException

module MailClient = 
    let getClient server port = 
        let client = new Pop3Client()
        client.Connect(server, port, false)
        client

    let authenticate (username: string) (password: string) (client: Pop3Client) =
        client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
        client.Authenticate(username, password)
        client

    let getMessageIds (predicator) (client: Pop3Client) = [ 
        for i in [0..client.Count] do
            let message = client.GetMessage(i)
            if predicator message.Subject 
            then yield (sprintf "get message %s \r\n" message.MessageId)
    ]

    let deleteMessages (client: Pop3Client) (uids: string list) = 
        client.DeleteMessages(uids.ToList()) |> ignore

module App =
    open MailClient
    let run predicator clientInfo = 
        try
            use client = getClient clientInfo.Server clientInfo.Port
            let messageIds = 
                client |> authenticate clientInfo.Username clientInfo.Password |> getMessageIds predicator

            deleteMessages client messageIds
            Success (sprintf "delete %s mails success" clientInfo.Username)
        with
        | :? IOException as ex -> IOError ex 


let clientInfo = { Username = "example@123.com"; Password = "123456"; Server = "pop.example.com"; Port = 110 } 
let result = App.run (fun subject -> subject.Contains("双11")) clientInfo

match result with
| Success info -> printfn "%s" info
| IOError ex -> printfn "%s" ex.Message

        




