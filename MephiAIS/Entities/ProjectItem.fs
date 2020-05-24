namespace RPSAdmin

    open System

    // === Task, UserStory or Issue ===
    type ProjectItem(id: int, title: string, description: string,
        startTime: DateTime, endTime: DateTime, status: string, category: string) =
        inherit Entity<ProjectItem>(id)

        member this.Title with get() = title
        member this.Description with get() = description
        member this.StartTime with get() = startTime
        member this.EndTime with get() = endTime
        member this.Status with get() = status
        member this.Category with get() = category