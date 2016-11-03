#r @"../packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open System
open System.IO

module Helper = 
    let (</>) a b = Path.Combine(a, b)

module BingWallpaper = 
    open Helper
    open FSharp.Data

    type Wallpaper = JsonProvider<"BingWallpaper.json">

    let downloadUrl = 
        let url = @"http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US"
        let data = Wallpaper.Load(url)
        (data.Images |> Array.head).Url

module Web =
    open System.Net

    let downloadFile (url:string) (dest:string) = 
        use wc = new WebClient()
        wc.DownloadFile(url, dest)      
    
module App =
    open Helper
    
    let run = 
        let filename = __SOURCE_DIRECTORY__ </> sprintf @"images/%s.jpg" (DateTime.Now.ToString("yyyyMMddHHmmss"))
        Directory.CreateDirectory(__SOURCE_DIRECTORY__ </> "images") |> ignore
        Web.downloadFile BingWallpaper.downloadUrl filename
        printfn @"%s" filename

App.run