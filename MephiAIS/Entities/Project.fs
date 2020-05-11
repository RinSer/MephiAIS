namespace RPSAdmin

    // === Проект ===
    type Project(id: int, title: string, description: string) =
        inherit Entity<Project>(id)

        let mutable _users: User array = [||]
        let mutable _projectItems: ProjectItem array = [||]

        member this.Title with get() = title
        member this.Description with get() = description

        member this.Users 
            with get() = _users
            and set(users: User array) = _users <- users
        member this.ProjectItems 
            with get() = _projectItems
            and set(projectItems: ProjectItem array) = _projectItems <- projectItems

        member this.addUser(user: User) = _users <- Array.append _users [| user |]

        member this.updateUser(user: User) = 
            let idx = Array.findIndex (fun(u: User) -> u.Id = user.Id) _users
            user.ProjectItems <- _users.[idx].ProjectItems
            _users.[idx] <- user

        member this.addProjectItem(item: ProjectItem, users: User array) = 
            for user in users do
                user.ProjectItems <- Array.append user.ProjectItems [| item |]
            _projectItems <- Array.append _projectItems [| item |]

        member this.updateProjectItem(item: ProjectItem, users: User array) = 
            let idx = Array.findIndex (fun(pi: ProjectItem) -> pi.Id = item.Id) _projectItems
            for user in this.Users do
                user.ProjectItems <- Array.filter (fun pi -> pi.Id <> item.Id) user.ProjectItems
            for user in users do
                user.ProjectItems <- Array.append user.ProjectItems [| item |]
            _projectItems.[idx] <- item

        override this.ToString() = sprintf "Title: %s, Description: %s" this.Title this.Description