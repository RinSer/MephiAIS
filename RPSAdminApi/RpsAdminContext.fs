namespace RPSAdminContext

open System
open Bogus
open RPSAdmin

module Context = 

    let ProjectsCount = 5
    let UsersCount = 20
    let UserRole = "Manager"
    let Categories = [| "Task"; "Issue"; "UserStory" |]
    let Statuses = [| "New"; "Pending"; "Over" |]

    let InitiateApp(app: App) =
        for i in 1..ProjectsCount do
            let faker = new Faker("ru")
            app.addProject(new Project(i, faker.Company.CompanyName(), faker.Company.CatchPhrase()))

        for project in app.Projects do
            for i in 1..UsersCount do
                let faker = new Faker("ru")
                let user = faker.Person
                project.addUser(new User(i, user.UserName, user.Phone, user.LastName, user.FirstName, 
                                        user.FirstName, UserRole))
        let mutable id = 0
        for project in app.Projects do
            for user in project.Users do
                for i in 1..(Categories.Length * Statuses.Length) do
                    let faker = new Faker("ru")
                    let category = Categories.[i % 3]
                    let status = Statuses.[i % 3]
                    project.addProjectItem(new ProjectItem(id, faker.Lorem.Sentence(), faker.Lorem.Paragraph(id),
                                                    DateTime.Now, DateTime.Now, status, category), [| user |])
                    id <- id + 1
        app