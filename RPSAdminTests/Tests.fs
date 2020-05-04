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
                Project.Add(new Project(i, faker.Company.CompanyName(), faker.Company.CatchPhrase()))
            for i in 1..Globes.UsersCount do
                let user = faker.Person
                User.Add(new User(i, user.UserName, user.Phone, user.LastName, user.FirstName, user.FirstName, Globes.UserRole))
            let mutable id = 0
            for project in Project.GetAll do
                for user in User.GetAll do
                    let category = if id % 2 = 0 then Globes.Task else Globes.Issue
                    ProjectItem.Add(new ProjectItem(id, faker.Lorem.Sentence(), faker.Lorem.Paragraph(id), project.Id, 
                                                    DateTime.Now, DateTime.Now, Globes.Status, category, user))
                    id <- id + 1

module RPSAdminTests =
    type RPSAdminTests() =

        // Project tests

        [<Fact>]
        let ``Should get all projects`` () =
            Assert.True(Project.GetAll.Length >= Globes.ProjectsCount)

        [<Fact>]
        let ``Should get a project`` () =
            let projectExpected = Project.GetAll.[0]
            let projectTested = Project.Get projectExpected.Id
            Assert.Equal(projectExpected, projectTested)

        [<Fact>]
        let ``Should add a project`` () =
            let projectToAdd = new Project(Project.GetAll.Length + 1, Globes.Faker.Company.CompanyName(), 
                                                                        Globes.Faker.Company.CatchPhrase())
            Project.Add projectToAdd
            Assert.Contains(projectToAdd, Project.GetAll)
            let addedProject = Project.Get projectToAdd.Id
            Assert.NotNull(addedProject)
            Assert.Equal(projectToAdd, addedProject)

        [<Fact>]
        let ``Should contain only project's users`` () =
            for project in Project.GetAll do
                Assert.All(project.Users, fun user -> Assert.True(Array.Exists(project.ProjectItems, fun item -> item.AssignedTo = user)))

        [<Fact>]
        let ``Should contain only project's project items`` () =
            for project in Project.GetAll do
                Assert.All(project.ProjectItems, fun item -> Assert.True(item.ProjectId = project.Id))

        // Users tests

        [<Fact>]
        let ``Should get all users`` () =
            Assert.True(User.GetAll.Length >= Globes.UsersCount)

        [<Fact>]
        let ``Should get a user`` () =
            let userExpected = User.GetAll.[0]
            let userTested = User.Get userExpected.Id
            Assert.Equal(userExpected, userTested)

        [<Fact>]
        let ``Should add a user`` () =
            let userData = Globes.Faker.Person
            let userToAdd = new User(User.GetAll.Length + 1, userData.UserName, userData.Phone, 
                                    userData.LastName, userData.FirstName, userData.FirstName, Globes.UserRole)
            User.Add userToAdd
            Assert.Contains(userToAdd, User.GetAll)
            let addedUser = User.Get userToAdd.Id
            Assert.Equal(userToAdd, addedUser)

        [<Fact>]
        let ``Should update a user`` () =
            let userExpected = User.GetAll.[0]
            let userToUpdate = new User(userExpected.Id, "NewLogin", "12345", userExpected.Surname, 
                                        userExpected.Name, userExpected.PatronymicName, userExpected.Role)
            User.Update userToUpdate
            let updatedUser = User.Get userExpected.Id
            Assert.Equal(userToUpdate.Login, updatedUser.Login)
            Assert.Equal(userToUpdate.Password, updatedUser.Password)

        // ProjectItems tests

        [<Fact>]
        let ``Should get all project items`` () =
            let projectItemsCount = Globes.ProjectsCount * Globes.UsersCount
            Assert.True(ProjectItem.GetAll.Length >= projectItemsCount)

        [<Fact>]
        let ``Should get a project item`` () =
            let projectItemExpected = ProjectItem.GetAll.[0]
            let projectItemTested = ProjectItem.Get projectItemExpected.Id
            Assert.Equal(projectItemExpected, projectItemTested)

        [<Fact>]
        let ``Should add a project item`` () =
            let projectItemToAdd = new ProjectItem(ProjectItem.GetAll.Length + 1, Globes.Faker.Lorem.Sentence(), 
                                                   Globes.Faker.Lorem.Paragraph(0), Project.GetAll.[0].Id, 
                                                    DateTime.Now, DateTime.Now, Globes.Status, Globes.Task, User.GetAll.[0])
            ProjectItem.Add projectItemToAdd
            Assert.Contains(projectItemToAdd, ProjectItem.GetAll)
            let addedProjectItem = ProjectItem.Get projectItemToAdd.Id
            Assert.NotNull(addedProjectItem)
            Assert.Equal(projectItemToAdd, addedProjectItem)

        [<Fact>]
        let ``Should get project items by categories`` () =
            Assert.All(ProjectItem.GetByCategory Globes.Task, fun item -> Assert.True(item.Category = Globes.Task))
            Assert.All(ProjectItem.GetByCategory Globes.Issue, fun item -> Assert.True(item.Category = Globes.Issue))

        interface IClassFixture<Fixtures.RPSAdminFixture>