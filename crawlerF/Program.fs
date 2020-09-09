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
    let nestingLevel = 2
    let filePath = "../../../../" + args.[1]    
    let baseUrl = "https://scrapethissite.com"
    let htmlPage = HtmlDocument.Load(baseUrl)

    let rec crawlPage (page : String, nestingLevel : int) =    
        HtmlDocument.Load(page)
        |> fun m -> m.CssSelect("a")
        |> List.map(fun a -> a.AttributeValue("href"))
        |> Seq.distinctBy id
        |> Seq.map (fun x -> baseUrl + x)
        |> Seq.map (fun x -> 
            match nestingLevel with
            | _ when (nestingLevel > 0) -> printfn "Test1 %s" x //crawlPage(x, (nestingLevel - 1))
            | _ when (nestingLevel <= 0) -> printfn "Test2 %s" x  //ignore
            | _ -> printfn "Test3 %s" x  //ignore // To silence warnings.
        )
    
    //printfn "Here: %A" (Array.append urls1, crawlPage baseUrl)

    let saw = crawlPage(baseUrl, nestingLevel)
    printfn "Final Print %A" saw

    // Create navigable links out of the retrieved attribute strings
   
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