using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Random;

return await Pulumi.Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();

    // TODO: 5. Generate a new password
    var password = new RandomPassword("password", new RandomPasswordArgs
    {
        Length = 20,
        Special = true,
    });

    // TODO: 6. Generate a random pet name
    var petName = new RandomPet("stack-pet", new RandomPetArgs
    {

    });

    // TODO: 7. Concatenate the `stackOwner` value with the random pet name (`ownerName-petName`)
    var ownerName = config.Require("stackOwner");
    var ownerPetName = Output.Format($"{petName.Id}, {ownerName}");
    ownerPetName.Apply(n => { Console.WriteLine($"Owner pet name: {n}!"); return n; });

    // TODO: 8. Use pulumi.all() to create a welcome message "`Hello dear <ownerName>, this is your pet <petName>.`"
    var welcomeMessage = Output.All(Output.Create(ownerName), petName.Id).Apply(t => $"Hello dear {t[0]}, this is your pet {t[1]}" );

    // TODO: 9. Create stack outputs (See README.md)
    return new Dictionary<string, object?>
    {
        ["password"] = password.Result,
        ["stackPet"] = petName.Id,
        ["ownerPetName"] = ownerPetName,
        ["apiKey"] = config.RequireSecret("apiKey"),
        ["insecureApiKey"] = Output.Unsecret(config.RequireSecret("apiKey")),
        ["welcomeMessage"] = welcomeMessage,
        // TODO: 11. Set a stack README for your stack
        ["readme"] = System.IO.File.ReadAllText("./Pulumi.README.md"),
    };
});
