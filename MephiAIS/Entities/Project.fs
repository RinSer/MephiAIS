namespace RPSAdmin

    // === Проекты ===
    type Project(id: int, title: string, description: string) =
        inherit Entity<Project>(id)

        member this.Title with get() = title
        member this.Description with get() = description
        
        member this.ProjectItems with get() =
            Array.filter (fun(item: ProjectItem) -> item.ProjectId = this.Id) Entity<ProjectItem>.GetAll

        member this.Users with get() =
            Array.map (fun(item: ProjectItem) -> item.AssignedTo) this.ProjectItems

        override this.ToString() = sprintf "Title: %s, Description: %s" this.Title this.Description