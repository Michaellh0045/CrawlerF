// Learn more about F# at http://fsharp.org

open System
open System.IO
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
    let baseUrl1 = "https://scrapethissite.com"
    let baseUrl2 = "https://cdc.gov"
    let https = "https://"
    //let baseU = "scrapethissite.com"
    //let htmlPage = HtmlDocument.Load(baseUrl)

    let strPrint (str : string) : string =
        printfn "After Distinct: %s" str
        str

    let rec crawlPage (page : String, nestingLevel : int) : list<string> =    
        System.Threading.Thread.Sleep 1200
        printfn "Crawling URL: %s" page
        printfn "Nesting Level: %i \n" nestingLevel
        try 
            HtmlDocument.Load(page)
            |> fun m -> m.CssSelect("a")
            |> List.map(fun a -> a.AttributeValue("href"))
            |> List.distinctBy id
            |> List.filter (fun x -> not(x.Contains "http"))
            |> List.map (fun x -> baseUrl1 + x) 
            |> List.map (fun x -> 
                match nestingLevel with
                | _ when (nestingLevel > 0) -> crawlPage(x, (nestingLevel - 1)) 
                | _ -> List.singleton x)
            |> List.concat
            |> List.distinctBy id
        with
        | :? System.Net.WebException -> printfn "Page: %s caused exception" page; List.empty
        | :? System.Exception -> printfn "Page: %s caused exception" page; List.empty

    let saw = crawlPage(baseUrl1, nestingLevel)
    saw |> List.iter (fun x -> printfn "Look Here -> %s" x)
    //printfn "Final Print %A" saw

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