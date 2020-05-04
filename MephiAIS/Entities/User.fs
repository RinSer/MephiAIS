namespace RPSAdmin

    // === Пользователи ===
    type User(id: int, login: string, password: string, surname: string, name: string, patronumicName: string, role: string) =
        inherit Entity<User>(id)

        member this.Login with get() = login
        member this.Password with get() = password
        member this.Surname with get() = surname
        member this.Name with get() = name
        member this.PatronymicName with get() = patronumicName
        member this.Role with get() = role

        static member Update(user: User) = 
            let existingIdx = Array.findIndex (fun(usr: User) -> usr.Id = user.Id) Entity<User>.GetAll
            Entity<User>.GetAll.[existingIdx] <- user

        override this.ToString() = sprintf "Login: %s, Name: %s" this.Login this.Name