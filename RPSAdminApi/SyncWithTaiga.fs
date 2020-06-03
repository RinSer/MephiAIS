module SyncWithTaiga

    open System
    open System.Collections.Generic
    open System.Linq
    open Microsoft.AspNetCore.Mvc
    open Hopac
    open HttpFs.Client
    open Newtonsoft.Json
    open RPSAdmin
    open TaigaTypes

    type TaigaSyncService(app: App) =

        let projectsUrl = "http://localhost:8000/api/v1/projects"
        let tasksByProject = "http://localhost:8000/api/v1/tasks?project=%d"
        let tasksUrl = "http://localhost:8000/api/v1/tasks"

        let getResponse(url: string) = Request.createUrl Get url |> Request.responseAsString |> run

        let addOrUpdateProjectTasks(projectId: int) =
            let url = sprintf "http://localhost:8000/api/v1/tasks?project=%d" projectId
            let tasksResponse = getResponse url
            let tasks = JsonConvert.DeserializeObject<List<TaigaTask>>(tasksResponse)
                                .Select(fun t -> (t.Project, new ProjectItem(t.Id, "", 
                                                                t.Subject,
                                                                t.Created_date,
                                                                t.Due_date,
                                                                t.Status_extra_info.Name,
                                                                "Task"), new User(t.Assigned_to_extra_info.Id, 
                                                                    t.Assigned_to_extra_info.Username, 
                                                                    t.Assigned_to_extra_info.Gravatar_id, 
                                                                    t.Assigned_to_extra_info.Full_name_display, 
                                                                    "", "", "Manager"))).ToList()
            for taskTuple in tasks do
                let (projectId, item, user) = taskTuple
                let project = app.Projects.FirstOrDefault(fun p -> p.Id = projectId)
                if Array.exists (fun(pi: ProjectItem) -> pi.Id = item.Id) project.ProjectItems then
                    project.updateProjectItem(item, [| user |])
                else project.addProjectItem(item, [| user |])

        let addOrUpdateProjects() =
            let projectsResponse = getResponse projectsUrl
            let projects = JsonConvert.DeserializeObject<List<TaigaProject>>(projectsResponse)
                                        .Select(fun p -> new Project(p.Id, p.Name, p.Description)).ToList()
            for project in projects do
                if Array.exists (fun(p: Project) -> p.Id = project.Id) app.Projects then
                    app.updateProject project
                else app.addProject project
                addOrUpdateProjectTasks(project.Id)

        let addOrUpdateTasks() =
            let tasksResponse = getResponse tasksUrl
            let tasks = JsonConvert.DeserializeObject<List<TaigaTask>>(tasksResponse)
                            .Select(fun t -> (t.Project, new ProjectItem(t.Id, "", 
                                                            t.Subject,
                                                            t.Created_date,
                                                            t.Due_date,
                                                            t.Status_extra_info.Name,
                                                            "Task"), new User(t.Assigned_to_extra_info.Id, 
                                                                t.Assigned_to_extra_info.Username, 
                                                                t.Assigned_to_extra_info.Gravatar_id, 
                                                                t.Assigned_to_extra_info.Full_name_display, 
                                                                "", "", "Manager"))).ToList()
            for taskTuple in tasks do
                let (projectId, item, user) = taskTuple
                let project = app.Projects.FirstOrDefault(fun p -> p.Id = projectId)
                if Array.exists (fun(pi: ProjectItem) -> pi.Id = item.Id) project.ProjectItems then
                    project.updateProjectItem(item, [| user |])
                else project.addProjectItem(item, [| user |])

        member this.UpdateTasksFromTaiga() =
            addOrUpdateProjects() |> ignore
            //addOrUpdateTasks() |> ignore
 