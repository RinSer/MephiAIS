namespace RPSAdmin

    open MongoDB.Driver

    // === Приложение ===
    type App(dbCollection: IMongoCollection<Project>) =
        let _dbCollection: IMongoCollection<Project> = dbCollection
        
        let mutable _projects: Project array = 
            match _dbCollection with
                | null -> [||]
                | _ -> [ for p in _dbCollection.AsQueryable<Project>().ToEnumerable<Project>() -> 
                                            p.setDbCollection(_dbCollection); p ] |> Seq.toArray
                
        member this.Projects = _projects

        member this.addProject(project: Project) =
            if _dbCollection <> null then
                project.setDbCollection(_dbCollection)
                _dbCollection.InsertOne(project)
            _projects <- Array.append _projects [| project |]
                

        member this.updateProject(project: Project) =
            let idx = Array.findIndex (fun(p: Project) -> p.Id = project.Id) _projects
            project.Users <- _projects.[idx].Users
            _projects.[idx] <- project
            if _dbCollection <> null then
                _projects.[idx].setDbCollection(_dbCollection)
                _projects.[idx].tryUpdateInDb()

        member this.getProjectItemsByCategory(category: string) =
            Array.filter (fun(projectItem: ProjectItem) -> projectItem.Category = category) 
                (Array.collect (fun(project: Project) -> project.ProjectItems) _projects)