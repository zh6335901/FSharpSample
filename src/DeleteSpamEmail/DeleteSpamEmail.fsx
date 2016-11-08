#r @"../packages/MailKit/lib/net45/MailKit.dll"
#r @"../packages/MimeKit/lib/net45/MimeKit.dll"

open System
open System.Linq
open System.Threading
open System.Net.Mail
open MailKit.Net.Pop3
open MimeKit

type ClientInfo = { Server: string; Port: int; Username: string; Password: string }
type Result = 
    | Success of string
    | SmtpError of SmtpException

module MailClient = 
    let getClient server port = 
        let client = new Pop3Client()
        client.Connect(server, port, false)
        client

    let authenticate (username: string) (password: string) (client: Pop3Client)  =
        client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
        client.Authenticate(username, password)
        client

    let getMessageIds (predicator) (client: Pop3Client) = [ 
        for i in [0..client.Count] do
            let message = client.GetMessage(i)
            if predicator message.Subject 
            then yield message.MessageId
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
        | :? SmtpException as ex -> SmtpError ex 


let clientInfo = { Username = "jeffrey.zhang@hirede.com"; Password = "zhang1991723"; Server = "smtp.ym.163.com"; Port = 110 } 
let result = App.run (fun subject -> subject.Contains("双11")) clientInfo

match result with
| Success info -> printfn "%s" info
| SmtpError ex -> printfn "%s" ex.Message

        




