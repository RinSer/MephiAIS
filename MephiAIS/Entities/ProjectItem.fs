namespace RPSAdmin

    open System

    // === Tasks, UserStories and Issues ===
    type ProjectItem(id: int, title: string, description: string, projectId: int,
        startTime: DateTime, endTime: DateTime, status: string, category: string, assignedTo: User) =
        inherit Entity<ProjectItem>(id)

        member this.Title with get() = title
        member this.Description with get() = description
        member this.ProjectId with get() = projectId
        member this.StartTime with get() = startTime
        member this.EndTime with get() = endTime
        member this.Status with get() = status
        member this.Category with get() = category
        member this.AssignedTo with get() = assignedTo

        static member GetByCategory(category: string) =
            Array.filter (fun(item: ProjectItem) -> item.Category = category) Entity<ProjectItem>.GetAll

        override this.ToString() = sprintf "Title: %s, Category: %s" this.Title this.Category