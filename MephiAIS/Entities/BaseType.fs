namespace RPSAdmin

    [<AbstractClass>]
    type Entity<'T when 'T :> Entity<'T>>(id: int) =
        
        let _id = id

        static let mutable Items: 'T array = [||]

        member this.Id = _id

        static member GetAll = Items

        static member Add(item: 'T) = Items <- Array.append Items [| item |]

        static member Get(id: int) = Array.find (fun(item: 'T) -> item.Id = id)  Items