namespace RPSAdmin.Tests

open System
open Xunit
open Bogus
open RPSAdmin

module Globes =
    let ProjectsCount = 3
    let UsersCount = 5
    let UserRole = "Manager"
    let Task = "Task"
    let Issue = "Issue"
    let Status = "Over"
    let Faker = new Faker("ru")

module Fixtures = 
    type RPSAdminFixture() =
        do 
            let faker = Globes.Faker
            for i in 1..Globes.ProjectsCount do
                App.addProject(new Project(i, faker.Company.CompanyName(), faker.Company.CatchPhrase()))
            for project in App.Projects do
                for i in 1..Globes.UsersCount do
                    let user = faker.Person
                    project.addUser(new User(i, user.UserName, user.Phone, user.LastName, user.FirstName, 
                                            user.FirstName, Globes.UserRole))
            let mutable id = 0
            for project in App.Projects do
                for user in project.Users do
                    let category = if id % 2 = 0 then Globes.Task else Globes.Issue
                    project.addProjectItem(new ProjectItem(id, faker.Lorem.Sentence(), faker.Lorem.Paragraph(id),
                                                    DateTime.Now, DateTime.Now, Globes.Status, category), [| user |])
                    id <- id + 1

module RPSAdminTests =
    type RPSAdminTests() =

        // Project tests

        [<Fact>]
        let ``Should get all projects`` () =
            Assert.True(App.Projects.Length >= Globes.ProjectsCount)

        [<Fact>]
        let ``Should add a project`` () =
            let projectToAdd = new Project(App.Projects.Length + 1, Globes.Faker.Company.CompanyName(), Globes.Faker.Company.CatchPhrase())
            App.addProject projectToAdd
            Assert.Contains(projectToAdd, App.Projects)

        [<Fact>]
        let ``Should upodate a project`` () =
            let oldProject = App.Projects.[0];
            let projectToUpdate = new Project(oldProject.Id, Globes.Faker.Company.CompanyName(), Globes.Faker.Company.CatchPhrase())
            App.updateProject projectToUpdate
            Assert.Contains(projectToUpdate, App.Projects)
            Assert.DoesNotContain(oldProject, App.Projects)

        [<Fact>]
        let ``Should contain only project user's project items`` () =
            for project in App.Projects do
                Assert.All(project.ProjectItems, fun item -> 
                    Assert.True(Array.Exists(project.Users, fun user -> Array.contains item user.ProjectItems)))

        // Users tests

        [<Fact>]
        let ``Should get all users`` () =
            Assert.True(Array.sumBy (fun(project: Project) -> project.Users.Length) App.Projects >= Globes.UsersCount)

        [<Fact>]
        let ``Should add a user`` () =
            let userData = Globes.Faker.Person
            let numUsers = Array.sumBy (fun(project: Project) -> project.Users.Length) App.Projects
            let userToAdd = new User(numUsers + 1, userData.UserName, userData.Phone, 
                                    userData.LastName, userData.FirstName, userData.FirstName, Globes.UserRole)
            for project in App.Projects do
                project.addUser userToAdd
            for project in App.Projects do
                Assert.Contains(userToAdd, project.Users)

        [<Fact>]
        let ``Should update a user`` () =
            for project in App.Projects do
                let oldUser = project.Users.[0]
                let userToUpdate = new User(oldUser.Id, "NewLogin", "12345", oldUser.Surname, 
                                            oldUser.Name, oldUser.PatronymicName, oldUser.Role)
                project.updateUser userToUpdate
                Assert.Contains(userToUpdate, project.Users)
                Assert.DoesNotContain(oldUser, project.Users)

        //// ProjectItems tests

        [<Fact>]
        let ``Should get all project items`` () =
            let projectItemsCount = Globes.ProjectsCount * Globes.UsersCount
            Assert.True(Array.sumBy (fun(project: Project) -> project.ProjectItems.Length) App.Projects >= projectItemsCount)

        [<Fact>]
        let ``Should add a project item for all project's users`` () =
            for project in App.Projects do
                let numProjectItems = Array.sumBy (fun(project: Project) -> project.ProjectItems.Length) App.Projects
                let projectItemToAdd = new ProjectItem(numProjectItems + 1, Globes.Faker.Lorem.Sentence(), Globes.Faker.Lorem.Paragraph(0),
                                                        DateTime.Now, DateTime.Now, Globes.Status, Globes.Task)
                project.addProjectItem(projectItemToAdd, project.Users)
                Assert.Contains(projectItemToAdd, project.ProjectItems)
                for user in project.Users do
                    Assert.Contains(projectItemToAdd, user.ProjectItems)

        [<Fact>]
        let ``Should add with a project item a new user if he does not belong to project`` () =
            for project in App.Projects do
                let numProjectItems = Array.sumBy (fun(project: Project) -> project.ProjectItems.Length) App.Projects
                let projectItemToAdd = new ProjectItem(numProjectItems + 1, Globes.Faker.Lorem.Sentence(), Globes.Faker.Lorem.Paragraph(0),
                                                        DateTime.Now, DateTime.Now, Globes.Status, Globes.Task)
                let userData = Globes.Faker.Person
                let numUsers = Array.sumBy (fun(project: Project) -> project.Users.Length) App.Projects
                let userToAdd = new User(numUsers + 1, userData.UserName, userData.Phone, 
                                        userData.LastName, userData.FirstName, userData.FirstName, Globes.UserRole)
                project.addProjectItem(projectItemToAdd, [| userToAdd |])
                Assert.Contains(projectItemToAdd, project.ProjectItems)
                Assert.Contains(userToAdd, project.Users)

        [<Fact>]
        let ``Should update a project item`` () =
            for project in App.Projects do
                let oldProjectItem = project.ProjectItems.[0]
                let projectItemToUpdate = new ProjectItem(oldProjectItem.Id, Globes.Faker.Lorem.Sentence(), Globes.Faker.Lorem.Paragraph(0),
                                                    DateTime.Now, DateTime.Now, Globes.Status, Globes.Task)
                let oldUsers = Array.filter (fun(user: User) -> Array.contains oldProjectItem user.ProjectItems) project.Users
                let updatedUsers = Array.filter (fun(user: User) -> not <| Array.contains user oldUsers) project.Users
                project.updateProjectItem(projectItemToUpdate, updatedUsers)
                Assert.Contains(projectItemToUpdate, project.ProjectItems)
                Assert.DoesNotContain(oldProjectItem, project.ProjectItems)
                for user in updatedUsers do
                    Assert.Contains(projectItemToUpdate, user.ProjectItems)
                    Assert.DoesNotContain(oldProjectItem, user.ProjectItems)
                for user in oldUsers do
                    Assert.DoesNotContain(projectItemToUpdate, user.ProjectItems)
                    Assert.DoesNotContain(oldProjectItem, user.ProjectItems)

        [<Fact>]
        let ``Should update a project item with adding a user to project`` () =
            for project in App.Projects do
                let oldProjectItem = project.ProjectItems.[0]
                let projectItemToUpdate = new ProjectItem(oldProjectItem.Id, Globes.Faker.Lorem.Sentence(), Globes.Faker.Lorem.Paragraph(0),
                                                    DateTime.Now, DateTime.Now, Globes.Status, Globes.Task)
                let oldUsers = Array.filter (fun(user: User) -> Array.contains oldProjectItem user.ProjectItems) project.Users
                let userData = Globes.Faker.Person
                let numUsers = Array.sumBy (fun(project: Project) -> project.Users.Length) App.Projects
                let usersToAdd = [| new User(numUsers + 1, userData.UserName, userData.Phone, 
                                        userData.LastName, userData.FirstName, userData.FirstName, Globes.UserRole) |]
                project.updateProjectItem(projectItemToUpdate, usersToAdd)
                Assert.Contains(projectItemToUpdate, project.ProjectItems)
                Assert.DoesNotContain(oldProjectItem, project.ProjectItems)
                for user in usersToAdd do
                    Assert.Contains(projectItemToUpdate, user.ProjectItems)
                    Assert.DoesNotContain(oldProjectItem, user.ProjectItems)
                    Assert.Contains(user, project.Users)
                for user in oldUsers do
                    Assert.DoesNotContain(projectItemToUpdate, user.ProjectItems)
                    Assert.DoesNotContain(oldProjectItem, user.ProjectItems)

        [<Fact>]
        let ``Should get project items by categories`` () =
            let tasks = App.getProjectItemsByCategory Globes.Task
            Assert.All(tasks, fun item -> Assert.True(item.Category = Globes.Task))
            let issues = App.getProjectItemsByCategory Globes.Issue
            Assert.All(issues, fun item -> Assert.True(item.Category = Globes.Issue))
            let numProjectItems = Array.sumBy (fun(project: Project) -> project.ProjectItems.Length) App.Projects
            Assert.StrictEqual(numProjectItems, tasks.Length + issues.Length);

        interface IClassFixture<Fixtures.RPSAdminFixture>