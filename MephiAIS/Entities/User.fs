namespace RPSAdmin

    // === Пользователь ===
    type User(id: int, login: string, password: string, surname: string, name: string, patronumicName: string, role: string) =
        inherit Entity<User>(id)

        let mutable _projectItems: ProjectItem array = [||]

        member this.Login with get() = login
        member this.Password with get() = password
        member this.Surname with get() = surname
        member this.Name with get() = name
        member this.PatronymicName with get() = patronumicName
        member this.Role with get() = role

        member this.ProjectItems 
            with get() = _projectItems
            and set(items: ProjectItem array) = _projectItems <- items

        override this.ToString() = sprintf "Login: %s, Name: %s" this.Login this.Name