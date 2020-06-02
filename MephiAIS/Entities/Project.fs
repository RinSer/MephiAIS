namespace RPSAdmin

    open MongoDB.Bson
    open MongoDB.Driver

    // === Проект ===
    type Project(id: int, title: string, description: string) =
        inherit Entity<Project>(id)

        let mutable _dbCollection: IMongoCollection<Project> = null

        let mutable _title = title
        let mutable _description = description

        let mutable _users: User array = [||]
        let mutable _projectItems: ProjectItem array = [||]

        member this.Title 
            with get() = _title
            and set(title: string) = _title <- title
        member this.Description 
            with get() = _description
            and set(description: string) = _description <- description

        member this.Users
            with get() = _users
            and set(users: User array) = _users <- users
        member this.ProjectItems 
            with get() = _projectItems
            and set(projectItems: ProjectItem array) = _projectItems <- projectItems

        member this.setDbCollection(dbCollection: IMongoCollection<Project>) =
            _dbCollection <- dbCollection

        member this.dbFilter() = 
            let filterString = sprintf "{ _id: { $eq: %d } }" this.Id
            let filterBson = BsonDocument.Parse(filterString)
            BsonDocumentFilterDefinition(filterBson)

        member this.tryUpdateInDb() =
            if _dbCollection <> null then
                let result = _dbCollection.DeleteOne(this.dbFilter())
                _dbCollection.InsertOne(this)

        member this.addUser(user: User) = 
            _users <- Array.append _users [| user |]
            this.tryUpdateInDb()

        member this.updateUser(user: User) = 
            let idx = Array.findIndex (fun(u: User) -> u.Id = user.Id) _users
            user.ProjectItems <- _users.[idx].ProjectItems
            _users.[idx] <- user
            this.tryUpdateInDb()

        member this.addProjectItem(item: ProjectItem, users: User array) = 
            for user in users do
                if not <| Array.contains user this.Users then
                    this.addUser(user)
                user.ProjectItems <- Array.append user.ProjectItems [| item |]
            _projectItems <- Array.append _projectItems [| item |]
            this.tryUpdateInDb()

        member this.updateProjectItem(item: ProjectItem, users: User array) = 
            let idx = Array.findIndex (fun(pi: ProjectItem) -> pi.Id = item.Id) _projectItems
            for user in this.Users do
                user.ProjectItems <- Array.filter (fun pi -> pi.Id <> item.Id) user.ProjectItems
            for user in users do
                if not <| Array.contains user this.Users then
                    this.addUser(user)
                user.ProjectItems <- Array.append user.ProjectItems [| item |]
            _projectItems.[idx] <- item
            this.tryUpdateInDb()