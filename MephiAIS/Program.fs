open System
open RPSAdmin

[<EntryPoint>]
let main argv =
    
    let project = new Project(0, "test", "test")
    let user = new User(0, "test", "test", "t", "t", "t", "manager")
    let projectItem = new ProjectItem(0, "test", "test", project.Id, DateTime.Now, DateTime.Now, "over", "Issue", user)
    
    Project.Add(project)
    User.Add(user)
    ProjectItem.Add(projectItem);

    let createdProject = Project.Get project.Id
    printfn "Created project: %A" createdProject
    printfn "The project has users: %A" createdProject.Users
    printfn "The project has project items: %A" createdProject.ProjectItems

    0 // return an integer exit code
