namespace RPSAdmin

    [<AbstractClass>]
    type Entity<'T when 'T :> Entity<'T>>(id: int) =
        
        let mutable _id = id

        member this.Id
            with get() = _id
            and set(id: int) = _id <- id