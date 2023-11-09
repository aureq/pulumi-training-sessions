import * as pulumi from "@pulumi/pulumi";
import * as random from "@pulumi/random";
import { readFileSync } from "fs";

const config = new pulumi.Config();

export = async () => {
  // TODO: 5. Generate a new password
  const password = new random.RandomPassword("password", {
                                                        /* FIXME */
  });

  // TODO: 6. Generate a random pet name
  const petName =                                       /* FIXME */

  // TODO: 7. Concatenate the `stackOwner` value with the random pet name (format: `owner-pet`)
  const ownerName = config.require("");                 /* FIXME */
  const ownerPetName = `${ownerName}-${petName.id}`;    /* FIXME */
  console.log("Owner pet name: ", ownerPetName);        /* FIXME */

  // TODO: 8. Use pulumi.all() to create a welcome message "`Hello dear <ownerName>, this is your pet <petName>.`"
  const welcomeMessage = `Hello dear ${o}, this is your pet ${p}.`;

  // TODO: 9. Create stack outputs (See README.md)
  return {
    password: ,                                         /* FIXME */
    stackPet: ,                                         /* FIXME */
    ownerPetName: ,                                     /* FIXME */
    apiKey: ,                                           /* FIXME */
    insecureApiKey: ,                                   /* FIXME */
    welcomeMessage: ,                                   /* FIXME */
    // TODO: 10. Set a stack README for your stack
    readme: ,                                           /* FIXME */
  };
};
