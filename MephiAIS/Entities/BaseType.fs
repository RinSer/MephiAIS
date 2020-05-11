namespace RPSAdmin

    [<AbstractClass>]
    type Entity<'T when 'T :> Entity<'T>>(id: int) =
        
        let _id = id

        member this.Id = _id