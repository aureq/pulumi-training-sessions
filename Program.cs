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
                                                                                    // FIXME
    });

    // TODO: 6. Generate a random pet name
    var petName = new                                                               // FIXME

    // TODO: 7. Concatenate the `stackOwner` value with the random pet name (`ownerName-petName`)
    var ownerName = config.Require("");                                             // FIXME
    var ownerPetName = $"{petName.Id}, {ownerName}";                                // FIXME
    Console.WriteLine($"Owner pet name: {n}!");                                     // FIXME

    // TODO: 8. Use pulumi.all() to create a welcome message "`Hello dear <ownerName>, this is your pet <petName>.`"
    var welcomeMessage = $"Hello dear {ownerName}, this is your pet {petName}" );   // FIXME

    // TODO: 9. Create stack outputs (See README.md)
    return new Dictionary<string, object?>
    {
        ["password"] = ,                                                            // FIXME
        ["stackPet"] = ,                                                            // FIXME
        ["ownerPetName"] = ,                                                        // FIXME
        ["apiKey"] = ,                                                              // FIXME
        ["insecureApiKey"] = ,                                                      // FIXME
        ["welcomeMessage"] = ,                                                      // FIXME
        // TODO: 10. Set a stack README for your stack
        ["readme"] = ,                                                              // FIXME
    };
});
