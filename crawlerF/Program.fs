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
    let filePath = "../../../../" + args.[1]
    let illegalChars = "<>:\"/\\|?*=."

    let stripChars = 
        String.map (fun s -> if Seq.exists((=)s) illegalChars then '_' else s)     
    
    let savePage (url : string, page : HtmlDocument) =
        let fileName = stripChars url
        printfn "File Created: %s" fileName
        use streamWriter = new StreamWriter("../../../Saved Pages/" + fileName + ".txt")
        streamWriter.WriteLine(page.ToString())
        page   

    let rec crawlPage (page : String, nestingLevel : int) : list<string> =    
        System.Threading.Thread.Sleep 1200
        printfn "Crawling URL: %s" page
        printfn "Nesting Level: %i \n" nestingLevel
        try 
            let pageHtml = HtmlDocument.Load(page)
            savePage(page, pageHtml)
            |> fun m -> m.CssSelect("a")
            |> List.map(fun a -> a.AttributeValue("href"))
            |> List.distinctBy id
            |> List.filter (fun x -> not(x.Contains "http"))
            |> List.map (fun x -> page + x) 
            |> List.map (fun x -> 
                match nestingLevel with
                | _ when (nestingLevel > 0) -> crawlPage(x, (nestingLevel - 1)) 
                | _ -> List.singleton x)
            |> List.concat
            |> List.distinctBy id
        with
        | :? System.Exception -> printfn "Page: %s caused exception" page; List.empty
   
    let urlList (filePath:string) = 
        use sr = new StreamReader (filePath)
        let arry = 
            sr.ReadToEnd()
            |> fun x -> x.Split([|','|])
        Seq.ofArray(arry)    

    let asyncCrawl (url : string) = 
        async {
           crawlPage(url, args.[0] |> int) |> ignore
        }
        
    let targetUrls = urlList(filePath)
    targetUrls |> Seq.map (fun x -> printfn "target url: %s" x) |> ignore

    targetUrls 
    |> Seq.map (fun x -> asyncCrawl x) 
    |> Seq.map (fun x -> Async.StartAsTask x) 
    |> System.Threading.Tasks.Task.WhenAll
    |> ignore
    
    printfn "Press any key to exit..."
    Console.ReadKey() |> ignore
    0 // return an integer exit code