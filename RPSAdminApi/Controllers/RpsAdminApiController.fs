namespace RPSAdminApi.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open RPSAdmin

[<ApiController>]
[<Route("[controller]")>]
type RPSAdminApiController (logger : ILogger<RPSAdminApiController>) =
    inherit ControllerBase()

    // === Projects ===

    [<HttpGet>]
    member __.GetAllProjects() : Project[] =
        App.Projects

    [<HttpPost>]
    member __.AddProject(project: Project): Project = 
        App.addProject(project)
        App.Projects.First(fun(p: Project) -> p.Id = project.Id)

    [<HttpPut>]
    member __.UpdateProject(project: Project): Project =
        App.updateProject(project)
        App.Projects.First(fun(p: Project) -> p.Id = project.Id)

    // === Users ===

    [<HttpGet("users")>]
    member __.GetAllUsers(): User[] =
        Array.distinct (Array.collect (fun(p: Project) -> p.Users) App.Projects)

    [<HttpGet("users/loginpassword")>]
    member __.GetUsersLoginPassword(): {|Login: string; Password: string|}[] =
        Array.distinct (Array.collect 
            (fun(p: Project) -> p.Users.Select(fun u -> {|Login = u.Login; Password = u.Password|}).ToArray()) App.Projects)

    [<HttpGet("users/{id}")>]
    member __.GetUserById(id: int): User =
        (Array.distinct (Array.collect (fun(p: Project) -> p.Users) App.Projects)).First(fun u -> u.Id = id)

    [<HttpPost("users/{projectId}")>]
    member __.AddUser(projectId: int, user: User): User =
        let project = App.Projects.First(fun p -> p.Id = projectId)
        project.addUser(user)
        project.Users.First(fun u -> u.Id = user.Id)

    [<HttpPut("users")>]
    member __.UpdateUser(user: User): User =
        for project in Array.filter (fun(p: Project) -> p.Users.Any(fun u -> u.Id = user.Id)) App.Projects do
            project.updateUser(user)
        user

    // === ProjectItems ===

    [<HttpGet("{category}")>]
    member __.GetProjectItemsByCategory(category: string): ProjectItem[] =
        App.getProjectItemsByCategory(category)

    [<HttpGet("projectItems")>]
    member __.GetAllProjectItems(): ProjectItem[] =
        Array.distinct (Array.collect (fun(p: Project) -> p.ProjectItems) App.Projects)

    [<HttpPost("projectItems/{projectId}")>]
    member __.AddProjectItem(projectId: int, projectItem: {| users: User[]; item: ProjectItem |}): ProjectItem =
        let project = App.Projects.First(fun p -> p.Id = projectId)
        project.addProjectItem(projectItem.item, projectItem.users)
        project.ProjectItems.First(fun i -> i.Id = projectItem.item.Id)

    [<HttpPut("projectItems/{projectId}")>]
    member __.UpdateProjectItem(projectId: int, projectItem: {| users: User[]; item: ProjectItem |}): ProjectItem =
        let project = App.Projects.First(fun p -> p.Id = projectId)
        project.updateProjectItem(projectItem.item, projectItem.users)
        project.ProjectItems.First(fun i -> i.Id = projectItem.item.Id)