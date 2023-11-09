# Pulumi training using TypeScript (2nd session)
Answers to the 2nd training session

## Answers ##

1. `pulumi stack init loginName/dev` and `dotnet restore`
2. `pulumi config set stackOwner Aurelien`
3. `pulumi config set subscriptionId 123-123-123`
4. `pulumi config set apiKey --secret`
5. `dotnet add package Pulumi.Random` and See [Program.cs](Program.cs)
6. See [Program.cs](Program.cs)
7. See [Program.cs](Program.cs)
8. See [Program.cs](Program.cs)
9. See [Program.cs](Program.cs)
10. `pulumi stack tag set pet $(pulumi stack output stackPet)`
11. See [Program.cs](Program.cs)