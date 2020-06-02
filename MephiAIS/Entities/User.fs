namespace RPSAdmin

    // === Пользователь ===
    type User(id: int, login: string, password: string, surname: string, name: string, patronymicName: string, role: string) =
        inherit Entity<User>(id)

        let mutable _login = login
        let mutable _password = password
        let mutable _surname = surname
        let mutable _name = name
        let mutable _patronymicName = patronymicName
        let mutable _role = role

        let mutable _projectItems: ProjectItem array = [||]

        member this.Login 
            with get() = _login
            and set(login: string) = _login <- login
        member this.Password 
            with get() = _password
            and set(password: string) = _password <- password
        member this.Surname 
            with get() = _surname
            and set(surname: string) = _surname <- surname
        member this.Name 
            with get() = _name
            and set(name: string) = _name <- name
        member this.PatronymicName 
            with get() = _patronymicName
            and set(patronymicName: string) = _patronymicName <- patronymicName
        member this.Role 
            with get() = _role
            and set(role: string) = _role <- role

        member this.ProjectItems 
            with get() = _projectItems
            and set(items: ProjectItem array) = _projectItems <- items

        member this.addProjectItem(item: ProjectItem) =
            _projectItems <- Array.append _projectItems [| item |]