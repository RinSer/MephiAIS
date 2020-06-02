namespace RPSAdmin

    open System

    // === Task, UserStory or Issue ===
    type ProjectItem(id: int, title: string, description: string,
        startTime: DateTime, endTime: DateTime, status: string, category: string) =
        inherit Entity<ProjectItem>(id)

        let mutable _title = title
        let mutable _description = description
        let mutable _startTime = startTime
        let mutable _endTime = endTime
        let mutable _status = status
        let mutable _category = category

        member this.Title 
            with get() = _title
            and set(title: string) = _title <- title
        member this.Description 
            with get() = _description
            and set(description: string) = _description <- description
        member this.StartTime 
            with get() = _startTime
            and set(startTime: DateTime) = _startTime <- startTime
        member this.EndTime 
            with get() = _endTime
            and set(endTime: DateTime) = _endTime <- endTime
        member this.Status 
            with get() = _status
            and set(status: string) = _status <- status
        member this.Category 
            with get() = _category
            and set(category: string) = _category <- category