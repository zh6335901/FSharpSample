[<AutoOpen>]
module Types = 
    open System

    type Id = Id of string
    type Message = Message of string
    type Author = { Name: string; Email: string }
    type Commit = {
        Id: Id
        Author: Author
        Date: DateTimeOffset
        Message: Message
    }

#r @"../packages/fparsec/lib/net40-client/fparseccs.dll"
#r @"../packages/fparsec/lib/net40-client/fparsec.dll" 

module GitlogParser =
    open System
    open FParsec

    let str_ws s = pstring s .>> spaces