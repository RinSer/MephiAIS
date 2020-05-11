open System
open RPSAdmin

[<EntryPoint>]
let main argv =
    
    let project = new Project(0, "test", "test")
    let user = new User(0, "test", "test", "t", "t", "t", "manager")
    let projectItem = new ProjectItem(0, "test", "test", DateTime.Now, DateTime.Now, "over", "Issue")
    
    project.addUser(user)
    project.addProjectItem(projectItem, [| user |])
    App.addProject(project)

    for project in App.Projects do
        printfn "Created project: %A" project
        printfn "The project has users: %A" project.Users
        printfn "The project has project items: %A" project.ProjectItems

    0 // return an integer exit code
