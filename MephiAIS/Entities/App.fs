namespace RPSAdmin

    // === Приложение ===
    type App() =
        static let mutable _projects: Project array = [||]

        static member Projects = _projects

        static member addProject(project: Project) =
            _projects <- Array.append _projects [| project |]

        static member updateProject(project: Project) =
            let idx = Array.findIndex (fun(p: Project) -> p.Id = project.Id) _projects
            project.Users <- _projects.[idx].Users
            project.ProjectItems <- _projects.[idx].ProjectItems
            _projects.[idx] <- project

        static member getProjectItemsByCategory(category: string) =
            Array.filter (fun(projectItem: ProjectItem) -> projectItem.Category = category) 
                (Array.collect (fun(project: Project) -> project.ProjectItems) _projects)