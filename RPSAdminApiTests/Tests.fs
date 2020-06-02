namespace RPSAdminApi.Tests

open System
open Xunit
open Bogus
open RPSAdmin
open RPSAdminContext
open RPSAdminApi.Controllers

module RPSAdminTests =
    
    let App = Context.InitiateApp(new App(null))
    let Controller = RPSAdminApiController(App, null)
    
    type RPSAdminTests() =

        // Project tests

        [<Fact>]
        let ``Should get all projects`` () =
            let projects = Controller.GetAllProjects()
            Assert.NotEmpty(projects)
            for project in projects do
                Assert.NotNull(project.Id)
                Assert.NotNull(project.Title)
                Assert.NotNull(project.Description)
                Assert.NotNull(project.Users)
                Assert.NotNull(project.ProjectItems)

        [<Fact>]
        let ``Should add a project`` () =
            let id = App.Projects.Length + 1
            let title = "New test project"
            let description = "New test project description"
            let project = Project(id, title, description)
            let newProject = Controller.AddProject(project)
            Assert.Contains(project, App.Projects)
            Assert.Equal(id, newProject.Id)
            Assert.Equal(title, newProject.Title)
            Assert.Equal(description, newProject.Description)

        [<Fact>]
        let ``Should update a project`` () =
            let newTitle = "New project title"
            let newDescription = "New project description"
            let oldProject = Controller.GetAllProjects().[0]
            let updatedProject = Project(oldProject.Id, newTitle, newDescription)
            let updated = Controller.UpdateProject(updatedProject)
            Assert.DoesNotContain(oldProject, App.Projects)
            Assert.Contains(updatedProject, App.Projects)
            Assert.Equal(newTitle, updated.Title)
            Assert.Equal(newDescription, updated.Description)

        // Users tests

        [<Fact>]
        let ``Should get all users`` () =
            let users = Controller.GetAllUsers()
            Assert.NotEmpty(users)
            for user in users do
                Assert.NotNull(user.Id)
                Assert.NotNull(user.Login)
                Assert.NotNull(user.Password)
                Assert.NotNull(user.Name)
                Assert.NotNull(user.PatronymicName)
                Assert.NotNull(user.Surname)
                Assert.NotNull(user.Role)
                Assert.NotNull(user.ProjectItems)

        [<Fact>]
        let ``Should get all users' logins and passwords`` () =
            let logins = Controller.GetUsersLoginPassword()
            Assert.NotEmpty(logins)
            for user in logins do
                Assert.NotNull(user.Login)
                Assert.NotNull(user.Password)

        [<Fact>]
        let ``Should get a user`` () =
            let testUser = App.Projects.[0].Users.[0]
            let user = Controller.GetUserById(testUser.Id)
            Assert.NotNull(user)
            Assert.Equal(testUser, user)

        [<Fact>]
        let ``Should add a user`` () =
            let projectId = App.Projects.[0].Id
            let id = Controller.GetAllUsers().Length + 1;
            let login = "TestUser"
            let password = "1234567890"
            let name = "Test"
            let patronymic = "Testovich"
            let surname = "Testov"
            let role = "Manager"
            let user = User(id, login, password, surname, name, patronymic, role)
            let newUser = Controller.AddUser(projectId, user)
            Assert.NotNull(newUser)
            Assert.Equal(user, newUser)
            Assert.Contains(newUser, Controller.GetAllUsers())

        [<Fact>]
        let ``Should update a user`` () =
            let oldUser = Controller.GetAllUsers().[0]
            let login = "UpdatedUser"
            let password = "1234567890"
            let name = "Updat"
            let patronymic = "Updatovich"
            let surname = "Updatov"
            let user = User(oldUser.Id, login, password, surname, name, patronymic, oldUser.Role)
            let updatedUser = Controller.UpdateUser(user)
            Assert.NotNull(updatedUser)
            Assert.Equal(login, updatedUser.Login)
            Assert.Equal(password, updatedUser.Password)
            Assert.Equal(name, updatedUser.Name)
            Assert.Equal(patronymic, updatedUser.PatronymicName)
            Assert.Equal(surname, updatedUser.Surname)
            Assert.DoesNotContain(oldUser, Controller.GetAllUsers())
            Assert.Contains(user, Controller.GetAllUsers())

        // Project item tests

        [<Fact>]
        let ``Should get project items by categories`` () =
            for category in Context.Categories do
                let items = Controller.GetProjectItemsByCategory(category)
                Assert.All(items, fun(pi: ProjectItem) -> Assert.Equal(category, pi.Category))

        [<Fact>]
        let ``Should get all project items`` () =
            let items = Controller.GetAllProjectItems()
            Assert.NotEmpty(items)
            for item in items do
                Assert.NotNull(item.Id)
                Assert.NotNull(item.Title)
                Assert.NotNull(item.Category)
                Assert.NotNull(item.Status)
                Assert.NotNull(item.Description)
                Assert.NotNull(item.StartTime)
                Assert.NotNull(item.EndTime)

        [<Fact>]
        let ``Should add a project item`` () =
            let project = App.Projects.[0]
            let id = Controller.GetAllProjectItems().Length + 1
            let title = "Test Item"
            let category = Context.Categories.[0]
            let status = Context.Statuses.[0]
            let description = "Test item description"
            let startTime = DateTime.Now
            let endTime = DateTime.Now.AddDays(100.0)
            let item = ProjectItem(id, title, description, startTime, endTime, status, category)
            let newItem = Controller.AddProjectItem(project.Id, {| item = item; users = project.Users |})
            Assert.NotNull(newItem)
            Assert.Contains(item, project.ProjectItems)
            Assert.Equal(item, newItem)
            Assert.Equal(title, newItem.Title)
            Assert.Equal(category, newItem.Category)
            Assert.Equal(description, newItem.Description)
            Assert.Equal(status, newItem.Status)
            Assert.Equal(startTime, newItem.StartTime)
            Assert.Equal(endTime, newItem.EndTime)
            Assert.All(project.Users, fun u -> Assert.Contains(item, u.ProjectItems))

        [<Fact>]
        let ``Should update a project item`` () =
            let project = App.Projects.[0]
            let oldItem = project.ProjectItems.[0]
            let title = "Updated Item"
            let category = (Array.filter (fun(c: string) -> c <> oldItem.Category) Context.Categories).[0]
            let status = (Array.filter (fun(s: string) -> s <> oldItem.Status) Context.Statuses).[0]
            let description = "Updated item description"
            let endTime = DateTime.Now.AddDays(1000.0)
            let item = ProjectItem(oldItem.Id, title, description, oldItem.StartTime, endTime, status, category)
            let updatedItem = Controller.UpdateProjectItem(project.Id, {| item = item; users = project.Users |})
            Assert.NotNull(updatedItem)
            Assert.Equal(item, updatedItem)
            Assert.DoesNotContain(oldItem, project.ProjectItems)
            Assert.Contains(item, project.ProjectItems)
            Assert.All(project.Users, fun u -> Assert.Contains(item, u.ProjectItems))