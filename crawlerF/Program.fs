// Learn more about F# at http://fsharp.org

open System
open System.IO
open FSharp.Data
open FSharp.Data

    // Process execution params
    // args(0) = nesting level
    // args(1) = csv file name
[<EntryPoint>]
let main (args: string[]) =
    printfn "Arguments passes in: %A" args
    // F# searches the following directory at runtime : C:\Users\userName\source\repos\crawlerF\crawlerF\bin\Debug\netcoreapp2.1\test.csv'.
    // It is therefore necessary to provide the top level of the repo for the file read by prepending ../../../../ to the file name
    let filePath = "../../../../" + args.[1]    
    let htmlPage = HtmlDocument.Load("https://scrapethissite.com/")
    printfn "%s" (string htmlPage)

    let links = 
        htmlPage.CssSelect("a")
        |> List.map(fun a -> a.AttributeValue("href"))
        |> Seq.distinctBy id
        |> Seq.toList
   
    printfn "Links: %A" links
   
    let urlList (filePath:string) = seq {
        use sr = new StreamReader (filePath)
        while not sr.EndOfStream do
            yield sr.ReadLine
    }
    

    //urlList(filePath)
    //|> printfn "Here %A"
    
    
    printfn "END"
    Console.ReadKey() |> ignore
    0 // return an integer exit code